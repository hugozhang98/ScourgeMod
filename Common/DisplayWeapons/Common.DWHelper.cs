using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public enum DWPlayerJointType
    {
        Body,
        Head,
        FrontHand,
        BackHand,
    }

    public enum DWVanillaAnimationType
    {
        Idle,
        Walking,
        Jumping,
        Other,
    }

    public static partial class DWHelper
    {
        public static Item GetDisplayItem(Player player)
        {
            return MenuPreviewItemSystem.GetDisplayItem(player);
        }

        public static bool GetDefaultVisibility(Player player, Item item)
        {
            //角色死亡不绘制
            if (player.dead)
                return false;

            //未持有物品不绘制
            if (item.IsAir)
                return false;

            //正在使用物品不绘制
            if (player.itemAnimation > 0)
                return false;

            //原版通用隐身状态（例如隐身药水）下，展示武器也应一并隐藏
            if (player.invis)
                return false;

            // 乘骑时不绘制
            if (player.mount.Active)
                return false;

            // 其他动作不绘制
            if (GetVanillaAnimationType(player) == DWVanillaAnimationType.Other)
                return false;

            // 正在发射弹药时不绘制
            if (player.ownedProjectileCounts[item.shoot] > 0)
                return false;

            return true;
        }

        public static float GetMoveSway(
            Player player,
            float intensity = 3f,
            float interval = 30f,
            bool isRadius = true
        )
        {
            // 根据水平速度计算摆动强度：静止为 0，速度> 6 时为 1
            float moveFactor = MathHelper.Clamp(MathF.Abs(player.velocity.X) / 6f, 0f, 1f);
            // 每约 60 帧完成一次摆动，最大摆动 6 度
            float sway =
                (player.velocity.Y != 0 || player.mount.Active)
                    ? 0f
                    : ProcessHelper.Swing((float)Main.GameUpdateCount, interval)
                        * (isRadius ? AngleHelper.DegToRad(intensity) : intensity)
                        * moveFactor;

            return sway * player.direction;
        }

        /// <summary>
        /// 获取原版当前正在绘制的基础身体动画。判断依据是 bodyFrame，
        /// 因此地面击退会返回 Walking，空中击飞会返回 Jumping。
        /// 待机返回 Idle；持物、飞行、游泳等无法仅凭身体帧精确分类的动作返回 Other。
        /// </summary>
        public static DWVanillaAnimationType GetVanillaAnimationType(Player player)
        {
            int bodyFrameIndex = GetBodyFrameIndex(player);

            return bodyFrameIndex switch
            {
                0 => DWVanillaAnimationType.Idle,
                >= 7 and <= 19 => DWVanillaAnimationType.Walking,
                5 => DWVanillaAnimationType.Jumping,
                _ => DWVanillaAnimationType.Other,
            };
        }

        public static Color GetDefaultColor(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.isDisplayDollOrInanimate || Main.gameMenu)
                return Color.White;

            Color color = Lighting.GetColor(drawInfo.drawPlayer.Center.ToTileCoordinates());

            // 使用 PlayerDrawSet 准备好的最终身体透明度，自动兼容变态人的刀、蘑菇矿隐身等效果。
            float opacity = drawInfo.colorArmorBody.A / 255f;
            return color * opacity;
        }
    }

    public static partial class DWHelper
    {
        /// <summary>
        /// 获取玩家关节点的屏幕坐标。坐标会跟随身体绘制偏移、身体动画帧、朝向和重力翻转。
        /// </summary>
        public static Vector2 GetPlayerJointPosition(
            PlayerDrawSet drawInfo,
            DWPlayerJointType jointType
        )
        {
            // FrontHand 使用 Player.HandPosition：这是原版计算出的实际主手位置，
            // 而不是仅用于特效附着的 OffsetsPlayerOnhand 原始坐标。
            // 若素材的握持点不在贴图中心，可在此添加小幅的额外校正。
            Vector2 _frontHandPalmOffset = Vector2.Zero;

            // 副手没有对应的原版公开坐标，仍以 OffsetsPlayerOffhand 为基准再校正到掌心。
            // 数值采用“面向右、正常重力”坐标；GetPlayerHandPosition 会负责镜像。
            // 返回值会被像素对齐；这里必须保持整数像素。
            // 3.5f 与末尾 Floor 叠加时，会在人物移动的子像素间隔中交替落到 +3 / +4，
            // 造成相对人物的单像素抖动。
            Vector2 _backHandPalmOffset = new(4f, 10f);

            Player player = drawInfo.drawPlayer;

            if (jointType == DWPlayerJointType.FrontHand)
                return GetPlayerFrontHandPosition(drawInfo, _frontHandPalmOffset);

            if (jointType == DWPlayerJointType.BackHand)
                return GetPlayerHandPosition(
                    drawInfo,
                    Main.OffsetsPlayerOffhand,
                    _backHandPalmOffset
                );

            Vector2 bodyPosition = drawInfo.Center - Main.screenPosition + player.bodyPosition;

            return bodyPosition + GetPlayerJointOffset(drawInfo, jointType);
        }

        public static Vector2 GetUpperBodyBobbing(PlayerDrawSet drawInfo)
        {
            // 保留旧接口，供非关节定位的展示武器继续只取上半身起伏。
            if (drawInfo.drawPlayer.velocity.Y != 0)
                return drawInfo.drawPlayer.bodyPosition;

            Vector2 offset = GetPlayerJointAnimationOffset(drawInfo, DWPlayerJointType.Head);

            if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically))
                offset *= -1f;

            return drawInfo.drawPlayer.bodyPosition + offset;
        }

        private static Vector2 GetPlayerJointOffset(
            PlayerDrawSet drawInfo,
            DWPlayerJointType jointType
        )
        {
            // Head 与 Body 使用身体中心作为基准；双手则直接使用原版的逐帧坐标表。
            Vector2 _eadBaseOffset = new(0f, -14f);

            Player player = drawInfo.drawPlayer;
            Vector2 offset = jointType switch
            {
                // Body 以 Head 的身体帧补偿为共同基准，再叠加各自的静态偏移。
                // 不能直接使用碰撞箱中心；它不会随走路身体帧上下起伏。
                DWPlayerJointType.Body => Vector2.Zero,
                DWPlayerJointType.Head => _eadBaseOffset,
                _ => Vector2.Zero,
            };

            offset.X *= player.direction;
            offset.Y *= player.gravDir;

            Vector2 animationOffset = GetPlayerJointAnimationOffset(drawInfo, jointType);

            if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically))
                animationOffset *= -1f;

            return offset + animationOffset;
        }

        private static Vector2 GetPlayerJointAnimationOffset(
            PlayerDrawSet drawInfo,
            DWPlayerJointType jointType
        )
        {
            Player player = drawInfo.drawPlayer;

            int frameIndex = GetBodyFrameIndex(player);
            Vector2 offset = jointType switch
            {
                DWPlayerJointType.Body => GetFrameOffset(Main.OffsetsPlayerHeadgear, frameIndex),
                DWPlayerJointType.Head => GetFrameOffset(Main.OffsetsPlayerHeadgear, frameIndex),
                _ => Vector2.Zero,
            };

            // 所有关节点都以 Headgear 的身体帧补偿为共同基准。
            // 该锚点比展示武器的身体中心基准高 2 像素。
            offset.Y -= 2f;

            return offset;
        }

        private static Vector2 GetPlayerHandPosition(
            PlayerDrawSet drawInfo,
            Vector2[] handOffsets,
            Vector2 palmOffset
        )
        {
            Player player = drawInfo.drawPlayer;
            int frameIndex = GetBodyFrameIndex(player);
            Vector2 handOffset = GetFrameOffset(handOffsets, frameIndex);

            // 原版以玩家碰撞箱左上角为原点记录手的位置。
            if (player.direction == -1)
                handOffset.X = player.width - handOffset.X;

            if (player.gravDir == -1f)
                handOffset.Y = player.height - handOffset.Y;

            palmOffset.X *= player.direction;
            palmOffset.Y *= player.gravDir;

            // 与原版 OffsetsPlayerOffhand 的附着点公式保持一致。
            // drawInfo.Position 已包含 gfxOffY；bodyPosition 只用于身体帧贴图基准，
            // 额外叠加会使副手在倒重力等状态下偏移。
            return drawInfo.Position - Main.screenPosition + handOffset + palmOffset;
        }

        private static Vector2 GetPlayerFrontHandPosition(
            PlayerDrawSet drawInfo,
            Vector2 palmOffset
        )
        {
            Player player = drawInfo.drawPlayer;

            // HandPosition 内部使用 OffsetsPlayerOnhand * 2，并处理身体帧、跳跃、
            // 重力、台阶偏移和完整旋转；直接使用可避免手动换算时遗漏缩放。
            Vector2 handPosition = player.HandPosition.GetValueOrDefault(player.Center);

            palmOffset.X *= player.direction;
            palmOffset.Y *= player.gravDir;

            return handPosition - Main.screenPosition + palmOffset;
        }

        private static Vector2 GetFrameOffset(Vector2[] offsets, int frameIndex)
        {
            int clampedFrameIndex = Math.Clamp(frameIndex, 0, offsets.Length - 1);
            return offsets[clampedFrameIndex];
        }

        private static int GetBodyFrameIndex(Player player)
        {
            return player.bodyFrame.Y / player.bodyFrame.Height;
        }
    }
}
