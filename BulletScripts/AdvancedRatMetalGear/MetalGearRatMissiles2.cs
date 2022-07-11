using System;
using System.Collections.Generic;
using System.Linq;
using Brave.BulletScript;
using System.Collections;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
	public class MetalGearRatMissiles2 : Script
	{
		public override IEnumerator Top()
		{
			int leftDelay = 0;
			int rightDelay = 60;
			if (BraveUtility.RandomBool())
			{
				BraveUtility.Swap<int>(ref leftDelay, ref rightDelay);
			}
			Vector2 leftBasePos = BulletBank.GetTransform("missile left shoot point").position;
			Vector2 rightBasePos = BulletBank.GetTransform("missile right shoot point").position;
			int[] leftDelays = new int[xOffsets.Length];
			int[] rightDelays = new int[xOffsets.Length];
			for (int j = 0; j < xOffsets.Length; j++)
			{
				leftDelays[j] = 30 + j * 30;
				rightDelays[j] = 30 + j * 30 + 15;
			}
			BraveUtility.RandomizeArray(leftDelays, 0, -1);
			BraveUtility.RandomizeArray(rightDelays, 0, -1);
			for (int i = 0; i < xOffsets.Length; i++)
			{
				int dx = xOffsets[xOffsets.Length - 1 - i];
				int dy = yOffsets[xOffsets.Length - 1 - i];
				Vector2 spawnPos = leftBasePos + PhysicsEngine.PixelToUnit(new IntVector2(dx + 1, dy - 7));
				Fire(Offset.OverridePosition(spawnPos), new Direction(-90f, DirectionType.Absolute, -1f), new HomingBullet(leftDelays[i] - i * 4));
				spawnPos = rightBasePos + PhysicsEngine.PixelToUnit(new IntVector2(-dx + 1, dy - 7));
				Fire(Offset.OverridePosition(spawnPos), new Direction(-90f, DirectionType.Absolute, -1f), new HomingBullet(rightDelays[i] - i * 4));
				yield return Wait(4);
			}
			yield return Wait(220);
			yield break;
		}

		static MetalGearRatMissiles2()
		{
			xOffsets = new int[]
			{
			0,
			-4,
			-7,
			-11,
			-14,
			-18,
			-21,
			-28
			};
			yOffsets = new int[]
			{
			0,
			-7,
			0,
			-7,
			0,
			-7,
			0,
			0
			};
		}

		private static int[] xOffsets;
		private static int[] yOffsets;

		private class HomingBullet : Bullet
		{
			public HomingBullet(int fireDelay = 0) : base("missile", false, false, false)
			{
				m_fireDelay = fireDelay;
			}

			public override void Initialize()
			{
				Projectile.spriteAnimator.StopAndResetFrameToDefault();
				BraveUtility.EnableEmission(Projectile.ParticleTrail, false);
				base.Initialize();
			}

			public override IEnumerator Top()
			{
				if (m_fireDelay > 0)
				{
					yield return Wait(m_fireDelay);
				}
				Projectile.spriteAnimator.Play();
				BraveUtility.EnableEmission(Projectile.ParticleTrail, true);
				PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
				PostWwiseEvent("Play_WPN_YariRocketLauncher_Shot_01", null);
				float t = UnityEngine.Random.value;
				Speed = Mathf.Lerp(8f, 14f, t);
				Vector2 toTarget = BulletBank.PlayerPosition() - Position;
				float travelTime = toTarget.magnitude / Speed * 60f - 1f;
				float magnitude = BraveUtility.RandomSign() * (1f - t) * 8f;
				Vector2 offset = magnitude * toTarget.Rotate(90f).normalized;
				ManualControl = true;
				int startTick = Tick;
				Vector2 truePosition = Position;
				Vector2 lastPosition = Position;
				Vector2 velocity = toTarget.normalized * Speed;
				int i = 0;
				while ((float)i < travelTime)
				{
					truePosition += velocity / 60f;
					lastPosition = Position;
					Position = truePosition + offset * Mathf.Sin((float)(Tick - startTick) / travelTime * 3.1415927f);
					Direction = (Position - lastPosition).ToAngle();
					if(i % 10 == 0)
					{
						Fire(new Direction(Direction - 180f + UnityEngine.Random.Range(-25f, 25f), DirectionType.Absolute, -1), new Speed(UnityEngine.Random.Range(8f, 11f), SpeedType.Absolute));
					}
					yield return Wait(1);
					i++;
				}
				Vector2 v = (Position - lastPosition) * 60f;
				Speed = v.magnitude;
				Direction = v.ToAngle();
				ManualControl = false;
				yield break;
			}

			public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (preventSpawningProjectiles)
				{
					return;
				}
				float num = RandomAngle();
				float num2 = 45f;
				for (int i = 0; i < 8; i++)
				{
					Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), null);
					PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
				}
				DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultFireGoop).TimedAddGoopCircle(Position, 3.5f, 0.5f, false);
			}

			private int m_fireDelay;
		}
	}

}
