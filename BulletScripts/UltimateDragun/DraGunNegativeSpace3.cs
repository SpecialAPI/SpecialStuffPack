using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunNegativeSpace3 : ScriptLite
    {
		public DraGunNegativeSpace3()
		{
			m_centerBullets = new List<bool>();
		}

		public override void Start()
		{
			ActivePlatformRadius = 4f;
			int num = 8;
			m_platformCenters = new List<Vector2>(10);
			m_platformCenters.Add(new Vector2(UnityEngine.Random.Range(-17f, 17f) / 2f, 0f));
			for (int i = 1; i < 10; i++)
			{
				Vector2 a = m_platformCenters[m_platformCenters.Count - 1];
				s_validPlatformIndices.Clear();
				for (int j = 0; j < PlatformAngles.Length; j++)
				{
					Vector2 a2 = a + BraveMathCollege.DegreesToVector(PlatformAngles[j], 2f * ActivePlatformRadius + PlatformDistances[j]);
					if (a2.x > -17f && a2.x < 17f && Vector2.Distance(a2, m_platformCenters[m_platformCenters.Count - 1]) > (float)num && (i < 2 || Vector2.Distance(a2, m_platformCenters[m_platformCenters.Count - 2]) > (float)num) && (i < 3 || Vector2.Distance(a2, m_platformCenters[m_platformCenters.Count - 3]) > (float)num))
					{
						s_validPlatformIndices.Add(j);
					}
				}
				if (s_validPlatformIndices.Count == 0)
				{
					s_validPlatformIndices.Add(2);
				}
				int num2 = BraveUtility.RandomElement<int>(s_validPlatformIndices);
				m_platformCenters.Add(a + BraveMathCollege.DegreesToVector(PlatformAngles[num2], 2f * ActivePlatformRadius + PlatformDistances[num2]));
				if (i % 2 == 1)
				{
					s_validPlatformIndices.Remove(num2);
					for (int k = s_validPlatformIndices.Count - 1; k >= 0; k--)
					{
						int num3 = s_validPlatformIndices[k];
						Vector2 a3 = a + BraveMathCollege.DegreesToVector(PlatformAngles[num3], 2f * ActivePlatformRadius + PlatformDistances[num3]);
						if (Vector2.Distance(a3, m_platformCenters[m_platformCenters.Count - 1]) < (float)num || Vector2.Distance(a3, m_platformCenters[m_platformCenters.Count - 2]) < (float)num)
						{
							s_validPlatformIndices.RemoveAt(k);
						}
					}
					if (s_validPlatformIndices.Count > 0)
					{
						num2 = BraveUtility.RandomElement<int>(s_validPlatformIndices);
						m_platformCenters.Add(a + BraveMathCollege.DegreesToVector(PlatformAngles[num2], 2f * ActivePlatformRadius + PlatformDistances[num2]));
					}
				}
			}
			m_verticalGap = 1.6f;
			m_lastCenterHeight = m_platformCenters[m_platformCenters.Count - 1].y;
			m_rowHeight = 0f;
			m_centerBullets.Clear();
			for (int l = 0; l < m_platformCenters.Count; l++)
			{
				m_centerBullets.Add(false);
			}
		}

		public override int Update(ref int state)
		{
			if (state != 0)
			{
				return Done();
			}
			if (m_rowHeight < m_lastCenterHeight)
			{
				for (int i = 0; i < 19; i++)
				{
					float num = SubdivideRange(-17f, 17f, 19, i, false);
					Vector2 a = new Vector2(num, m_rowHeight);
					bool suppressOffset = false;
					for (int j = 0; j < m_platformCenters.Count; j++)
					{
						if (Vector2.Distance(a, m_platformCenters[j]) < ActivePlatformRadius)
						{
							Vector2 vector;
							Vector2 vector2;
							int num2 = BraveMathCollege.LineCircleIntersections(m_platformCenters[j], ActivePlatformRadius, new Vector2(-17f, m_rowHeight), new Vector2(17f, m_rowHeight), out vector, out vector2);
							if (num2 == 1)
							{
								num = vector.x;
							}
							else
							{
								num = ((Mathf.Abs(num - vector.x) >= Mathf.Abs(num - vector2.x)) ? vector2.x : vector.x);
							}
							suppressOffset = true;
						}
					}
					Fire(new Offset(num, 18f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new WiggleBullet(suppressOffset, false));
				}
				m_rowHeight += m_verticalGap;
				for (int k = 0; k < m_platformCenters.Count; k++)
				{
					if (!m_centerBullets[k] && m_platformCenters[k].y < m_rowHeight - 2f)
					{
						WiggleBullet bullet = new WiggleBullet(true, true);
						Fire(new Offset(m_platformCenters[k].x, 20.5f - m_rowHeight + m_platformCenters[k].y, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), bullet);
						m_centerBullets[k] = true;
					}
				}
				return Wait(16);
			}
			state++;
			return Wait(120);
		}

		static DraGunNegativeSpace3()
		{
			PlatformAngles = new float[]
			{
			155f,
			125f,
			90f,
			55f,
			25f
			};
			PlatformDistances = new float[]
			{
			1f,
			2.5f,
			3f,
			2.5f,
			1f
			};
			s_validPlatformIndices = new List<int>();
		}

		private static float[] PlatformAngles;
		private static float[] PlatformDistances;
		private static List<int> s_validPlatformIndices;
		private float ActivePlatformRadius;
		private List<Vector2> m_platformCenters;
		private List<bool> m_centerBullets;
		private float m_verticalGap;
		private float m_lastCenterHeight;
		private float m_rowHeight;

		public class WiggleBullet : BulletLite
		{
			public WiggleBullet(bool suppressOffset, bool isBig) : base("default_novfx", false, false)
			{
				m_suppressOffset = suppressOffset;
				big = isBig;
			}

			public override void Start()
			{
                if (big)
                {
					Projectile.RuntimeUpdateScale(2f);
                }
				ManualControl = true;
				m_truePosition = Position;
				m_offset = Vector2.zero;
				m_xMagnitude = UnityEngine.Random.Range(0f, 0.6f);
				m_xPeriod = UnityEngine.Random.Range(1f, 2.5f);
				m_yMagnitude = UnityEngine.Random.Range(0f, 0.4f);
				m_yPeriod = UnityEngine.Random.Range(1f, 2.5f);
				m_delta = BraveMathCollege.DegreesToVector(Direction, Speed / 60f);
			}

			public override int Update(ref int state)
			{
				if (Tick >= 360)
				{
					Vanish(false);
					return Done();
				}
				if (!m_suppressOffset)
				{
					float num = 0.5f + (float)Tick / 60f * m_xPeriod;
					num = Mathf.Repeat(num, 2f);
					float num2 = 1f - Mathf.Abs(num - 1f);
					num2 = Mathf.Clamp01(num2);
					num2 = (float)(-2.0 * (double)num2 * (double)num2 * (double)num2 + 3.0 * (double)num2 * (double)num2);
					m_offset.x = (float)((double)m_xMagnitude * (double)num2 + (double)(-(double)m_xMagnitude) * (1.0 - (double)num2));
					float num3 = 0.5f + (float)Tick / 60f * m_yPeriod;
					num3 = Mathf.Repeat(num3, 2f);
					float num4 = 1f - Mathf.Abs(num3 - 1f);
					num4 = Mathf.Clamp01(num4);
					num4 = (float)(-2.0 * (double)num4 * (double)num4 * (double)num4 + 3.0 * (double)num4 * (double)num4);
					m_offset.y = (float)((double)m_yMagnitude * (double)num4 + (double)(-(double)m_yMagnitude) * (1.0 - (double)num4));
				}
				m_truePosition += m_delta;
				Position = m_truePosition + m_offset;
				return Wait(1);
			}

			private bool m_suppressOffset;
			private Vector2 m_truePosition;
			private Vector2 m_offset;
			private float m_xMagnitude;
			private float m_xPeriod;
			private float m_yMagnitude;
			private float m_yPeriod;
			private Vector2 m_delta;
			private bool big;
		}
	}
}
