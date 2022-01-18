using SpecialStuffPack.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Behaviors.WizardKnight
{
    public class WizardKnightChangeStateBehavior : BasicAttackBehavior
    {
        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            m_wizard = gameObject.GetComponent<WizardKnightController>();
        }

        public float ContinueStateTimer => m_continousUpdateTimer;

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
            m_wizard.CurrentState = StateToSwitchTo;
            m_continousUpdateTimer = 0f;
            m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            m_wizard.CurrentState = WizardKnightController.State.None;
            m_isRunningContinous = false;
            m_continousUpdateTimer = 0f;
            m_updateEveryFrame = false;
            UpdateCooldowns();
        }

        public override void Upkeep()
        {
            base.Upkeep();
            if (m_isRunningContinous)
            {
                m_continousUpdateTimer += m_deltaTime;
            }
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            if(m_continousUpdateTimer >= ContinueStateTime)
            {
                return ContinuousBehaviorResult.Finished;
            }
            m_isRunningContinous = true;
            return ContinuousBehaviorResult.Continue;
        }

        private bool m_isRunningContinous;
        private float m_continousUpdateTimer;
        public float ContinueStateTime;
        public WizardKnightController.State StateToSwitchTo;
        private WizardKnightController m_wizard;
    }
}
