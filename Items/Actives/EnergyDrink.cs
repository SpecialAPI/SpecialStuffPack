using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class EnergyDrink : PlayerItem
    {
        public static void Init()
        {
            string name = "Energy Drink";
            string shortdesc = "Instant energy";
            string longdesc = "Temporarily boosts speed, rate of fire and refills the user's current gun's clip.";
            var item = ItemBuilder.EasyInit<EnergyDrink>("items/energydrink", "sprites/energy_drink_001", name, shortdesc, longdesc, ItemQuality.B);
            item.SetCooldownType(ItemBuilder.CooldownType.Damage, 250f);
            item.AddToTrorkShop();
            item.AddToGooptonShop();
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            user.DecayingStatModifier(PlayerStats.StatType.RateOfFire, 5f, 5f, true);
            user.DecayingStatModifier(PlayerStats.StatType.MovementSpeed, 2f, 5f, true);
            user.CurrentGun?.ForceImmediateReload(true);
            this.HandleDuration(5f, user, null);
        }
    }
}
