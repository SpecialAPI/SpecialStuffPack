using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    public static class CosmonautRework
    {
        public static void Init()
        {
            var cosmo = (GameObject)BraveResources.Load("PlayerCosmonaut");
            SpecialAssets.assets.Add(cosmo);
            var cosmoPlayer = cosmo.GetComponent<PlayerController>();
            cosmoPlayer.AddToBreach("cosmonautbreachflag", new(23.5f, 18.25f), new(), new(8, 1, 10, 4), new(7, 1, 12, 19), new(-1, 0), "cosmonautoverheadpanel", "The Cosmonaut", "cosmonaut", 5, "cosmonaut_items");
        }
    }
}
