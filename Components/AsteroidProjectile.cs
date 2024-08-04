using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class AsteroidProjectile : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
                projectile.OnDestruction += OnDestruction;
                if (InheritDamage)
                {
                    if (projectile.Owner != null && projectile.Owner is PlayerController && (projectile.Owner as PlayerController).CurrentGun != null && (projectile.Owner as PlayerController).CurrentGun != null &&
                        (projectile.Owner as PlayerController).CurrentGun.DefaultModule != null)
                    {
                        if ((projectile.Owner as PlayerController).CurrentGun.DefaultModule.chargeProjectiles.Where((ProjectileModule.ChargeProjectile charg) => charg.Projectile != null).ToList().Count > 0)
                        {
                            Projectile proj = null;
                            float maxChargeTime = float.MinValue;
                            foreach (ProjectileModule.ChargeProjectile charg in (projectile.Owner as PlayerController).CurrentGun.DefaultModule.chargeProjectiles.Where((ProjectileModule.ChargeProjectile charg2) => charg2.Projectile != null).ToList())
                            {
                                if (charg.ChargeTime > maxChargeTime)
                                {
                                    proj = charg.Projectile;
                                    maxChargeTime = charg.ChargeTime;
                                }
                            }
                            if (proj != null)
                            {
                                projectile.baseData.damage *= proj.baseData.damage;
                            }
                        }
                        else if ((projectile.Owner as PlayerController).CurrentGun.DefaultModule.projectiles.Count > 0 && (projectile.Owner as PlayerController).CurrentGun.DefaultModule.projectiles[0] != null)
                        {
                            projectile.baseData.damage *= (projectile.Owner as PlayerController).CurrentGun.DefaultModule.projectiles[0].baseData.damage;
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if(specRigidbody != null)
            {
                GlobalSparksDoer.DoRandomParticleBurst(3, specRigidbody.UnitBottomLeft, specRigidbody.UnitTopRight, UnityEngine.Random.insideUnitCircle, 0f, 0f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            }
        }

        public void OnDestruction(Projectile proj)
        {
            Exploder.DoRadialIgnite(Fire, proj.specRigidbody.UnitCenter, IgniteRadius, null);
        }

        public bool InheritDamage;
        public GameActorFireEffect Fire;
        public float IgniteRadius;
    }
}
