using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BloodyHand : PassiveItem
    {
        public static void Init()
        {
            var name = "Bloody Hand";
            var shortdesc = "Smells like potatoes...";
            var longdesc = "Shooting consumes health but greatly increases damage. Bullets affected by this effect heal the owner when hittin an enemy. Greatly increases accuracy.\n\nA severed pale hand covered in blood.";
            var item = EasyItemInit<BloodyHand>("bloodyhand", "bloody_hand_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 0.25f, ModifyMethodE.TrueMultiplicative);
            item.AddPassiveStatModifier(PlayerStats.StatType.RangeMultiplier, 2f, ModifyMethodE.TrueMultiplicative);
            item.AddToCursulaShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += HealthForDamage;
        }

        public void HealthForDamage(Projectile proj, float f)
        {
            if (proj.TreatedAsNonProjectileForChallenge)
            {
                return;
            }
            var h = Owner.healthHaver.GetCurrentHealth();
            if (h > 0.5f)
            {
                Owner.healthHaver.ForceSetCurrentHealth(h - 0.5f);
                proj.baseData.damage *= 3f;
                proj.AddComponent<BloodyHandProjectile>();
                proj.pierceMinorBreakables = true;
                proj.AddComponent<UncatchableProjectile>();
                proj.AdjustPlayerProjectileTint(Color.red, 0, 0f);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player == null)
            {
                return;
            }
            player.PostProcessProjectile -= HealthForDamage;
            base.DisableEffect(player);
        }
    }
}
