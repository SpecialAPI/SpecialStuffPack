using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.Behaviors
{
	public class UltimateDraGunThrowKnifeBehavior : BasicAttackBehavior
	{
		public override void Start()
		{
			base.Start();
			m_dragun = m_aiActor.GetComponent<DraGunController>();
			if (aiAnimator)
			{
				tk2dSpriteAnimator spriteAnimator = aiAnimator.spriteAnimator;
				spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(AnimationEventTriggered));
			}
		}

		public override void Upkeep()
		{
			base.Upkeep();
			DecrementTimer(ref m_timer, false);
		}

		public override BehaviorResult Update()
		{
			BehaviorResult behaviorResult = base.Update();
			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}
			if (!IsReady())
			{
				return BehaviorResult.Continue;
			}
			if (delay <= 0f)
			{
				StartThrow();
			}
			else
			{
				m_timer = delay;
				m_isAttacking = false;
			}
			m_updateEveryFrame = true;
			return BehaviorResult.RunContinuous;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();
			if (!m_isAttacking)
			{
				if (m_timer <= 0f)
				{
					StartThrow();
				}
			}
			else
			{
				bool flag = true;
				if (unityAnimation)
				{
					flag &= !unityAnimation.IsPlaying(unityShootAnim);
				}
				if (aiAnimator)
				{
					flag &= !aiAnimator.IsPlaying(aiShootAnim);
				}
				if (flag)
				{
					return ContinuousBehaviorResult.Finished;
				}
			}
			return ContinuousBehaviorResult.Continue;
		}

		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			if (aiAnimator)
			{
				aiAnimator.EndAnimation();
			}
			if (unityAnimation)
			{
				unityAnimation.Stop();
				unityAnimation.GetClip(unityShootAnim).SampleAnimation(unityAnimation.gameObject, 1000f);
				unityAnimation.GetComponent<DraGunArmController>().UnclipHandSprite();
			}
			m_isAttacking = false;
			m_updateEveryFrame = false;
			UpdateCooldowns();
		}

		public override bool IsOverridable()
		{
			return false;
		}

		public override bool IsReady()
		{
			if (!base.IsReady())
			{
				return false;
			}
			List<AIActor> activeEnemies = m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			for (int i = 0; i < activeEnemies.Count; i++)
			{
				if (activeEnemies[i].name.Contains("knife", true))
				{
					return false;
				}
			}
			return true;
		}

		private void AnimationEventTriggered(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip, int frame)
		{
			if (m_isAttacking && clip.GetFrame(frame).eventInfo == "fire")
			{
				m_dragun.bulletBank.CreateProjectileFromBank(ShootPoint.transform.position, angle, "knife", null, false, true, false);
				m_dragun.bulletBank.CreateProjectileFromBank(ShootPoint.transform.position, angle + Mathf.Sign(angle) * 10, "knife", null, false, true, false);
			}
		}

		private void StartThrow()
		{
			if (unityAnimation)
			{
				unityAnimation.Play(unityShootAnim);
			}
			if (aiAnimator)
			{
				aiAnimator.PlayUntilCancelled(aiShootAnim, false, null, -1f, false);
			}
			m_isAttacking = true;
		}

		public float delay;
		public GameObject ShootPoint;
		public string BulletName;
		public float angle;
		public Animation unityAnimation;
		public string unityShootAnim;
		public AIAnimator aiAnimator;
		public string aiShootAnim;
		private DraGunController m_dragun;
		private float m_timer;
		private bool m_isAttacking;
	}
}
