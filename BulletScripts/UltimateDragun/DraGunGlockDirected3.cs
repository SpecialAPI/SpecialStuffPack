using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunGlockDirected3 : Script
    {
		protected virtual string BulletName
		{
			get
			{
				return "glock";
			}
		}

		protected virtual bool IsHard
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator Top()
		{
			float num = BraveMathCollege.ClampAngle180(Direction);
			bool flag = num > -91f && num < -89f;
			float aimDirection = AimDirection;
			int num4;
			if (flag || BraveMathCollege.AbsAngleBetween(aimDirection, -90f) < 45f)
			{
				float num2 = (aimDirection + -180f) / 2f;
				float num3 = 90f;
				num4 = 20;
				for (int i = 0; i < num4; i++)
				{
					Fire(new Direction(SubdivideArc(num2 - num3, num3 * 2f, num4, i, false), DirectionType.Absolute, -1f), new Speed((float)UnityEngine.Random.Range(6, 11), SpeedType.Absolute), new SpeedChangingBullet(BulletName, 11f, 60, -1, false));
				}
				Fire(new Direction(aimDirection, DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), new SpeedChangingBullet(BulletName, 11f, 60, -1, false));
				if (IsHard)
				{
					for (int j = 0; j < num4; j++)
					{
						Fire(new Direction(SubdivideArc(num2 - num3, num3 * 2f, num4, j, false), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(BulletName, (float)UnityEngine.Random.Range(9, 12), 60, -1, false));
					}
					Fire(new Direction(aimDirection, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(BulletName, 11f, 60, -1, false));
				}
			}
			num4 = 18;
			float startAngle = RandomAngle();
			for (int k = 0; k < num4; k++)
			{
				Fire(new Direction(SubdivideCircle(startAngle, num4, k, 1f, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(BulletName + "_spin", false, false, false));
			}
			if (IsHard)
			{
				for (int l = 0; l < num4; l++)
				{
					Fire(new Direction(SubdivideCircle(startAngle, num4, l, 1f, true), DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new SpeedChangingBullet(BulletName + "_spin", 9f, 60, -1, false));
				}
			}
			Fire(new Direction(AimDirection, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(BulletName, false, false, false));
			yield break;
		}
	}
}
