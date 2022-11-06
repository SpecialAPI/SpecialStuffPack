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
            if(proj is WhipProjectile)
            {
                var whip = proj as WhipProjectile;
                if (whip.child != null)
                {
                    var isup = !isUp;
                    var child = whip.child;
                    var i = whip.Length;
                    while (child != null && i > 0)
                    {
                        OwnedShootProjectile(projToSpawn, isup ? child.sprite.WorldTopCenter : child.sprite.WorldBottomCenter, child.transform.up.XY().ToAngle() * (isup ? 1 : -1), proj.Owner);
                        child = child.child;
                        isup = !isup;
                        i--;
                    }
                }
                if (whip.parent != null)
                {
                    var isup = !isUp;
                    var parent = whip.parent;
                    var i = whip.Length;
                    while (parent != null && i > 0)
                    {
                        OwnedShootProjectile(projToSpawn, isup ? parent.sprite.WorldTopCenter : parent.sprite.WorldBottomCenter, parent.transform.up.XY().ToAngle() * (isup ? 1 : -1), proj.Owner);
                        parent = parent.parent;
                        isup = !isup;
                        i--;
                    }
                }
            }
        }

        public Projectile projToSpawn;
        public bool isUp;
    }
}
