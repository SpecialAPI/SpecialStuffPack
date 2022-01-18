using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace SpecialStuffPack.Behaviors
{
	public abstract class SpawnAndModifyReinforcementsBehavior : BasicAttackBehavior
	{
		public SpawnAndModifyReinforcementsBehavior()
		{
			MaxRoomOccupancy = -1;
			OverrideMaxOccupancyToSpawn = -1;
			staggerDelay = 1f;
			StopDuringAnimation = true;
			DisableDrops = true;
		}

		public abstract void ModifyEnemy(AIActor enemy);

		public override void Start()
		{
			base.Start();
		}

		public override void Upkeep()
		{
			base.Upkeep();
			if (s_staticCooldown > 0f && s_lastStaticUpdateFrameNum != Time.frameCount)
			{
				s_staticCooldown = Mathf.Max(0f, s_staticCooldown - m_deltaTime);
				s_lastStaticUpdateFrameNum = Time.frameCount;
			}
			DecrementTimer(ref m_staggerTimer, false);
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
			m_reinforceIndex = ((indexType != IndexType.Ordered) ? BraveUtility.RandomElement<int>(ReinforcementIndices) : ReinforcementIndices[m_timesReinforced]);
			m_thingsToSpawn = m_aiActor.ParentRoom.GetEnemiesInReinforcementLayer(m_reinforceIndex);
			int num = MaxRoomOccupancy;
			if (OverrideMaxOccupancyToSpawn > 0)
			{
				num = OverrideMaxOccupancyToSpawn;
			}
			if (num >= 0)
			{
				int count = m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count;
				if (count >= num)
				{
					m_timesReinforced++;
					UpdateCooldowns();
					return BehaviorResult.Continue;
				}
				m_thingsToSpawn = MaxRoomOccupancy - count;
			}
			m_timesReinforced++;
			s_staticCooldown += StaticCooldown;
			if (!string.IsNullOrEmpty(DirectionalAnimation))
			{
				m_aiAnimator.PlayUntilFinished(DirectionalAnimation, true, null, -1f, false);
			}
			if (HideGun)
			{
				m_aiShooter.ToggleGunAndHandRenderers(false, "SpawnAndModifyReinforcementBehavior");
			}
			if (StopDuringAnimation)
			{
				m_aiActor.ClearPath();
			}
			RoomHandler parentRoom = m_aiActor.ParentRoom;
			parentRoom.OnEnemyRegistered += ModifyEnemy;
			if (StaggerSpawns)
			{
				m_reinforceSubIndex = 0;
				if (staggerMode == StaggerMode.Animation)
				{
					tk2dSpriteAnimator spriteAnimator = m_aiAnimator.spriteAnimator;
					spriteAnimator.AnimationEventTriggered += AnimationEventTriggered;
				}
				else if (staggerMode == StaggerMode.Timer)
				{
					m_staggerTimer = staggerDelay;
				}
			}
			else if (m_thingsToSpawn > 0)
			{
				int reinforceIndex = m_reinforceIndex;
				bool removeLayer = false;
				bool disableDrops = DisableDrops;
				int thingsToSpawn = m_thingsToSpawn;
				parentRoom.TriggerReinforcementLayer(reinforceIndex, removeLayer, disableDrops, -1, thingsToSpawn, false);
			}
			if (StopDuringAnimation || StaggerSpawns)
			{
				m_updateEveryFrame = true;
				m_state = State.Spawning;
				return BehaviorResult.RunContinuous;
			}
			UpdateCooldowns();
			return BehaviorResult.SkipRemainingClassBehaviors;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			if (m_state == State.Spawning)
			{
				bool flag = false;
				if (!StaggerSpawns)
				{
					if (!m_aiAnimator.IsPlaying(DirectionalAnimation))
					{
						flag = true;
					}
				}
				else if (staggerMode == StaggerMode.Timer)
				{
					if (m_staggerTimer <= 0f)
					{
						SpawnOneDude();
						m_staggerTimer = staggerDelay;
						if (m_thingsToSpawn <= 0)
						{
							flag = true;
						}
					}
				}
				else if (staggerMode == StaggerMode.Animation && !m_aiAnimator.IsPlaying(DirectionalAnimation))
				{
					if (m_thingsToSpawn > 0)
					{
						m_aiActor.ParentRoom.TriggerReinforcementLayer(m_reinforceIndex, false, DisableDrops, m_reinforceSubIndex, m_thingsToSpawn, false);
					}
					flag = true;
				}
				if (flag)
				{
					if (DelayAfterSpawn > 0f)
					{
						m_timer = DelayAfterSpawn;
						m_state = State.PostSpawnDelay;
						return ContinuousBehaviorResult.Continue;
					}
					return ContinuousBehaviorResult.Finished;
				}
			}
			else if (m_state == State.PostSpawnDelay)
			{
				DecrementTimer(ref m_timer, false);
				if (DelayAfterSpawnMinOccupancy > 0)
				{
					int count = m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count;
					if (count < DelayAfterSpawnMinOccupancy)
					{
						return ContinuousBehaviorResult.Finished;
					}
				}
				if (m_timer <= 0f)
				{
					return ContinuousBehaviorResult.Finished;
				}
			}
			return ContinuousBehaviorResult.Continue;
		}

		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			if (HideGun)
			{
				m_aiShooter.ToggleGunAndHandRenderers(true, "SpawnAndModifyReinforcementBehavior");
			}
			if (StaggerSpawns && staggerMode == StaggerMode.Animation)
			{
				tk2dSpriteAnimator spriteAnimator = m_aiAnimator.spriteAnimator;
				spriteAnimator.AnimationEventTriggered += AnimationEventTriggered;
			}
			RoomHandler parentRoom = m_aiActor.ParentRoom;
			parentRoom.OnEnemyRegistered -= ModifyEnemy;
			m_updateEveryFrame = false;
			UpdateCooldowns();
		}

		public override bool IsReady()
		{
			return base.IsReady() && s_staticCooldown <= 0f;
		}

		private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNum)
		{
			if (clip.GetFrame(frameNum).eventInfo == "spawn" && m_thingsToSpawn > 0)
			{
				SpawnOneDude();
			}
		}

		private void SpawnOneDude()
		{
			m_aiActor.ParentRoom.TriggerReinforcementLayer(m_reinforceIndex, false, DisableDrops, m_reinforceSubIndex, 1, false);
			m_reinforceSubIndex++;
			m_thingsToSpawn--;
		}

		static SpawnAndModifyReinforcementsBehavior()
		{
			s_lastStaticUpdateFrameNum = -1;
		}

		public int MaxRoomOccupancy;
		public int OverrideMaxOccupancyToSpawn;
		public List<int> ReinforcementIndices;
		public IndexType indexType;
		public bool StaggerSpawns;
		public StaggerMode staggerMode;
		public float staggerDelay;
		public bool StopDuringAnimation;
		public bool DisableDrops;
		public float DelayAfterSpawn;
		public int DelayAfterSpawnMinOccupancy;
		public string DirectionalAnimation;
		public bool HideGun;
		public float StaticCooldown;
		private int m_timesReinforced;
		private int m_reinforceIndex;
		private int m_reinforceSubIndex;
		private int m_thingsToSpawn;
		private float m_staggerTimer;
		private float m_timer;
		private static float s_staticCooldown;
		private static int s_lastStaticUpdateFrameNum;
		private State m_state;

		public enum IndexType
		{
			Random,
			Ordered
		}

		public enum StaggerMode
		{
			Animation,
			Timer
		}

		private enum State
		{
			Idle,
			Spawning,
			PostSpawnDelay
		}
	}
}
