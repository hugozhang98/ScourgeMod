using Terraria;

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

            return true;
        }
    }
}
