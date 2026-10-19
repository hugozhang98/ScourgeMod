using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class ChainKnifeDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Torso);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_ChainKnife(item);

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
                + new Vector2(visibleFrame.Width * 0.5f, visibleFrame.Height * 1f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            Vector2 offset = new Vector2(0f, 18f);

            DWAdjust_ChainGuillotines.TryAdjustPosition(player, item, ref offset);

            return offset;
        }

        protected override float GetWeaponSway(Player player) => 3f;

        protected override float GetScale(Item item)
        {
            float defaultScale = 1f;

            DWAdjust_ChainGuillotines.TryAdjustScale(item, ref defaultScale);

            return defaultScale;
        }
    }
}
