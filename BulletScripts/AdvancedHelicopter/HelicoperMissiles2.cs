using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Brave.BulletScript;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedHelicopter
{
	public class HelicopterMissiles2 : Script
	{
		public HelicopterMissiles2()
		{
			s_targets = new string[]
			{
				"shoot point 1",
				"shoot point 2",
				"shoot point 3",
				"shoot point 4"
			};
		}

		public override IEnumerator Top()
		{
			for (int i = 0; i < 12; i++)
			{
				float t = UnityEngine.Random.value;
				float speed = Mathf.Lerp(8f, 11f, t);
				Vector2 target = (!BraveUtility.RandomBool()) ? GetPredictedTargetPositionExact(1f, speed) : BulletManager.PlayerPosition();
				Fire(new Offset(s_targets[i % 4]), new Speed(speed, SpeedType.Absolute), new ArcBullet(target, t));
				PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
				yield return Wait(10);
			}
			yield break;
		}

		public string[] s_targets;

		public class ArcBullet : Bullet
		{
			public ArcBullet(Vector2 target, float t) : base("missile", false, false, false)
			{
				m_target = target;
				m_t = t;
			}

			public override void Initialize()
			{
				tk2dSpriteAnimator spriteAnimator = Projectile.spriteAnimator;
				spriteAnimator.Play();
				spriteAnimator.SetFrame(spriteAnimator.CurrentClip.frames.Length - 1);
				base.Initialize();
			}

			public override IEnumerator Top()
			{
				StartTask(HandleTrail());
				Vector2 toTarget = m_target - Position;
				float trueDirection = toTarget.ToAngle();
				Vector2 truePosition = Position;
				Vector2 lastPosition = Position;
				float travelTime = toTarget.magnitude / Speed * 60f - 1f;
				float magnitude = BraveUtility.RandomSign() * (1f - m_t) * 8f;
				Vector2 offset = magnitude * toTarget.Rotate(90f).normalized;
				ManualControl = true;
				Direction = trueDirection;
				int i = 0;
				while ((float)i < travelTime)
				{
					float angleRad = trueDirection * 0.017453292f;
					Velocity.x = Mathf.Cos(angleRad) * Speed;
					Velocity.y = Mathf.Sin(angleRad) * Speed;
					truePosition += Velocity / 60f;
					lastPosition = Position;
					Position = truePosition + offset * Mathf.Sin((float)Tick / travelTime * 3.1415927f);
					Direction = (Position - lastPosition).ToAngle();
					yield return Wait(1);
					i++;
				}
				Vector2 v = (Position - lastPosition) * 60f;
				Speed = v.magnitude;
				Direction = v.ToAngle();
				ManualControl = false;
				yield break;
			}

			private IEnumerator HandleTrail()
            {
				while(this != null && Projectile != null)
				{
					Fire(new LightningBullet(20));
					yield return Wait(4);
				}
			}

			private Vector2 m_target;
			private float m_t;
		}

		public class LightningBullet : Bullet
        {
			public LightningBullet(int lifetime) : base(null, false, false, false)
            {
				m_lifetime = lifetime;
            }

            public override IEnumerator Top()
            {
				yield return Wait(m_lifetime);
				Vanish(true);
				yield break;
            }

            private int m_lifetime;
        }
	}
}
