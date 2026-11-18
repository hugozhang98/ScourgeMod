using System;
using Microsoft.Xna.Framework;
using ScourgeMod.Content.Dusts;
using Terraria;
using Terraria.ID;
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

        //每帧调用，在指定圆周上生成向外渐隐的粒子，圆心和半径使用世界像素
        public static void SpawnSceneBoundaryDust(Vector2 center, float radius)
        {
            if (
                Main.netMode == NetmodeID.Server
                || radius <= 0f
                || Main.GameUpdateCount % SceneBoundaryDust.SpawnInterval != 0
            )
                return;

            const float dustSpacing = 16f;
            const float dustSpeed = 1.5f;
            const int screenPadding = 64;
            Rectangle screenBounds = new Rectangle(
                (int)Main.screenPosition.X - screenPadding,
                (int)Main.screenPosition.Y - screenPadding,
                Main.screenWidth + screenPadding * 2,
                Main.screenHeight + screenPadding * 2
            );

            int count = (int)Math.Ceiling(MathHelper.TwoPi * radius / dustSpacing);
            int dustType = ModContent.DustType<SceneBoundaryDust>();
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count;
                Vector2 outward = angle.ToRotationVector2();
                Vector2 position = center + outward * radius;
                if (!screenBounds.Contains(position.ToPoint()))
                    continue;

                Dust.NewDustPerfect(position, dustType, outward * dustSpeed, Scale: 1.5f);
            }
        }
    }
}
