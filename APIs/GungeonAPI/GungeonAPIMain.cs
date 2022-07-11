using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.GungeonAPI
{
    public static class GungeonAPIMain
    {
        public static void Init()
        {
            Tools.Init();
            StaticReferences.Init();
            DungeonHandler.Init();
        }
    }
}
