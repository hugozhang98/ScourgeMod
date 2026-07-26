using ScourgeMod.Helper;
using Terraria;
using Terraria.ModLoader;

namespace ScourgeMod.Common.BackWeapons
{
    internal class BackWeaponGlobalItem : GlobalItem
    {
        public override void HoldItemFrame(Item item, Player player)
        {
            base.HoldItemFrame(item, player);

            Hold_BackSword_Shoulder(item, player);
        }

        private void Hold_BackSword_Shoulder(Item item, Player player)
        {
            if (BackWeaponRegistry.IsBackSword_Shoulder(item))
            {
                player.SetCompositeArmBack(
                    enabled: true,
                    Player.CompositeArmStretchAmount.Full,
                    AngleHelper.DegToRad(-player.direction * 65)
                );
            }
        }
    }
}
