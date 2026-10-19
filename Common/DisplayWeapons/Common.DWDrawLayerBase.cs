using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public abstract class DWDrawLayerBase : PlayerDrawLayer
    {
        public sealed override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);

            return DWHelper.GetDefaultVisibility(player, item) && MatchWeapon(item);
        }

        protected sealed override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item item = DWHelper.GetDisplayItem(player);
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player);
            Vector2 origin = GetOrigin(texture, frame, effects, item, player);
            Vector2 position = GetPosition(drawInfo, player, item);
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

        public abstract bool MatchWeapon(Item item);

        protected virtual SpriteEffects GetEffects(Player player)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

            return effects;
        }

        protected virtual Vector2 GetOrigin(
            Texture2D texture,
            Rectangle frame,
            SpriteEffects effects,
            Item item,
            Player player
        ) => frame.Size() * 0.5f;

        protected virtual Vector2 GetPosition(PlayerDrawSet drawInfo, Player player, Item item)
        {
            Vector2 position = drawInfo.Center - Main.screenPosition;

            Vector2 offset = GetPositionOffset(drawInfo, player, item);

            position += new Vector2(player.direction * offset.X, player.gravDir * offset.Y);

            position += DWHelper.GetUpperBodyBobbing(drawInfo);

            return position.Floor();
        }

        protected virtual Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        ) => Vector2.Zero;

        protected virtual float GetRotation(Player player)
        {
            float baseRotation = AngleHelper.DegToRad(player.direction * GetRotationOffset(player));

            baseRotation += DWHelper.GetMoveSway(player, GetWeaponSway(player));

            baseRotation *= player.gravDir;

            return baseRotation;
        }

        protected virtual float GetRotationOffset(Player player) => 0f;

        protected virtual float GetWeaponSway(Player player) => 0f;

        protected virtual Color GetColor(PlayerDrawSet drawInfo) =>
            DWHelper.GetDefaultColor(drawInfo);

        protected virtual float GetScale(Item item) => 1f;
    }
}
