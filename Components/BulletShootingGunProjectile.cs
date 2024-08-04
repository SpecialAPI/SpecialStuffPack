using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class BulletShootingGunProjectile : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
                projectile.OnPostUpdate += VelocityModification;
            }
        }

        public void VelocityModification(Projectile proj)
        {
            proj.specRigidbody.Velocity *= proj.specRigidbody.Velocity.magnitude / Mathf.Max(Mathf.Abs(proj.specRigidbody.Velocity.x), Mathf.Abs(proj.specRigidbody.Velocity.y));
        }
    }
}
