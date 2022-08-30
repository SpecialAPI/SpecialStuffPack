using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class DoubleChargeActive : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += DoubleChargeActiveItems;
        }

        public void DoubleChargeActiveItems(Projectile sourceProjectile, SpeculativeRigidbody hitRigidbody, bool fatal)
        {
            if(hitRigidbody == null || hitRigidbody.healthHaver == null)
            {
                return;
            }
            if(sourceProjectile.Owner is PlayerController player)
            {
                var dmg = Mathf.Clamp(sourceProjectile.ModifiedDamage, 0f, hitRigidbody.healthHaver.GetMaxHealth());
                if(dmg <= 0f)
                {
                    return;
                }
                for (int i = 0; i < player.activeItems.Count; i++)
                {
                    player.activeItems[i].DidDamage(player, dmg * (cooldownMultiplier - 1));
                }
            }
        }

        public float cooldownMultiplier;
    }
}
