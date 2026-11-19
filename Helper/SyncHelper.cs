using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ScourgeMod.Helper
{
    /// <summary>
    /// 通用数据包收发与分发。服务端处理客户端请求后，由业务决定是否向客户端同步。
    /// </summary>
    public static class SyncHelper
    {
        private const int MaxDataCount = 256;

        private static readonly Dictionary<
            byte,
            (Action<object[], int> ServerHandler, Action<object[], int> ClientHandler)
        > Handlers = new();

        //保留旧 SyncUtil 的基础类型编号，收发两端必须使用相同的字段顺序和类型。
        private enum DataType : byte
        {
            Int32 = 0,
            Single = 1,
            String = 2,
            Boolean = 3,
            Double = 4,
            Int64 = 5,
            Int16 = 6,
            Byte = 7,
            UInt32 = 8,
        }

        /// <summary>
        /// 注册消息处理器，通常在 Load 中调用。同一个消息编号只能注册一次。
        /// serverHandler 的第二个参数是发包玩家编号；clientHandler 中该参数为服务器编号。
        /// 处理器应检查字段数量、类型及业务权限，不能信任数据内自报的玩家编号。
        /// </summary>
        public static void RegisterHandler(
            byte syncId,
            Action<object[], int> serverHandler = null,
            Action<object[], int> clientHandler = null
        )
        {
            if (serverHandler == null && clientHandler == null)
            {
                throw new ArgumentException("至少需要提供一个接收端处理器。");
            }

            if (!Handlers.TryAdd(syncId, (serverHandler, clientHandler)))
            {
                throw new InvalidOperationException($"同步消息编号 {syncId} 已被注册。");
            }
        }

        /// <summary>
        /// 客户端向服务器发送数据。单机或服务器调用时不发送，返回 false。
        /// 支持 int、float、string、bool、double、long、short、byte、uint，不支持 null。
        /// </summary>
        public static bool SendToServer(byte syncId, params object[] data)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                return false;
            }

            SendPacket(syncId, data);
            return true;
        }

        /// <summary>
        /// 服务器向客户端发送数据。toClient 为 -1 时广播，ignoreClient 为排除的玩家编号。
        /// 单机或客户端调用时不发送，返回 false。
        /// </summary>
        public static bool SendToClients(
            byte syncId,
            object[] data,
            int toClient = -1,
            int ignoreClient = -1
        )
        {
            if (Main.netMode != NetmodeID.Server)
            {
                return false;
            }

            ValidateClientIndex(toClient, nameof(toClient));
            ValidateClientIndex(ignoreClient, nameof(ignoreClient));
            SendPacket(syncId, data, toClient, ignoreClient);
            return true;
        }

        private static void SendPacket(
            byte syncId,
            object[] data,
            int toClient = -1,
            int ignoreClient = -1
        )
        {
            using ModPacket packet = ScourgeMod.Instance.GetPacket();
            WritePacket(packet, syncId, data);

            //ModPacket 的长度字段为 ushort，长度包含框架写入的包头。
            if (packet.BaseStream.Length > ushort.MaxValue)
            {
                throw new ArgumentException("同步数据超过单个数据包的长度上限。", nameof(data));
            }

            packet.Send(toClient, ignoreClient);
        }

        private static void ValidateClientIndex(int clientIndex, string paramName)
        {
            if (clientIndex < -1 || clientIndex >= Main.maxPlayers)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        /// <summary>
        /// 写入消息编号、字段数量和带类型标记的字段。数据字段最多 256 个。
        /// 不支持的类型直接报错，不写入无法正确解析的占位值。
        /// </summary>
        public static void WritePacket(BinaryWriter writer, byte syncId, params object[] data)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(data);

            if (data.Length > MaxDataCount)
            {
                throw new ArgumentException(
                    $"同步数据字段不能超过 {MaxDataCount} 个。",
                    nameof(data)
                );
            }

            writer.Write(syncId);
            writer.Write(data.Length);

            foreach (object value in data)
            {
                WriteValue(writer, value);
            }
        }

        private static void WriteValue(BinaryWriter writer, object value)
        {
            switch (value)
            {
                case int intValue:
                    writer.Write((byte)DataType.Int32);
                    writer.Write(intValue);
                    break;
                case float floatValue:
                    writer.Write((byte)DataType.Single);
                    writer.Write(floatValue);
                    break;
                case string stringValue:
                    writer.Write((byte)DataType.String);
                    writer.Write(stringValue);
                    break;
                case bool boolValue:
                    writer.Write((byte)DataType.Boolean);
                    writer.Write(boolValue);
                    break;
                case double doubleValue:
                    writer.Write((byte)DataType.Double);
                    writer.Write(doubleValue);
                    break;
                case long longValue:
                    writer.Write((byte)DataType.Int64);
                    writer.Write(longValue);
                    break;
                case short shortValue:
                    writer.Write((byte)DataType.Int16);
                    writer.Write(shortValue);
                    break;
                case byte byteValue:
                    writer.Write((byte)DataType.Byte);
                    writer.Write(byteValue);
                    break;
                case uint uintValue:
                    writer.Write((byte)DataType.UInt32);
                    writer.Write(uintValue);
                    break;
                default:
                    throw new NotSupportedException(
                        $"不支持同步类型 {value?.GetType().FullName ?? "null"}。"
                    );
            }
        }

        /// <summary>
        /// 读取完整数据包。非法字段数量、未知类型或截断数据会抛出异常。
        /// </summary>
        public static (byte SyncId, object[] Data) ReceivePacket(BinaryReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);

            byte syncId = reader.ReadByte();
            int dataCount = reader.ReadInt32();

            if (dataCount < 0 || dataCount > MaxDataCount)
            {
                throw new InvalidDataException($"非法同步字段数量：{dataCount}。");
            }

            object[] data = new object[dataCount];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = ReadValue(reader);
            }

            return (syncId, data);
        }

        private static object ReadValue(BinaryReader reader)
        {
            DataType dataType = (DataType)reader.ReadByte();

            return dataType switch
            {
                DataType.Int32 => reader.ReadInt32(),
                DataType.Single => reader.ReadSingle(),
                DataType.String => reader.ReadString(),
                DataType.Boolean => reader.ReadBoolean(),
                DataType.Double => reader.ReadDouble(),
                DataType.Int64 => reader.ReadInt64(),
                DataType.Int16 => reader.ReadInt16(),
                DataType.Byte => reader.ReadByte(),
                DataType.UInt32 => reader.ReadUInt32(),
                _ => throw new InvalidDataException($"未知同步字段类型：{(byte)dataType}。"),
            };
        }

        /// <summary>
        /// 由 Mod.HandlePacket 调用，只分发给当前接收端的处理器，不自动转发。
        /// 无效发送者、未注册消息以及没有对应方向处理器的消息会被忽略。
        /// </summary>
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }

            if (Main.netMode == NetmodeID.Server && (whoAmI < 0 || whoAmI >= Main.maxPlayers))
            {
                return;
            }

            byte syncId;
            object[] data;

            try
            {
                (syncId, data) = ReceivePacket(reader);
            }
            catch (Exception exception)
                when (exception is IOException or InvalidDataException or FormatException)
            {
                ScourgeMod.Instance.Logger.Warn(
                    $"忽略来自 {whoAmI} 的非法同步包：{exception.Message}"
                );
                return;
            }

            if (!Handlers.TryGetValue(syncId, out var handlers))
            {
                return;
            }

            Action<object[], int> handler =
                Main.netMode == NetmodeID.Server ? handlers.ServerHandler : handlers.ClientHandler;

            //业务异常不作为协议错误吞掉，方便定位处理器内的问题。
            handler?.Invoke(data, whoAmI);
        }

        internal static void Unload()
        {
            Handlers.Clear();
        }
    }
}
