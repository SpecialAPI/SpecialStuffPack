using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class RaggedBullets : PassiveItem
    {
        public static void Init()
        {
            var name = "Ragged Bullets";
            var shortdesc = "They are Whole Again";
            var longdesc = "Bullets Link enemies. Getting hit deals damage to all linked enemies";
            var item = EasyItemInit<RaggedBullets>("raggedbullets", "ragged_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.chance = 0.4f;
            link = LinkedStatusEffect.LinkedGenerator(6f);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += LinkChance;
            player.healthHaver.Ext().OnDamagedContextLater += Catalyst;
        }

        public void Catalyst(HealthHaver hh, float damage, string source, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection, bool ignoreInvulnerabilityFrames = false, bool ignoreDamageCaps = false)
        {
            if (damageTypes != CoreDamageTypesE.Linked && hh != null && hh.gameActor != null)
            {
                LinkedStatusEffect.LinkEffect(hh.gameActor, damage, source, damageDirection);
            }
        }

        public void LinkChance(Projectile proj, float f)
        {
            if (chance.ScaledRandom(f))
            {
                proj.statusEffectsToApply.Add(link);
                proj.AdjustPlayerProjectileTint(Color.red, 0, 0f);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player != null)
            {
                player.PostProcessProjectile -= LinkChance;
                player.healthHaver.Ext().OnDamagedContextLater -= Catalyst;
            }
        }

        public static LinkedStatusEffect link;
        public float chance;
    }
}
