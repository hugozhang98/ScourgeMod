using System;
using Microsoft.Xna.Framework;
using ScourgeMod.Content.Dusts;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ScourgeMod.Helper
{
    public static class BattleHelper
    {
        public static bool TryFindTarget(NPC npc, float maxDistance)
        {
            return TryFindTarget(npc, npc.Center, maxDistance);
        }

        //以指定的世界坐标为中心筛选玩家
        public static bool TryFindTarget(NPC npc, Vector2 searchCenter, float maxDistance)
        {
            float maxDistanceSquared = maxDistance * maxDistance;
            float bestScore = float.PositiveInfinity;
            int bestTarget = -1;
            int tankPetIndex = -1;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                //跳过无效或超出战斗范围的玩家
                Player player = Main.player[i];
                if (!PlayerHelper.IsActive(player))
                    continue;

                if (Vector2.DistanceSquared(searchCenter, player.Center) > maxDistanceSquared)
                    continue;

                //分数最低的玩家优先
                float score = GetPlayerTargetScore(npc, player);
                if (score < bestScore)
                {
                    bestTarget = i;
                    bestScore = score;
                    tankPetIndex = -1;
                }

                if (CanFaceTankPet(npc, player, bestScore))
                    tankPetIndex = player.tankPet;
            }

            if (bestTarget == -1)
                return false;

            ApplyTarget(npc, bestTarget, tankPetIndex);
            return true;
        }

        //按原版距离和玩家仇恨计算评分
        public static float GetPlayerTargetScore(NPC npc, Player player)
        {
            float score = GetVanillaTargetDistance(npc, player.Center) - player.aggro;
            if (player.npcTypeNoAggro[npc.type] && npc.direction != 0)
                score += 1000f;

            return score;
        }

        //检查能吸引NPC朝向的宠物，原版由星尘守卫触发
        public static bool CanFaceTankPet(NPC npc, Player player, float bestPlayerScore)
        {
            if (player.tankPet < 0 || player.npcTypeNoAggro[npc.type])
                return false;

            Vector2 tankPetCenter = Main.projectile[player.tankPet].Center;
            float tankPetScore = GetVanillaTargetDistance(npc, tankPetCenter) - 200f;
            return tankPetScore < bestPlayerScore
                && tankPetScore < 200f
                && Collision.CanHit(npc.Center, 1, 1, tankPetCenter, 1, 1);
        }

        public static void ApplyTarget(NPC npc, int targetIndex, int tankPetIndex)
        {
            Player target = Main.player[targetIndex];
            bool targetChanged = npc.target != targetIndex;
            int oldDirection = npc.direction;
            int oldDirectionY = npc.directionY;

            //用宠物的碰撞箱决定朝向，保留玩家目标
            npc.target = targetIndex;
            npc.targetRect =
                tankPetIndex >= 0 ? Main.projectile[tankPetIndex].Hitbox : target.Hitbox;

            //免仇恨且已有朝向时保持朝向
            bool hasNoAggro = target.npcTypeNoAggro[npc.type] && npc.direction != 0;

            //普通NPC已有玩家目标时，低仇恨且未用物品的目标不会让NPC转向
            bool keepFacingAtLowAggro =
                !npc.boss
                && npc.oldTarget >= 0
                && npc.oldTarget < Main.maxPlayers
                && target.itemAnimation == 0
                && target.aggro < 0;
            if (tankPetIndex >= 0 || (!hasNoAggro && !keepFacingAtLowAggro))
                npc.FaceTarget();

            //混乱时反转水平朝向
            if (npc.confused)
                npc.direction *= -1;

            //换目标或转向时同步
            if (targetChanged || npc.direction != oldDirection || npc.directionY != oldDirectionY)
                npc.netUpdate = true;
        }

        public static float GetVanillaTargetDistance(NPC npc, Vector2 candidateCenter)
        {
            //沿用原版TargetClosest的横纵距离算法
            return Math.Abs(candidateCenter.X - npc.position.X + npc.width / 2)
                + Math.Abs(candidateCenter.Y - npc.position.Y + npc.height / 2);
        }

        public static void Announce(ModNPC npc, string suffix, params object[] substitutions)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            NetworkText message = NetworkText.FromKey(
                npc.GetLocalizationKey(suffix),
                substitutions
            );
            ChatHelper.BroadcastChatMessage(message, new Color(175, 75, 255));
        }

        /// <summary>每帧调用，在指定圆周上生成向外缩小、渐隐的粉尘。圆心和半径使用世界像素。</summary>
        /// <remarks>
        /// 自定义粉尘沿用 OnSpawn、GetAlpha 和 PreDraw，由边界接管 Update。
        /// 完全自绘的 PreDraw 应使用 dust.scale 和 dust.alpha 来呈现逐层缩小和渐隐。
        /// </remarks>
        /// <param name="center">边界圆心的世界坐标。</param>
        /// <param name="radius">边界半径，单位为像素。</param>
        /// <param name="dustId">原版 DustID 或 ModContent.DustType&lt;T&gt;() 返回的粉尘类型。</param>
        /// <param name="initialScale">首层粉尘的缩放倍率。</param>
        /// <param name="initialOpacity">首层不透明度，0 为全透明，1 为完全不透明。</param>
        /// <param name="layerAngleOffsetDegrees">每个外层相对相邻内层的角度偏移，正值为顺时针。</param>
        /// <param name="outwardLayerCount">首层以外的飘出层数。设为 0 时只显示首层。</param>
        /// <param name="layerInterval">相邻两圈的生成间隔，单位为游戏刻，须大于 0。</param>
        /// <param name="dustSpacing">同一圈内相邻粉尘沿圆周的目标间距，单位为像素，须大于 0。</param>
        public static void SpawnSceneBoundaryDust(
            Vector2 center,
            float radius,
            int dustId,
            float initialScale = 1.5f,
            float initialOpacity = 1f,
            float layerAngleOffsetDegrees = 0f,
            int outwardLayerCount = SceneBoundaryDust.DefaultOutwardLayerCount,
            int layerInterval = SceneBoundaryDust.SpawnInterval,
            float dustSpacing = 20f
        )
        {
            if (
                Main.netMode == NetmodeID.Server
                || radius <= 0f
                || !float.IsFinite(radius)
                || dustId < 0
                || initialScale <= 0f
                || initialOpacity <= 0f
                || layerInterval <= 0
                || dustSpacing <= 0f
                || !float.IsFinite(dustSpacing)
                || Main.GameUpdateCount % (uint)layerInterval != 0
            )
                return;

            ModDust source = DustLoader.GetDust(dustId);
            if (dustId >= DustID.Count && source == null)
                return;

            const float dustSpeed = 1.5f;
            outwardLayerCount = Math.Max(outwardLayerCount, 0);
            float driftDistance = outwardLayerCount * (float)layerInterval * dustSpeed;
            //覆盖能飘入画面的粒子，原版粉尘生成范围最多延伸到屏外 400 像素。
            int screenPadding = (int)Math.Clamp(driftDistance + dustSpacing, 64f, 400f);
            Rectangle screenBounds = new Rectangle(
                (int)Main.screenPosition.X - screenPadding,
                (int)Main.screenPosition.Y - screenPadding,
                Main.screenWidth + screenPadding * 2,
                Main.screenHeight + screenPadding * 2
            );
            Vector2 halfViewSize = new Vector2(screenBounds.Width, screenBounds.Height) * 0.5f;
            float centerDistance = Vector2.Distance(center, screenBounds.TopLeft() + halfViewSize);
            if (MathF.Abs(centerDistance - radius) > halfViewSize.Length())
                return;

            int count = (int)
                Math.Min(
                    Math.Ceiling(MathHelper.TwoPi * (double)radius / dustSpacing),
                    Main.maxDust
                );
            //每批首层反向错开一个角度，已经向外飘的上一批便相对新首层正向错开。
            double batch = Main.GameUpdateCount / (uint)layerInterval;
            float phase =
                outwardLayerCount == 0
                    ? 0f
                    : MathHelper.ToRadians((float)(-batch * layerAngleOffsetDegrees % 360.0));
            var fade = new SceneBoundaryDust.FadeSettings(
                initialScale,
                Math.Min(initialOpacity, 1f),
                outwardLayerCount,
                layerInterval
            );
            for (int i = 0; i < count; i++)
            {
                float angle = phase + MathHelper.TwoPi * i / count;
                Vector2 outward = angle.ToRotationVector2();
                Vector2 position = center + outward * radius;
                if (!screenBounds.Contains(position.ToPoint()))
                    continue;

                SceneBoundaryDust.Spawn(position, outward * dustSpeed, dustId, source, fade);
            }
        }
    }
}
