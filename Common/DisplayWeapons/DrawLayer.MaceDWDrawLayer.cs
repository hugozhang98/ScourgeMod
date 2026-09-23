using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class MaceDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin); //rrzz 图层是否正确

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = player.HeldItem;

            return DWHelper.GetDefaultVisibility(player, item) && DWRegistry.IsDW_Mace(item);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player = drawInfo.drawPlayer;
            Texture2D texture = TextureAssets.Item[player.HeldItem.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player);
            Vector2 origin = GetOrigin(frame);
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
            SpriteEffects effects = SpriteEffects.FlipVertically;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects &= ~SpriteEffects.FlipVertically;

            return effects;
        }

        private Vector2 GetOrigin(Rectangle frame)
        {
            return frame.Size() * 0.5f;
        }

        private Vector2 GetPosition(PlayerDrawSet drawInfo, Player player)
        {
            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(-player.direction * 8f, player.gravDir * 9f);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(-player.direction * 55f);

            baseRotation += DWHelper.GetMoveSway(player, 7f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale() => 1f;
    }
}
