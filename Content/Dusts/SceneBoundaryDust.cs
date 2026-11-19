using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ScourgeMod.Content.Dusts
{
    public sealed class SceneBoundaryDust : ModDust
    {
        public const int SpawnInterval = 6;
        internal const int DefaultOutwardLayerCount = 5;

        internal sealed record FadeSettings(
            float InitialScale, float InitialOpacity, int OutwardLayerCount, int LayerInterval
        )
        {
            public float DriftTicks => OutwardLayerCount * (float)LayerInterval;

            public void Apply(Dust dust, float visibility)
            {
                dust.scale = InitialScale * visibility;
                dust.alpha = 255 - (int)MathF.Round(255f * InitialOpacity * visibility);
            }
        }

        private sealed class CustomAppearance(ModDust source, Dust dust, FadeSettings fade)
        {
            public readonly ModDust Source = source;
            public readonly FadeSettings Fade = fade;
            public object Data = dust.customData;
            public float FadeIn = dust.fadeIn;
            public Color LightColor = Color.White;
        }

        public override string Texture => "Terraria/Images/Dust";

        internal static void Spawn(
            Vector2 position, Vector2 velocity, int dustId, ModDust source, FadeSettings fade
        )
        {
            int boundaryType = ModContent.DustType<SceneBoundaryDust>();
            Dust dust = Dust.NewDustPerfect(
                position, source?.Type ?? boundaryType, velocity, Scale: fade.InitialScale
            );
            if (!dust.active)
                return;

            if (source != null && source.Type != boundaryType)
            {
                //保留源粉尘 OnSpawn 选好的纹理帧和绘制状态，运动交给边界控制。
                dust.customData = new CustomAppearance(source, dust, fade);
                dust.type = boundaryType;
                dust.fadeIn = 0f;
            }
            else
            {
                if (source == null)
                    SetFrame(dust, dustId);
                dust.customData = fade;
            }
            fade.Apply(dust, 1f);
        }

        public override void OnSpawn(Dust dust)
        {
            SetFrame(dust, DustID.Blood);
        }

        private static void SetFrame(Dust dust, int dustId)
        {
            //原版图集每行 100 种粉尘，每种有三个纵向随机帧。
            const int frameStride = 10;
            const int rowHeight = 3 * frameStride;
            dust.frame.X = dustId % 100 * frameStride;
            dust.frame.Y = dustId / 100 * rowHeight + dust.frame.Y % rowHeight;
        }

        public override bool Update(Dust dust)
        {
            FadeSettings fade = dust.customData is CustomAppearance appearance
                ? appearance.Fade : dust.customData as FadeSettings;
            if (fade == null)
            {
                fade = new FadeSettings(dust.scale, 1f, DefaultOutwardLayerCount, SpawnInterval);
                dust.customData = fade;
            }

            //首层在边界停留一轮，下一批补上后才开始向外飘。
            float driftAge = ++dust.fadeIn - fade.LayerInterval;
            if (driftAge <= 0f)
                return false;

            if (driftAge > fade.DriftTicks)
            {
                dust.active = false;
                return false;
            }

            dust.position += dust.velocity;
            //最后一圈在本轮仍可见，下一刻淡出。
            fade.Apply(dust, 1f - driftAge / (fade.DriftTicks + 1f));
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            if (dust.customData is CustomAppearance appearance)
                appearance.LightColor = lightColor;
            return Color.White * ((255 - dust.alpha) / 255f);
        }

        public override bool PreDraw(Dust dust)
        {
            if (dust.customData is not CustomAppearance appearance)
                return true;

            //绘制钩子应看到源粉尘自己的类型和状态，临时绘制变换只在这一帧生效。
            Vector2 boundaryPosition = dust.position;
            Vector2 boundaryVelocity = dust.velocity;
            float boundaryScale = dust.scale;
            int boundaryAlpha = dust.alpha;
            float boundaryAge = dust.fadeIn;
            dust.type = appearance.Source.Type;
            dust.customData = appearance.Data;
            dust.fadeIn = appearance.FadeIn;
            try
            {
                //先取源颜色，再统一施加边界不透明度，避免 GetAlpha 将其重复衰减。
                dust.alpha = 0;
                Color color = dust.GetAlpha(appearance.LightColor);
                dust.alpha = boundaryAlpha;
                if (appearance.Source.PreDraw(dust))
                {
                    Texture2D texture = appearance.Source.Texture2D.Value;
                    Vector2 position = dust.position - Main.screenPosition;
                    Vector2 origin = new Vector2(4f, 4f);
                    float opacity = (255 - boundaryAlpha) / 255f;
                    Main.spriteBatch.Draw(
                        texture, position, dust.frame, color * opacity,
                        dust.rotation, origin, dust.scale, SpriteEffects.None, 0f
                    );
                    if (dust.color != default)
                        Main.spriteBatch.Draw(
                            texture, position, dust.frame, dust.GetColor(color) * opacity,
                            dust.rotation, origin, dust.scale, SpriteEffects.None, 0f
                        );
                }
            }
            finally
            {
                appearance.Data = dust.customData;
                appearance.FadeIn = dust.fadeIn;
                dust.type = Type;
                dust.customData = appearance;
                dust.position = boundaryPosition;
                dust.velocity = boundaryVelocity;
                dust.scale = boundaryScale;
                dust.fadeIn = boundaryAge;
                dust.alpha = boundaryAlpha;
            }
            return false;
        }
    }
}
