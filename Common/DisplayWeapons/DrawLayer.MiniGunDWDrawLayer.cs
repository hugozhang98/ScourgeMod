using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class MiniGunDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() =>
            new AfterParent(PlayerDrawLayers.WaistAcc); //rrzz 图层是否正确

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = player.HeldItem;

            return DWHelper.GetDefaultVisibility(player, item)
                && DWAdjust_Anchor.GetDefaultVisibility(player, item)
                && DWRegistry.IsDW_MiniGun(item);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player = drawInfo.drawPlayer;
            Item item = player.HeldItem;
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player);
            Vector2 origin = GetOrigin(texture, frame, effects, item);
            Vector2 position = GetPosition(drawInfo, player, item);
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

        private Vector2 GetOrigin(Texture2D texture, Rectangle frame, SpriteEffects effects, Item item)
        {
            Rectangle visibleFrame = TextureHelper.GetVisibleFrame(texture);

            if (visibleFrame == Rectangle.Empty)
                return frame.Size() * 0.5f;

            // 原点在完整贴图中的位置
            Vector2 origin =
                visibleFrame.Location.ToVector2() + new Vector2(visibleFrame.Width * 0.33f, visibleFrame.Height * 0.3f);

            return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        }

        private Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item)
        {
            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(player.direction * 15f, player.gravDir * -0f);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(player.direction * 5f);

            baseRotation += DWHelper.GetMoveSway(player, 4f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale()
        {
            float defaultScale = 0.8f;

            return defaultScale;
        }
    }
}
