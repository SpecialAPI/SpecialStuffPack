using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    public static class NinjaRework
    {
        public static void Init()
        {
            var ninja = (GameObject)BraveResources.Load("PlayerNinja");
            SpecialAssets.assets.Add(ninja);
            var ninjaPlayer = ninja.GetComponent<PlayerController>();
            ninjaPlayer.AddToBreach("ninjabreachflag", new(12.5f, 22f), new(), new(8, 1, 10, 4), new(7, 1, 12, 19), new(-1, 0), "ninjaoverheadpanel", "The Ninja", "ninja", 5, "ninja_items");
        }
    }
}
