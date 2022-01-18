using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using System.Collections;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
	public class ResourcefulRatQuickDaggers2 : Script
	{
		public ResourcefulRatQuickDaggers2()
		{
			m_reticles = new List<GameObject>();
		}

		protected override IEnumerator Top()
		{
			float[] angles = new float[4];
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			for (int i = 0; i < 1; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					float num = AimDirection;
					float d = 0.43333334f;
					if (j != 3)
					{
						int num2 = j / 2;
						bool flag = j % 2 == 1;
						Vector2 vector = IntVector2.CardinalsAndOrdinals[num2].ToVector2();
						float d2 = (!flag) ? 7f : 8.15f;
						Vector2 vector2 = BulletManager.PlayerPosition();
						Vector2 a = vector.normalized * d2;
						vector2 += a * d;
						Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, BulletManager.PlayerVelocity(), Position, 60f);
						num = (predictedPosition - Position).ToAngle();
					}
					for (int k = 0; k < j; k++)
					{
						if (!float.IsNaN(angles[k]) && BraveMathCollege.AbsAngleBetween(angles[k], num) < 3f)
						{
							num = float.NaN;
						}
					}
					angles[j] = num;
					if (!float.IsNaN(angles[j]))
					{
						ResourcefulRatController component = BulletBank.GetComponent<ResourcefulRatController>();
						float num3 = 20f;
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(Position, Position + BraveMathCollege.DegreesToVector(num, 60f), area.UnitBottomLeft, area.UnitTopRight - new Vector2(0f, 6f), ref zero))
						{
							num3 = (zero - Position).magnitude;
						}
						GameObject gameObject = SpawnManager.SpawnVFX(component.ReticleQuad, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(Position.x, Position.y, Position.y) + BraveMathCollege.DegreesToVector(num, 2f).ToVector3ZUp(0f);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, num);
						component2.dimensions = new Vector2((num3 - 3f) * 16f, 5f);
						component2.UpdateZDepth();
						m_reticles.Add(gameObject);
					}
				}
				yield return Wait(26);
				CleanupReticles();
				float randomAngle = RandomAngle();
				for (int a = 0; a < 30; a++)
				{
					Fire(new Offset(new Vector2(0.5f, 0f), randomAngle + a * 12f, string.Empty, DirectionType.Absolute), new Direction(randomAngle + a * 12f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet("dagger", true,
						false, false));
				}
				GameObject[] mouseTraps = BulletBank.GetComponent<ResourcefulRatController>().MouseTraps;
				for (int l = 0; l < 4; l++)
				{
					if (!float.IsNaN(angles[l]))
					{
						Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(60f, SpeedType.Absolute), new ResourcefulRatDaggers2.DaggerBullet(4,
							BraveUtility.RandomElement(mouseTraps), 1f));
					}
				}
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
			return BraveMathCollege.GetPredictedPosition(vector, BulletManager.PlayerVelocity(), base.Position, speed);
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
	}
}
