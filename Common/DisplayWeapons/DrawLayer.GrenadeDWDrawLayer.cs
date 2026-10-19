using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GrenadeDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() =>
            new BeforeParent(PlayerDrawLayers.ArmOverItem);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Grenade(item);

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
                + new Vector2(visibleFrame.Width * 0.5f, visibleFrame.Height * 0.9f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        protected override Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item) =>
            DWHelper.GetPlayerJointPosition(drawInfo, DWPlayerJointType.FrontHand).Floor();

        protected override float GetRotationOffset(Player player)
        {
            float baseRotation = 55f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                baseRotation = 0f;
            }

            return baseRotation;
        }

        protected override float GetWeaponSway(Player player) => 7f;

        protected override float GetScale(Item item)
        {
            float defaultScale = 1f;

            DWAdjust_FlyingKnife.TryAdjustScale(item, ref defaultScale);
            DWAdjust_RottenEgg.TryAdjustScale(item, ref defaultScale);

            return defaultScale;
        }
    }
}
