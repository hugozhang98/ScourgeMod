using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GreatSwordDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            DWHelper.GetDefaultVisibility(drawInfo) && DWRegistry.IsDW_GreatSword(drawInfo);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            SpriteEffects effects = GetEffects(drawInfo);
            Vector2 origin = GetOrigin(drawInfo, effects);
            Vector2 position = GetPosition(drawInfo);
            float rotation = GetRotation(drawInfo);
            Color lightColor = GetColor(drawInfo);
            float scale = GetScale(drawInfo);

            DrawData drawData = new DrawData(
                texture,
                position.Floor(),
                frame,
                lightColor,
                rotation,
                origin,
                scale,
                effects
            );

            drawInfo.DrawDataCache.Add(drawData);
        }

        private SpriteEffects GetEffects(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            SpriteEffects effects = SpriteEffects.None;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

            return effects;
        }

        private Vector2 GetOrigin(PlayerDrawSet drawInfo, SpriteEffects effects)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            Rectangle visibleFrame = TextureHelper.GetVisibleFrame(texture);

            if (visibleFrame == Rectangle.Empty)
                return frame.Size() * 0.5f;

            // 原点在完整贴图中的位置
            Vector2 origin =
                visibleFrame.Location.ToVector2()
                + new Vector2(visibleFrame.Width * 0f, visibleFrame.Height * 1f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        private Vector2 GetPosition(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(player.direction * 18f, player.gravDir * 3f);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            float baseRotation = AngleHelper.DegToRad(-player.direction * 100f);

            baseRotation += DWHelper.GetMoveSway(player, 2f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale(PlayerDrawSet drawInfo)
        {
            float scale = 1f;

            return scale;
        }
    }
}
