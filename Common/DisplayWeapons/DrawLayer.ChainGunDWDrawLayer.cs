using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class ChainGunDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_ChainGun(item);

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        )
        {
            Vector2 offset = new Vector2(-9f, -3f);

            DWAdjust_Anchor.TryAdjustPosition(player, item, ref offset);

            return offset;
        }

        protected override float GetRotationOffset(Player player) => 100f;

        protected override float GetWeaponSway(Player player) => 4f;

        protected override float GetScale(Item item) => 0.9f;
    }
}
