using SpecialStuffPack.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Behaviors.WizardKnight.Sword
{
    public class WizardKnightSwordSpinBehaviour : BasicAttackBehavior
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
            if(result != BehaviorResult.Continue)
            {
                return result;
            }
            if (!IsReady())
            {
                return BehaviorResult.Continue;
            }
            if(m_bulletSource == null)
            {
                m_bulletSource = BulletScriptShootPoint.GetOrAddComponent<BulletScriptSource>();
            }
            m_bulletSource.BulletManager = m_bulletBank;
            m_bulletSource.BulletScript = SpinBulletScript;
            m_bulletSource.Initialize();
            m_aiActor.BehaviorVelocity = Vector2.zero;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            Quaternion rotation = RotationTransform.rotation;
            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z + DegreesPerSecond * m_deltaTime);
            RotationTransform.rotation = rotation;
            if (m_sword != null && m_sword.State == RequiredState)
            {
                return ContinuousBehaviorResult.Continue;
            }
            return ContinuousBehaviorResult.Finished;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if(m_bulletSource != null && !m_bulletSource.IsEnded)
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

        private WizardKnightSwordController m_sword;
        private BulletScriptSource m_bulletSource;
        private AIBulletBank m_bulletBank;
        public BulletScriptSelector SpinBulletScript;
        public Transform BulletScriptShootPoint;
        public float DegreesPerSecond;
        public Transform RotationTransform;
        public WizardKnightController.State RequiredState;
    }
}
