using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunRocket3 : Script
    {
        public override IEnumerator Top()
        {
            Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new Rocket());
			yield return Wait(120);
			Fire(new Direction(-60f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new RocketWeak());
			yield return Wait(60);
			Fire(new Direction(-120f, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new RocketWeak());
			yield break;
        }

		public class Rocket : Bullet
		{
			public Rocket() : base("rocket", false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				return null;
			}

			public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				for (int i = 0; i < 42; i++)
				{
					Fire(new Direction(SubdivideArc(-10f, 200f, 42, i, false), DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("default_novfx", false, false, false));
					if (i < 41)
					{
						Fire(new Direction(SubdivideArc(-10f, 200f, 42, i, true), DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
					}
					Fire(new Direction(SubdivideArc(-10f, 200f, 42, i, false), DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
				}
				for (int j = 0; j < 5; j++)
				{
					Fire(new Offset(new Vector2(0f, -1f), 0f, string.Empty, DirectionType.Absolute), new Direction(180f, DirectionType.Absolute, -1f), new Speed(16 - j * 4, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
					Fire(new Offset(new Vector2(0f, -1f), 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Absolute, -1f), new Speed(16 - j * 4, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
				}
				for (int k = 0; k < 16; k++)
				{
					float direction;
					if (k % 2 == 0)
					{
						direction = UnityEngine.Random.Range(150f, 182f);
					}
					else
					{
						direction = UnityEngine.Random.Range(0f, 35f);
					}
					Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(6f, 14f), SpeedType.Absolute), new DraGunRocket2.ShrapnelBullet());
				}
			}
		}

		public class RocketWeak : Bullet
		{
			public RocketWeak() : base("rocket", false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				return null;
			}

			public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				for (int i = 0; i < 21; i++)
				{
					Fire(new Direction(SubdivideArc(-10f, 200f, 21, i, false), DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("default_novfx", false, false, false));
					if (i < 20)
					{
						Fire(new Direction(SubdivideArc(-10f, 200f, 21, i, true), DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
					}
					Fire(new Direction(SubdivideArc(-10f, 200f, 21, i, false), DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
				}
				for (int j = 0; j < 2; j++)
				{
					Fire(new Offset(new Vector2(0f, -1f), 0f, string.Empty, DirectionType.Absolute), new Direction(180f, DirectionType.Absolute, -1f), new Speed(16 - j * 4, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
					Fire(new Offset(new Vector2(0f, -1f), 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Absolute, -1f), new Speed(16 - j * 4, SpeedType.Absolute), new SpeedChangingBullet("default_novfx", 12f, 60, -1, false));
				}
				for (int k = 0; k < 8; k++)
				{
					float direction;
					if (k % 2 == 0)
					{
						direction = UnityEngine.Random.Range(150f, 182f);
					}
					else
					{
						direction = UnityEngine.Random.Range(0f, 35f);
					}
					Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(6f, 14f), SpeedType.Absolute), new DraGunRocket2.ShrapnelBullet());
				}
			}
		}

		public class ShrapnelBullet : Bullet
		{
			public ShrapnelBullet() : base("shrapnel", false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				ChangeSpeed(new Speed(12f, SpeedType.Absolute), 60);
				BounceProjModifier bounce = Projectile.GetComponent<BounceProjModifier>();
				bool hasBounced = false;
				ManualControl = true;
				yield return Wait(UnityEngine.Random.Range(0, 10));
				Vector2 truePosition = Position;
				float trueDirection = Direction;
				for (int i = 0; i < 360; i++)
				{
					if (!hasBounced && bounce.numberOfBounces == 0)
					{
						trueDirection = BraveMathCollege.QuantizeFloat(trueDirection, 90f) + 180f;
						Speed = 18f;
						hasBounced = true;
					}
					float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
					Vector2 lastPosition = truePosition;
					truePosition += BraveMathCollege.DegreesToVector(trueDirection, Speed / 60f);
					Position = truePosition + BraveMathCollege.DegreesToVector(trueDirection - 90f, offsetMagnitude);
					Direction = (truePosition - lastPosition).ToAngle();
					Projectile.transform.rotation = Quaternion.Euler(0f, 0f, Direction);
					yield return Wait(1);
				}
				Vanish(false);
				yield break;
			}
		}
	}
}
