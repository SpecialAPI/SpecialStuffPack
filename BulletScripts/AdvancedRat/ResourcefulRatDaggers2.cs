using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeonator;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
	public class ResourcefulRatDaggers2 : Script
	{
		public ResourcefulRatDaggers2()
		{
			m_reticles = new List<GameObject>();
		}

		protected override IEnumerator Top()
		{
			yield return Wait(18);
			float[] angles = new float[17];
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			int totalAttackTicks = 56;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 17; j++)
				{
					float angle = AimDirection;
					float timeUntilFire = (float)(totalAttackTicks - j) / 60f;
					if (j != 16)
					{
						int num = j / 2;
						bool flag = j % 2 == 1;
						Vector2 vector = IntVector2.CardinalsAndOrdinals[num].ToVector2();
						float d = (!flag) ? 7f : 8.15f;
						Vector2 vector2 = BulletManager.PlayerPosition();
						Vector2 a = vector.normalized * d;
						vector2 += a * timeUntilFire;
						Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, BulletManager.PlayerVelocity(), Position, 60f);
						angle = (predictedPosition - Position).ToAngle();
					}
					for (int k = 0; k < j; k++)
					{
						if (!float.IsNaN(angles[k]) && BraveMathCollege.AbsAngleBetween(angles[k], angle) < 3f)
						{
							angle = float.NaN;
						}
					}
					angles[j] = angle;
					if (!float.IsNaN(angles[j]))
					{
						ResourcefulRatController component = BulletBank.GetComponent<ResourcefulRatController>();
						float num2 = 20f;
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(Position, Position + BraveMathCollege.DegreesToVector(angle, 60f), area.UnitBottomLeft, area.UnitTopRight - new Vector2(0f, 6f), ref zero))
						{
							num2 = (zero - Position).magnitude;
						}
						GameObject gameObject = SpawnManager.SpawnVFX(component.ReticleQuad, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(Position.x, Position.y, Position.y) + BraveMathCollege.DegreesToVector(angle, 2f).ToVector3ZUp(0f);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2((num2 - 3f) * 16f, 5f);
						component2.UpdateZDepth();
						m_reticles.Add(gameObject);
					}
					if (j < 16)
					{
						yield return Wait(1);
					}
				}
				yield return Wait(15);
				CleanupReticles();
				yield return Wait(25);
				float randomAngle = RandomAngle();
				int numBulletsToFire = 18;
				for(int a = 0; a < numBulletsToFire; a++)
                {
					Fire(new Offset(new Vector2(0.5f, 0f), randomAngle + a * (360 / numBulletsToFire), string.Empty, DirectionType.Absolute), new Direction(randomAngle + a * (360 / numBulletsToFire), DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("dagger", true, 
						false, false));
                }
				for (int l = 0; l < 17; l++)
				{
					if (!float.IsNaN(angles[l]))
					{
						Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(60f, SpeedType.Absolute), new Bullet("dagger", true, false, false));
						if(l % 2 == 0)
						{
							DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopLine(Position + BraveMathCollege.DegreesToVector(angles[l], 0.5f), Position + BraveMathCollege.DegreesToVector(angles[l], 30f), 1f, 
								1f);
						}
					}
				}
				yield return Wait(12);
				for (int m = 0; m < 17; m++)
				{
					if (!float.IsNaN(angles[m]))
					{
						Fire(new Offset(new Vector2(0.5f, 0f), angles[m], string.Empty, DirectionType.Absolute), new Direction(angles[m], DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new Bullet("dagger", true, false, false));
					}
				}
				yield return Wait(24);
			}
			yield break;
		}

		public override void OnForceEnded()
		{
			CleanupReticles();
		}

		public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
		{
			Vector2 vector = BulletManager.PlayerPosition();
			Vector2 a = BulletManager.PlayerVelocity();
			vector += a * fireDelay;
			return BraveMathCollege.GetPredictedPosition(vector, BulletManager.PlayerVelocity(), Position, speed);
		}

		private void CleanupReticles()
		{
			for (int i = 0; i < m_reticles.Count; i++)
			{
				SpawnManager.Despawn(m_reticles[i]);
			}
			m_reticles.Clear();
		}

		private List<GameObject> m_reticles;

		public class DaggerBullet : Bullet
        {
			public DaggerBullet(int mouseTrapSpawnInterval, GameObject mouseTrap, float trapTriggerTime) : base("dagger", true, false, false)
            {
				m_trapSpawnInterval = mouseTrapSpawnInterval;
				m_mouseTrapObject = mouseTrap;
				m_trapTriggerTime = trapTriggerTime;
            }

            protected override IEnumerator Top()
            {
                while (Projectile != null)
                {
					if(Projectile != null)
                    {
						GameObject go = UnityEngine.Object.Instantiate(m_mouseTrapObject, Projectile.specRigidbody.UnitCenter + new Vector2(-0.8f, -1.2f), Quaternion.identity);
						go.GetComponent<SpeculativeRigidbody>().Initialize();
						if (m_trapTriggerTime > 0f)
						{
							go.GetComponent<BasicTrapController>().StartCoroutine(TimedTrapTrigger(m_trapTriggerTime, go));
						}
					}
					yield return Wait(m_trapSpawnInterval);
                }
				yield break;
            }

			public static IEnumerator TimedTrapTrigger(float delay, GameObject trap)
			{
				yield return new WaitForSeconds(delay);
				if (trap != null && trap.GetComponent<BasicTrapController>() != null)
				{
					trap.GetComponent<BasicTrapController>().Trigger();
					UnityEngine.Object.Destroy(trap, 1.5f);
				}
			}

			private int m_trapSpawnInterval;
			private GameObject m_mouseTrapObject;
			private float m_trapTriggerTime;
		}
	}
}
