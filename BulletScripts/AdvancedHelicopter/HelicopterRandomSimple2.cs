using System;
using System.Collections.Generic;
using System.Linq;
using Brave.BulletScript;
using System.Collections;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedHelicopter
{
	public class HelicopterRandomSimple2 : Script
	{
		public override IEnumerator Top()
		{
			if (UnityEngine.Random.value < 0.5f)
			{
				int numBullets = 12;
				float startDirection = RandomAngle();
				string transform = "shoot point 1";
				string transform2 = "shoot point 4";
				if (BraveUtility.RandomBool())
				{
					BraveUtility.Swap<string>(ref transform, ref transform2);
				}
				for (int i = 0; i < numBullets; i++)
				{
					Fire(new Offset(transform), new Direction(SubdivideCircle(startDirection, numBullets, i, 1f, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BigBullet());
				}
				yield return Wait(15);
				for (int j = 0; j < numBullets; j++)
				{
					Fire(new Offset(transform2), new Direction(SubdivideCircle(startDirection, numBullets, j, 1f, true), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BigBullet());
				}
			}
			else
			{
				int numBullets2 = 8;
				float arc = 45f;
				string transform3 = "shoot point 2";
				string transform4 = "shoot point 3";
				if (BraveUtility.RandomBool())
				{
					BraveUtility.Swap(ref transform3, ref transform4);
				}
				float aimDirection = GetAimDirection(transform3);
				for (int k = 0; k < numBullets2; k++)
				{
					Fire(new Offset(transform3), new Direction(SubdivideArc(aimDirection - arc, arc * 2f, numBullets2, k, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BigBullet());
				}
				yield return Wait(15);
				aimDirection = GetAimDirection(transform4);
				for (int l = 0; l < numBullets2; l++)
				{
					Fire(new Offset(transform4), new Direction(SubdivideArc(aimDirection - arc, arc * 2f, numBullets2, l, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BigBullet());
				}
			}
			yield break;
		}

		public class BigBullet : Bullet
		{
			public BigBullet() : base("big", false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				Projectile.Ramp(UnityEngine.Random.Range(2f, 3f), 2f);
				return null;
			}
		}
	}
}
