using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GrenadeDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() =>
            new BeforeParent(PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);

            return DWHelper.GetDefaultVisibility(player, item)
                && DWRegistry.IsDW_Grenade(item)
                && DWAdjust_Class_Grenade.GetDefaultVisibility(player, item);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player);
            Vector2 origin = GetOrigin(texture, frame, effects, item);
            Vector2 position = GetPosition(drawInfo, player);
            float rotation = GetRotation(player);
            Color lightColor = GetColor(drawInfo);
            float scale = GetScale(item);

            DrawData drawData = new DrawData(
                texture,
                position,
                frame,
                lightColor,
                rotation,
                origin,
                scale,
                effects
            );

            drawInfo.DrawDataCache.Add(drawData);
        }

        private SpriteEffects GetEffects(Player player)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

            return effects;
        }

        private Vector2 GetOrigin(
            Texture2D texture,
            Rectangle frame,
            SpriteEffects effects,
            Item item
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

        private Vector2 GetPosition(PlayerDrawSet drawInfo, Player player)
        {
            return DWHelper.GetPlayerJointPosition(drawInfo, DWPlayerJointType.FrontHand).Floor();
        }

        private float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(player.direction * 55f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                baseRotation = AngleHelper.DegToRad(player.direction * 0f);
            }

            baseRotation += DWHelper.GetMoveSway(player, 7f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale(Item item)
        {
            float defaultScale = 1f;

            DWAdjust_FlyingKnife.TryAdjustScale(item, ref defaultScale);
            DWAdjust_RottenEgg.TryAdjustScale(item, ref defaultScale);

            return defaultScale;
        }
    }
}
