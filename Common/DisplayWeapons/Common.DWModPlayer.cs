using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class DWModPlayer : ModPlayer
    {
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (ShouldDrawShield(drawInfo.drawPlayer))
            {
                PlayerDrawLayers.Shield.Hide();
                PlayerDrawLayers.SolarShield.Hide();
            }
        }

        private static bool ShouldDrawShield(Player player)
        {
            Item item = DWHelper.GetDisplayItem(player);

            return DWRegistry.IsDW_Gun(item)
                || DWRegistry.IsDW_GreatSword(item)
                || DWRegistry.IsDW_MiniGun(item)
                || DWRegistry.IsDW_Bow(item)
                || DWRegistry.IsDW_Crossbow(item)
                || DWRegistry.IsDW_Rapier(item)
                || DWRegistry.IsDW_Spear(item)
                || DWRegistry.IsDW_Lance(item)
            //   || DWRegistry.IsDW_(item)
            //    || DWRegistry.IsDW_(item)
            //     || DWRegistry.IsDW_(item)
            //      || DWRegistry.IsDW_(item)
            ;
        }
    }
}
