using System.IO;
using ScourgeMod.Helper;
using Terraria.ModLoader;

namespace ScourgeMod
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class ScourgeMod : Mod
    {
        public static ScourgeMod Instance { get; private set; }

        public override void Load()
        {
            base.Load();

            Instance = this;
        }

        public override void Unload()
        {
            base.Unload();

            SyncHelper.Unload();
            Instance = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            SyncHelper.HandlePacket(reader, whoAmI);
        }
    }
}
