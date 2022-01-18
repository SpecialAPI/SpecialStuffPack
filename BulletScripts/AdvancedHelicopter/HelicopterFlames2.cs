using System;
using System.Collections.Generic;
using System.Linq;
using Brave.BulletScript;
using UnityEngine;
using System.Collections;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedHelicopter
{
	public class HelicopterFlames2 : Script
	{
		protected override IEnumerator Top()
		{
			List<AIActor> spawnedActors = new List<AIActor>();
			Vector2 basePos = BulletBank.aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(5f, 22.8f);
			float height = 2.1f;
			float radius = 3f;
			float[] xPos = new float[]
			{
			(float)UnityEngine.Random.Range(4, 13),
			(float)UnityEngine.Random.Range(21, 30)
			};
			for (int i = 0; i < 2; i++)
			{
				float num = xPos[i];
				float num2 = UnityEngine.Random.Range(0f, 0.8f * height);
				for (int j = 0; j < 9; j++)
				{
					Vector2 vector = basePos + new Vector2(num - radius, (float)j * -height - num2);
					float direction = (vector - Position).ToAngle();
					Fire(new Offset(s_Transforms[i * 2]), new Direction(direction, DirectionType.Absolute, -1f), new FlameBullet(vector, spawnedActors, 60 + 5 * j));
					vector.x += radius;
					direction = (vector - Position).ToAngle();
					Fire(new Offset(s_Transforms[i * 2 + 1]), new Direction(direction, DirectionType.Absolute, -1f), new FlameBullet(vector, spawnedActors, 60 + 5 * j));
					vector.x += radius;
					direction = (vector - Position).ToAngle();
					Fire(new Offset(s_Transforms[i * 2 + 1]), new Direction(direction, DirectionType.Absolute, -1f), new FlameBullet(vector, spawnedActors, 60 + 5 * j));
				}
			}
			yield return Wait(105);
			GoopDefinition goop = BulletBank.GetComponent<GoopDoer>().goopDefinition;
			DeadlyDeadlyGoopManager gooper = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goop);
			for (int k = 0; k < 2; k++)
			{
				Vector2 vector2 = basePos + new Vector2(xPos[k], 0f);
				Vector2 p = vector2 + new Vector2(0f, -18f);
				gooper.TimedAddGoopLine(vector2, p, radius, 1f);
			}
			yield break;
		}

		static HelicopterFlames2()
		{
			s_Transforms = new string[]
			{
				"shoot point 1",
				"shoot point 2",
				"shoot point 3",
				"shoot point 4"
			};
		}

		private static string[] s_Transforms;

		private class FlameBullet : Bullet
		{
			public FlameBullet(Vector2 goalPos, List<AIActor> spawnedActors, int flightTime) : base("flame", false, false, false)
			{
				m_goalPos = goalPos;
				m_flightTime = flightTime;
			}

			protected override IEnumerator Top()
			{
				Projectile.IgnoreTileCollisionsFor((float)(m_flightTime - 5) / 60f);
				Projectile.spriteAnimator.Play();
				ManualControl = true;
				Direction = (m_goalPos - Position).ToAngle();
				Speed = Vector2.Distance(m_goalPos, Position) / ((float)m_flightTime / 60f);
				Vector2 truePosition = Position;
				for (int i = 0; i < m_flightTime; i++)
				{
					truePosition += BraveMathCollege.DegreesToVector(Direction, Speed / 60f);
					Position = truePosition + new Vector2(0f, Mathf.Sin((float)i / (float)m_flightTime * 3.1415927f) * 5f);
					yield return Wait(1);
				}
				yield return Wait(480);
				Vanish(false);
				yield break;
			}

			private Vector2 m_goalPos;
			private int m_flightTime;
		}
	}
}
