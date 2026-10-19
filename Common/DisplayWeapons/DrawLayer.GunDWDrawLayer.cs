using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GunDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Gun(item);

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
                + new Vector2(visibleFrame.Width * 0.33f, visibleFrame.Height * 0.3f);

            DWAdjust_Megashark.TryAdjustOrigin(item, ref origin);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override float GetRotationOffset(Player player) => 10f;

        protected override float GetWeaponSway(Player player) => 4f;

        protected override float GetScale(Item item) => 0.9f;
    }
}
