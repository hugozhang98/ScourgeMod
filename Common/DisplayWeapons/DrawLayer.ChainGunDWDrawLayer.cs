using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class ChainGunDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() =>
            new BeforeParent(PlayerDrawLayers.MountBack); //rrzz 图层是否正确

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            DWHelper.GetDefaultVisibility(drawInfo)
            && DWAdjust_Anchor.TryGetDefaultVisibility(drawInfo)
            && DWRegistry.IsDW_ChainGun(drawInfo);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            SpriteEffects effects = GetEffects(drawInfo);
            Vector2 origin = GetOrigin(drawInfo);
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

        private Vector2 GetOrigin(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            return frame.Size() * 0.5f;
        }

        private Vector2 GetPosition(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(player.direction * -9f, player.gravDir * -3f);

            DWAdjust_Anchor.TryAdjustPosition(drawInfo, ref position);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(PlayerDrawSet drawInfo)
        {
            var (player, item, texture, frame) = DWHelper.GetHeldItemDrawData(drawInfo);

            float baseRotation = AngleHelper.DegToRad(player.direction * 100f);

            baseRotation += DWHelper.GetMoveSway(player, 4f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale(PlayerDrawSet drawInfo)
        {
            float defaultScale = 0.9f;

            return defaultScale;
        }
    }
}
