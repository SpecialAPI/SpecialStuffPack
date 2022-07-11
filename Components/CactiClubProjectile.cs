using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class CactiClubProjectile : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += (x, x2, x3) =>
            {
                for(int i = 0; i < NumProjectilesToShoot; i++)
                {
                    var proj = CodeShortcuts.OwnedShootProjectile(projectileToShoot, x.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), x.Owner);
                    proj.GetOrAddComponent<PierceProjModifier>().penetration += 1;
                    proj.baseData.damage *= DamageMultiplier;
                }
            };
        }

        public int NumProjectilesToShoot => 3;
        public float DamageMultiplier => 2f;

        public Projectile projectileToShoot;
    }
}
