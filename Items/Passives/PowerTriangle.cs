using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class PowerTriangle : PassiveItem
    {
        public static void Init()
        {
            var name = "Triangle of Power";
            var shortdesc = "Just Dodge the Bullets";
            var longdesc = "Grants health and damage, but permanently decreases damage when taking damage.\n\nAn ancient artifact from an alien planet, this strange triangle radiates with power.";
            var item = EasyItemInit<PowerTriangle>("powertriangle", "power_triangle_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.3f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += DamageDown;
        }

        public void DamageDown(PlayerController play)
        {
            play.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Damage, ModifyMethodE.TrueMultiplicative, 0.95f));
        }

        public override void DisableEffect(PlayerController player)
        {
            player.OnReceivedDamage -= DamageDown;
            base.DisableEffect(player);
        }
    }
}
