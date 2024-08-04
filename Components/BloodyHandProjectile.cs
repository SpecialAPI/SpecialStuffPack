using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class BloodyHandProjectile : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
                if (autoDamagePlayer)
                {
                    if(projectile.Owner == null || projectile.Owner.healthHaver == null || projectile.Owner.healthHaver.currentHealth <= 0.5f)
                    {
                        return;
                    }
                    projectile.Owner.healthHaver.ForceSetCurrentHealth(projectile.Owner.healthHaver.currentHealth - 0.5f);
                }
                projectile.OnHitEnemy += HealOwner;
            }
        }

        public void HealOwner(Projectile proj, SpeculativeRigidbody spec, bool b)
        {
            if(proj != null && proj.Owner != null && proj.Owner.healthHaver != null)
            {
                proj.Owner.healthHaver.ApplyHealingUnmodified(0.5f);
                proj.OnHitEnemy -= HealOwner;
            }
        }

        public bool autoDamagePlayer;
    }
}
