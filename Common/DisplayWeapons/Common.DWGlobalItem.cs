using Terraria;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class DWGlobalItem : GlobalItem
    {
        public override void HoldItemFrame(Item item, Player player)
        {
            base.HoldItemFrame(item, player);

            DWAdjust_Class_GreatSword.HoldItemFrame(item, player);
            DWAdjust_Class_Gun.HoldItemFrame(item, player);
            DWAdjust_Class_MiniGun.HoldItemFrame(item, player);
            DWAdjust_Class_Bow.HoldItemFrame(item, player);
            DWAdjust_Class_Crossbow.HoldItemFrame(item, player);
            DWAdjust_Class_Rapier.HoldItemFrame(item, player);
            DWAdjust_Class_Stick.HoldItemFrame(item, player);
            DWAdjust_Class_Spear.HoldItemFrame(item, player);
            DWAdjust_Class_Lance.HoldItemFrame(item, player);
        }
    }
}
