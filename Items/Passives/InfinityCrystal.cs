using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class InfinityCrystal : PassiveItem
    {
        public static void Init()
        {
            string name = "Infinity Crystal";
            string shortdesc = "Neverending";
            string longdesc = "Shooting consumes no ammo, but damage is decreased and clip size is halved.";
            var item = EasyItemInit<InfinityCrystal>("items/infinitycrystal", "sprites/infinity_crystal", name, shortdesc, longdesc, ItemQuality.S);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, .75f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, .5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddToCursulaShop();
        }

        public override void Pickup(PlayerController player)
        {
            player.stats.AdditionalVolleyModifiers += Neverending;
            base.Pickup(player);
        }

        public void Neverending(ProjectileVolleyData volley)
        {
            volley.projectiles.ForEach(x => x.ammoCost = 0);
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.stats.AdditionalVolleyModifiers -= Neverending;
            base.DisableEffect(disablingPlayer);
        }
    }
}
