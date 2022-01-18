using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunSweepFlameBreath3 : Script
    {
        protected override IEnumerator Top()
        {
			for (; ; )
			{
				Fire(new Direction(UnityEngine.Random.Range(-45f, 45f), DirectionType.Relative, -1f), new Speed(14f, SpeedType.Absolute), new Bullet("Sweep", false, false, false));
				if (Tick % 2 == 1)
				{
					Fire(new Direction(UnityEngine.Random.Range(-15f, 15f), DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(2, 8), SpeedType.Absolute), new SpeedChangingBullet("Sweep", 14f, 120, -1, false));
				}
				else
				{
					Fire(new Direction(UnityEngine.Random.Range(-15f, 15f), DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(16, 22), SpeedType.Absolute), new SpeedChangingBullet("Sweep", 14f, 120, -1, false));
				}
				yield return Wait(1);
			}
		}
    }
}
