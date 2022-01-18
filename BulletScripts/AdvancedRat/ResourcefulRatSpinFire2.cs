using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
    public class ResourcefulRatSpinFire2 : Script
    {
        protected override IEnumerator Top()
        {
			yield return Wait(5);
			float deltaAngle = 15.652174f;
			float deltaT = 3.0434783f;
			float t = 0f;
			int i = 0;
			while ((float)i < 23f)
			{
				float angle = -90f - (float)i * deltaAngle;
				for (t += deltaT; t > 1f; t -= 1f)
				{
					yield return Wait(1);
				}
				Vector2 offset = BraveMathCollege.GetEllipsePoint(Vector2.zero, 1.39f, 0.92f, angle);
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle - 6f, DirectionType.Absolute, -1f), new Speed(16f, SpeedType.Absolute), new Bullet("cheese", true, false, false));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle, DirectionType.Absolute, -1f), new Speed(16f, SpeedType.Absolute), new Bullet("cheese", true, false, false));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 6f, DirectionType.Absolute, -1f), new Speed(16f, SpeedType.Absolute), new Bullet("cheese", true, false, false));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle - 6f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 6f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle - 186f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle - 180f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 174f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("cheese", 16f, 50, -1, true));
				i++;
			}
			yield break;
		}
    }
}
