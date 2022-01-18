using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.AdvancedRat
{
    public class ResourcefulRatTail2 : Script
    {
		public bool ShouldTell;
		public bool Done;
		public float FireAngle;

		protected override IEnumerator Top()
		{
			EndOnBlank = true;
			yield return Wait(10);
			for (int i = 0; i < 33; i++)
			{
				Vector2 pos = GetPosition(i, Tick + 1);
				TailBullet bullet = new TailBullet(this, i);
				Fire(Offset.OverridePosition(pos), bullet);
				Vector2 oppositePos = GetPosition(i, Tick + 1, true);
				TailBullet oppositeBullet = new TailBullet(this, i, true, true);
				Fire(Offset.OverridePosition(oppositePos), oppositeBullet);
				if (i % 2 == 0)
				{
					yield return Wait(3);
				}
			}
			int spinTime = 167;
			float currentAngle = 0f;
			for (int j = 0; j < spinTime + 60; j++)
			{
				currentAngle = (GetPosition(16, Tick) - Position).ToAngle();
				if (j > spinTime && BraveMathCollege.AbsAngleBetween(currentAngle, AimDirection + 45f) < 10f)
				{
					break;
				}
				if (j == spinTime - 1)
				{
					ShouldTell = true;
				}
				yield return Wait(1);
			}
			Done = true;
			FireAngle = currentAngle - 90f;
			yield break;
		}

		public Vector2 GetPosition(int index, int tick, bool isOpposite = false)
		{
			float num;
			if (tick <= 120)
			{
				num = -90f + -90f * ((float)tick / 60f) * ((float)tick / 60f);
			}
			else
			{
				num = -450f + (float)(tick - 120) / 60f * -360f;
			}
			float num2 = BraveMathCollege.AbsAngleBetween(num, -90f);
			float num3 = Mathf.Lerp(0.5f, 0.75f, num2 / 70f);
			float magnitude = 1f + (float)index * num3;
			float num4 = (float)index * Mathf.Lerp(0f, 3f, num2 / 120f);
			return BraveMathCollege.DegreesToVector(num + num4 + (isOpposite ? 180 : 0), magnitude) + Position;
		}

		public class TailBullet : Bullet
		{
			public TailBullet(ResourcefulRatTail2 parentScript, int index, bool isOpposite = false, bool forceJammed = false) : base("tail", true, false, forceJammed)
			{
				m_spawnCountdown = -1;
				m_parentScript = parentScript;
				m_index = index;
				m_isOpposite = isOpposite;
			}

			protected override IEnumerator Top()
			{
				ManualControl = true;
				Projectile.specRigidbody.CollideWithTileMap = false;
				Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
				bool hasTold = false;
				while (!m_parentScript.Destroyed && !m_parentScript.IsEnded && !m_parentScript.Done)
				{
					Position = m_parentScript.GetPosition(m_index, m_parentScript.Tick / (m_isOpposite ? 2 : 1), m_isOpposite);
					if (m_parentScript.ShouldTell)
					{
						if (!hasTold)
						{
							Projectile.sprite.spriteAnimator.Play();
						}
					}
					else
					{
						m_spawnCountdown--;
						if (m_spawnCountdown == 0)
						{
							Fire(new SubtailBullet(m_parentScript));
							Projectile.sprite.spriteAnimator.StopAndResetFrameToDefault();
							m_spawnCountdown = -1;
						}
					}
					yield return Wait(1);
				}
				if (m_isOpposite)
				{
					DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(Projectile.specRigidbody.UnitCenter, 3.5f, 0.5f, false);
					Vanish(false);
				}
                else
				{
					Speed = 20f + UnityEngine.Random.Range(-2f, 2f);
					Direction = m_parentScript.FireAngle + UnityEngine.Random.Range(-15f, 15f);
					Projectile.sprite.spriteAnimator.StopAndResetFrameToDefault();
					AkSoundEngine.PostEvent("Play_BOSS_Rat_Tail_Whip_01", GameManager.Instance.gameObject);
					ManualControl = false;
					Projectile.specRigidbody.CollideWithTileMap = true;
					Projectile.BulletScriptSettings.surviveRigidbodyCollisions = false;
				}
				yield break;
			}

			private ResourcefulRatTail2 m_parentScript;
			private int m_index;
			private int m_spawnCountdown;
			private bool m_isOpposite;
		}

		public class SubtailBullet : Bullet
		{
			public SubtailBullet(ResourcefulRatTail2 parentScript) : base(null, true, false, false)
			{
				m_parentScript = parentScript;
			}

			protected override IEnumerator Top()
			{
				while (!m_parentScript.Destroyed && !m_parentScript.IsEnded && !m_parentScript.Done)
				{
					yield return Wait(1);
				}
				Vanish(false);
				yield break;
			}

			private ResourcefulRatTail2 m_parentScript;
		}
	}
}
