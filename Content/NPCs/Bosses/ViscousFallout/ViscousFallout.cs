using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using ScourgeMod.Helper;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ScourgeMod.Content.NPCs.Bosses.ViscousFallout
{
    [AutoloadBossHead]
    public class ViscousFallout : ModNPC
    {
        #region 常量
        private const string MainTexture = "ScourgeMod/Assets/NPC_ViscousFallout";

        private const string CrownTexture = "ScourgeMod/Assets/Extra_ViscousFallout_Crown";

        //基础碰撞箱宽度
        private const int BaseWidth = 98;

        //基础碰撞箱高度
        private const int BaseHeight = 92;

        //行为-退出
        private const int Exiting = -1;

        //行为-等待
        private const int Waiting = 0;

        //行为-跳跃
        private const int Jumping = 1;

        //行为-闪现
        private const int Blinking = 2;

        //行为-喷发
        private const int Erupting = 3;

        //闪现前摇阶段
        private const int BlinkWindUpPhase = 0;

        //闪现阶段
        private const int BlinkPhase = 1;

        //闪现后摇阶段
        private const int BlinkRecoveryPhase = 3;

        //静默帧数
        private const int SilentFrameCount = 10;

        //闪现前摇帧数
        private const int BlinkWindUpFrameCount = 60;

        //闪现前摇期间锁敌帧数
        private const int BlinkFindTargetFrameCount = BlinkWindUpFrameCount - 20;

        //闪现后摇帧数
        private const int BlinkRecoveryFrameCount = 30;

        //闪现动画最小缩放
        private const float BlinkAnimationMinScale = 0.5f;

        //闪现动画最小透明度
        private const float BlinkAnimationMinOpacity = 0.5f;

        //Y轴阻塞间距
        private const float YTroubleSpacing = 160f;

        private const float XTroubleSpacing = 1500f;

        private const float XTroubleSpacingII = 1000f;

        private const float XTroubleSpacingIII = 500f;

        private const float XTroubleSpacingIV = 200f;

        //脱战距离
        private const float ExitDistance = 1500f;

        //脱战帧数
        private const int ExitFrameCount = 60;

        private const int XFarBoundary = 250;
        private const int YFarBoundary = 100;
        private const int XNearBoundary = 100;

        #endregion

        #region 计算属性
        //是否单机或服务器权威
        private bool IsAuthority => Main.netMode != NetmodeID.MultiplayerClient;

        //血量基础缩放：满血1.25，75%血1，半血及以下0.75
        private float HealthScale =>
            MathHelper.Clamp((float)NPC.life / NPC.lifeMax + 0.25f, 0.75f, 1.25f);

        //是否找到有效闪现位置
        private bool IsFoundBlinkPosition => BlinkBottom != Vector2.Zero;
        #endregion

        #region 字段
        //全场计时器
        private int GlobalTimer { get; set; }

        //当前行为
        private int Action
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        //当前阶段
        private int Phase
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        //当前行为计时器
        private int ActionTimer
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        //当前阶段计时器
        private int PhaseTimer
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        //阻塞计数器
        private int BlockCounter { get; set; }

        //严重阻塞计数器
        private int SevereBlockCounter { get; set; }

        //闪现位置（底部中心）
        private Vector2 BlinkBottom { get; set; }

        //闪现前摇音效槽位
        private SlotId BlinkWindupSoundSlot { get; set; } = SlotId.Invalid;

        private Vector2 SceneCenter { get; set; }
        #endregion

        public override string Texture => MainTexture;

        public override void SetStaticDefaults()
        {
            //使用这张贴图原有的动画帧数，确保正确切分图片
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.KingSlime];
        }

        public override void SetDefaults()
        {
            //碰撞箱大小
            NPC.width = BaseWidth;
            NPC.height = BaseHeight;

            //基础数值
            //最大生命值
            NPC.lifeMax = 4500;
            //伤害值
            NPC.damage = 50;
            //防御值
            NPC.defense = 15;
            //击退抵抗值（0～1，0为免疫，1为完全承受）
            NPC.knockBackResist = 0f;
            //是否不受重力影响
            NPC.noGravity = false;
            //是否不受地形碰撞影响
            NPC.noTileCollide = false;

            //身份信息
            //是否为boss
            NPC.boss = true;
            //占用的npc槽位数
            NPC.npcSlots = 5f;

            //预设AI行为（-1表示不使用）
            NPC.aiStyle = -1;

            //帧动画配置
            AnimationType = NPCID.KingSlime;

            //音效
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            //音乐
            Music = MusicID.Boss1;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawCrown(spriteBatch, screenPos, drawColor);
        }

        public override void OnSpawn(IEntitySource source)
        {
            //原版 Boss 召唤入口会自行发出苏醒公告。
            if (source is not EntitySource_BossSpawn)
            {
                BattleHelper.Announce(this, $"SpawnAnnouncement{Main.rand.Next(1, 5)}");
            }
        }

        public override void AI()
        {
            Init();

            ActionTimer++;
            PhaseTimer++;

            AdjustSync();

            if (!IsAuthority)
                return;

            GlobalTimer++;

            if (IsSilent())
                return;

            if (Action != Exiting && !BattleHelper.TryFindTarget(NPC, SceneCenter, ExitDistance))
            {
                SetAction(Exiting);
            }

            switch (Action)
            {
                case Exiting:
                    OnExitAction();
                    break;
                case Waiting:
                    OnWaitAction();
                    break;
                case Jumping:
                    OnJumpAction();
                    break;
                case Blinking:
                    OnBlinkAction();
                    break;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return Action != Exiting;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SceneCenter);
            writer.WriteVector2(BlinkBottom);
            writer.Write(NPC.width);
            writer.Write(NPC.height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SceneCenter = reader.ReadVector2();
            BlinkBottom = reader.ReadVector2();
            NPC.width = reader.ReadInt32();
            NPC.height = reader.ReadInt32();
        }

        private void SetAction(int action)
        {
            Action = action;
            Phase = 0;
            ActionTimer = 0;
            PhaseTimer = 0;
            BlinkBottom = Vector2.Zero;
            NPC.netUpdate = true;
        }

        private void SetPhase(int phase)
        {
            Phase = phase;
            PhaseTimer = 0;
            NPC.netUpdate = true;
        }

        private void DrawCrown(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //王冠使用模组独立贴图，绘制在身体前面
            Texture2D crownTexture = ModContent.Request<Texture2D>(CrownTexture).Value;

            //根据身体当前帧，调整王冠高度，使它跟随身体伸缩
            int frameIndex = NPC.frame.Height > 0 ? NPC.frame.Y / NPC.frame.Height : 0;
            float crownOffset = frameIndex switch
            {
                0 => 2f,
                1 => -6f,
                2 => 2f,
                3 => 10f,
                4 => 2f,
                5 => 0f,
                _ => 0f,
            };

            Vector2 crownPosition = NPC.Center;
            crownPosition.Y += NPC.gfxOffY - (70f - crownOffset) * NPC.scale;

            //王冠向画面左侧移动24像素、下移8像素，并向左倾斜20度
            crownPosition += new Vector2(-24f, 8f) * NPC.scale;
            float crownRotation = MathHelper.ToRadians(-10f);
            SpriteEffects effects =
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //与原版一致：位置随身体缩放调整，王冠本身保持固定大小
            spriteBatch.Draw(
                crownTexture,
                crownPosition - screenPos,
                null,
                Color.White,
                crownRotation,
                crownTexture.Size() / 2f,
                1f,
                effects,
                0f
            );
        }

        private int GetHitBoxDistanceY()
        {
            Player target = Main.player[NPC.target];

            Rectangle bossHitbox = NPC.Hitbox;
            Rectangle playerHitbox = target.Hitbox;

            return Math.Max(
                0,
                Math.Max(bossHitbox.Top - playerHitbox.Bottom, playerHitbox.Top - bossHitbox.Bottom)
            );
        }

        private int GetHitBoxDistanceX()
        {
            Player target = Main.player[NPC.target];

            Rectangle bossHitbox = NPC.Hitbox;
            Rectangle playerHitbox = target.Hitbox;

            return Math.Max(
                0,
                Math.Max(bossHitbox.Left - playerHitbox.Right, playerHitbox.Left - bossHitbox.Right)
            );
        }

        private bool IsSilent()
        {
            return GlobalTimer <= SilentFrameCount;
        }

        private bool IsYFar()
        {
            return GetHitBoxDistanceY() > YTroubleSpacing;
        }

        private bool IsFar(float spacing)
        {
            Player target = Main.player[NPC.target];

            return GetHitBoxDistanceX() > spacing;
        }

        private bool TryOutTrouble(int val = 1)
        {
            int BlockCountBoundary = 60;
            int SevereBlockInterval = 10;

            int previewVal = BlockCounter;

            BlockCounter += val;

            int previewSevereLevel = previewVal / SevereBlockInterval;
            int currentSevereLevel = BlockCounter / SevereBlockInterval;

            if (
                currentSevereLevel > previewSevereLevel
                && TryOutSevereTrouble(currentSevereLevel - previewSevereLevel)
            )
            {
                return true;
            }

            int previousLevel = previewVal / BlockCountBoundary;
            int currentLevel = BlockCounter / BlockCountBoundary;

            if (currentLevel > previousLevel)
            {
                SetAction(Blinking);
                return true;
            }

            return false;
        }

        private bool TryOutSevereTrouble(int val = 1)
        {
            int SevereBlockCountBoundary = 60;

            int previewVal = SevereBlockCounter;

            SevereBlockCounter += val;

            int previewSevereLevel = previewVal / SevereBlockCountBoundary;
            int currentSevereLevel = SevereBlockCounter / SevereBlockCountBoundary;

            bool isSevereBlock = currentSevereLevel > previewSevereLevel;

            if (isSevereBlock)
            {
                //rrzz 后续作严重阻塞逻辑
            }

            return isSevereBlock;
        }

        private bool TryStop()
        {
            //判断是否接触地面
            //用水平速度归零来间接判断
            if (NPC.velocity.Y != 0f)
                return false;

            //减速阈值
            float decelerationBoundary = 0.1f;
            //摩擦系数
            float frictionFactor = 0.8f;

            //减速
            NPC.velocity.X *= frictionFactor;

            //如果小于减速阈值，直接停止
            if (Math.Abs(NPC.velocity.X) <= decelerationBoundary)
            {
                NPC.velocity.X = 0f;
            }

            return NPC.velocity.X == 0f;
        }

        private bool IsStop()
        {
            return NPC.velocity.X == 0f && NPC.velocity.Y == 0f;
        }

        private void Init()
        {
            if (GlobalTimer == 0)
                //rrzz 临时写死
                SceneCenter = new Vector2(51280, 7296);
        }

        private void AdjustSync()
        {
            if (NPC.velocity.Y == 0f)
            {
                TryStop();
            }
            else
            {
                TryKeepXVelocity();
            }

            if (Action == Blinking)
            {
                if (Phase == BlinkWindUpPhase)
                {
                    BlinkWindupAnimation();
                }
                else if (Phase == BlinkPhase)
                {
                    KeepBlinkAnimation();
                }
                else if (Phase == BlinkRecoveryPhase)
                {
                    ClearBlinkNetOffset();
                    BlinkRecoveryAnimation();
                }
            }
            else if (Action == Exiting)
            {
                NPC.dontTakeDamage = true;

                if (ActionTimer < ExitFrameCount)
                {
                    ExitAnimation();
                }
            }

            if (Action != Blinking)
                ReStoreBlinkAnimation();

            if (Action != Exiting)
                BattleHelper.SpawnSceneBoundaryDust(
                    center: SceneCenter,
                    radius: ExitDistance,
                    dustId: DustID.Blood,
                    initialScale: 1.5f,
                    initialOpacity: 1f,
                    layerAngleOffsetDegrees: 0f,
                    outwardLayerCount: 5,
                    layerInterval: 15,
                    dustSpacing: 25f
                );
        }

        private void AdjustScale(float animationScale = 1f)
        {
            Vector2 bottom = NPC.Bottom;
            float scale = HealthScale * animationScale;

            NPC.scale = scale;
            NPC.width = (int)(BaseWidth * scale);
            NPC.height = (int)(BaseHeight * scale);

            //保持脚底位置不变
            NPC.Bottom = bottom;
        }

        private void ClearBlinkNetOffset()
        {
            NPC.netOffset = Vector2.Zero;
        }

        private void OnWaitAction()
        {
            int troubleBoundary = 600;

            if (!IsStop())
            {
                if (ActionTimer >= troubleBoundary)
                {
                    TryOutTrouble();
                }

                return;
            }

            int farTroubleValue = 0;

            if (IsFar(XTroubleSpacing))
            {
                farTroubleValue = 60;
            }
            else if (IsFar(XTroubleSpacingII))
            {
                farTroubleValue = 40;
            }
            else if (IsFar(XTroubleSpacingIII))
            {
                farTroubleValue = 25;
            }
            else if (IsFar(XTroubleSpacingIV))
            {
                farTroubleValue = 15;
            }

            if (farTroubleValue > 0 && TryOutTrouble(farTroubleValue))
            {
                return;
            }

            if (IsYFar() && TryOutTrouble(20))
            {
                return;
            }

            SetAction(Jumping);
        }

        private void TryKeepXVelocity()
        {
            if (NPC.velocity.Y == 0f)
            {
                return;
            }

            const float AirSpeed = 3f;
            const float AirAcceleration = 0.2f;

            //朝当前方向的横速不足时，才进行调整
            bool needsAcceleration =
                (NPC.direction == 1 && NPC.velocity.X < AirSpeed)
                || (NPC.direction == -1 && NPC.velocity.X > -AirSpeed);

            if (!needsAcceleration)
                return;

            //已经同向移动，或者基本静止
            bool movingForwardOrNearlyStopped =
                (NPC.direction == -1 && NPC.velocity.X < 0.1f)
                || (NPC.direction == 1 && NPC.velocity.X > -0.1f);

            if (movingForwardOrNearlyStopped)
            {
                NPC.velocity.X += AirAcceleration * NPC.direction;
            }
            else
            {
                //正在反向移动，先逐渐减速
                NPC.velocity.X *= 0.93f;
            }
        }

        private Vector2 GetJumpVector()
        {
            /*
                【普通跳】

                起跳速度   X 5   Y 8（px/tick，Y 向上）
                跳跃结果 · 空气
                已落回起跳高度   总计 128 tick（2.13 秒）
                前摇 60 帧   腾空 53 帧   后摇 15 帧

                速度统计                px/tick     px/s   格/s (约)
                ---------------------   -------   ------   ---------
                腾空平均 X 速度           5.000   300.00       18.75
                含前后摇的平均 X 速度     2.070   124.22        7.76

                距离统计         px   格 (约)
                ----------   ------   -------
                X 最远距离   265.00     16.56
                Y 上升高度   102.70      6.42
            */
            Vector2 jumpVector = new Vector2(5, 8);

            /*
                【平跳】

                起跳速度   X 12   Y 5（px/tick，Y 向上）
                跳跃结果 · 空气
                已落回起跳高度   总计 108 tick（1.80 秒）
                前摇 60 帧   腾空 33 帧   后摇 15 帧

                速度统计                px/tick     px/s   格/s (约)
                ---------------------   -------   ------   ---------
                腾空平均 X 速度          12.000   720.00       45.00
                含前后摇的平均 X 速度     3.667   220.00       13.75

                距离统计         px   格 (约)
                ----------   ------   -------
                X 最远距离   396.00     24.75
                Y 上升高度    39.20      2.45
            */
            Vector2 jumpVectorII = new Vector2(12f, 5);

            /*
                【高跳】

                起跳速度   X 3   Y 10（px/tick，Y 向上）
                跳跃结果 · 空气
                已落回起跳高度   总计 141 tick（2.35 秒）
                前摇 60 帧   腾空 66 帧   后摇 15 帧

                速度统计                px/tick     px/s   格/s (约)
                ---------------------   -------   ------   ---------
                腾空平均 X 速度           3.000   180.00       11.25
                含前后摇的平均 X 速度     1.404    84.26        5.27

                距离统计         px   格 (约)
                ----------   ------   -------
                X 最远距离   198.00     12.38
                Y 上升高度   161.70     10.11
            */
            Vector2 jumpVectorIII = new Vector2(3, 10);

            /*
                【小跳】

                起跳速度   X 4.3   Y 5（px/tick，Y 向上）
                跳跃结果 · 空气
                已落回起跳高度   总计 108 tick（1.80 秒）
                前摇 60 帧   腾空 33 帧   后摇 15 帧

                速度统计                px/tick     px/s   格/s (约)
                ---------------------   -------   ------   ---------
                腾空平均 X 速度           4.300   258.00       16.12
                含前后摇的平均 X 速度     1.314    78.83        4.93

                距离统计         px   格 (约)
                ----------   ------   -------
                X 最远距离   141.90      8.87
                Y 上升高度    39.20      2.45
            */
            Vector2 jumpVectorIV = new Vector2(4.3f, 5);

            float jumpWeight = 3;
            float jumpWeightII = 1;
            float jumpWeightIII = 1;
            float jumpWeightIV = 1;

            Player target = Main.player[NPC.target];

            //每个轴取两碰撞箱相邻边的间距，接触或重叠时为 0
            int xDistance = GetHitBoxDistanceX();
            int yDistance = GetHitBoxDistanceY();

            float distance = Vector2.Distance(target.Center, NPC.Center);

            bool isLongJump = xDistance > XFarBoundary;
            bool isHighJump = yDistance > YFarBoundary;
            bool isSmallJump = distance < XNearBoundary && !isLongJump && !isHighJump;

            //远距离特化平跳，并排除小跳
            if (isLongJump)
            {
                jumpWeight = 1f;
                jumpWeightII = 9f;
                jumpWeightIII = 0f;
                jumpWeightIV = 0f;
            }

            //高处特化高跳，可与远距离同时触发
            if (isHighJump)
            {
                jumpWeight = 1f;
                jumpWeightII = 0f;
                jumpWeightIII = 9f;
                jumpWeightIV = 0f;
            }

            //近距离特化小跳，并排除平跳和高跳
            if (isSmallJump)
            {
                jumpWeight = 2f;
                jumpWeightII = 0f;
                jumpWeightIII = 1f;
                jumpWeightIV = 5f;
            }

            WeightedRandom<Vector2> jumpRandom = new WeightedRandom<Vector2>(Main.rand);
            jumpRandom.Add(jumpVector, jumpWeight);
            jumpRandom.Add(jumpVectorII, jumpWeightII);
            jumpRandom.Add(jumpVectorIII, jumpWeightIII);
            jumpRandom.Add(jumpVectorIV, jumpWeightIV);

            return jumpRandom.Get();
        }

        private void Jump(Vector2 jumpVector)
        {
            NPC.velocity.X = jumpVector.X * NPC.direction;
            NPC.velocity.Y = -jumpVector.Y;
            NPC.netUpdate = true;
        }

        private void OnJumpAction()
        {
            //持续监测是否停止移动
            if (!IsStop())
            {
                if (!TryOutTrouble())
                {
                    SetAction(Waiting);
                }

                return;
            }

            int windUpPhase = 0;
            int jumpPhase = 1;

            //前摇帧数
            int windUpFrameCount = 60;

            //前摇
            if (Phase == windUpPhase)
            {
                if (PhaseTimer >= windUpFrameCount)
                {
                    SetPhase(jumpPhase);
                }
                return;
            }

            Jump(GetJumpVector());
            SetAction(Waiting);
        }

        private bool TryFindBlinkPosition(Vector2 targetBottom, out Vector2 blinkBottom)
        {
            //所有返回路径都需要给 out 参数赋值
            blinkBottom = Vector2.Zero;

            const float BlinkLiftHeight = TextureHelper.TileSize * 2f; // 向上抬高2格

            Vector2 baseBottom = targetBottom - new Vector2(0f, BlinkLiftHeight);

            //依次尝试这些偏移，单位为像素
            Vector2[] offsets =
            {
                new Vector2(0f, 0f),
                new Vector2(-160f, 0f),
                new Vector2(160f, 0f),
                new Vector2(-240f, 0f),
                new Vector2(240f, 0f),
                new Vector2(-360f, 0f),
                new Vector2(360f, 0f),
                new Vector2(-480f, 0f),
                new Vector2(480f, 0f),
            };

            foreach (Vector2 offset in offsets)
            {
                Vector2 candidateBottom = baseBottom + offset;

                if (!IsValidBlinkPosition(candidateBottom))
                    continue;

                //找到合法位置，交给调用方
                blinkBottom = candidateBottom;
                return true;
            }

            //所有候选都无效
            return false;
        }

        private bool IsValidBlinkPosition(Vector2 bottom)
        {
            //按恢复后的完整体型检查，避免缩小后进入狭窄位置
            float scale = HealthScale;
            int width = (int)(BaseWidth * scale);
            int height = (int)(BaseHeight * scale);
            Vector2 topLeft = bottom - new Vector2(width / 2f, height);

            if (!PositionHelper.InsideWorld(topLeft, width, height))
                return false;

            return !Collision.SolidCollision(topLeft, width, height);
        }

        private void Blink(Vector2 bottom)
        {
            //传入的位置是经过校验的底部中心
            NPC.Bottom = bottom;
            //清除原有运动，让 Boss 从新位置快速下落
            NPC.velocity = Vector2.Zero;
            NPC.netUpdate = true;
        }

        private void SpawnBloodDust(
            Vector2 center,
            float radius,
            float direction = 1f,
            int count = 10
        )
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < count; i++)
            {
                // 两个半径相同，生成范围就是圆形
                Vector2 offset = Main.rand.NextVector2Circular(radius, radius);
                Vector2 spawnPosition = center + offset;

                // 按指定坐标生成，避免再次随机偏移
                Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.Blood);

                dust.noGravity = true;
                dust.scale = 1.6f;

                dust.velocity = direction * offset.SafeNormalize(Vector2.Zero) * 2f;
            }
        }

        private void PlayBlinkWindupSound()
        {
            int blinkWindUpSoundFrameCount = Math.Min(BlinkWindUpFrameCount, 40);

            BlinkWindupSoundSlot = SoundHelper.PlayLooped(
                BlinkWindupSoundSlot,
                SoundID.Mummy with
                {
                    Volume = 0.6f,
                    Identifier = $"ViscousFalloutBlinkWindup_{NPC.whoAmI}",
                    PauseBehavior = PauseBehavior.PauseWithGame,
                },
                () => NPC.Center,
                () =>
                    NPC.active
                    && Action == Blinking
                    && Phase == BlinkWindUpPhase
                    && PhaseTimer < blinkWindUpSoundFrameCount
            );
        }

        private void PlayBlinkSound()
        {
            SoundHelper.PlayOnce(
                SoundID.NPCDeath27 with
                {
                    Volume = 0.6f,
                    Identifier = $"ViscousFalloutBlink_{NPC.whoAmI}",
                    PauseBehavior = PauseBehavior.PauseWithGame,
                },
                () => NPC.Center,
                () => NPC.active
            );
        }

        private void BlinkWindupAnimation()
        {
            float progress = MathHelper.Clamp((float)PhaseTimer / BlinkWindUpFrameCount, 0f, 1f);

            AdjustScale(MathHelper.Lerp(1f, BlinkAnimationMinScale, progress));
            NPC.Opacity = MathHelper.Lerp(1f, BlinkAnimationMinOpacity, progress);

            if (progress < 1f)
            {
                Vector2 center = NPC.Center + new Vector2(0f, 15f);
                float radius = 100f * NPC.scale;

                SpawnBloodDust(center, radius, -1);

                if (IsFoundBlinkPosition && PhaseTimer > BlinkFindTargetFrameCount)
                {
                    SpawnBloodDust(BlinkBottom, 50f, 1, 3);
                }

                PlayBlinkWindupSound();
            }
        }

        private void BlinkRecoveryAnimation()
        {
            float progress = MathHelper.Clamp((float)PhaseTimer / BlinkRecoveryFrameCount, 0f, 1f);

            AdjustScale(MathHelper.Lerp(BlinkAnimationMinScale, 1f, progress));
            NPC.Opacity = MathHelper.Lerp(BlinkAnimationMinOpacity, 1f, progress);

            if (progress < 1f)
            {
                Vector2 center = NPC.Center + new Vector2(0f, 15f);
                float radius = 100f * NPC.scale;

                SpawnBloodDust(center, radius, 1);
            }
        }

        private void KeepBlinkAnimation()
        {
            AdjustScale(BlinkAnimationMinScale);
            NPC.Opacity = BlinkAnimationMinOpacity;

            if (PhaseTimer == 1)
            {
                PlayBlinkSound();
            }
        }

        private void ReStoreBlinkAnimation()
        {
            AdjustScale();
            NPC.Opacity = 1f;
        }

        private void OnBlinkAction()
        {
            //前摇
            if (Phase == BlinkWindUpPhase)
            {
                if (PhaseTimer <= BlinkFindTargetFrameCount)
                {
                    Player target = Main.player[NPC.target];

                    if (TryFindBlinkPosition(target.Bottom, out Vector2 targetBottom))
                    {
                        BlinkBottom = targetBottom;
                        NPC.netUpdate = true;
                    }
                }

                if (PhaseTimer >= BlinkWindUpFrameCount)
                {
                    SetPhase(BlinkPhase);
                }
                return;
            }

            //后摇
            if (Phase == BlinkRecoveryPhase)
            {
                if (PhaseTimer >= BlinkRecoveryFrameCount)
                {
                    SetAction(Waiting);
                }
                return;
            }

            //释放前再次检查，防止锁定后地形发生变化
            if (!IsFoundBlinkPosition || !IsValidBlinkPosition(BlinkBottom))
            {
                if (!TryOutSevereTrouble())
                {
                    SetPhase(BlinkRecoveryPhase);
                }
                return;
            }

            Blink(BlinkBottom);
            SetPhase(BlinkRecoveryPhase);
        }

        private void ExitAnimation()
        {
            float progress = MathHelper.Clamp((float)PhaseTimer / ExitFrameCount, 0f, 1f);

            AdjustScale(MathHelper.Lerp(1f, BlinkAnimationMinScale, progress));
            NPC.Opacity = MathHelper.Lerp(1f, BlinkAnimationMinOpacity, progress);

            if (progress < 1f)
            {
                Vector2 center = NPC.Center + new Vector2(0f, 15f);
                float radius = 100f * NPC.scale;

                SpawnBloodDust(center, radius, -1);
            }
        }

        private void OnExitAction()
        {
            if (!NPC.active || ActionTimer < ExitFrameCount)
                return;

            NPC.active = false;
            NPC.netUpdate = true;
            BattleHelper.Announce(this, "DespawnAnnouncement", DisplayName.ToNetworkText());
        }
    }
}
