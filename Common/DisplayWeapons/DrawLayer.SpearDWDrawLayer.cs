using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class SpearDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);

            return DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Spear(item);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player);
            Vector2 origin = GetOrigin(frame, player, effects, texture);
            Vector2 position = GetPosition(drawInfo, player);
            float rotation = GetRotation(player);
            Color lightColor = GetColor(drawInfo);
            float scale = GetScale();

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
            Rectangle frame,
            Player player,
            SpriteEffects effects,
            Texture2D texture
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

        private Vector2 GetPosition(PlayerDrawSet drawInfo, Player player)
        {
            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(player.direction * 14f, player.gravDir * 0f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                position += new Vector2(player.direction * 0f, player.gravDir * 27f);
            }

            DWAdjust_TheRottedFork.TryAdjustPosition(player, ref position);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(-player.direction * -10f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                baseRotation = AngleHelper.DegToRad(player.direction * -45f);
            }

            baseRotation += DWHelper.GetMoveSway(player, 2f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale() => 1.2f;
    }
}
