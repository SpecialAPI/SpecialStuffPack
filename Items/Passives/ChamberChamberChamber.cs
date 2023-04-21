using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class ChamberChamberChamber : PassiveItem
    {
        public static void Init()
        {
            string name = "Chamber Chamber Chamber";
            string shortdesc = "OOOOOOOOOOO";
            string longdesc = "A gun cylinder with no chambers. Even though this otherworldly object doesn't have any eyes, holding it makes you fee like you're being watched.\n\nChamber chamber chamber chamber chamber chamber chamber chamber chamber chamber chamber chamber chamber chamber chamber";
            var item = EasyItemInit<ChamberChamberChamber>("chamberchamberchamber", "chamber_chamber_chamber_idle_001", name, shortdesc, longdesc, ItemQuality.S, null, "oooo:chamber_chamber_chamber");
        }
    }
}
