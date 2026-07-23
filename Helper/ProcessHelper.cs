using System;
using Microsoft.Xna.Framework;

namespace ScourgeMod.Helper
{
    /// <summary>
    /// 常用周期函数工具类。
    /// time 表示持续增长的时间，period 表示完成一次循环所需的时间。
    /// phaseOffset 表示周期偏移，单位是“圈”：0.25f 为四分之一圈，1f 为一整圈。
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 循环进度：随时间从 0 线性增长到接近 1，然后瞬间回到 0。
        /// 输出范围：[0, 1)。
        /// 适合：循环计时、贴图帧、轨道角度、其他周期函数的基础输入。
        /// </summary>
        public static float Loop(float time, float period, float phaseOffset = 0f)
        {
            if (period <= 0f)
            {
                return 0f;
            }

            return PositiveModulo(time / period + phaseOffset, 1f);
        }

        /// <summary>
        /// 正弦波：从 0 开始，依次经过 1、0、-1，再回到 0。
        /// 输出范围：[-1, 1]。
        /// 适合：左右摆动、上下悬浮、后坐力偏移、呼吸式旋转。
        /// </summary>
        public static float Swing(float time, float period, float phaseOffset = 0f)
        {
            return MathF.Sin(Loop(time, period, phaseOffset) * MathHelper.TwoPi);
        }

        /// <summary>
        /// 余弦往返波：从 0 开始，平滑增长到 1，再平滑回到 0。
        /// 输出范围：[0, 1]，起点和最高点的速度均为 0。
        /// 适合：呼吸缩放、淡入淡出、平滑蓄能、光芒明暗变化。
        /// </summary>
        public static float SmoothBackAndForth(float time, float period, float phaseOffset = 0f)
        {
            float progress = Loop(time, period, phaseOffset);
            return (1f - MathF.Cos(progress * MathHelper.TwoPi)) * 0.5f;
        }

        /// <summary>
        /// 三角波：从 0 匀速增长到 1，再匀速回到 0。
        /// 输出范围：[0, 1]。
        /// 适合：机械往返、扫描线、匀速伸缩、规则摆动。
        /// </summary>
        public static float BackAndForth(float time, float period, float phaseOffset = 0f)
        {
            float progress = Loop(time, period, phaseOffset);
            return 1f - MathF.Abs(progress * 2f - 1f);
        }

        private static float PositiveModulo(float value, float modulus)
        {
            float result = value % modulus;
            return result < 0f ? result + modulus : result;
        }
    }
}
