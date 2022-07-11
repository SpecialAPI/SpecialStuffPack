using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Brave.BulletScript;
using System.Collections;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
	public class ResourcefulRatMouseTraps2 : Script
	{
		public override IEnumerator Top()
		{
			yield return Wait(56);
			if (s_xValues == null || s_yValues == null)
			{
				s_xValues = new int[10];
				s_yValues = new int[10];
				for (int j = 0; j < 10; j++)
				{
					s_xValues[j] = j;
					s_yValues[j] = j;
				}
			}
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			Vector2 roomLowerLeft = area.UnitBottomLeft;
			Vector2 dimensions = area.dimensions.ToVector2() - new Vector2(0f, 6f);
			Vector2 delta = new Vector2(dimensions.x / 10f, dimensions.y / 10f);
			Vector2 safeZoneLowerLeft = area.UnitBottomLeft + new Vector2(15f, 9f);
			Vector2 safeZoneUpperRight = area.UnitBottomLeft + new Vector2(21f, 15f);
			BraveUtility.RandomizeArray(s_xValues, 0, -1);
			BraveUtility.RandomizeArray(s_yValues, 0, -1);
			for (int i = 0; i < 10; i++)
			{
				int baseX = s_xValues[i];
				int baseY = s_yValues[i];
				Vector2 goalPos = roomLowerLeft + new Vector2(((float)baseX + UnityEngine.Random.value) * delta.x, ((float)baseY + UnityEngine.Random.value) * delta.y);
				if (goalPos.IsWithin(safeZoneLowerLeft, safeZoneUpperRight))
				{
					if (BraveUtility.RandomBool())
					{
						baseX += (int)(BraveUtility.RandomSign() * 3f);
					}
					else
					{
						baseY += (int)(BraveUtility.RandomSign() * 3f);
					}
					goalPos = roomLowerLeft + new Vector2(((float)baseX + UnityEngine.Random.value) * delta.x, ((float)baseY + UnityEngine.Random.value) * delta.y);
				}
				Fire(goalPos);
				yield return Wait(2);
			}
			for(int i = 0; i < 5; i++)
            {
				Fire(BulletBank.aiActor.TargetRigidbody.UnitCenter + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(9f, 11f));
			}
			yield break;
		}

		private void Fire(Vector2 goal, float triggerTime = -1f)
		{
			float direction = (goal - Position).ToAngle();
			GameObject[] mouseTraps = BulletBank.GetComponent<ResourcefulRatController>().MouseTraps;
			Fire(new Direction(direction, DirectionType.Absolute, -1f), new MouseTrapBullet(goal, BraveUtility.RandomElement(mouseTraps), triggerTime));
		}

		private static int[] s_xValues;
		private static int[] s_yValues;

		private class MouseTrapBullet : Bullet
		{
			public MouseTrapBullet(Vector2 goalPos, GameObject mouseTrapPrefab, float triggerTime = -1f) : base("mousetrap", true, false, false)
			{
				m_goalPos = goalPos;
				m_mouseTrapPrefab = mouseTrapPrefab;
				m_triggerTime = triggerTime;
			}

			public override IEnumerator Top()
			{
				ManualControl = true;
				Direction = (m_goalPos - Position).ToAngle();
				Speed = Vector2.Distance(m_goalPos, Position) / 1f;
				Vector2 truePosition = Position;
				for (int i = 0; i < 60; i++)
				{
					truePosition += BraveMathCollege.DegreesToVector(Direction, Speed / 60f);
					Position = truePosition + new Vector2(0f, Mathf.Sin((float)i / 60f * 3.1415927f) * 3.5f);
					yield return Wait(1);
				}
				GameObject go = UnityEngine.Object.Instantiate(m_mouseTrapPrefab, Projectile.specRigidbody.UnitCenter + new Vector2(-0.8f, -1.2f), Quaternion.identity);
				go.GetComponent<SpeculativeRigidbody>().Initialize();
				if(m_triggerTime > 0f)
                {
					go.GetComponent<BasicTrapController>().StartCoroutine(TimedTrapTrigger(m_triggerTime, go));
                }
				DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(Projectile.specRigidbody.UnitCenter, 3.5f, 0.5f, false);
				Vanish(true);
				yield break;
			}

			public static IEnumerator TimedTrapTrigger(float delay, GameObject trap)
            {
				yield return new WaitForSeconds(delay);
				if(trap != null && trap.GetComponent<BasicTrapController>() != null)
                {
					trap.GetComponent<BasicTrapController>().Trigger();
					UnityEngine.Object.Destroy(trap, 1.5f);
                }
            }

			private Vector2 m_goalPos;
			private GameObject m_mouseTrapPrefab;
			private float m_triggerTime;
		}
	}
}
