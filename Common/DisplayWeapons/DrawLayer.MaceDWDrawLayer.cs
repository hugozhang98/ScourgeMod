using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class MaceDWDrawLayer : DWDrawLayerBase
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool MatchWeapon(Item item) => DWRegistry.IsDW_Mace(item);

        protected override SpriteEffects GetEffects(Player player)
        {
            SpriteEffects effects = SpriteEffects.FlipVertically;

            if (player.direction == -1)
                effects |= SpriteEffects.FlipHorizontally;

            if (player.gravDir == -1f)
                effects &= ~SpriteEffects.FlipVertically;

            return effects;
        }

        protected override Vector2 GetPositionOffset(
            PlayerDrawSet drawInfo,
            Player player,
            Item item
        ) => new Vector2(8f, 9f);

        protected override float GetRotationOffset(Player player) => -55f;

        protected override float GetWeaponSway(Player player) => 7f;
    }
}
