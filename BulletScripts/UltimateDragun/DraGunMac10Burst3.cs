using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunMac10Burst3 : Script
    {
		public override IEnumerator Top()
		{
			yield return Wait(1);
			Vector2 lastPosition = Position;
			PostWwiseEvent("Play_BOSS_Dragun_Uzi_01", null);
			for (; ; )
			{
				if (Vector2.Distance(lastPosition, Position) > 1f)
				{
					Fire(new Offset((lastPosition - Position) * 0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Relative, -1f), new DraGunMac10Burst2.UziBullet());
					Fire(new Offset((lastPosition - Position) * 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Relative, -1f), new DraGunMac10Burst2.UziBullet());
					Fire(new Offset((lastPosition - Position) * 0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new Bullet("UziBullet"));
					Fire(new Offset((lastPosition - Position) * 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new Bullet("UziBullet"));
					Fire(new Offset((lastPosition - Position) * 0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(8, SpeedType.Absolute), new Bullet("UziBullet"));
					Fire(new Offset((lastPosition - Position) * 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(8, SpeedType.Absolute), new Bullet("UziBullet"));
					Fire(new Offset((lastPosition - Position) * 0.33f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(12, SpeedType.Absolute), new Bullet("UziBullet"));
					Fire(new Offset((lastPosition - Position) * 0.66f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(12, SpeedType.Absolute), new Bullet("UziBullet"));
				}
				Fire(new Direction(0f, DirectionType.Relative, -1f), new DraGunMac10Burst2.UziBullet());
				Fire(new Direction(0f, DirectionType.Relative, -1f), new Speed(10, SpeedType.Absolute), new Bullet("UziBullet"));
				Fire(new Direction(0f, DirectionType.Relative, -1f), new Speed(8, SpeedType.Absolute), new Bullet("UziBullet"));
				Fire(new Direction(0f, DirectionType.Relative, -1f), new Speed(12, SpeedType.Absolute), new Bullet("UziBullet"));
				lastPosition = Position;
				yield return Wait(UnityEngine.Random.Range(2, 4));
			}
		}

		public class UziBullet : Bullet
		{
			public UziBullet() : base("UziBullet", false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				yield return Wait(30);
				Fire(new Direction(UnityEngine.Random.Range(-45f, 45f), DirectionType.Relative, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("UziBurst", false, false, false));
				yield return Wait(30);
				Fire(new Direction(0f, DirectionType.Relative, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("UziBurst", false, false, false));
				yield return Wait(30);
				Speed = 12f;
				Direction = RandomAngle();
				yield break;
			}
		}
	}
}
