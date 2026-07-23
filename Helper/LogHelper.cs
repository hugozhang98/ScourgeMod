using Terraria;

namespace ScourgeMod.Helper
{
    /// <summary>
    /// 调试日志工具类
    /// </summary>
    public class rrzz
    {
        public static readonly uint Th_Interval = 100;

        public static uint CurrentUpdateCount = 0;
        public static uint NextUpdateCount = Th_Interval;

        /// <summary>
        /// 带游戏时间帧的msg
        /// </summary>
        /// <param name="msg">打印信息</param>
        /// <returns></returns>
        public static string MsgPlusGameFrame(string msg) => $"[{Main.GameUpdateCount}] {msg}";

        /// <summary>
        /// 输出日志信息
        /// </summary>
        /// <param name="msg">打印信息</param>
        public static void Log(string msg)
        {
            ScourgeMod.Instance.Logger.Info(MsgPlusGameFrame(msg));
        }

        /// <summary>
        /// 输出日志信息(节流版)
        /// </summary>
        /// <param name="msg">打印信息</param>
        public static void LogTh(string msg)
        {
            if (
                Main.GameUpdateCount != CurrentUpdateCount
                && Main.GameUpdateCount != NextUpdateCount
            )
                return;

            if (Main.GameUpdateCount == NextUpdateCount && CurrentUpdateCount != NextUpdateCount)
            {
                CurrentUpdateCount = NextUpdateCount;
                NextUpdateCount += Th_Interval;
            }

            ScourgeMod.Instance.Logger.Info(MsgPlusGameFrame(msg));
        }
    }
}
