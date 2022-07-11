using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
	public class MetalGearRatTailgun2 : Script
	{
		private bool Center;
		private bool Done;

		public override IEnumerator Top()
		{
			EndOnBlank = true;
			TargetDummy targetDummy = new TargetDummy();
			targetDummy.Position = BulletBank.aiActor.ParentRoom.area.UnitCenter + new Vector2(0f, 4.5f);
			targetDummy.Direction = AimDirection;
			targetDummy.BulletManager = BulletManager;
			targetDummy.Initialize();
			for (int j = 0; j < 16; j++)
			{
				float angle = SubdivideCircle(0f, 16, j, 1f, false);
				Vector2 overridePosition = targetDummy.Position + BraveMathCollege.DegreesToVector(angle, 0.75f);
				Fire(Offset.OverridePosition(overridePosition), new TargetBullet(this, targetDummy));
			}
			Fire(Offset.OverridePosition(targetDummy.Position), new TargetBullet(this, targetDummy));
			for (int k = 0; k < 5; k++)
			{
				float angle2 = (float)(k * 72);
				for (int l = 1; l < 4; l++)
				{
					float magnitude = 0.75f + Mathf.Lerp(0f, 0.625f, (float)l / 3f);
					Vector2 overridePosition2 = targetDummy.Position + BraveMathCollege.DegreesToVector(angle2, magnitude);
					Fire(Offset.OverridePosition(overridePosition2), new TargetBullet(this, targetDummy));
				}
			}
			for (int i = 0; i < 360; i++)
			{
				targetDummy.DoTick();
				yield return Wait(1);
			}
			Fire(Offset.OverridePosition(targetDummy.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new BigBullet());
			PostWwiseEvent("Play_BOSS_RatMech_Whistle_01", null);
			Center = true;
			yield return Wait(60);
			Done = true;
			yield return Wait(60);
			yield break;
		}

		public class TargetDummy : Bullet
		{
			public TargetDummy() : base(null, false, false, false)
			{
			}

			public override IEnumerator Top()
			{
				for (; ; )
				{
					float distToTarget = (BulletManager.PlayerPosition() - Position).magnitude;
					if (Tick < 30)
					{
						Speed = 0f;
					}
					else
					{
						float a = Mathf.Lerp(12f, 4f, Mathf.InverseLerp(7f, 4f, distToTarget));
						Speed = Mathf.Min(a, (float)(Tick - 30) / 60f * 10f);
					}
					ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
					yield return Wait(1);
				}
			}
		}

		public class TargetBullet : Bullet
		{
			public TargetBullet(MetalGearRatTailgun2 parent, TargetDummy targetDummy) : base("target", false, false, false)
			{
				m_parent = parent;
				m_targetDummy = targetDummy;
			}

			public override IEnumerator Top()
			{
				Vector2 toCenter = Position - m_targetDummy.Position;
				float angle = toCenter.ToAngle();
				float radius = toCenter.magnitude;
				float deltaRadius = radius / 60f;
				ManualControl = true;
				Projectile.specRigidbody.CollideWithTileMap = false;
				Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
				while (!m_parent.Destroyed && !m_parent.IsEnded && !m_parent.Done)
				{
					if (Tick < 60)
					{
						radius += deltaRadius * 3f;
					}
					if (m_parent.Center)
					{
						radius -= deltaRadius * 2f;
					}
					angle += 1.3333334f;
					Position = m_targetDummy.Position + BraveMathCollege.DegreesToVector(angle, radius);
					yield return Wait(1);
				}
				Vanish(false);
				PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
				yield break;
			}

			private MetalGearRatTailgun2 m_parent;
			private TargetDummy m_targetDummy;
		}

		private class BigBullet : Bullet
		{
			public BigBullet() : base("big_one", false, false, false)
			{
			}

			public override void Initialize()
			{
				Projectile.spriteAnimator.StopAndResetFrameToDefault();
				base.Initialize();
			}

			public override IEnumerator Top()
			{
				Projectile.specRigidbody.CollideWithTileMap = false;
				Projectile.specRigidbody.CollideWithOthers = false;
				yield return Wait(60);
				Speed = 0f;
				Projectile.spriteAnimator.Play();
				float startingAngle = RandomAngle();
				for (int i = 0; i < 8; i++)
				{
					bool flag = i % 2 == 0;
					for (int j = 0; j < 28; j++)
					{
						float startAngle = startingAngle;
						int numBullets = 39;
						int i2 = j;
						bool offset = flag;
						float direction = SubdivideCircle(startAngle, numBullets, i2, 1f, offset);
						Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(12f, 17 * i, -1));
					}
				}
				yield return Wait(30);
				Vanish(true);
				yield break;
			}

			public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (preventSpawningProjectiles)
				{
					return;
				}
			}
		}
	}
}
