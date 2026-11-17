using Microsoft.Xna.Framework;
using Terraria;

namespace ScourgeMod.Helper
{
    public class PositionHelper
    {
        public static bool InsideWorld(Vector2 topLeft, float width, float height)
        {
            return topLeft.X >= 0f
                && topLeft.Y >= 0f
                && topLeft.X + width <= Main.maxTilesX * TextureHelper.TileSize
                && topLeft.Y + height <= Main.maxTilesY * TextureHelper.TileSize;
        }
    }
}
