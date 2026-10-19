using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class SidearmDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Sidearm(item);

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        ) => new Vector2(-3f, 6f);

        protected override float GetRotationOffset(Player player) => 90f;

        protected override float GetWeaponSway(Player player) => 4f;
    }
}
