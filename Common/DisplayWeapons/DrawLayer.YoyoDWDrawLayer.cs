using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class YoyoDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Yoyo(item);

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            Vector2 offset = new Vector2(-7f, 10f);

            if (DWHelper.GetVanillaAnimationType(player) == DWVanillaAnimationType.Jumping)
            {
                offset += new Vector2(0f, -5f);
            }

            return offset;
        }

        protected override float GetRotationOffset(Player player) => -180f;

        protected override float GetWeaponSway(Player player) => 7f;

        protected override float GetScale(Item item) => 0.55f;
    }
}
