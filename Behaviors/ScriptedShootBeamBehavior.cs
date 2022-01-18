using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.Behaviors
{
	public class ScriptedShootBeamBehavior : BasicAttackBehavior
	{
		public ScriptedShootBeamBehavior()
		{
			minUnitRadius = 5f;
			m_currentBeamShooters = new List<AIBeamShooter>();
		}

		public override void Start()
		{
			base.Start();
			m_allBeamShooters = new List<AIBeamShooter>(m_aiActor.GetComponents<AIBeamShooter>());
			if (!string.IsNullOrEmpty(TellAnimation))
			{
				tk2dSpriteAnimator spriteAnimator = m_aiAnimator.spriteAnimator;
				spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(AnimEventTriggered));
				if (m_aiAnimator.ChildAnimator)
				{
					tk2dSpriteAnimator spriteAnimator2 = m_aiAnimator.ChildAnimator.spriteAnimator;
					spriteAnimator2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(AnimEventTriggered));
				}
			}
		}

		public override void Upkeep()
		{
			base.Upkeep();
			if (m_aiActor.TargetRigidbody)
			{
				m_targetPosition = m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
				m_backupTarget = m_aiActor.TargetRigidbody;
			}
			else if (m_backupTarget)
			{
				m_targetPosition = m_backupTarget.GetUnitCenter(ColliderType.HitBox);
			}
		}

		public override BehaviorResult Update()
		{
			base.Update();
			BehaviorResult behaviorResult = base.Update();
			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}
			if (!IsReady())
			{
				return BehaviorResult.Continue;
			}
			if (!string.IsNullOrEmpty(TellAnimation))
			{
				m_aiAnimator.PlayUntilFinished(TellAnimation, true, null, -1f, false);
				SetState(State.WaitingForTell);
			}
			else
			{
				Fire();
			}
			m_updateEveryFrame = true;
			return BehaviorResult.RunContinuous;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();
			if (state == State.WaitingForTell)
			{
				if (!m_aiAnimator.IsPlaying(TellAnimation))
				{
					Fire();
				}
				return ContinuousBehaviorResult.Continue;
			}
			if (state == State.Firing)
			{
				m_firingTime += m_deltaTime;
				m_timer -= m_deltaTime;
				if (m_timer > 0f && m_currentBeamShooters[0].IsFiringLaser)
				{
					float num = 0f;
					if (trackingType == TrackingType.Follow)
					{
						AIBeamShooter aibeamShooter = m_currentBeamShooters[0];
						Vector2 laserFiringCenter = aibeamShooter.LaserFiringCenter;
						float num2 = Vector2.Distance(m_targetPosition, laserFiringCenter);
						num2 = Mathf.Max(minUnitRadius, num2);
						float num3 = (m_targetPosition - laserFiringCenter).ToAngle();
						float num4 = BraveMathCollege.ClampAngle180(num3 - aibeamShooter.LaserAngle);
						float f = num4 * num2 * 0.017453292f;
						float num5 = maxUnitTurnRate;
						float num6 = Mathf.Sign(num4);
						if (m_unitOvershootTimer > 0f)
						{
							num6 = m_unitOvershootFixedDirection;
							m_unitOvershootTimer -= m_deltaTime;
							num5 = unitOvershootSpeed;
						}
						m_currentUnitTurnRate = Mathf.Clamp(m_currentUnitTurnRate + num6 * unitTurnRateAcceleration * m_deltaTime, -num5, num5);
						float num7 = m_currentUnitTurnRate / num2 * 57.29578f;
						float num8 = 0f;
						if (useDegreeCatchUp && Mathf.Abs(num4) > minDegreesForCatchUp)
						{
							float b = Mathf.InverseLerp(minDegreesForCatchUp, 180f, Mathf.Abs(num4)) * degreeCatchUpSpeed;
							num8 = Mathf.Max(num8, b);
						}
						if (useUnitCatchUp && Mathf.Abs(f) > minUnitForCatchUp)
						{
							float num9 = Mathf.InverseLerp(minUnitForCatchUp, maxUnitForCatchUp, Mathf.Abs(f)) * unitCatchUpSpeed;
							float b2 = num9 / num2 * 57.29578f;
							num8 = Mathf.Max(num8, b2);
						}
						if (useUnitOvershoot && Mathf.Abs(f) < minUnitForOvershoot)
						{
							m_unitOvershootFixedDirection = (float)((m_currentUnitTurnRate <= 0f) ? -1 : 1);
							m_unitOvershootTimer = unitOvershootTime;
						}
						num8 *= Mathf.Sign(num4);
						num = num7 + num8;
					}
					else if (trackingType == TrackingType.ConstantTurn)
					{
						num = maxDegTurnRate;
					}
					else if (trackingType == TrackingType.AccelTurn)
					{
						m_currentDegTurnRate = Mathf.Clamp(m_currentDegTurnRate + degTurnRateAcceleration * m_deltaTime, -maxDegTurnRate, maxDegTurnRate);
						num = m_currentDegTurnRate;
					}
					for (int i = 0; i < m_currentBeamShooters.Count; i++)
					{
						AIBeamShooter aibeamShooter2 = m_currentBeamShooters[i];
						aibeamShooter2.LaserAngle = BraveMathCollege.ClampAngle360(aibeamShooter2.LaserAngle + num * m_deltaTime);
						if (restrictBeamLengthToAim && m_aiActor.TargetRigidbody)
						{
							float magnitude = (m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - aibeamShooter2.LaserFiringCenter).magnitude;
							aibeamShooter2.MaxBeamLength = magnitude + beamLengthOFfset;
							if (beamLengthSinMagnitude > 0f && beamLengthSinPeriod > 0f)
							{
								aibeamShooter2.MaxBeamLength += Mathf.Sin(m_firingTime / beamLengthSinPeriod * 3.1415927f) * beamLengthSinMagnitude;
								if (aibeamShooter2.MaxBeamLength < 0f)
								{
									aibeamShooter2.MaxBeamLength = 0f;
								}
							}
						}
					}
					return ContinuousBehaviorResult.Continue;
				}
				StopLasers();
				if (!string.IsNullOrEmpty(PostFireAnimation))
				{
					SetState(State.WaitingForPostAnim);
					m_aiAnimator.PlayUntilFinished(PostFireAnimation, false, null, -1f, false);
					return ContinuousBehaviorResult.Continue;
				}
				return ContinuousBehaviorResult.Finished;
			}
			else
			{
				if (state == State.WaitingForPostAnim)
				{
					return (!m_aiAnimator.IsPlaying(PostFireAnimation)) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
				}
				return ContinuousBehaviorResult.Continue;
			}
		}

		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			if (!string.IsNullOrEmpty(TellAnimation))
			{
				m_aiAnimator.EndAnimationIf(TellAnimation);
			}
			if (!string.IsNullOrEmpty(FireAnimation))
			{
				m_aiAnimator.EndAnimationIf(FireAnimation);
			}
			if (!string.IsNullOrEmpty(PostFireAnimation))
			{
				m_aiAnimator.EndAnimationIf(PostFireAnimation);
			}
			StopLasers();
			SetState(State.Idle);
			m_aiAnimator.LockFacingDirection = false;
			m_updateEveryFrame = false;
			UpdateCooldowns();
		}

		public override void OnActorPreDeath()
		{
			base.OnActorPreDeath();
			StopLasers();
		}

		private void AnimEventTriggered(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			if (state == State.WaitingForTell && frame.eventInfo == "fire")
			{
				Fire();
			}
		}

		private void Fire()
		{
			if (!string.IsNullOrEmpty(FireAnimation))
			{
				m_aiAnimator.EndAnimation();
				m_aiAnimator.PlayUntilFinished(FireAnimation, false, null, -1f, false);
			}
			if (stopWhileFiring)
			{
				m_aiActor.ClearPath();
			}
			if (beamSelection == BeamSelection.All)
			{
				m_currentBeamShooters.AddRange(m_allBeamShooters);
			}
			else if (beamSelection == BeamSelection.Random)
			{
				m_currentBeamShooters.Add(BraveUtility.RandomElement<AIBeamShooter>(m_allBeamShooters));
			}
			else if (beamSelection == BeamSelection.Specify)
			{
				m_currentBeamShooters.Add(specificBeamShooter);
			}
			float facingDirection = m_currentBeamShooters[0].CurrentAiAnimator.FacingDirection;
			float num = (!randomInitialAimOffsetSign) ? 1f : BraveUtility.RandomSign();
			for (int i = 0; i < m_currentBeamShooters.Count; i++)
			{
				AIBeamShooter aibeamShooter = m_currentBeamShooters[i];
				if (restrictBeamLengthToAim && m_aiActor.TargetRigidbody)
				{
					float magnitude = (m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - aibeamShooter.LaserFiringCenter).magnitude;
					aibeamShooter.MaxBeamLength = magnitude;
				}
				float num2 = 0f;
				if (initialAimType == InitialAimType.FacingDirection)
				{
					num2 = facingDirection;
				}
				else if (initialAimType == InitialAimType.Aim)
				{
					if (m_aiActor.TargetRigidbody)
					{
						num2 = (m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - aibeamShooter.LaserFiringCenter).ToAngle();
					}
				}
				else if (initialAimType == InitialAimType.Absolute)
				{
					num2 = 0f;
				}
				else if (initialAimType == InitialAimType.Transform)
				{
					num2 = aibeamShooter.beamTransform.eulerAngles.z;
				}
				num2 += num * initialAimOffset;
				aibeamShooter.StartFiringLaser(num2);
			}
			m_timer = firingTime;
			if (!m_bulletSource)
			{
				m_bulletSource = ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
			}
			m_bulletSource.BulletManager = m_aiActor.bulletBank;
			m_bulletSource.BulletScript = BulletScript;
			m_bulletSource.Initialize();
			m_currentUnitTurnRate = 0f;
			m_currentDegTurnRate = 0f;
			m_firingTime = 0f;
			SetState(State.Firing);
		}

		private void StopLasers()
		{
			for (int i = 0; i < m_currentBeamShooters.Count; i++)
			{
				m_currentBeamShooters[i].StopFiringLaser();
			}
			if(m_bulletSource != null)
            {
				m_bulletSource.ForceStop();
            }
			m_currentBeamShooters.Clear();
		}

		private State state
		{
			get
			{
				return m_state;
			}
		}

		private void SetState(State value)
        {
			if (m_state != value)
			{
				m_state = value;
			}
		}

		public BeamSelection beamSelection;
		public AIBeamShooter specificBeamShooter;
		public float firingTime;
		public bool stopWhileFiring;
		public InitialAimType initialAimType;
		public float initialAimOffset;
		public bool randomInitialAimOffsetSign;
		public bool restrictBeamLengthToAim;
		public float beamLengthOFfset;
		public float beamLengthSinMagnitude;
		public float beamLengthSinPeriod;
		public TrackingType trackingType;
		public float maxUnitTurnRate;
		public float unitTurnRateAcceleration;
		public float minUnitRadius;
		public bool useDegreeCatchUp;
		public float minDegreesForCatchUp;
		public float degreeCatchUpSpeed;
		public bool useUnitCatchUp;
		public float minUnitForCatchUp;
		public float maxUnitForCatchUp;
		public float unitCatchUpSpeed;
		public bool useUnitOvershoot;
		public float minUnitForOvershoot;
		public float unitOvershootTime;
		public float unitOvershootSpeed;
		public float maxDegTurnRate;
		public float degTurnRateAcceleration;
		public string TellAnimation;
		public string FireAnimation;
		public string PostFireAnimation;
		public BulletScriptSelector BulletScript;
		public Transform ShootPoint;
		private List<AIBeamShooter> m_allBeamShooters;
		private readonly List<AIBeamShooter> m_currentBeamShooters;
		private float m_timer;
		private float m_firingTime;
		private Vector2 m_targetPosition;
		private float m_currentUnitTurnRate;
		private float m_currentDegTurnRate;
		private float m_unitOvershootFixedDirection;
		private float m_unitOvershootTimer;
		private SpeculativeRigidbody m_backupTarget;
		private State m_state;
		private BulletScriptSource m_bulletSource;

		public enum BeamSelection
		{
			All,
			Random,
			Specify
		}

		public enum TrackingType
		{
			Follow,
			ConstantTurn,
			AccelTurn
		}

		public enum InitialAimType
		{
			FacingDirection,
			Aim,
			Absolute,
			Transform
		}

		private enum State
		{
			Idle,
			WaitingForTell,
			Firing,
			WaitingForPostAnim
		}
	}
}
