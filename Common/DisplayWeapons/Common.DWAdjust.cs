using Microsoft.Xna.Framework;
using ScourgeMod.Helper;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.DisplayWeapons
{
    public static class DWAdjust_ChainGuillotines
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.ChainGuillotines;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }

        public static bool TryAdjustPosition(Player player, Item item, ref Vector2 position)
        {
            if (AppliesTo(item))
            {
                position += new Vector2(player.direction * 1f, player.gravDir * -3f);
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_Anchor
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.Anchor;

        public static bool GetDefaultVisibility(Player player, Item item)
        {
            return !(AppliesTo(item) && player.ownedProjectileCounts[item.shoot] > 0);
        }

        public static bool TryAdjustPosition(Player player, Item item, ref Vector2 position)
        {
            if (AppliesTo(item))
            {
                position += new Vector2(player.direction * 6f, player.gravDir * 4f);
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_Megashark
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.Megashark;

        // GetOrigin
        public static bool TryAdjustOrigin(Item item, ref Vector2 origin)
        {
            if (AppliesTo(item))
            {
                origin += new Vector2(0f, 3f);
                return true;
            }

            return false;
        }

    }

    public static class DWAdjust_Class_GreatSword
    {
        public static void HoldItemFrame(Item item, Player player)
        {
            if (DWRegistry.IsDW_GreatSword(item))
            {
                player.SetCompositeArmBack(
                    enabled: true,
                    Player.CompositeArmStretchAmount.Full,
                    AngleHelper.DegToRad(-player.direction * 70)
                );
            }
        }
    }

    public static class DWAdjust_Class_Gun
    {
        public static void HoldItemFrame(Item item, Player player)
        {


            if (DWRegistry.IsDW_Gun(item))
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
    }

    public static class DWAdjust_Class_MiniGun
    {
        public static void HoldItemFrame(Item item, Player player)
        {


            if (DWRegistry.IsDW_MiniGun(item))
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
    }
}
