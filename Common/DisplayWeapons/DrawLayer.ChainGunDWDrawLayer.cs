using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class ChainGunDWDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() =>
            new BeforeParent(PlayerDrawLayers.MountBack); //rrzz 图层是否正确

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = player.HeldItem;

            return DWHelper.GetDefaultVisibility(player, item)
                && DWAdjust_Anchor.GetDefaultVisibility(player, item)
                && DWRegistry.IsDW_ChainGun(item);
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
            Vector2 origin = GetOrigin(frame);
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

        private Vector2 GetOrigin(Rectangle frame)
        {
            return frame.Size() * 0.5f;
        }

        private Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item)
        {
            Vector2 position = drawInfo.Center - Main.screenPosition;

            position += new Vector2(player.direction * -9f, player.gravDir * -3f);

            DWAdjust_Anchor.TryAdjustPosition(player, item, ref position);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        private float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(player.direction * 100f);

            baseRotation += DWHelper.GetMoveSway(player, 4f);

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        private Color GetColor(PlayerDrawSet drawInfo) => DWHelper.GetDefaultColor(drawInfo);

        private float GetScale()
        {
            float defaultScale = 0.9f;

            return defaultScale;
        }
    }
}
