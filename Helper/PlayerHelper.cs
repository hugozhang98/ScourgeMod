using Terraria;

namespace ScourgeMod.Helper
{
    public static class PlayerHelper
    {
        public static bool IsActive(Player player)
        {
            return player.active && !player.dead && !player.ghost;
        }
    }
}
