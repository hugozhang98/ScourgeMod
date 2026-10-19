using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class LanceDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Lance(item);

        protected override SpriteEffects GetEffects(Player player)
        {
            SpriteEffects effects = SpriteEffects.FlipHorizontally;

            if (player.direction == -1)
                effects ^= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

            return effects;
        }

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
                + new Vector2(visibleFrame.Width * 0f, visibleFrame.Height * 1f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            Vector2 position = new Vector2(10f, 27f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Walking)
            {
                position += new Vector2(-29f, -16f);
            }

            return position;
        }

        protected override float GetRotationOffset(Player player)
        {
            float baseRotation = 43f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Walking)
            {
                baseRotation = 120f;
            }

            return baseRotation;
        }

        protected override float GetWeaponSway(Player player) => 1f;

        protected override float GetScale(Item item) => 1.2f;
    }
}
