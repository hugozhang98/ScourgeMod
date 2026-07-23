using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScourgeMod.Helper;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ScourgeMod.Common.BackWeapons
{
    public class BackSwordDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            return BackWeaponHelper.GetDefaultVisibility(heldItem, player)
                && BackWeaponRegistry.IsBackSword(heldItem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            Texture2D texture = TextureAssets.Item[heldItem.type].Value;
            Rectangle frame = texture.Frame();

            //视觉翻转
            SpriteEffects effects = SpriteEffects.None;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

            //以贴图中心作为旋转中心
            Vector2 origin = frame.Size() * 0.5f;

            //玩家当前的屏幕绘制中心
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position += new Vector2(0f, -3f);

            // 背剑倾斜角度base
            float baseRotation = AngleHelper.DegToRad(180f - player.direction * 10f);
            // 根据水平速度计算摆动强度：静止为 0，速度> 6 时为 1
            float moveFactor = MathHelper.Clamp(MathF.Abs(player.velocity.X) / 6f, 0f, 1f);
            // 每约 60 帧完成一次摆动，最大摆动 6 度
            float sway =
                ProcessHelper.Swing((float)Main.GameUpdateCount, 60f)
                * AngleHelper.DegToRad(6f)
                * moveFactor;
            float rotation = baseRotation + sway * player.direction;

            //颜色
            Color lightColor = Color.White;
            //    Lighting.GetColor(
            //    player.Center.ToTileCoordinates()
            //);

            //缩放
            float scale = 1f;

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
    }
}
