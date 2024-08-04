using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class SecondHeart
    {
        public static void Init()
        {
            var name = "Second Heart";
            var shortdesc = "Two Hearts are Better than One";
            var longdesc = "All health is doubled.\n\nWith how dangerous the Gungeon is, it's surprising that more gungeoneers don't have backup hearts.";
            var item = EasyItemInit<PassiveItem>("secondheart", "second_heart_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.S, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 2f, ModifyMethodE.TrueMultiplicative);
        }
    }
}
