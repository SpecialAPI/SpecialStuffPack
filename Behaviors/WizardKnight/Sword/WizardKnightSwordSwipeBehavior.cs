using SpecialStuffPack.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Behaviors.WizardKnight.Sword
{
    public class WizardKnightSwordSwipeBehavior : BasicAttackBehavior
    {
        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            m_updateEveryFrame = true;
            m_sword = gameObject.AddComponent<WizardKnightSwordController>();
        }

        public override BehaviorResult Update()
        {
            BehaviorResult result = base.Update();
            if(result != BehaviorResult.Continue)
            {
                return result;
            }
            if (!IsReady())
            {
                return BehaviorResult.Continue;
            }
            m_hasSpawnedScript = false;
            m_hasPlayedAudio = false;
            m_aiActor.BehaviorVelocity = Vector2.zero;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            Vector2 vec = m_aiActor.TargetRigidbody.UnitCenter - m_aiActor.specRigidbody.UnitCenter;
            if (m_sword != null && m_sword.Owner != null && m_sword.Owner.behaviorSpeculator != null && m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup != null && m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior != null &&
                m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior is SequentialAttackBehaviorGroup) 
            {
                AttackBehaviorBase behav = (m_sword.Owner.behaviorSpeculator.AttackBehaviorGroup.CurrentBehavior as SequentialAttackBehaviorGroup).currentBehavior;
                if(behav != null && behav is WizardKnightChangeStateBehavior)
                {
                    WizardKnightChangeStateBehavior stateBehav = behav as WizardKnightChangeStateBehavior;
                    if (m_sword.State == WizardKnightController.State.SwordSwipeBegin)
                    {
                        RotationTransform.rotation = Quaternion.Euler(0f, 0f, vec.ToAngle() + Mathf.Lerp(0f, StartSwipeAngle, stateBehav.ContinueStateTimer / stateBehav.ContinueStateTime));
                        m_aiActor.sprite.transform.localRotation = Quaternion.identity;
                    }
                    else if(m_sword.State == WizardKnightController.State.SwordSwipe)
                    {
                        if (!m_hasPlayedAudio)
                        {
                            m_aiActor.bulletBank.PostWwiseEvent("Play_WPN_Gun_Shot_01", "Blasphemy");
                            m_hasPlayedAudio = true;
                        }
                        RotationTransform.rotation = Quaternion.Euler(0f, 0f, vec.ToAngle() + Mathf.Lerp(StartSwipeAngle, EndSwipeAngle, stateBehav.ContinueStateTimer / stateBehav.ContinueStateTime));
                        m_aiActor.sprite.transform.localRotation = Quaternion.identity;
                        if((stateBehav.ContinueStateTimer / stateBehav.ContinueStateTime) >= ScriptFirePercentage && !m_hasSpawnedScript)
                        {
                            if(m_scriptSource == null)
                            {
                                m_scriptSource = ScriptSpawnTransform.GetOrAddComponent<BulletScriptSource>();
                            }
                            m_scriptSource.BulletManager = m_aiActor.GetComponent<AIBulletBank>();
                            m_scriptSource.BulletScript = SwipeScript;
                            m_scriptSource.Initialize();
                            m_hasSpawnedScript = true;
                        }
                    }
                    else if (m_sword.State == WizardKnightController.State.SwordSwipeReturn)
                    {
                        RotationTransform.rotation = Quaternion.Euler(0f, 0f, vec.ToAngle() + Mathf.Lerp(EndSwipeAngle, 0f, stateBehav.ContinueStateTimer / stateBehav.ContinueStateTime));
                        m_aiActor.sprite.transform.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        return ContinuousBehaviorResult.Finished;
                    }
                    return ContinuousBehaviorResult.Continue;
                }
            }
            return ContinuousBehaviorResult.Finished;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
            {
                return false;
            }
            return m_sword != null && m_sword.State == WizardKnightController.State.SwordSwipeBegin;
        }

        private WizardKnightSwordController m_sword;
        private bool m_hasSpawnedScript;
        private bool m_hasPlayedAudio;
        private BulletScriptSource m_scriptSource;
        public Transform RotationTransform;
        public BulletScriptSelector SwipeScript;
        public float StartSwipeAngle;
        public float EndSwipeAngle;
        public float ScriptFirePercentage;
        public Transform ScriptSpawnTransform;
    }
}
