using ScourgeMod.Helper;
using Terraria;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class DWGlobalItem : GlobalItem
    {
        public override void HoldItemFrame(Item item, Player player)
        {
            base.HoldItemFrame(item, player);

            GreatSwordHoldItemFrame(item, player);
            GunHoldItemFrame(item, player);
            MiniGunHoldItemFrame(item, player);
            BowHoldItemFrame(item, player);
            CrossbowHoldItemFrame(item, player);
            RapierHoldItemFrame(item, player);
            StickHoldItemFrame(item, player);
            SpearHoldItemFrame(item, player);
            LanceHoldItemFrame(item, player);
            StaffHoldItemFrame(item, player);
        }

        private void GreatSwordHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_GreatSword(item))
            {
                player.SetCompositeArmBack(
                    enabled: true,
                    Player.CompositeArmStretchAmount.Full,
                    AngleHelper.DegToRad(-player.direction * 70)
                );
            }
        }

        private void GunHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Gun(item))
            {
                float backArmRad = 45f;
                float frontArmRad = 45f;

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .ThreeQuarters;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );

                player.SetCompositeArmFront(
                    enabled: true,
                    frontArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -frontArmRad)
                );
            }
        }

        private void MiniGunHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_MiniGun(item))
            {
                float backArmRad = 45f;
                float frontArmRad = 70f;

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );

                player.SetCompositeArmFront(
                    enabled: true,
                    frontArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -frontArmRad)
                );
            }
        }

        private void BowHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Bow(item))
            {
                float backArmRad = 45f;
                float frontArmRad = 70f;

                if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
                {
                    backArmRad = 15f;
                    frontArmRad = 35f;
                }

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Quarter;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );

                player.SetCompositeArmFront(
                    enabled: true,
                    frontArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -frontArmRad)
                );
            }
        }

        private void CrossbowHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Crossbow(item))
            {
                float backArmRad = 45f;
                float frontArmRad = 45f;

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .ThreeQuarters;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );

                player.SetCompositeArmFront(
                    enabled: true,
                    frontArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -frontArmRad)
                );
            }
        }

        private void RapierHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Rapier(item))
            {
                if (DWHelper.GetVanillaAnimationType(player) != DWVanillaAnimationType.Idle)
                {
                    return;
                }

                float backArmRad = 62f;

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );
            }
        }

        private void StickHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Stick(item))
            {
                float frontArmRad = -30f;

                Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                player.SetCompositeArmFront(
                    enabled: true,
                    frontArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -frontArmRad)
                );
            }
        }

        private void SpearHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Spear(item))
            {
                if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
                {
                    float backArmRad = 60f;

                    Player.CompositeArmStretchAmount backArmStretchAmount = Player
                        .CompositeArmStretchAmount
                        .ThreeQuarters;

                    player.SetCompositeArmBack(
                        enabled: true,
                        backArmStretchAmount,
                        AngleHelper.DegToRad(player.direction * -backArmRad)
                    );
                }
                else
                {
                    float backArmRad = 50f;
                    float frontArmRad = 40f;

                    Player.CompositeArmStretchAmount backArmStretchAmount = Player
                        .CompositeArmStretchAmount
                        .ThreeQuarters;

                    Player.CompositeArmStretchAmount frontArmStretchAmount = Player
                        .CompositeArmStretchAmount
                        .Full;

                    player.SetCompositeArmBack(
                        enabled: true,
                        backArmStretchAmount,
                        AngleHelper.DegToRad(player.direction * -backArmRad)
                    );

                    player.SetCompositeArmFront(
                        enabled: true,
                        frontArmStretchAmount,
                        AngleHelper.DegToRad(player.direction * -frontArmRad)
                    );
                }
            }
        }

        private void LanceHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Lance(item))
            {
                float backArmRad = 50f;

                Player.CompositeArmStretchAmount backArmStretchAmount = Player
                    .CompositeArmStretchAmount
                    .Full;

                player.SetCompositeArmBack(
                    enabled: true,
                    backArmStretchAmount,
                    AngleHelper.DegToRad(player.direction * -backArmRad)
                );
            }
        }

        private void StaffHoldItemFrame(Item item, Player player)
        {
            if (DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Staff(item))
            {
                if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
                {
                    float backArmRad = 60f;

                    Player.CompositeArmStretchAmount backArmStretchAmount = Player
                        .CompositeArmStretchAmount
                        .ThreeQuarters;

                    player.SetCompositeArmBack(
                        enabled: true,
                        backArmStretchAmount,
                        AngleHelper.DegToRad(player.direction * -backArmRad)
                    );
                }
            }
        }
    }
}
