using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Tools
{
    public static class VFXDatabase
    {
        public static void Init()
        {
            MiniBlank = (GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab");
            BigBlank = (GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab");
        }

        public static GameObject MiniBlank;
        public static GameObject BigBlank;
    }
}
