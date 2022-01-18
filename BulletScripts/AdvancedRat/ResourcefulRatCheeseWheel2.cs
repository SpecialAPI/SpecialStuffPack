using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;
using System.Collections;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
	public class ResourcefulRatCheeseWheel2 : Script
	{
		protected override IEnumerator Top()
		{
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			Vector2 roomLowerLeft = area.UnitBottomLeft;
			Vector2 roomUpperRight = area.UnitTopRight - new Vector2(0f, 3f);
			Vector2 roomCenter = area.UnitCenter - new Vector2(0f, 2.5f);
			PostWwiseEvent("Play_BOSS_Rat_Cheese_Summon_01", null);
			for (int i = 0; i < 3; i++)
			{
				int misfireIndex = UnityEngine.Random.Range(0, 15);
				for (int j = 0; j < 20; j++)
				{
					Vector2 vector = new Vector2(roomLowerLeft.x, SubdivideRange(roomLowerLeft.y, roomUpperRight.y, 21, j, true));
					vector += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
					vector.x -= 1.25f;
					bool isMisfire = j >= misfireIndex && j < misfireIndex + 5;
					FireWallBullet(0f, vector, roomCenter, isMisfire);
				}
				misfireIndex = UnityEngine.Random.Range(0, 15);
				for (int k = 0; k < 20; k++)
				{
					Vector2 vector2 = new Vector2(SubdivideRange(roomLowerLeft.x, roomUpperRight.x, 21, k, true), roomUpperRight.y);
					vector2 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
					vector2.y += 3.25f;
					bool isMisfire2 = k >= misfireIndex && k < misfireIndex + 5;
					FireWallBullet(-90f, vector2, roomCenter, isMisfire2);
				}
				misfireIndex = UnityEngine.Random.Range(0, 15);
				for (int l = 0; l < 20; l++)
				{
					Vector2 vector3 = new Vector2(roomUpperRight.x, SubdivideRange(roomLowerLeft.y, roomUpperRight.y, 21, l, true));
					vector3 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
					vector3.x += 1.25f;
					bool isMisfire3 = l >= misfireIndex && l < misfireIndex + 5;
					FireWallBullet(180f, vector3, roomCenter, isMisfire3);
				}
				misfireIndex = UnityEngine.Random.Range(0, 15);
				for (int m = 0; m < 20; m++)
				{
					Vector2 vector4 = new Vector2(SubdivideRange(roomLowerLeft.x, roomUpperRight.x, 21, m, true), roomLowerLeft.y);
					vector4 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
					vector4.y -= 1.25f;
					bool isMisfire4 = m >= misfireIndex && m < misfireIndex + 5;
					FireWallBullet(90f, vector4, roomCenter, isMisfire4);
				}
				if (i == 2)
				{
					EndOnBlank = true;
				}
				yield return Wait(75);
			}
			yield return Wait(125);
			AIActor aiActor = BulletBank.aiActor;
			aiActor.aiAnimator.PlayUntilFinished("cheese_wheel_out", false, null, -1f, false);
			aiActor.IsGone = true;
			aiActor.specRigidbody.CollideWithOthers = false;
			Fire(Offset.OverridePosition(roomCenter), new Speed(0f, SpeedType.Absolute), new CheeseWheelBullet());
			yield return Wait(65);
			aiActor.IsGone = false;
			aiActor.specRigidbody.CollideWithOthers = true;
			yield return Wait(105);
			yield break;
		}

		public override void OnForceEnded()
		{
			AIActor aiActor = base.BulletBank.aiActor;
			aiActor.IsGone = false;
			aiActor.specRigidbody.CollideWithOthers = true;
		}

		private void FireWallBullet(float facingDir, Vector2 spawnPos, Vector2 roomCenter, bool isMisfire)
		{
			float angleDeg = (spawnPos - roomCenter).ToAngle();
			int num = Mathf.RoundToInt(BraveMathCollege.ClampAngle360(angleDeg) / 45f) % 8;
			float num2 = (float)num * 45f;
			Vector2 targetPos = (roomCenter + BraveMathCollege.DegreesToVector(num2, 0.875f) + TargetOffsets[num]).Quantize(0.0625f);
			Fire(Offset.OverridePosition(spawnPos), new Direction(facingDir, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new CheeseWedgeBullet(this, TargetNames[num], RampHeights[num], targetPos, num2 + 180f, isMisfire,
				roomCenter));
		}

		static ResourcefulRatCheeseWheel2()
		{
			TargetNames = new string[]
			{
			"cheeseWedge0",
			"cheeseWedge1",
			"cheeseWedge2",
			"cheeseWedge3",
			"cheeseWedge4",
			"cheeseWedge5",
			"cheeseWedge6",
			"cheeseWedge7"
			};
			RampHeights = new float[]
			{
			2f,
			1f,
			0f,
			1f,
			2f,
			3f,
			4f,
			2f
			};
			TargetOffsets = new Vector2[]
			{
			new Vector2(0f, 0.0625f),
			new Vector2(0.0625f, -0.0625f),
			new Vector2(0.0625f, 0f),
			new Vector2(0.0625f, -0.0625f),
			new Vector2(0.0625f, 0.0625f),
			new Vector2(0f, 0f),
			new Vector2(0.0625f, 0f),
			new Vector2(0.125f, -0.125f)
			};
		}

		private static string[] TargetNames;
		private static float[] RampHeights;
		private static Vector2[] TargetOffsets;

		public class CheeseWedgeBullet : Bullet
		{
			public CheeseWedgeBullet(ResourcefulRatCheeseWheel2 parent, string bulletName, float additionalRampHeight, Vector2 targetPos, float endingAngle, bool isMisfire, Vector2 roomCenter) : base(bulletName, true, false, false)
			{
				m_parent = parent;
				m_targetPos = targetPos;
				m_endingAngle = endingAngle;
				m_isMisfire = isMisfire;
				m_additionalRampHeight = additionalRampHeight;
				m_roomCenter = roomCenter;
			}

			protected override IEnumerator Top()
			{
				int travelTime = UnityEngine.Random.Range(90, 136);
				Projectile.IgnoreTileCollisionsFor(90f);
				Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
				Projectile.sprite.HeightOffGround = 10f + m_additionalRampHeight + UnityEngine.Random.value / 2f;
				Projectile.sprite.ForceRotationRebuild();
				Projectile.sprite.UpdateZDepth();
				int r = UnityEngine.Random.Range(0, 20);
				yield return Wait(15 + r);
				Speed = 2.5f;
				yield return Wait(50 - r);
				Speed = 0f;
				if (m_isMisfire)
				{
					Direction += 180f;
					Speed = 2.5f;
					yield return Wait(180);
					Vanish(true);
					yield break;
				}
				Direction = (m_targetPos - Position).ToAngle();
				ChangeSpeed(new Speed((m_targetPos - Position).magnitude / ((float)(travelTime - 15) / 60f), SpeedType.Absolute), 30);
				yield return Wait(travelTime);
				Speed = 0f;
				Position = m_targetPos;
				Direction = m_endingAngle;
				m_parent.Fire(Offset.OverridePosition(m_roomCenter), new Direction(RandomAngle(), DirectionType.Absolute, -1), new Speed(10f, SpeedType.Absolute), new Bullet("cheese", true, false, BraveUtility.RandomBool()));
				if (Projectile && Projectile.sprite)
				{
					Projectile.sprite.HeightOffGround -= 1f;
					Projectile.sprite.UpdateZDepth();
				}
				int totalTime = 350;
				yield return Wait(totalTime - m_parent.Tick);
				Vanish(true);
				yield break;
			}

			private ResourcefulRatCheeseWheel2 m_parent;
			private Vector2 m_targetPos;
			private float m_endingAngle;
			private bool m_isMisfire;
			private Vector2 m_roomCenter;
			private float m_additionalRampHeight;
		}

		public class CheeseWheelBullet : Bullet
		{
			public CheeseWheelBullet() : base("cheeseWheel", true, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				Projectile.spriteAnimator.Play("cheese_wheel_burst");
				Projectile.ImmuneToSustainedBlanks = true;
				yield return Wait(45);
				Projectile.Ramp(-1.5f, 100f);
				yield return Wait(80);
				for (int i = 0; i < 80; i++)
				{
					Bullet bullet = new Bullet("cheese", true, false, false);
					Fire(new Direction(RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(12f, 33f), SpeedType.Absolute), bullet);
					bullet.Projectile.ImmuneToSustainedBlanks = true;
				}
				float randomAngle = RandomAngle();
				for(int i = 0; i < 30; i++)
				{
					Bullet bullet = new Bullet("cheese", true, false, false);
					Fire(new Direction(randomAngle + i * 12f, DirectionType.Absolute, -1f), new Speed(12), bullet);
					bullet.Projectile.ImmuneToSustainedBlanks = true;
					Bullet bullet2 = new Bullet("cheese", true, false, false);
					Fire(new Direction(randomAngle + i * 12f, DirectionType.Absolute, -1f), new Speed(33), bullet2);
					bullet2.Projectile.ImmuneToSustainedBlanks = true;
				}
				if (BulletBank)
				{
					ResourcefulRatController component = BulletBank.GetComponent<ResourcefulRatController>();
					if (component)
					{
						GameManager.Instance.MainCameraController.DoScreenShake(component.cheeseSlamScreenShake, null, false);
					}
				}
				yield return Wait(25);
				Vanish(true);
				yield break;
			}
		}
	}
}
