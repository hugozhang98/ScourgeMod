using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GloveDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() =>
            new AfterParent(PlayerDrawLayers.ArmOverItem);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Glove(item);

        protected override Vector2 GetOrigin(
            Texture2D texture,
            Rectangle frame,
            SpriteEffects effects,
            Item item,
            Player player
        )
        {
            Rectangle visibleFrame = TextureHelper.GetVisibleFrame(texture);

            if (visibleFrame == Rectangle.Empty)
                return frame.Size() * 0.5f;

            // 原点在完整贴图中的位置
            Vector2 origin =
                visibleFrame.Location.ToVector2()
                + new Vector2(visibleFrame.Width * 0.35f, visibleFrame.Height * 0.65f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item) =>
            DWHelper.GetPlayerJointPosition(drawInfo, DWPlayerJointType.FrontHand).Floor();

        protected override float GetRotationOffset(Player player)
        {
            float baseRotation = 140f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Walking)
            {
                baseRotation = 30f;
            }

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                baseRotation = -40f;
            }

            return baseRotation;
        }

        protected override float GetScale(Item item) => 0.65f;
    }
}
