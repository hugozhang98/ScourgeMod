namespace ScourgeMod.Common.BackWeapons
{
    public class BackSwordWaistDrawLayer //: PlayerDrawLayer
    {
        //public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Skin);

        //public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        //{
        //    Player player = drawInfo.drawPlayer;
        //    Item heldItem = player.HeldItem;

        //    return DWHelper.GetDefaultVisibility(heldItem, player)
        //        && BackWeaponRegistry.IsBackSword_Waist(heldItem);
        //}

        //protected override void Draw(ref PlayerDrawSet drawInfo)
        //{
        //    if (drawInfo.shadow != 0f)
        //        return;

        //    Player player = drawInfo.drawPlayer;
        //    Item heldItem = player.HeldItem;

        //    Texture2D texture = TextureAssets.Item[heldItem.type].Value;
        //    Rectangle frame = texture.Frame();

        //    SpriteEffects effects = GetEffects(player);
        //    Vector2 origin = GetOrigin(frame, player, heldItem);
        //    Vector2 position = GetPosition(drawInfo.Center, player, heldItem, drawInfo);
        //    float rotation = GetRotation(player, heldItem);
        //    Color lightColor = GetColor(player);
        //    float scale = GetScale();

        //    DrawData drawData = new DrawData(
        //        texture,
        //        position.Floor(),
        //        frame,
        //        lightColor,
        //        rotation,
        //        origin,
        //        scale,
        //        effects,
        //        0
        //    );

        //    drawInfo.DrawDataCache.Add(drawData);
        //}

        //private SpriteEffects GetEffects(Player player)
        //{
        //    SpriteEffects effects = SpriteEffects.None;
        //    if (player.direction == -1)
        //        effects |= SpriteEffects.FlipHorizontally;
        //    if (player.gravDir == -1f)
        //        effects |= SpriteEffects.FlipVertically;

        //    return effects;
        //}

        //private Vector2 GetOrigin(Rectangle frame, Player player, Item item) => frame.Size() * 0.5f;

        //private Vector2 GetPosition(
        //    Vector2 basePosition,
        //    Player player,
        //    Item item,
        //    PlayerDrawSet drawInfo
        //)
        //{
        //    //默认坐标（屏幕坐标）
        //    Vector2 position = basePosition - Main.screenPosition;

        //    position += DWHelper.GetUpperBodyBobbing(drawInfo);

        //    //默认偏移
        //    position += new Vector2(player.direction * 2f, player.gravDir * 9f);

        //    return position.Floor();
        //}

        //private float GetRotation(Player player, Item item)
        //{
        //    //默认旋转角度
        //    float baseRotation = AngleHelper.DegToRad(0f - player.direction * 150f);

        //    if (item.type == ItemID.Ruler)
        //    {
        //        baseRotation = AngleHelper.DegToRad(0f - player.direction * 110f);
        //    }

        //    float moveSway = DWHelper.GetMoveSway(player, 4f);

        //    return (baseRotation + moveSway) * player.gravDir;
        //}

        //private Color GetColor(Player player) =>
        //    Lighting.GetColor(player.Center.ToTileCoordinates());

        //private float GetScale() => 1f;
    }
}
