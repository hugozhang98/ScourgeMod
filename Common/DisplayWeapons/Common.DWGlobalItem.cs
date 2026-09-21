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
        }

        //private void Hold_BackSword_Shoulder(Item item, Player player)
        //{
        //    if (BackWeaponRegistry.IsBackSword_Shoulder(item))
        //    {
        //        player.SetCompositeArmBack(
        //            enabled: true,
        //            Player.CompositeArmStretchAmount.Full,
        //            AngleHelper.DegToRad(-player.direction * 65)
        //        );
        //    }
        //}

        //private void Hold_BackGun(Item item, Player player)
        //{
        //    if (BackWeaponRegistry.IsBackGun(item))
        //    {
        //        float backArmRad = 45f;
        //        float frontArmRad = 45f;

        //        Player.CompositeArmStretchAmount backArmStretchAmount = Player
        //            .CompositeArmStretchAmount
        //            .Full;
        //        Player.CompositeArmStretchAmount frontArmStretchAmount = Player
        //            .CompositeArmStretchAmount
        //            .ThreeQuarters;

        //        if (item.type == ItemID.RedRyder)
        //        {
        //            backArmRad = 68f;
        //        }

        //        if (item.type == ItemID.FlintlockPistol)
        //        {
        //            backArmRad = 30f;
        //            backArmStretchAmount = Player.CompositeArmStretchAmount.Quarter;
        //        }

        //        if (item.type == ItemID.Musket)
        //        {
        //            backArmRad = 45f;
        //            backArmStretchAmount = Player.CompositeArmStretchAmount.ThreeQuarters;
        //        }

        //        if (item.type == ItemID.TheUndertaker)
        //        {
        //            backArmRad = 50f;
        //        }

        //        if (item.type == ItemID.Revolver)
        //        {
        //            backArmRad = 35f;
        //            backArmStretchAmount = Player.CompositeArmStretchAmount.Quarter;
        //        }

        //        if (item.type == ItemID.Boomstick)
        //        {
        //            backArmRad = 50f;
        //        }

        //        if (item.type == ItemID.Minishark)
        //        {
        //            frontArmRad = 25f;
        //            frontArmStretchAmount = Player.CompositeArmStretchAmount.Full;
        //        }

        //        //=====================

        //        player.SetCompositeArmBack(
        //            enabled: true,
        //            backArmStretchAmount,
        //            AngleHelper.DegToRad(-player.direction * backArmRad)
        //        );

        //        player.SetCompositeArmFront(
        //            enabled: true,
        //            frontArmStretchAmount,
        //            AngleHelper.DegToRad(-player.direction * frontArmRad)
        //        );
        //    }
        //}
    }
}
