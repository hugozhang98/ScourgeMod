using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace ScourgeMod.Helper
{
    public static class TextureHelper
    {
        private static readonly Dictionary<(Texture2D texture, byte alphaThreshold), Rectangle> VisibleFrameCache = new();

        public static Rectangle GetVisibleFrame(Texture2D texture, byte alphaThreshold = 1)
        {
            var cacheKey = (texture, alphaThreshold);
            if (VisibleFrameCache.TryGetValue(cacheKey, out Rectangle visibleFrame))
                return visibleFrame;

            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            int minX = texture.Width;
            int minY = texture.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    Color color = pixels[x + y * texture.Width];

                    if (color.A < alphaThreshold)
                        continue;

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }

            visibleFrame =
                maxX < minX || maxY < minY
                    ? Rectangle.Empty
                    : new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);

            VisibleFrameCache.Add(cacheKey, visibleFrame);
            return visibleFrame;
        }

        public static void ClearVisibleFrameCache()
        {
            VisibleFrameCache.Clear();
        }

        public static Vector2 ApplyFlipToOrigin(
            Vector2 origin,
            Rectangle frame,
            SpriteEffects effects
        )
        {
            if (effects.HasFlag(SpriteEffects.FlipHorizontally))
                origin.X = frame.Width - origin.X;

            if (effects.HasFlag(SpriteEffects.FlipVertically))
                origin.Y = frame.Height - origin.Y;

            return origin;
        }
    }

    public class TextureHelperSystem : ModSystem
    {
        public override void Unload()
        {
            base.Unload();

            TextureHelper.ClearVisibleFrameCache();
        }
    }
}
