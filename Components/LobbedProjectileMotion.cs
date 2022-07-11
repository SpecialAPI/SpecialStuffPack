using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class LobbedProjectileMotion : BraveBehaviour
    {
        public void Start()
        {
            m_canCollide = false;
            PierceProjModifier pierce = this.GetOrAddComponent<PierceProjModifier>();
            pierce.penetratesBreakables = true;
            pierce.penetration = 999999;
            pierce.preventPenetrationOfActors = false;
            m_currentHeightSpeed = initialSpeed;
            transform.rotation = Quaternion.identity;
            if(projectile.sprite != null)
            {
                projectile.sprite.transform.localRotation = transform.rotation;
                lastSpritePosition = projectile.sprite.transform.position;
                m_originalHeightOffGround = projectile.sprite.HeightOffGround;
            }
            specRigidbody.OnPreMovement += HandleHeight;
            specRigidbody.OnPreRigidbodyCollision += OnPreCollision;
            specRigidbody.OnRigidbodyCollision += OnRigidbodyCollision;
            projectile.angularVelocity = 0f;
        }

        public void HandleHeight(SpeculativeRigidbody body)
        {
            var speedmult = m_originalProjectileSpeed.GetValueOrDefault() / 15f;
            m_currentHeightSpeed += speedCurve.Evaluate(m_elapsedBounceTime) * 60 * projectile.LocalDeltaTime * speedmult;
            m_elapsedBounceTime += projectile.LocalDeltaTime * speedmult;
            m_currentHeight += (m_currentHeightSpeed * projectile.LocalDeltaTime) * speedmult;
            if(projectile.sprite != null)
            {
                projectile.sprite.transform.localPosition = new Vector3(projectile.sprite.transform.localPosition.x, Mathf.Max(m_currentHeight, 0f));
                projectile.sprite.HeightOffGround = m_originalHeightOffGround + m_currentHeight;
                if (transform.rotation.eulerAngles.z != 0f)
                {
                    Quaternion rotation = projectile.sprite.transform.rotation;
                    rotation.eulerAngles = new Vector3(projectile.sprite.transform.rotation.eulerAngles.x, projectile.sprite.transform.rotation.eulerAngles.y, projectile.sprite.transform.rotation.eulerAngles.z + transform.rotation.eulerAngles.z);
                    projectile.sprite.transform.rotation = rotation;
                    transform.rotation = Quaternion.identity;
                }
                if (projectile.shouldRotate)
                {
                    projectile.sprite.transform.rotation = Quaternion.Euler(0f, 0f, (projectile.sprite.transform.position - lastSpritePosition).XY().ToAngle());
                }
                lastSpritePosition = projectile.sprite.transform.position;
            }
            if (m_currentHeight <= 0f && m_currentHeightSpeed < 0f && !m_isDestroying)
            {
                StartCoroutine(HandleDestruction());
                m_isDestroying = true;
            }
        }

        public void OnPreCollision(SpeculativeRigidbody body, PixelCollider collider, SpeculativeRigidbody collision, PixelCollider collisionCollider)
        {
            if (!m_canCollide && collision.GetComponentInParent<DungeonDoorController>() == null && (collision.GetComponent<MajorBreakable>() == null || !collision.GetComponent<MajorBreakable>().IsSecretDoor))
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
        }

        public void SetDestination(Vector2 destination)
        {
            m_originalProjectileSpeed ??= projectile.baseData.speed;
            float? simulatedTime = SimulateTime();
            if (simulatedTime != null)
            {
                projectile.baseData.speed *= Vector2.Distance(transform.position.XY(), destination) / (simulatedTime.Value * Time.timeScale) / 15f;
            }
        }

        public float? SimulateTime()
        {
            if (speedCurve.keys[speedCurve.length - 1].value >= 0f)
            {
                return null;
            }
            float time = 0f;
            float speed = initialSpeed;
            float height = 0f;
            float actualSpeed = 1f;
            float distance = 0f;
            bool breakNextFrame = false;
            var speedmult = m_originalProjectileSpeed.GetValueOrDefault() / 15f;
            while (true)
            {
                if(speedmult == 0f)
                {
                    break;
                }
                time += GameManager.INVARIANT_DELTA_TIME * speedmult;
                speed += speedCurve.Evaluate(time) * 60 * GameManager.INVARIANT_DELTA_TIME * speedmult;
                height += speed * GameManager.INVARIANT_DELTA_TIME * speedmult;
                distance += actualSpeed * GameManager.INVARIANT_DELTA_TIME;
                if (breakNextFrame)
                {
                    break;
                }
                if (height <= 0f && speed < 0f)
                {
                    breakNextFrame = true;
                }
            }
            return time * Mathf.Sign(speedmult);
        }

        public void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            projectile.m_hasPierced = false;
        }

        protected IEnumerator HandleDestruction()
        {
            m_canCollide = true;
            yield return null;
            if (GetComponent<BounceProjModifier>() != null && GetComponent<BounceProjModifier>().numberOfBounces > 0)
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
                projectile.DieInAir(false, true, true, false);
            }
            yield break;
        }

        public AnimationCurve speedCurve;
        public float initialSpeed;
        public float flySpeedMultiplier;
        public Vector2 destinationOffset;
        private Vector3 lastSpritePosition;
        private float m_elapsedBounceTime;
        private bool m_isDestroying;
        private float m_currentHeightSpeed;
        private float m_currentHeight;
        private float m_originalHeightOffGround;
        private float? m_originalProjectileSpeed;
        private bool m_canCollide;
    }
}
