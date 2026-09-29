using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class MenuPreviewItemSystem : ModSystem
    {
        private static readonly ConditionalWeakTable<Player, Item> PreviewHeldItems = new();

        private static FieldInfo _playerField;
        private static MethodInfo _drawSelfMethod;

        private delegate void OrigDrawSelf(UICharacter self, SpriteBatch spriteBatch);

        private delegate void HookDrawSelf(
            OrigDrawSelf orig,
            UICharacter self,
            SpriteBatch spriteBatch
        );

        public override void Load()
        {
            if (Main.dedServ)
                return;

            _playerField =
                typeof(UICharacter).GetField(
                    "_player",
                    BindingFlags.Instance | BindingFlags.NonPublic
                ) ?? throw new MissingFieldException(typeof(UICharacter).FullName, "_player");

            _drawSelfMethod =
                typeof(UICharacter).GetMethod(
                    "DrawSelf",
                    BindingFlags.Instance | BindingFlags.NonPublic
                ) ?? throw new MissingMethodException(typeof(UICharacter).FullName, "DrawSelf");

            MonoModHooks.Add(_drawSelfMethod, (HookDrawSelf)UICharacter_DrawSelf);
        }

        public override void Unload()
        {
            _playerField = null;
            _drawSelfMethod = null;
        }

        private static void UICharacter_DrawSelf(
            OrigDrawSelf orig,
            UICharacter self,
            SpriteBatch spriteBatch
        )
        {
            Player player = (Player)_playerField.GetValue(self);

            // 此刻原版尚未把 inventory[selectedItem] 替换为 _blankItem。
            Item realHeldItem = player.HeldItem;
            PreviewHeldItems.Remove(player);
            PreviewHeldItems.Add(player, realHeldItem);

            try
            {
                orig(self, spriteBatch);
            }
            finally
            {
                // 原版 DrawPlayer 与你的 PlayerDrawLayer 已执行完，立即清理。
                PreviewHeldItems.Remove(player);
            }
        }

        public static Item GetDisplayItem(Player player)
        {
            return PreviewHeldItems.TryGetValue(player, out Item item) ? item : player.HeldItem;
        }
    }
}
