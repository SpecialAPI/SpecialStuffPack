using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class RocketLauncherProjectile : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnDestruction += Explode;
        }

        public void Explode(Projectile proj)
        {
            var explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
            ExplosionData newExplosionData = new();
            newExplosionData.CopyFrom(explosionData);
            newExplosionData.damage = Damage;
            newExplosionData.doDestroyProjectiles = false;
            Exploder.Explode(proj.specRigidbody.UnitCenter, newExplosionData, Vector3.zero, null, true, CoreDamageTypes.None, false);
        }

        public float Damage;
    }
}
