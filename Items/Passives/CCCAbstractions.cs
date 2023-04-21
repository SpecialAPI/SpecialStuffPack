using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class CCCAbstractions
    {
        public static void Init()
        {
            var ab1 = "OOOOO OO O OOO";
            var oab1 = "00o";
            IdOfTheFirstThing = EasyItemInit<PassiveItem>(oab1, "00_idle_001", "00", ab1, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab1.ToMTGId()}").PickupObjectId;
            var ab2 = "OOOOOOOO";
            var oab2 = "00oo";
            EasyItemInit<PassiveItem>(oab2, "00_idle_001", "00", ab2, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab2.ToMTGId()}");
            var ab3 = "Bababababa";
            var oab3 = "00baba";
            EasyItemInit<PassiveItem>(oab3, "00_idle_001", "00", ab3, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab3.ToMTGId()}");
            var ab4 = "(Baba)";
            var oab4 = "00()";
            EasyItemInit<PassiveItem>(oab4, "00_idle_001", "00", ab4, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab4.ToMTGId()}");
            var ab5 = "AAAA";
            var oab5 = "00A";
            EasyItemInit<PassiveItem>(oab5, "00_idle_001", "00", ab5, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab5.ToMTGId()}");
            var ab6 = "Abab Cd Efg";
            var oab6 = "00abc";
            EasyItemInit<PassiveItem>(oab6, "00_idle_001", "00", ab6, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab6.ToMTGId()}");
            var ab7 = "123";
            var oab7 = "00123";
            EasyItemInit<PassiveItem>(oab7, "00_idle_001", "00", ab7, "", PickupObject.ItemQuality.SPECIAL, null, $"oooo:{ab7.ToMTGId()}");
        }

        public static int IdOfTheFirstThing;
    }
}
