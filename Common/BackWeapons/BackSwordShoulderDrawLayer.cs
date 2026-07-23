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
    public class BackSwordShoulderDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            return BackWeaponHelper.GetDefaultVisibility(heldItem, player)
                && BackWeaponRegistry.IsBackSword_Shoulder(heldItem);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player = drawInfo.drawPlayer;
            Item heldItem = player.HeldItem;

            Texture2D texture = TextureAssets.Item[heldItem.type].Value;
            Rectangle frame = texture.Frame();

            //绘制中心
            Vector2 origin =
                frame.Size() * (player.direction == 1 ? new Vector2(0, 1f) : new Vector2(1, 1f));

            //位置坐标（屏幕坐标）
            Vector2 position = drawInfo.Center - Main.screenPosition;
            //添加偏移
            position += new Vector2(player.direction * 18f, 4f);

            //旋转角度
            float baseRotation = AngleHelper.DegToRad(0 - player.direction * 90);
            // 根据水平速度计算摆动强度：静止为 0，速度> 6 时为 1
            float moveFactor = MathHelper.Clamp(MathF.Abs(player.velocity.X) / 6f, 0f, 1f);
            // 每约 60 帧完成一次摆动，最大摆动 6 度
            float sway =
                ProcessHelper.Swing((float)Main.GameUpdateCount, 60f)
                * AngleHelper.DegToRad(6f)
                * moveFactor;
            float rotation = baseRotation + sway * player.direction;

            // 获取玩家所在位置的环境光照
            Color lightColor = Color.White;
            //    Lighting.GetColor(
            //    player.Center.ToTileCoordinates()
            //);

            float scale = 1f;

            SpriteEffects effects = SpriteEffects.None;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects |= SpriteEffects.FlipVertically;

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
