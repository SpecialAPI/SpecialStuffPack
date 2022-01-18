using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
    public class MetalGearRatSweepLaserBullets2 : Script
    {
        protected override IEnumerator Top()
        {
            AIBeamShooter[] beams = BulletBank.GetComponents<AIBeamShooter>();
            yield return Wait(15);
            for(; ; )
            {
                foreach(AIBeamShooter beam in beams)
                {
                    if (beam != null && beam.LaserBeam != null)
                    {
                        Vector2 overridePosition = (beam.LaserBeam as BasicBeamController).GetPointOnBeam(1f);
                        Fire(Offset.OverridePosition(overridePosition), new Direction(GetAimDirection(overridePosition, 0f, 0f), DirectionType.Absolute, -1), new Speed(12f, SpeedType.Absolute));
                    }
                }
                yield return Wait(2);
            }
        }
    }
}
