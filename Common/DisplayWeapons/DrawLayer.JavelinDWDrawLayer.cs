using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class JavelinDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() =>
            new BeforeParent(PlayerDrawLayers.ArmOverItem);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Javelin(item);

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
                + new Vector2(visibleFrame.Width * 0.4f, visibleFrame.Height * 0.6f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item) =>
            DWHelper.GetPlayerJointPosition(drawInfo, DWPlayerJointType.FrontHand).Floor();

        protected override float GetRotationOffset(Player player)
        {
            float rotation = 30f;
            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                rotation = -37f;
            }
            return rotation;
        }

        protected override float GetWeaponSway(Player player) => 2f;

        protected override float GetScale(Item item)
        {
            float scale = 1f;
            DWAdjust_MolotovCocktail.TryAdjustScale(item, ref scale);
            return scale;
        }
    }
}
