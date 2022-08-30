using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ChanceBasedSpawnTimedProjectileOnKill : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += ChanceToSpawnItem;
        }

        public void ChanceToSpawnItem(Projectile sourceProjectile, SpeculativeRigidbody hitRigidbody, bool fatal)
        {
            if (hitRigidbody == null)
            {
                return;
            }
            if (fatal && Random.value <= chance)
            {
                var proj = OwnedShootProjectile(toShoot, hitRigidbody.UnitBottomCenter + Vector2.up / 2f, 0f, sourceProjectile.Owner);
                if(proj != null)
                {
                    proj.baseData.range = 99999f;
                    var pierce = proj.GetOrAddComponent<PierceProjModifier>();
                    pierce.penetration = 99999;
                    pierce.preventPenetrationOfActors = false;
                    pierce.penetratesBreakables = true;
                    pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
                    proj.StartCoroutine(TimedDestroyProjectile(proj, time));
                }
            }
        }

        public static IEnumerator TimedDestroyProjectile(Projectile proj, float time)
        {
            yield return new WaitForSeconds(time);
            if(proj != null)
            {
                proj.DieInAir(false, true, true, false);
            }
            yield break;
        }

        public float chance;
        public Projectile toShoot;
        public float time;
    }
}