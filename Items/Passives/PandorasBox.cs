using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class PandorasBox
    {
        public static void Init()
        {
            var name = "Pandora's Box";
            var shortdesc = "Don't open it";
            var longdesc = "Provides various benefits, but all curse is doubled.\n\nA shiny golden box said to contain a thousand deadly curses.";
            var item = EasyItemInit<PassiveItem>("pandorasbox", "pandoras_box_idle_001", name, shortdesc, longdesc, PickupObject.ItemQuality.A, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.25f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.ProjectileSpeed, 0.25f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, ModifyMethodE.TrueMultiplicative);
        }
    }
}
