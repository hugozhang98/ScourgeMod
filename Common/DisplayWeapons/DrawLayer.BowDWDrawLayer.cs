using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class BowDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Bow(item);

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            Vector2 offset = new Vector2(8f, 5f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                offset += new Vector2(-5f, 5f);
            }

            return offset;
        }

        protected override float GetRotationOffset(Player player)
        {
            float offset = 30f;

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Idle)
            {
                offset += 30f;
            }

            return offset;
        }

        protected override float GetWeaponSway(Player player) => 1f;
    }
}
