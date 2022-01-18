using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpecialStuffPack.Components;
using System.Text;

namespace SpecialStuffPack.Behaviors.WizardKnight.Sword
{
    public class WizardKnightSwordSpinSlashBehavior : BasicAttackBehavior
    {
        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            m_sword = gameObject.GetComponent<WizardKnightSwordController>();
            m_bulletBank = gameObject.GetComponent<AIBulletBank>();
            m_updateEveryFrame = true;
        }

        public override BehaviorResult Update()
        {
            BehaviorResult result = base.Update();
            if (result != BehaviorResult.Continue)
            {
                return result;
            }
            if (!IsReady())
            {
                return BehaviorResult.Continue;
            }
            if (m_bulletSource == null)
            {
                m_bulletSource = BulletScriptShootPoint.GetOrAddComponent<BulletScriptSource>();
            }
            m_bulletSource.BulletManager = m_bulletBank;
            m_bulletSource.BulletScript = SpinBulletScript;
            m_bulletSource.Initialize();
            m_aiActor.BehaviorVelocity = Vector2.zero;
            m_startZ = RotationTransform.rotation.eulerAngles.z;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            if (m_sword != null && m_sword.State == RequiredState && m_sword.Owner != null && m_sword.Owner.behaviorSpeculator != null && m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup != null && 
                m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior != null && m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior is WizardKnightChangeStateBehavior)
            {
                WizardKnightChangeStateBehavior behav = m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior as WizardKnightChangeStateBehavior;
                Quaternion rotation = RotationTransform.rotation;
                rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, m_startZ + Mathf.Lerp(0f, 360f, behav.ContinueStateTimer / behav.ContinueStateTime));
                RotationTransform.rotation = rotation;
                return ContinuousBehaviorResult.Continue;
            }
            return ContinuousBehaviorResult.Finished;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (m_bulletSource != null && !m_bulletSource.IsEnded)
            {
                m_bulletSource.ForceStop();
            }
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
            {
                return false;
            }
            return m_sword != null && m_sword.State == RequiredState;
        }

        private float m_startZ;
        private WizardKnightSwordController m_sword;
        private BulletScriptSource m_bulletSource;
        private AIBulletBank m_bulletBank;
        public BulletScriptSelector SpinBulletScript;
        public Transform BulletScriptShootPoint;
        public Transform RotationTransform;
        public WizardKnightController.State RequiredState;
    }
}
