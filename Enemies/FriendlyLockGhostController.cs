using SpecialStuffPack.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Enemies
{
    public class FriendlyLockGhostController : BraveBehaviour
    {
        public IEnumerator Start()
        {
            currentState = State.Charging;
            CanCollide = false;
            renderer.enabled = true;
            Vector2 position = sprite.WorldCenter;
            specRigidbody.OnTriggerCollision += EnemyCollision;
            GameObject light = new GameObject("light");
            AdditionalBraveLight braveLight = light.AddComponent<AdditionalBraveLight>();
            light.transform.position = sprite.WorldCenter;
            braveLight.LightColor = Color.red;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;
            float dura = DashChargeTime * 0.95f;
            float ela = 0f;
            AkSoundEngine.PostEvent("Play_CHR_ghost_appear_01", gameObject);
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                braveLight.LightIntensity = Mathf.Lerp(0f, 4.25f, ela / dura);
                braveLight.LightRadius = Mathf.Lerp(0f, 5.4375f, ela / dura);
                sprite.scale = Vector2.one * Mathf.Lerp(10f, 1f, ela / dura);
                sprite.color = sprite.color.WithAlpha(Mathf.Lerp(0f, 1f, ela / dura));
                sprite.PlaceAtPositionByAnchor(position, tk2dBaseSprite.Anchor.MiddleCenter);
                yield return null;
            }
            sprite.scale = Vector2.one;
            sprite.color = sprite.color.WithAlpha(1f);
            braveLight.LightIntensity = 4.25f;
            braveLight.LightRadius = 5.4375f;
            light.transform.parent = transform;
            CanCollide = true;
            specRigidbody.Reinitialize();
            specRigidbody.ForceRegenerate();
            yield break;
        }

        public void Update()
        {
            if(target != null)
            {
                if(currentState == State.Charging)
                {
                    specRigidbody.Velocity = (specRigidbody.UnitCenter - target.specRigidbody.UnitCenter).normalized;
                    dashChargeTime += BraveTime.DeltaTime;
                    if(dashChargeTime >= DashChargeTime)
                    {
                        dashChargeTime = 0f;
                        currentState = State.Dashing;
                        aiAnimator.LockFacingDirection = false;
                        aiAnimator.PlayUntilCancelled("dash", false, null, -1, false);
                        AkSoundEngine.PostEvent("Play_ENM_cannonball_launch_01", gameObject);
                        StartCoroutine(Dash());
                    }
                }
            }
        }

        public IEnumerator Dash()
        {
            Vector2 direction = (target.specRigidbody.UnitCenter - specRigidbody.UnitCenter).normalized;
            float ela = 0f;
            Vector2 center = sprite.WorldCenter;
            transform.rotation = Quaternion.Euler(0f, 0f, direction.ToAngle() - 90);
            sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
            while (ela < DashTime)
            {
                direction = (target.specRigidbody.UnitCenter - specRigidbody.UnitCenter).normalized;
                ela += BraveTime.DeltaTime;
                specRigidbody.Velocity = direction * DashSpeed;
                HandleParticles(-direction);
                yield return null;
            }
            Fade();
            yield break;
        }

        public void HandleParticles(Vector2 direction)
        {
            Vector3 vector = specRigidbody.UnitBottomLeft.ToVector3ZisY(0f);
            Vector3 vector2 = specRigidbody.UnitTopRight.ToVector3ZisY(0f);
            float num = (vector2.y - vector.y) * (vector2.x - vector.x);
            float num2 = 25f * num;
            int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
            int num4 = num3;
            Vector3 minPosition = vector;
            Vector3 maxPosition = vector2;
            float angleVariance = 25f;
            float magnitudeVariance = 0.2f;
            float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.25f));
            GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
        }

        public void EnemyCollision(SpeculativeRigidbody specRigidbody, SpeculativeRigidbody myRigidbody, CollisionData collisionData)
        {
            if(specRigidbody.GetComponent<AIActor>() != null && specRigidbody.GetComponent<HealthHaver>() != null && !hasCollided && CanCollide)
            {
                AIActor enemy = specRigidbody.GetComponent<AIActor>();
                if (enemy == target)
                {
                    float hitDamage = HitDamage * (owner != null ? owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1f);
                    enemy.healthHaver.SetHealthMaximum(Mathf.Max(enemy.healthHaver.GetMaxHealth() - hitDamage, 0f), -hitDamage, false);
                    enemy.healthHaver.ForceSetCurrentHealth(Mathf.Max(enemy.healthHaver.GetCurrentHealth(), 0f));
                    if (enemy.healthHaver.GetCurrentHealth() == 0f && enemy.healthHaver.Armor == 0f)
                    {
                        enemy.healthHaver.NextShotKills = false;
                        if (!enemy.healthHaver.SuppressDeathSounds)
                        {
                            AkSoundEngine.PostEvent("Play_ENM_death", enemy.healthHaver.gameObject);
                            AkSoundEngine.PostEvent(string.IsNullOrEmpty(enemy.healthHaver.overrideDeathAudioEvent) ? "Play_CHR_general_death_01" : enemy.healthHaver.overrideDeathAudioEvent, enemy.healthHaver.gameObject);
                        }
                        enemy.healthHaver.Die(specRigidbody.Velocity.normalized);
                    }
                    else if (StunTime > 0f && enemy.behaviorSpeculator != null)
                    {
                        enemy.behaviorSpeculator.Stun(StunTime, true);
                    }
                    hasCollided = true;
                    StopAllCoroutines();
                    Fade();
                }
            }
        }

        public void Fade()
        {
            AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", gameObject);
            hasCollided = true;
            currentState = State.Fading;
            StartCoroutine(FadeCR());
        }

        private IEnumerator FadeCR()
        {
            Vector2 startVelocity = specRigidbody.Velocity;
            float ela = 0f;
            AdditionalBraveLight braveLight = GetComponentInChildren<AdditionalBraveLight>();
            while(ela < FadeTime)
            {
                ela += BraveTime.DeltaTime;
                specRigidbody.Velocity = Vector2.Lerp(startVelocity, Vector2.zero, ela / FadeTime);
                if(braveLight != null)
                {
                    braveLight.LightIntensity = Mathf.Lerp(4.25f, 0f, ela / FadeTime);
                    braveLight.LightRadius = Mathf.Lerp(5.4375f, 0f, ela / FadeTime);
                }
                if(specRigidbody.Velocity != Vector2.zero)
                {
                    HandleParticles(-specRigidbody.Velocity.normalized);
                }
                sprite.scale = Vector2.one * Mathf.Lerp(1f, 10f, ela / FadeTime);
                sprite.color = sprite.color.WithAlpha(Mathf.Lerp(1f, 0f, ela / FadeTime));
                yield return null;
            }
            Destroy(gameObject);
            yield break;
        }

        [NonSerialized]
        public AIActor target;
        [NonSerialized]
        public PlayerController owner;
        private bool CanCollide;
        private bool hasCollided;
        private float dashChargeTime;
        public State currentState;
        public float DashChargeTime;
        public float FadeTime;
        public float DashTime;
        public float DashSpeed;
        public float HitDamage;
        public float StunTime;
        public enum State
        {
            Charging,
            Dashing,
            Fading
        }
    }
}
