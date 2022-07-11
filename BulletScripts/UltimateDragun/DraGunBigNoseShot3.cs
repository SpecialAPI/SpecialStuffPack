using Brave.BulletScript;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunBigNoseShot3 : Script
    {
		public DraGunBigNoseShot3()
		{
			NumTraps = 4;
		}

		public override IEnumerator Top()
		{
			if (s_xValues == null || s_yValues == null)
			{
				s_xValues = new int[NumTraps];
				s_yValues = new int[NumTraps];
				for (int i = 0; i < NumTraps; i++)
				{
					s_xValues[i] = i;
					s_yValues[i] = i;
				}
			}
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			Vector2 a = area.UnitBottomLeft + new Vector2(1f, 20f);
			Vector2 vector = new Vector2(34f, 11f);
			Vector2 vector2 = new Vector2(vector.x / NumTraps, vector.y / NumTraps);
			BraveUtility.RandomizeArray(s_xValues, 0, -1);
			BraveUtility.RandomizeArray(s_yValues, 0, -1);
			for (int j = 0; j < NumTraps; j++)
			{
				int num = s_xValues[j];
				int num2 = s_yValues[j];
				Vector2 goalPos = a + new Vector2((num + UnityEngine.Random.value) * vector2.x, (num2 + UnityEngine.Random.value) * vector2.y);
				Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new EnemyBullet(goalPos));
				for(int h = 0; h < 4; h++)
				{
					Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new EnemyBullet(goalPos, 1.5f, h * 90f, 60, true));
				}
			}
			return null;
		}

		public int NumTraps;
		private static int[] s_xValues;
		private static int[] s_yValues;

		public class EnemyBullet : Bullet
		{
			public EnemyBullet(Vector2 goalPos, float orbitRadius = 0f, float orbitOffset = 0f, int ticksToExpand = 0, bool isSmall = false) : base("homing", false, false, false)
			{
				m_goalPos = goalPos;
				radius = orbitRadius;
				offset = orbitOffset;
				small = isSmall;
				this.ticksToExpand = ticksToExpand;
			}

			public override IEnumerator Top()
			{
                if (small)
                {
					Projectile.RuntimeUpdateScale(0.75f);
                }
				ManualControl = true;
				Vector2 startPos = Position;
				truePosition = Position;
				StartTask(HandleRotation());
				int travelTime = (int)((m_goalPos - Position).magnitude / Speed * 60f);
				int lifeTime = Mathf.RoundToInt(UnityEngine.Random.Range(480, 600) * (small ? 0.75f : 1f));
				int nextShot = 60 + UnityEngine.Random.Range(45, 90);
				AIAnimator aiAnimator = Projectile.sprite.aiAnimator;
				for (int i = 0; i < travelTime; i++)
				{
					truePosition = Vector2.Lerp(startPos, m_goalPos, (float)i / travelTime);
					aiAnimator.FacingDirection = -90f;
					yield return Wait(1);
				}
				while (Tick < lifeTime)
				{
					if (Tick >= nextShot)
					{
						aiAnimator.PlayUntilFinished("attack", false, null, -1f, false);
						yield return Wait(30);
						Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), null);
						nextShot = Tick + UnityEngine.Random.Range(45, 90);
					}
					aiAnimator.FacingDirection = AimDirection;
					yield return Wait(1);
				}
				Vanish(false);
				yield break;
			}

			private IEnumerator HandleRotation()
            {
				float rotationPerTick = 6;
                while (true)
                {
					if(ticksToExpand == 0)
                    {
						Position = truePosition;
					}
                    else
					{
						Position = truePosition + BraveMathCollege.DegreesToVector(Tick * rotationPerTick + offset, Mathf.Lerp(0f, radius, (float)Tick / ticksToExpand));
					}
					yield return Wait(1);
                }
            }

			private float radius;
			private float offset;
			private bool small;
			private int ticksToExpand;
			private Vector2 truePosition;
			private Vector2 m_goalPos;
		}
	}
}
