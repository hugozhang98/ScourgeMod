using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class StaffDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Staff(item);

        protected override Vector2 GetOrigin(
            Texture2D texture,
            Rectangle frame,
            SpriteEffects effects,
            Item item,
            Player player
        )
        {
            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                Rectangle visibleFrame = TextureHelper.GetVisibleFrame(texture);

                if (visibleFrame == Rectangle.Empty)
                    return frame.Size() * 0.5f;

                // 原点在完整贴图中的位置
                Vector2 origin =
                    visibleFrame.Location.ToVector2()
                    + new Vector2(visibleFrame.Width * 0f, visibleFrame.Height * 1f);

                return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
            }
            else
            {
                return frame.Size() * 0.5f;
            }
        }

        protected override Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item)
        {
            if (DWHelper.GetVanillaAnimationType(player) != DWVanillaAnimationType.Idle)
            {
                return DWHelper.GetPlayerJointPosition(drawInfo, DWPlayerJointType.BackHand);
            }

            return base.GetPosition(drawInfo, player, item);
        }

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                return new Vector2(15f, 27f);
            }

            return base.GetPositionOffset(drawInfo, player, item);
        }

        protected override float GetRotationOffset(Player player)
        {
            float baseRotation = -47f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Walking)
            {
                baseRotation = 5f;
            }

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                baseRotation = -100f;
            }

            return baseRotation;
        }

        protected override float GetWeaponSway(Player player) => 1f;
    }
}
