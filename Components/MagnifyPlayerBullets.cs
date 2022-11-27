using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class MagnifyPlayerBullets : BraveBehaviour
    {
        public void Start()
        {
            orb = GetComponent<PlayerOrbital>();
            specRigidbody.OnTriggerCollision += OnTrigger;
        }

        public void OnTrigger(SpeculativeRigidbody specRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
            if(specRigidbody != null && specRigidbody.projectile != null && specRigidbody.projectile.Owner != null && specRigidbody.projectile.Owner is PlayerController p)
            {
                if (p.IsStealthed || Time.time - lastTimeWhenStealthed <= stealthForgivenessTime)
                {
                    specRigidbody.projectile.RuntimeUpdateScale(scaleMultiplierStealthed);
                    specRigidbody.projectile.baseData.damage *= damageMultiplierStealthed;
                    specRigidbody.GetOrAddComponent<PierceProjModifier>().penetration += stealthedAdditionalPierces;
                    specRigidbody.GetOrAddComponent<DontLoseDamageOnPierce>();
                }
                else
                {
                    specRigidbody.projectile.RuntimeUpdateScale(scaleMultiplier);
                    specRigidbody.projectile.baseData.damage *= damageMultiplier;
                }
                specRigidbody.RegisterTemporaryCollisionException(sourceSpecRigidbody, 0.1f, 0.5f);
                sourceSpecRigidbody.RegisterTemporaryCollisionException(specRigidbody, 0.1f, 0.5f);
            }
        }

        public void LateUpdate()
        {
            if (orb != null && orb.Owner != null && orb.Owner.IsStealthed)
            {
                lastTimeWhenStealthed = Time.time;
            }
        }

        public float stealthForgivenessTime;
        public float scaleMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float scaleMultiplierStealthed = 1f;
        public float damageMultiplierStealthed = 1f;
        public int stealthedAdditionalPierces;
        private float lastTimeWhenStealthed = -99f;
        private PlayerOrbital orb;
    }
}
