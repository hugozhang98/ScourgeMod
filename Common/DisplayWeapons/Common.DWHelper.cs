using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public static class DWHelper
    {
        //public static Item GetDisplayItem(Player player)
        //{
        //    Item heldItem = player.HeldItem;

        //    if (!Main.gameMenu)
        //        return heldItem;

        //    // 人物选择页不会可靠保留玩家退出前选中的快捷栏槽位。
        //    // 预览时回退到背包中第一件可以展示的武器。
        //    foreach (Item item in player.inventory)
        //    {
        //        if (!item.IsAir)
        //            return item;
        //    }

        //    return heldItem;
        //}

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

        public static Vector2 GetUpperBodyBobbing(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.velocity.Y != 0)
                return Vector2.Zero;

            int frameIndex = player.bodyFrame.Y / player.bodyFrame.Height;

            // 原版按走路身体帧查出的离散像素偏移
            Vector2 offset = Main.OffsetsPlayerHeadgear[frameIndex];

            // 与身体绘制基准对齐
            offset.Y -= 2f;

            // 重力翻转时，起伏方向也翻转
            if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically))
                offset *= -1f;

            return player.bodyPosition + offset;
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
}
