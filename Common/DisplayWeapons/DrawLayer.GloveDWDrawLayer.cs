using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class GloveDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Glove(item);

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        ) => new Vector2(7f, 8f);

        protected override float GetRotationOffset(Player player) => 80f;

        protected override float GetWeaponSway(Player player) => 4f;

        protected override float GetScale(Item item) => 0.65f;
    }
}
