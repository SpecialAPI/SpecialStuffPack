using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
	public class MetalGearRatJumpPound2 : Script
	{
		public override IEnumerator Top()
		{
			float deltaAngle = 8.372093f;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 43; k++)
					{
						float num = -90f - ((float)k + (float)j * 0.5f) * deltaAngle;
						Vector2 ellipsePointSmooth = BraveMathCollege.GetEllipsePointSmooth(Vector2.zero, 6f, 2f, num);
						Fire(new Offset(ellipsePointSmooth, 0f, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(13f, SpeedType.Absolute), new DelayedBullet("default_noramp", j * 4));
					}
				}
				yield return Wait(40);
			}
			yield break;
		}
	}
}
