using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class WhipProjectile : Projectile
    {
        public override void Start()
        {
            var num = transform.eulerAngles.z;
            base.Start();
            if (IsFirst && num < 270f && num > 90f)
            {
                flipMode = true;
            }
            if (flipMode)
            {
                startAngle = -startAngle;
                endAngle = -endAngle;
            }
            if(OverrideMotionModule != null && OverrideMotionModule is HelixProjectileMotionModule helix)
            {
                if (helix.ForceInvert)
                {
                    startAngle = -startAngle;
                    endAngle = -endAngle;
                }
                OverrideMotionModule = null;
            }
            if (IsFirst)
            {
                Direction = BraveMathCollege.DegreesToVector(PossibleSourceGun.barrelOffset.right.XY().ToAngle() + startAngle);
            }
            else
            {
                Direction = BraveMathCollege.DegreesToVector(parent.Direction.ToAngle() + startAngle);
            }
            if (shouldRotate)
            {
                m_transform.eulerAngles = new Vector3(0f, 0f, Direction.ToAngle());
            }
            if (IsFirst && Length > 1)
            {
                CreateSegment(Length - 1);
            }
            else if (currentLength > 1)
            {
                CreateSegment(currentLength - 1);
            }
            var scal = minScale * (this.PlayerOwner()?.stats?.GetStatValue(PlayerStats.StatType.RangeMultiplier) ?? 1f);
            sprite.scale = sprite.scale.WithX(sprite.scale.x / lastScale * scal);
            lastScale = scal;
            OnHitEnemy += ShareCollisionException;
            usedToHaveParent = !IsFirst;
        }

        public void ShareCollisionException(Projectile proj, SpeculativeRigidbody rigidbody, bool fatal)
        {
            if(rigidbody != null)
            {
                bool secretWall = rigidbody.majorBreakable && rigidbody.majorBreakable.IsSecretDoor;
                if (!rigidbody.majorBreakable && rigidbody.name.StartsWith("secret exit collider"))
                {
                    secretWall = true;
                }
                if (secretWall)
                {
                    return;
                }
                if (rigidbody.ReflectProjectiles)
                {
                    specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0.01f, new float?(1f));
                    if (child != null)
                    {
                        var child = this.child;
                        var i = Length;
                        while (child != null && i > 0)
                        {
                            child.specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0f, new float?(1f));
                            rigidbody.RegisterTemporaryCollisionException(child.specRigidbody, 0f, new float?(1f));
                            child = child.child;
                            i--;
                        }
                    }
                    if (parent != null)
                    {
                        var parent = this.parent;
                        var i = Length;
                        while (parent != null && i > 0)
                        {
                            parent.specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0f, new float?(1f));
                            rigidbody.RegisterTemporaryCollisionException(parent.specRigidbody, 0f, new float?(1f));
                            parent = parent.parent;
                            i--;
                        }
                    }
                    return;
                }
                specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0.5f, new float?(1f));
                if (child != null)
                {
                    var child = this.child;
                    var i = Length;
                    while (child != null && i > 0)
                    {
                        child.specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0.5f, new float?(1f));
                        child = child.child;
                        i--;
                    }
                }
                if (parent != null)
                {
                    var parent = this.parent;
                    var i = Length;
                    while (parent != null && i > 0)
                    {
                        parent.specRigidbody.RegisterTemporaryCollisionException(rigidbody, 0.5f, new float?(1f));
                        parent = parent.parent;
                        i--;
                    }
                }
            }
        }

        public override void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            base.OnRigidbodyCollision(rigidbodyCollision);
            m_hasPierced = false;
        }

        public override void Update()
        {
            if (OverrideMotionModule != null)
            {
                if (OverrideMotionModule is OrbitProjectileMotionModule && !appliedOrbitChanges)
                {
                    startAngle *= 2.5f;
                    endAngle *= 2.5f;
                    appliedOrbitChanges = true;
                }
                OverrideMotionModule = null;
            }
            base.Update();
        }

        public override void Move()
        {
            if((usedToHaveParent && IsFirst) || (this.PlayerOwner() != null && PossibleSourceGun != null && this.PlayerOwner().CurrentGun != PossibleSourceGun))
            {
                DieInAir(false, true, true, true);
                return;
            }
            base.Move();
            if(ElapsedTime > time)
            {
                DieInAir(false, true, true, false);
                return;
            }
            var lerpT = ElapsedTime / time / Mathf.Max(m_currentSpeed, 0.001f);
            currentAngle = Mathf.SmoothStep(startAngle, endAngle, lerpT);
            var scal = Mathf.SmoothStep(maxScale, minScale, Mathf.Abs(lerpT - 0.5f) * 2) * (this.PlayerOwner()?.stats?.GetStatValue(PlayerStats.StatType.RangeMultiplier) ?? 1f);
            sprite.scale = sprite.scale.WithX(sprite.scale.x / lastScale * scal);
            lastScale = scal;
            if (specRigidbody != null)
            {
                specRigidbody.UpdateCollidersOnScale = true;
            }
            if (IsFirst)
            {
                Direction = BraveMathCollege.DegreesToVector(PossibleSourceGun.barrelOffset.right.XY().ToAngle() + currentAngle);
            }
            else
            {
                Direction = BraveMathCollege.DegreesToVector(parent.Direction.ToAngle() + currentAngle);
            }
            if (shouldRotate)
            {
                m_transform.eulerAngles = new Vector3(0f, 0f, Direction.ToAngle());
            }
            if (IsFirst)
            {
                desiredPosition = PossibleSourceGun.barrelOffset.position;
            }
            else
            {
                desiredPosition = parent.Right();
            }
            specRigidbody.Velocity = (desiredPosition.GetValueOrDefault() - transform.position.XY()) / BraveTime.DeltaTime;
        }

        public override void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            if(otherRigidbody?.aiActor != null && otherRigidbody.aiActor.ParentRoom != null && myRigidbody.UnitCenter.GetAbsoluteRoom() != otherRigidbody.aiActor.ParentRoom)
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            base.OnPreCollision(myRigidbody, myCollider, otherRigidbody, otherCollider);
        }

        public Vector2 Right()
        {
            return (desiredPosition ?? transform.position.XY()) + sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleRight).Rotate(Direction.ToAngle());
        }

        public WhipProjectile CreateSegment(int length)
        {
            var path = whipPrefab;
            if (currentLength <= 2 && !IsFirst && !string.IsNullOrEmpty(whipEndPrefab))
            {
                path = whipEndPrefab;
            }
            var go = SpawnManager.SpawnProjectile(AssetBundleManager.Load<GameObject>(path), Right(), Quaternion.Euler(0f, 0f, Direction.ToAngle()));
            var whip = go.GetComponent<WhipProjectile>();
            whip.Owner = Owner;
            whip.Shooter = Shooter;
            whip.currentLength = length;
            whip.parent = this;
            child = whip;
            whip.flipMode = flipMode;
            if (Owner is PlayerController)
            {
                var player = Owner as PlayerController;
                PlayerStats stats = player.stats;
                whip.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
                whip.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                whip.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                whip.baseData.range *= stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                if (player.inventory.DualWielding)
                {
                    whip.baseData.damage *= Gun.s_DualWieldFactor;
                }
                if(PossibleSourceGun != null)
                {
                    if (PossibleSourceGun.CanSneakAttack && player.IsStealthed)
                    {
                        whip.baseData.damage *= PossibleSourceGun.SneakAttackDamageMultiplier;
                    }
                    if (PossibleSourceGun.m_isCritting)
                    {
                        whip.baseData.damage *= PossibleSourceGun.CriticalDamageMultiplier;
                        whip.IsCritical = true;
                    }
                    if (PossibleSourceGun.UsesBossDamageModifier)
                    {
                        if (PossibleSourceGun.CustomBossDamageModifier >= 0f)
                        {
                            whip.BossDamageMultiplier = PossibleSourceGun.CustomBossDamageModifier;
                        }
                        else
                        {
                            whip.BossDamageMultiplier = 0.8f;
                        }
                    }
                }
            }
            this.PlayerOwner()?.DoPostProcessProjectile(whip);
            return whip;
        }

        public bool IsFirst => parent == null || parent == this;

        public int Length;
        public float startAngle = -30f;
        public float endAngle = 30f;
        public float time = 0.5f;
        public float minScale = 0.75f;
        public float maxScale = 1.75f;
        public string whipPrefab;
        public string whipEndPrefab;
        private float currentAngle;
        private int currentLength;
        private float lastScale = 1f;
        private bool appliedOrbitChanges;
        private bool flipMode;
        private Vector2? desiredPosition;
        [NonSerialized]
        public WhipProjectile parent;
        [NonSerialized]
        public WhipProjectile child;
        private bool usedToHaveParent;
    }
}
