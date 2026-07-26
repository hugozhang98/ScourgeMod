using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ScourgeMod.Common.BackWeapons
{
    public class BackMaceDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            return BackWeaponHelper.GetDefaultVisibility(heldItem, player)
                && BackWeaponRegistry.IsBackMace(heldItem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            Texture2D texture = TextureAssets.Item[heldItem.type].Value;
            Rectangle frame = texture.Frame();

            SpriteEffects effects = GetEffects(player, heldItem);
            Vector2 origin = GetOrigin(frame, player, heldItem);
            Vector2 position = GetPosition(drawInfo.Center, player, heldItem);
            float rotation = GetRotation(player, heldItem);
            Color lightColor = GetColor(player);
            float scale = GetScale();

            DrawData drawData = new DrawData(
                texture,
                position.Floor(),
                frame,
                lightColor,
                rotation,
                origin,
                scale,
                effects,
                0
            );

            drawInfo.DrawDataCache.Add(drawData);
        }

        private SpriteEffects GetEffects(Player player, Item item)
        {
            SpriteEffects effects = SpriteEffects.FlipVertically;
            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;
            if (player.gravDir == -1f)
                effects &= ~SpriteEffects.FlipVertically;

            return effects;
        }

        private Vector2 GetOrigin(Rectangle frame, Player player, Item item) => frame.Size() * 0.5f;

        private Vector2 GetPosition(Vector2 basePosition, Player player, Item item)
        {
            //默认坐标（屏幕坐标）
            Vector2 position = basePosition - Main.screenPosition;
            //默认偏移
            position += new Vector2(player.direction * -8f, player.gravDir * 10f);

            if (new int[] { ItemID.ChainKnife, ItemID.ChainGuillotines }.Contains(item.type))
            {
                position += new Vector2(player.direction * -5f, player.gravDir * -5f);
            }

            if (new int[] { ItemID.Anchor, ItemID.KOCannon, ItemID.GolemFist }.Contains(item.type))
            {
                position += new Vector2(player.direction * 4f, player.gravDir * -15f);
            }

            return position.Floor();
        }

        private float GetRotation(Player player, Item item)
        {
            //默认旋转角度
            float baseRotation = AngleHelper.DegToRad(-player.direction * 55f);

            if (
                new int[] { ItemID.ChainKnife, ItemID.ChainGuillotines, ItemID.Anchor }.Contains(
                    item.type
                )
            )
            {
                baseRotation += AngleHelper.DegToRad(player.direction * 150f);
            }

            if (new int[] { ItemID.KOCannon, ItemID.GolemFist }.Contains(item.type))
            {
                baseRotation += AngleHelper.DegToRad(player.direction * 180f);
            }

            float moveSway = BackWeaponHelper.GetMoveSway(player, 7f);

            return (baseRotation + moveSway) * player.gravDir;
        }

        private Color GetColor(Player player) =>
            Lighting.GetColor(player.Center.ToTileCoordinates());

        private float GetScale() => 1f;
    }
}
