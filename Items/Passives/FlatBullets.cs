using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class FlatBullets
    {
        public static void Init()
        {
            string name = "Flat Bullets";
            string shortdesc = "Flat Damage";
            string longdesc = "Increases damage.";
            var item = EasyItemInit<PassiveItem>("items/flatbullets", "sprites/flat_bullets_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.B);
            item.AddPassiveStatModifier("UnscaledFlatDamage", 2f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddToTrorkShop();
        }
    }
}
