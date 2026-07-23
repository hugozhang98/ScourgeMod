using System;

namespace ScourgeMod.Helper
{
    public class AngleHelper
    {
        /// <summary>
        /// 角度 转 弧度
        /// </summary>
        public static float DegToRad(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        /// <summary>
        /// 弧度 转 角度
        /// </summary>
        public static float RadToDeg(float radians)
        {
            return radians * 180f / (float)Math.PI;
        }
    }
}
