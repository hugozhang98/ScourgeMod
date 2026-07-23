using System;
using Microsoft.Xna.Framework;

namespace ScourgeMod.Helper
{
    /// <summary>
    /// 常用缓动函数工具类。
    /// 输入 t 通常是 0 到 1 的动画进度，返回值是“处理后的进度”。
    /// 用法通常是：MathHelper.Lerp(起点, 终点, EaseUtil.OutCubic(t))
    /// </summary>
    public static class EaseHelper
    {
        /// <summary>
        /// 线性：匀速，没有加速或减速。
        /// 感觉：机械、稳定、普通。
        /// 适合：持续移动、简单计时、激光匀速移动。
        /// 进度示意：0 -> 0.25 -> 0.5 -> 0.75 -> 1
        /// </summary>
        public static float Linear(float t)
        {
            return MathHelper.Clamp(t, 0f, 1f);
        }

        /// <summary>
        /// 缓入正弦：开始慢，后面逐渐加速。
        /// 感觉：从静止开始发力，有起步感。
        /// 适合：蓄力启动、武器起手、飞弹刚启动。
        /// 进度示意：0 -> 0.08 -> 0.29 -> 0.62 -> 1
        /// </summary>
        public static float InSine(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return 1f - MathF.Cos(t * MathHelper.PiOver2);
        }

        /// <summary>
        /// 缓出正弦：开始快，结尾逐渐减速。
        /// 感觉：顺滑停下，比较自然。
        /// 适合：收刀、停下、粒子散开、轻柔位移。
        /// 进度示意：0 -> 0.38 -> 0.71 -> 0.92 -> 1
        /// </summary>
        public static float OutSine(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathF.Sin(t * MathHelper.PiOver2);
        }

        /// <summary>
        /// 缓入缓出正弦：开始慢，中间快，结尾慢。
        /// 感觉：最平滑、最自然，不夸张。
        /// 适合：普通挥砍、镜头移动、平台移动、UI 平滑滑动。
        /// 进度示意：0 -> 0.15 -> 0.5 -> 0.85 -> 1
        /// </summary>
        public static float InOutSine(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return -(MathF.Cos(MathHelper.Pi * t) - 1f) / 2f;
        }

        /// <summary>
        /// 缓入三次方：前段非常慢，后段突然加速。
        /// 感觉：压住力量，然后爆发出去。
        /// 适合：重武器起手、强蓄力、怪物扑击前段。
        /// 进度示意：0 -> 0.02 -> 0.13 -> 0.42 -> 1
        /// </summary>
        public static float InCubic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * t;
        }

        /// <summary>
        /// 缓出三次方：开始很快，结尾逐渐减速。
        /// 感觉：利落、爽快，有出刀速度。
        /// 适合：刀光挥出、击退位移、冲击后收尾、普通攻击挥砍。
        /// 进度示意：0 -> 0.58 -> 0.88 -> 0.98 -> 1
        /// </summary>
        public static float OutCubic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float inverse = 1f - t;
            return 1f - inverse * inverse * inverse;
        }

        /// <summary>
        /// 缓入缓出三次方：起止都慢，中间加速明显。
        /// 感觉：比 InOutSine 更有重量和动作感。
        /// 适合：前摇移动、蓄力位移、从 A 点滑到 B 点。
        /// 进度示意：0 -> 0.06 -> 0.5 -> 0.94 -> 1
        /// </summary>
        public static float InOutCubic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            if (t < 0.5f)
            {
                return 4f * t * t * t;
            }

            return 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
        }

        /// <summary>
        /// 回弹缓出：快速到目标，稍微超过目标，再回到目标。
        /// 感觉：甩出去、弹一下、过冲，有动作游戏的力量感。
        /// 适合：夸张挥砍、重击收尾、UI 弹出、爪子抓出去。
        /// overshoot 越大，超过终点越明显。
        /// 进度示意：0 -> 0.82 -> 1.03 -> 1.06 -> 1
        /// </summary>
        public static float OutBack(float t, float overshoot = 1.2f)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float shifted = t - 1f;

            return 1f + shifted * shifted * ((overshoot + 1f) * shifted + overshoot);
        }

        /// <summary>
        /// 指数缓出：一开始几乎瞬间冲出去，后面慢慢贴近目标。
        /// 感觉：瞬间爆发、闪一下、速度非常快。
        /// 适合：瞬斩、闪现残影、冲击波扩散、弹幕突然飞出。
        /// 进度示意：0 -> 0.82 -> 0.97 -> 0.99 -> 1
        /// </summary>
        public static float OutExpo(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            if (t >= 1f)
            {
                return 1f;
            }

            return 1f - MathF.Pow(2f, -10f * t);
        }

        /// <summary>
        /// 弹性缓出：冲过目标，弹回来，再小幅弹几次，最后停住。
        /// 感觉：弹簧、Q 弹、魔法感、卡通感。
        /// 适合：UI 弹出、魔法球出现、史莱姆弹跳、弹性护盾。
        /// 不太适合严肃狠厉的武器挥砍。
        /// 进度示意：0 -> 1.12 -> 0.91 -> 1.02 -> 1
        /// </summary>
        public static float OutElastic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            if (t == 0f || t == 1f)
            {
                return t;
            }

            const float c4 = MathHelper.TwoPi / 3f;

            return MathF.Pow(2f, -10f * t) * MathF.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        /// <summary>
        /// 对 float 做带缓动的插值。
        /// 用法：EaseUtil.Lerp(起点, 终点, 原始进度, 缓动函数)
        /// 例子：EaseUtil.Lerp(20f, 80f, progress, EaseUtil.OutCubic)
        /// 等价于：MathHelper.Lerp(20f, 80f, EaseUtil.OutCubic(progress))
        /// </summary>
        public static float Lerp(float from, float to, float t, Func<float, float> ease)
        {
            return MathHelper.Lerp(from, to, ease(t));
        }

        /// <summary>
        /// 对 Vector2 做带缓动的插值。
        /// 用法：EaseUtil.Lerp(起点位置, 终点位置, 原始进度, 缓动函数)
        /// 例子：EaseUtil.Lerp(startPos, endPos, progress, EaseUtil.InOutSine)
        /// 等价于：Vector2.Lerp(startPos, endPos, EaseUtil.InOutSine(progress))
        /// </summary>
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t, Func<float, float> ease)
        {
            return Vector2.Lerp(from, to, ease(t));
        }
    }
}
