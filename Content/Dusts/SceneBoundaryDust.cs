using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ScourgeMod.Content.Dusts
{
    public sealed class SceneBoundaryDust : ModDust
    {
        public const int SpawnInterval = 6;
        private const int FadeTicks = 24;

        public override string Texture => "Terraria/Images/Dust";

        public override void OnSpawn(Dust dust)
        {
            //保留随机选中的纵向帧，横向定位到血粒子所在的列
            const int frameStride = 10;
            dust.frame.X = DustID.Blood * frameStride;
        }

        public override bool Update(Dust dust)
        {
            //用 fadeIn 记录年龄，粒子先在边界停留一轮，等下一批补上
            dust.fadeIn++;
            if (dust.fadeIn <= SpawnInterval)
                return false;

            dust.position += dust.velocity;
            if (dust.fadeIn >= SpawnInterval + FadeTicks)
                dust.active = false;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            float progress = (dust.fadeIn - SpawnInterval) / FadeTicks;
            float opacity = MathHelper.SmoothStep(1f, 0f, progress);
            return Color.White * opacity;
        }
    }
}
