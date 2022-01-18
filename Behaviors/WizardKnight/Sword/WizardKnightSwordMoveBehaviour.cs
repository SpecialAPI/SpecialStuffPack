using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Behaviors.WizardKnight.Sword
{
    public class WizardKnightSwordMoveBehaviour : MovementBehaviorBase
    {
        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            m_rotationTransform = gameObject.transform.Find(RotationTransform) ?? m_aiActor.transform;
        }

        public override BehaviorResult Update()
        {
            m_aiActor.BehaviorOverridesVelocity = true;
            Vector2 vec = m_aiActor.TargetRigidbody.UnitCenter - m_aiActor.specRigidbody.UnitCenter;
            if(vec.magnitude > PreferredDistance)
            {
                float speed = Mathf.Lerp(MinSpeed, MaxSpeed, (vec.magnitude - MinSpeedDistance) / (MaxSpeedDistance - MinSpeedDistance));
                m_aiActor.BehaviorVelocity = vec.normalized * speed;
            }
            else
            {
                m_aiActor.BehaviorVelocity = Vector2.zero;
            }
            m_rotationTransform.rotation = Quaternion.Euler(0f, 0f, vec.ToAngle());
            m_aiActor.sprite.transform.localRotation = Quaternion.identity;
            return BehaviorResult.Continue;
        }

        private Transform m_rotationTransform;
        public float PreferredDistance;
        public float MinSpeed;
        public float MaxSpeed;
        public float MinSpeedDistance;
        public float MaxSpeedDistance;
        public string RotationTransform;
    }
}
