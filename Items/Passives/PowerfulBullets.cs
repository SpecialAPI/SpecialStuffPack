using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class PowerfulBullets
    {
        public static void Init()
        {
            var name = "Powerful Bullets";
            var shortdesc = "Power Value of 2";
            var longdesc = "Exponentially increases damage.\n\nPeer-reviewed studies have shown that the power value of these bullets is exactly 2, although nobody is sure what that means.";
            var item = EasyItemInit<PassiveItem>("powerfulbullets", "powerful_bullets_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.S, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 2f, ModifyMethodE.Exponent);
        }
    }
}
