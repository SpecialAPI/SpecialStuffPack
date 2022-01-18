using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class LobbedProjectile : Projectile
    {
        public override void Start()
        {
            m_canCollide = false;
            PierceProjModifier pierce = this.GetOrAddComponent<PierceProjModifier>();
            pierce.penetratesBreakables = true;
            pierce.penetration = 999999;
            pierce.preventPenetrationOfActors = false;
            m_currentHeightSpeed = initialSpeed;
            base.Start();
            sprite.transform.localRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            m_originalHeightOffGround = sprite.HeightOffGround;
            specRigidbody.OnPreMovement += HandleHeight;
        }

        public void HandleHeight(SpeculativeRigidbody body)
        {
            m_currentHeightSpeed += speedCurve.Evaluate(m_elapsedBounceTime) * 60 * LocalDeltaTime * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
            m_elapsedBounceTime += LocalDeltaTime * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
            m_currentHeight += (m_currentHeightSpeed * LocalDeltaTime) * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, Mathf.Max(m_currentHeight, 0f));
            sprite.HeightOffGround = m_originalHeightOffGround + m_currentHeight;
            if (transform.rotation.eulerAngles.z != 0f)
            {
                Quaternion rotation = sprite.transform.rotation;
                rotation.eulerAngles = new Vector3(sprite.transform.rotation.eulerAngles.x, sprite.transform.rotation.eulerAngles.y, sprite.transform.rotation.eulerAngles.z + transform.rotation.eulerAngles.z);
                sprite.transform.rotation = rotation;
                transform.rotation = Quaternion.identity;
            }
            if (m_currentHeight <= 0f && m_currentHeightSpeed < 0f && !m_isDestroying)
            {
                StartCoroutine(HandleDestruction());
                m_isDestroying = true;
            }
        }

        protected override void OnPreCollision(SpeculativeRigidbody body, PixelCollider collider, SpeculativeRigidbody collision, PixelCollider collisionCollider)
        {
            if(!m_canCollide && collision.GetComponentInParent<DungeonDoorController>() == null && (collision.GetComponent<MajorBreakable>() == null || !collision.GetComponent<MajorBreakable>().IsSecretDoor))
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            base.OnPreCollision(body, collider, collision, collisionCollider);
        }

        public void SetDestination(Vector2 destination)
        {
            float? simulatedTime = SimulateTime();
            if (simulatedTime != null)
            {
                baseData.speed *= Vector2.Distance(transform.position.XY(), destination) / (simulatedTime.Value * Time.timeScale);
            }
        }

        public float? SimulateTime()
        {
            if(speedCurve.keys[speedCurve.length - 1].value >= 0f)
            {
                return null;
            }
            float time = 0f;
            float speed = initialSpeed;
            float height = 0f;
            float actualSpeed = 1f;
            float distance = 0f;
            bool breakNextFrame = false;
            while (true)
            {
                time += GameManager.INVARIANT_DELTA_TIME * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
                speed += speedCurve.Evaluate(time) * 60 * GameManager.INVARIANT_DELTA_TIME * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
                height += speed * GameManager.INVARIANT_DELTA_TIME * (Owner is PlayerController ? (Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed) : 1f);
                distance += actualSpeed * GameManager.INVARIANT_DELTA_TIME;
                if (breakNextFrame)
                {
                    break;
                }
                if(height <= 0f && speed < 0f)
                {
                    breakNextFrame = true;
                }
            }
            return time;
        }

        protected override void Move()
        {
            if(angularVelocity != 0f)
            {
                sprite.transform.RotateAround(sprite.transform.position.XY(), Vector3.forward, angularVelocity * LocalDeltaTime);
            }
            float original = angularVelocity;
            angularVelocity = 0f;
            base.Move();
            angularVelocity += original;
        }

        protected override void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            base.OnRigidbodyCollision(rigidbodyCollision);
            m_hasPierced = false;
        }

        protected IEnumerator HandleDestruction()
        {
            m_canCollide = true;
            yield return null;
            if(GetComponent<BounceProjModifier>() != null && GetComponent<BounceProjModifier>().numberOfBounces > 0)
            {
                m_canCollide = false;
                m_elapsedBounceTime = 0f;
                m_currentHeightSpeed = initialSpeed;
                m_currentHeight = 0f;
                GetComponent<BounceProjModifier>().numberOfBounces--;
                m_isDestroying = false;
            }
            else
            {
                DieInAir(false, true, true, false);
            }
            yield break;
        }

        public AnimationCurve speedCurve;
        public float initialSpeed;
        public float flySpeedMultiplier;
        public Vector2 destinationOffset;
        private float m_elapsedBounceTime;
        private bool m_isDestroying;
        private float m_currentHeightSpeed;
        private float m_currentHeight;
        private float m_originalHeightOffGround;
        private bool m_canCollide;
    }
}
