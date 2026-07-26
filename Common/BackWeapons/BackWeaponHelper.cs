using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ScourgeMod.Helper;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.BackWeapons
{
    public static class BackWeaponHelper
    {
        public static bool GetDefaultVisibility(Item item, Player player)
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

            //特殊的一些物品，使用后有残留弹幕。需要等弹幕消失后才绘制
            if (
                new int[] { ItemID.Anchor }.Contains(item.type)
                && player.ownedProjectileCounts[item.shoot] > 0
            )
                return false;

            return true;
        }

        public static float GetMoveSway(Player player, float intensity = 3f)
        {
            // 根据水平速度计算摆动强度：静止为 0，速度> 6 时为 1
            float moveFactor = MathHelper.Clamp(MathF.Abs(player.velocity.X) / 6f, 0f, 1f);
            // 每约 60 帧完成一次摆动，最大摆动 6 度
            float sway =
                player.velocity.Y != 0
                    ? 0f
                    : ProcessHelper.Swing((float)Main.GameUpdateCount, 30f)
                        * AngleHelper.DegToRad(intensity)
                        * moveFactor;

            return sway * player.direction;
        }
    }
}
