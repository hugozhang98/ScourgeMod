using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class SpearDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Spear(item);

        protected override Vector2 GetOrigin(
            Texture2D texture,
            Rectangle frame,
            SpriteEffects effects,
            Item item,
            Player player
        )
        {
            if (DWHelper.GetVanillaAnimationType(player) != DWVanillaAnimationType.Idle)
            {
                return frame.Size() * 0.5f;
            }

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
            Vector2 position = new Vector2(14f, 0f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                position += new Vector2(0f, 27f);
            }

            DWAdjust_TheRottedFork.TryAdjustPosition(player, ref position);

            return position;
        }

        protected override float GetRotationOffset(Player player)
        {
            float baseRotation = 10f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                baseRotation = -45f;
            }

            return baseRotation;
        }

        protected override float GetWeaponSway(Player player) => 2f;

        protected override float GetScale(Item item) => 1.2f;
    }
}
