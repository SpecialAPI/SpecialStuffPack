using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Behaviors.Ninja
{
    public class NinjaTeleportBehavior : BasicAttackBehavior
    {
        public override void Start()
        {
            base.Start();
			m_aiActor.healthHaver.OnDeath += x => ClearBlinkShadow();
			m_updateEveryFrame = true;
        }

		public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
		{
			base.Init(gameObject, aiActor, aiShooter);
			m_bulletBank = m_behaviorSpeculator.bulletBank;
		}

		public override BehaviorResult Update()
        {
			BehaviorResult behaviorResult = base.Update();
			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}
			if (m_aiActor.TargetRigidbody == null)
			{
				return BehaviorResult.Continue;
			}
			blinkPosition = m_aiActor.TargetRigidbody.gameActor.CenterPosition + (m_aiActor.TargetRigidbody.gameActor.CenterPosition - m_aiActor.CenterPosition).normalized * 2f;
			var canBlink = CanBlinkToPoint(blinkPosition, m_aiActor.transform.position.XY() - m_aiActor.specRigidbody.UnitCenter);
			UpdateBlinkShadow(canBlink);
			if (!IsReady())
			{
				return BehaviorResult.Continue;
			}
			if (!canBlink)
			{
				return BehaviorResult.Continue;
			}
			m_state = State.PreTeleport;
			m_preTeleportTimer = PreTeleportTime;
			return BehaviorResult.RunContinuous;
		}

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            if (m_state == State.PreTeleport)
            {
				if(m_preTeleportTimer <= 0f)
				{
					ClearBlinkShadow();
					m_state = State.Gone;
					m_aiActor.PlayEffectOnActor(CoolEffects.ScarfPoof, Vector3.zero, false, true, false);
					AkSoundEngine.PostEvent("Play_CHR_ninja_dash_01", m_aiActor.gameObject);
					m_aiActor.specRigidbody.CollideWithOthers = false;
					m_aiActor.sprite.renderer.enabled = false;
					SpriteOutlineManager.ToggleOutlineRenderers(m_aiActor.sprite, false);
					m_aiActor.ToggleShadowVisiblity(false);
					m_aiActor.IsGone = true;
					m_goneTimer = GoneTime;
				}
            }
            else
            {
				if(m_goneTimer <= 0f)
                {
					m_aiActor.transform.position = blinkPosition + (m_aiActor.sprite.transform.position.XY() - m_aiActor.specRigidbody.UnitCenter);
					m_aiActor.specRigidbody.Reinitialize();
					m_aiActor.PlayEffectOnActor(CoolEffects.ScarfPoof, Vector3.zero, false, true, false);
					AkSoundEngine.PostEvent("Play_CHR_ninja_dash_01", m_aiActor.gameObject);
					m_aiActor.specRigidbody.CollideWithOthers = true;
					m_aiActor.sprite.renderer.enabled = true;
					SpriteOutlineManager.ToggleOutlineRenderers(m_aiActor.sprite, true);
					m_aiActor.ToggleShadowVisiblity(true);
					m_aiActor.IsGone = false;
					if (!m_bulletSource)
					{
						m_bulletSource = ShootPoint.GetOrAddComponent<BulletScriptSource>();
					}
					m_bulletSource.BulletManager = m_bulletBank;
					m_bulletSource.BulletScript = BulletScript;
					m_bulletSource.Initialize();
					return ContinuousBehaviorResult.Finished;
				}
            }
			return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
			UpdateCooldowns();
        }

        public override void Upkeep()
        {
            base.Upkeep();
			DecrementTimer(ref m_preTeleportTimer, false);
			DecrementTimer(ref m_goneTimer, false);
		}

		protected bool CanBlinkToPoint(Vector2 point, Vector2 centerOffset)
		{
			bool flag = IsValidPlayerPosition(point + centerOffset);
			if (flag && m_aiActor.ParentRoom != null)
			{
				CellData cellData = GameManager.Instance.Dungeon.data[point.ToIntVector2(VectorConversions.Floor)];
				if (cellData == null)
				{
					return false;
				}
				RoomHandler nearestRoom = cellData.nearestRoom;
				if (cellData.type != CellType.FLOOR)
				{
					flag = false;
				}
				if (nearestRoom != m_aiActor.ParentRoom)
				{
					flag = false;
				}
				if (cellData.isExitCell)
				{
					flag = false;
				}
				if (nearestRoom.visibility == RoomHandler.VisibilityStatus.OBSCURED || nearestRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED)
				{
					flag = false;
				}
			}
			if (m_aiActor.ParentRoom == null)
			{
				flag = false;
			}
			return flag;
		}

		public bool IsValidPlayerPosition(Vector2 newPosition)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(newPosition.ToIntVector2(VectorConversions.Floor) + new IntVector2(i, j)))
					{
						return false;
					}
				}
			}
			int value = CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);
			Func<SpeculativeRigidbody, bool> func = rigidbody => rigidbody.minorBreakable;
			PhysicsEngine instance = PhysicsEngine.Instance;
			SpeculativeRigidbody specRigidbody = m_aiActor.specRigidbody;
			List<CollisionData> overlappingCollisions = null;
			bool collideWithTiles = true;
			bool collideWithRigidbodies = true;
			int? overrideCollisionMask = null;
			int? ignoreCollisionMask = value;
			Func<SpeculativeRigidbody, bool> rigidbodyExcluder = func;
			bool flag = instance.OverlapCast(specRigidbody, 
				overlappingCollisions,
				collideWithTiles,
				collideWithRigidbodies, overrideCollisionMask, ignoreCollisionMask, false, new Vector2?(newPosition), rigidbodyExcluder, new SpeculativeRigidbody[0]);
			return !flag;
		}

		protected void UpdateBlinkShadow(bool canWarpDirectly)
		{
			if (m_extantBlinkShadow == null)
			{
				GameObject go = new GameObject("blinkshadow");
				m_extantBlinkShadow = tk2dSprite.AddComponent(go, m_aiActor.sprite.Collection, m_aiActor.sprite.spriteId);
				m_extantBlinkShadow.transform.position = blinkPosition + (m_aiActor.sprite.transform.position.XY() - m_aiActor.specRigidbody.UnitCenter);
				tk2dSpriteAnimator tk2dSpriteAnimator = m_extantBlinkShadow.gameObject.AddComponent<tk2dSpriteAnimator>();
				tk2dSpriteAnimator.Library = m_aiActor.spriteAnimator.Library;
				m_extantBlinkShadow.renderer.material.SetColor("_OverrideColor", (!canWarpDirectly) ? new Color(0.4f, 0f, 0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f));
				m_extantBlinkShadow.usesOverrideMaterial = true;
				m_extantBlinkShadow.FlipX = m_aiActor.sprite.FlipX;
				m_extantBlinkShadow.FlipY = m_aiActor.sprite.FlipY;
			}
			else
			{
				m_extantBlinkShadow.spriteAnimator.Stop();
				m_extantBlinkShadow.SetSprite(m_aiActor.sprite.Collection, m_aiActor.sprite.spriteId);
				m_extantBlinkShadow.renderer.material.SetColor("_OverrideColor", (!canWarpDirectly) ? new Color(0.4f, 0f, 0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f));
				m_extantBlinkShadow.transform.position = blinkPosition + (m_aiActor.sprite.transform.position.XY() - m_aiActor.specRigidbody.UnitCenter);
			}
			m_extantBlinkShadow.FlipX = m_aiActor.sprite.FlipX;
			m_extantBlinkShadow.FlipY = m_aiActor.sprite.FlipY;
		}

		protected void ClearBlinkShadow()
		{
			if (m_extantBlinkShadow)
			{
				UnityEngine.Object.Destroy(m_extantBlinkShadow.gameObject);
				m_extantBlinkShadow = null;
			}
		}

		public GameObject ShootPoint;
		public BulletScriptSelector BulletScript;
		public float PreTeleportTime;
		public float GoneTime;
		private float m_goneTimer;
		private float m_preTeleportTimer;
		private State m_state;
		private tk2dSprite m_extantBlinkShadow;
		private Vector2 blinkPosition;
		private BulletScriptSource m_bulletSource;
		private AIBulletBank m_bulletBank;
		public enum State
        {
			PreTeleport,
			Gone
        }
	}
}
