using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ChangeOrbitSettingsOnStealth : BraveBehaviour
    {
        public void Start()
        {
            orb = GetComponent<PlayerOrbital>();
            if(orb != null)
            {
                originalOrbitRadius = orb.orbitRadius;
                originalDegreesPerSecond = orb.orbitDegreesPerSecond;
            }
        }

        public void Update()
        {
            if(orb != null && orb.Owner != null)
            {
                if (orb.Owner.IsStealthed)
                {
                    lastTimeWhenStealthed = Time.time;
                    orb.orbitRadius = stealthedOrbitRadius;
                    orb.orbitDegreesPerSecond = stealthedDegreesPerSecond;
                }
                else if (Time.time - lastTimeWhenStealthed <= stealthForgivenessTime)
                {
                    orb.orbitRadius = stealthedOrbitRadius;
                    orb.orbitDegreesPerSecond = stealthedDegreesPerSecond;
                }
                else
                {
                    orb.orbitRadius = originalOrbitRadius;
                    orb.orbitDegreesPerSecond = originalDegreesPerSecond;
                }
            }
        }

        private float lastTimeWhenStealthed = -99f;
        private PlayerOrbital orb;
        private float originalOrbitRadius;
        private float originalDegreesPerSecond;
        public float stealthedOrbitRadius;
        public float stealthedDegreesPerSecond;
        public float stealthForgivenessTime;
    }
}
