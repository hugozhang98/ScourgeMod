using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.DisplayWeapons
{
    public static class DWAdjust_PiercingStarlight
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.PiercingStarlight;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_SolarEruption
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.SolarEruption;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_MolotovCocktail
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.MolotovCocktail;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.6f;
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_FlyingKnife
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.FlyingKnife;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_RottenEgg
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.RottenEgg;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_ChainGuillotines
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.ChainGuillotines;

        public static bool TryAdjustScale(Item item, ref float scale)
        {
            if (AppliesTo(item))
            {
                scale = 0.8f;
                return true;
            }

            return false;
        }

        public static bool TryAdjustPosition(Player player, Item item, ref Vector2 position)
        {
            if (AppliesTo(item))
            {
                position += new Vector2(-3f, 0f);
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_Anchor
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.Anchor;

        public static bool TryAdjustPosition(Player player, Item item, ref Vector2 position)
        {
            if (AppliesTo(item))
            {
                position += new Vector2(6f, 4f);
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_Megashark
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.Megashark;

        public static bool TryAdjustOrigin(Item item, ref Vector2 origin)
        {
            if (AppliesTo(item))
            {
                origin += new Vector2(0f, 3f);
                return true;
            }

            return false;
        }
    }

    public static class DWAdjust_TheRottedFork
    {
        public static bool AppliesTo(Item item) => item.type == ItemID.TheRottedFork;

        public static bool TryAdjustPosition(Player player, ref Vector2 position)
        {
            if (AppliesTo(DWHelper.GetDisplayItem(player)))
            {
                position += new Vector2(-4f, 0f);
                return true;
            }

            return false;
        }
    }
}
