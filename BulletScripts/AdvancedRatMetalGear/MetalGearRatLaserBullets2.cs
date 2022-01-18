using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
    public class MetalGearRatLaserBullets2 : Script
    {
        protected override IEnumerator Top()
        {
			AIBeamShooter[] beams = BulletBank.GetComponents<AIBeamShooter>();
			yield return Wait(19);
			for (; ; )
			{
				yield return Wait(1);
				if (beams == null || beams.Length == 0)
				{
					break;
				}
				AIBeamShooter beam = beams[UnityEngine.Random.Range(1, beams.Length)];
				if (beam && beam.LaserBeam)
				{
					Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;
					Fire(Offset.OverridePosition(overridePosition), new Direction(RandomAngle(), DirectionType.Absolute, -1), new Speed(10f, SpeedType.Absolute));
				}
			}
			yield break;
		}
    }
}
