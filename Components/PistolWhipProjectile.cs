using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class PistolWhipProjectile : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += OnHitEnemy;
        }

        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody spec, bool b)
        {
            OwnedShootProjectile(projToSpawn, isUp ? sprite.WorldTopCenter : sprite.WorldBottomCenter, transform.up.XY().ToAngle() * (isUp ? 1 : -1), proj.Owner);
        }

        public Projectile projToSpawn;
        public bool isUp;
    }
}
