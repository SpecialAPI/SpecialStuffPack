using System;
using System.Collections.Generic;
using System.Linq;
using Brave.BulletScript;
using UnityEngine;
using System.Collections;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedHelicopter
{
	public class HelicopterLightning2 : Script
	{
		protected override IEnumerator Top()
		{
			for (int i = 0; i < 3; i++)
			{
				float direction = BraveMathCollege.QuantizeFloat(AimDirection, 45f);
				PostWwiseEvent("Play_BOSS_agunim_ribbons_01", null);
				Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, -1f, 40, -4, null));
				Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, -1f, 40, 4, null));
				Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, -1f, 40, 4, null));
				Fire(new Offset(0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, 1f, 40, 4, null));
				Fire(new Offset(0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, 1f, 40, 4, null));
				Fire(new Offset(0.5f, 0.5f, 0f, string.Empty, DirectionType.Absolute), new LightningBullet(direction, 1f, 40, -4, null));
				yield return Wait(60);
			}
			yield break;
		}

		private class LightningBullet : Bullet
		{
			public LightningBullet(float direction, float sign, int maxRemainingBullets, int timeSinceLastTurn, Vector2? truePosition = null) : base(null, false, false, false)
			{
				m_direction = direction;
				m_sign = sign;
				m_maxRemainingBullets = maxRemainingBullets;
				m_timeSinceLastTurn = timeSinceLastTurn;
				m_truePosition = truePosition;
			}

			protected override IEnumerator Top()
			{
				if (Projectile != null)
				{
					Projectile.RuntimeUpdateScale(2f);
				}
				yield return Wait(2);
				Vector2? truePosition = m_truePosition;
				if (truePosition == null)
				{
					m_truePosition = new Vector2?(Position);
				}
				if (m_maxRemainingBullets > 0)
				{
					if (m_timeSinceLastTurn > 0 && m_timeSinceLastTurn != 2 && m_timeSinceLastTurn != 3 && UnityEngine.Random.value < 0.2f)
					{
						m_sign *= -1f;
						m_timeSinceLastTurn = 0;
					}
					float num = m_direction + m_sign * 30f;
					Vector2 vector = m_truePosition.Value + BraveMathCollege.DegreesToVector(num, 0.8f);
					Vector2 vector2 = vector + BraveMathCollege.DegreesToVector(num + 90f, UnityEngine.Random.Range(-0.3f, 0.3f));
					if (!IsPointInTile(vector2))
					{
						if(Projectile != null)
                        {
							Projectile.RuntimeUpdateScale(0.5f);
                        }
						LightningBullet lightningBullet = new LightningBullet(m_direction, m_sign, m_maxRemainingBullets - 1, m_timeSinceLastTurn + 1, new Vector2?(vector));
						Fire(Offset.OverridePosition(vector2), lightningBullet);
						if (lightningBullet.Projectile && lightningBullet.Projectile.specRigidbody && PhysicsEngine.Instance.OverlapCast(lightningBullet.Projectile.specRigidbody, null, true, false, null, null, false, null, null, new 
							SpeculativeRigidbody[0]))
						{
							lightningBullet.Projectile.DieInAir(false, true, true, false);
						}
					}
				}
				yield return Wait(30);
				Vanish(true);
				yield break;
			}

			private float m_direction;
			private float m_sign;
			private int m_maxRemainingBullets;
			private int m_timeSinceLastTurn;
			private Vector2? m_truePosition;
		}
	}
}
