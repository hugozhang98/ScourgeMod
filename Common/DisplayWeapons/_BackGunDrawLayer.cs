namespace ScourgeMod.Common.BackWeapons
{
    public class BackGunDrawLayer //: PlayerDrawLayer
    {
        //public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WaistAcc);

        //public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        //{
        //    Player player = drawInfo.drawPlayer;
        //    Item heldItem = player.HeldItem;

        //    return DWHelper.GetDefaultVisibility(heldItem, player)
        //        && BackWeaponRegistry.IsBackGun(heldItem);
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
        //    Vector2 origin = GetOrigin(texture, frame, effects);
        //    Vector2 position = GetPosition(drawInfo.Center, player, heldItem, drawInfo);
        //    float rotation = GetRotation(player, heldItem);
        //    Color lightColor = GetColor(player);
        //    float scale = GetScale(heldItem);

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

        //private Vector2 GetOrigin(Texture2D texture, Rectangle frame, SpriteEffects effects)
        //{
        //    Rectangle visibleFrame = TextureHelper.GetVisibleFrame(texture);

        //    if (visibleFrame == Rectangle.Empty)
        //        return frame.Size() * 0.5f;

        //    // 原点在完整贴图中的位置
        //    Vector2 origin =
        //        visibleFrame.Location.ToVector2() + new Vector2(visibleFrame.Width * 0.33f, 4f);

        //    return TextureHelper.ApplyFlipToOrigin(origin, frame, effects);
        //}

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

        //    return position.Floor();
        //}

        //private float GetRotation(Player player, Item item)
        //{
        //    //默认旋转角度
        //    float baseRotation = AngleHelper.DegToRad(0f - player.direction * 5f);

        //    float moveSway = DWHelper.GetMoveSway(player, 3f);

        //    return (baseRotation + moveSway) * player.gravDir;
        //}

        //private Color GetColor(Player player) =>
        //    Lighting.GetColor(player.Center.ToTileCoordinates());

        //private float GetScale(Item item)
        //{
        //    float scale = 1f;
        //    return scale;
        //}
    }
}
