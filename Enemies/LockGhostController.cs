using SpecialStuffPack.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Enemies
{
    public class LockGhostController : BraveBehaviour
    {
        public IEnumerator Start()
        {
            m_instance = this;
            if(GameStatsManager.HasInstance && encounterTrackable != null)
            {
                GameStatsManager.Instance.HandleEncounteredObject(encounterTrackable);
            }
            specRigidbody.enabled = false;
            aiAnimator.LockFacingDirection = true;
            aiAnimator.FacingDirection = 90;
            isSpawning = true;
            renderer.enabled = true;
            Vector2 position = sprite.WorldCenter;
            preventMovement = true;
            specRigidbody.OnTriggerCollision += PlayerCollision;
            if(target == null)
            {
                foreach(PlayerController play in GameManager.Instance.AllPlayers)
                {
                    if(PassiveItem.IsFlagSetForCharacter(play, typeof(GoldKey)))
                    {
                        target = play;
                    }
                }
            }
            GameObject light = new GameObject("light");
            AdditionalBraveLight braveLight = light.AddComponent<AdditionalBraveLight>();
            light.transform.position = sprite.WorldCenter;
            braveLight.LightColor = Color.red;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;
            float dura = 2f;
            float ela = 0f; 
            AkSoundEngine.PostEvent("Play_ENM_reaper_spawn_01", gameObject);
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
            AkSoundEngine.PostEvent("Play_CHR_ghost_appear_01", gameObject);
            light.transform.parent = transform;
            isSpawning = false;
            yield return new WaitForSeconds(1.5f);
            preventMovement = false;
            aiAnimator.LockFacingDirection = false;
            specRigidbody.enabled = true;
            specRigidbody.Reinitialize();
            specRigidbody.ForceRegenerate();
            yield break;
        }

        public static LockGhostController Instance
        {
            get
            {
                return m_instance;
            }
        }

        public bool ShouldMoveNormally
        {
            get
            {
                if (GameManager.IsBossIntro || BossKillCam.BossDeathCamRunning)
                {
                    return false;
                }
                if(GameManager.HasInstance && GameManager.Instance.PreventPausing)
                {
                    return false;
                }
                if(target != null && (target.CurrentInputState == PlayerInputState.NoInput || target.CurrentInputState == PlayerInputState.NoMovement))
                {
                    return false;
                }
                if (TimeTubeCreditsController.IsTimeTubing)
                {
                    gameObject.SetActive(false);
                    return false;
                }
                return true;
            }
        }

        public void Update()
        {
            if(target != null)
            {
                if(currentState == State.Chasing)
                {
                    Vector2 vec = target.CenterPosition - specRigidbody.UnitCenter;
                    float speed = Mathf.Lerp(MinSpeed, MaxSpeed, (vec.magnitude - MinSpeedDistance) / (MaxSpeedDistance - MinSpeedDistance));
                    specRigidbody.Velocity = vec.normalized * speed;
                    if (Vector2.Distance(specRigidbody.UnitCenter, target.CenterPosition) <= PreferredDistance || preventMovement)
                    {
                        specRigidbody.Velocity = Vector2.zero;
                    }
                    if (Vector2.Distance(specRigidbody.UnitCenter, target.CenterPosition) <= DashTimerDistance && ShouldMoveNormally && !questFailDashActive)
                    {
                        nearPlayerTime += BraveTime.DeltaTime;
                        if(nearPlayerTime >= DashCooldownTime)
                        {
                            currentState = State.Charging;
                            aiAnimator.LockFacingDirection = true;
                            nearPlayerTime = 0f;
                            aiAnimator.PlayUntilCancelled("charge", false, null, -1, false);
                            AkSoundEngine.PostEvent("Play_CHR_ghost_appear_01", gameObject);
                        }
                    }
                    else
                    {
                        nearPlayerTime = Mathf.Max(nearPlayerTime - BraveTime.DeltaTime, 0f);
                    }
                }
                else if(currentState == State.Charging)
                {
                    specRigidbody.Velocity = (specRigidbody.UnitCenter - target.CenterPosition).normalized;
                    dashChargeTime += BraveTime.DeltaTime;
                    if (!ShouldMoveNormally)
                    {
                        dashChargeTime = 0f;
                        currentState = State.Chasing;
                        aiAnimator.LockFacingDirection = false;
                        aiAnimator.EndAnimation();
                        return;
                    }
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

        public void LateUpdate()
        {
            if(!cachedShouldMoveNormally && ShouldMoveNormally)
            {
                if(target != null && target.specRigidbody != null && specRigidbody != null)
                {
                    specRigidbody.ForceRegenerate();
                    target.specRigidbody.ForceRegenerate();
                    specRigidbody.RegisterTemporaryCollisionException(target.specRigidbody, 0f, 1f);
                    target.specRigidbody.RegisterTemporaryCollisionException(specRigidbody, 0f, 1f);
                }
            }
            cachedShouldMoveNormally = ShouldMoveNormally;
        }

        public void QuestFailed(bool isEndTimes)
        {
            if (!questFailDashActive)
            {
                StopAllCoroutines();
                StartCoroutine(QuestFailDash(isEndTimes));
                questFailDashActive = true;
            }
        }

        private IEnumerator QuestFailDash(bool isEndTimes)
        {
            yield return null;
            while (preventMovement)
            {
                yield return null;
            }
            if (!isEndTimes)
            {
                yield return new WaitForSeconds(3);
            }
            if (aiAnimator != null)
            {
                aiAnimator.LockFacingDirection = false;
                aiAnimator.EndAnimation();
            }
            currentState = State.Dashing;
            AkSoundEngine.PostEvent("Play_ENM_cannonball_launch_01", gameObject);
            while (true)
            {
                if (target != null && ShouldMoveNormally)
                {
                    Vector2 direction = (target.CenterPosition - specRigidbody.UnitCenter).normalized;
                    transform.rotation = Quaternion.Euler(0f, 0f, direction.ToAngle() - 90);
                    specRigidbody.Velocity = direction * DashSpeed;
                    HandleParticles(-direction);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                    specRigidbody.Velocity = Vector2.zero;
                }
                yield return null;
            }
        }

        public IEnumerator Dash()
        {
            Vector2 direction = (target.CenterPosition - specRigidbody.UnitCenter).normalized;
            float ela = 0f;
            Vector2 center = sprite.WorldCenter;
            transform.rotation = Quaternion.Euler(0f, 0f, direction.ToAngle() - 90);
            sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
            while (ela < DashTime)
            {
                ela += BraveTime.DeltaTime;
                specRigidbody.Velocity = direction * DashSpeed;
                HandleParticles(-direction);
                if (!ShouldMoveNormally)
                {
                    center = sprite.WorldCenter;
                    transform.rotation = Quaternion.identity;
                    sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                    aiAnimator.EndAnimation();
                    currentState = State.Chasing;
                    yield break;
                }
                yield return null;
            }
            ela = 0f;
            while(ela < DashSlowdownTime)
            {
                specRigidbody.Velocity = direction * Mathf.Lerp(DashSpeed, 0f, ela / DashSlowdownTime);
                center = sprite.WorldCenter;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(direction.ToAngle() - 90, 0f, ela / DashSlowdownTime));
                sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                ela += BraveTime.DeltaTime;
                HandleParticles(-direction);
                if (!ShouldMoveNormally)
                {
                    center = sprite.WorldCenter;
                    transform.rotation = Quaternion.identity;
                    sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                    aiAnimator.EndAnimation();
                    currentState = State.Chasing;
                    yield break;
                }
                yield return null;
            }
            center = sprite.WorldCenter;
            transform.rotation = Quaternion.identity;
            sprite.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
            aiAnimator.EndAnimation();
            currentState = State.Chasing;
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

        public void PlayerCollision(SpeculativeRigidbody specRigidbody, SpeculativeRigidbody myRigidbody, CollisionData collisionData)
        {
            if(specRigidbody.GetComponent<PlayerController>() != null && specRigidbody.GetComponent<HealthHaver>() != null && !hasCollided && ShouldMoveNormally)
            {
                PlayerController player = specRigidbody.GetComponent<PlayerController>();
                bool didIgnoreDamage = false;
                if (player == target && player.healthHaver.ModifyDamage != null)
                {
                    HealthHaver.ModifyDamageEventArgs modifyDamageEventArgs = new HealthHaver.ModifyDamageEventArgs
                    {
                        InitialDamage = 0.5f,
                        ModifiedDamage = 0.5f
                    };
                    player.healthHaver.ModifyDamage(player.healthHaver, modifyDamageEventArgs);
                    didIgnoreDamage  = modifyDamageEventArgs.ModifiedDamage <= 0f;
                }
                if (player == target && !player.spriteAnimator.QueryInvulnerabilityFrame() && player.healthHaver.IsVulnerable && !didIgnoreDamage)
                {
                    player.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, -3f));
                    player.carriedConsumables.KeyBullets = 0;
                    player.carriedConsumables.Currency = 0;
                    player.Blanks = 0;
                    player.RemovePassiveItem(GoldKey.GoldKeyId);
                    Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab"), collisionData.Contact, Quaternion.identity);
                    AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", player.gameObject);
                    AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", player.gameObject);
                    StopAllCoroutines();
                    Fade();
                }
                else if(player == target && didIgnoreDamage)
                {
                    if(specRigidbody != null && myRigidbody != null)
                    {
                        specRigidbody.ForceRegenerate();
                        myRigidbody.ForceRegenerate();
                        specRigidbody.RegisterTemporaryCollisionException(myRigidbody, 0f, new float?(0.5f));
                        myRigidbody.RegisterTemporaryCollisionException(specRigidbody, 0f, new float?(0.5f));
                    }
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
        public bool preventMovement;
        [NonSerialized]
        public bool isSpawning;
        [NonSerialized]
        public PlayerController target;
        private static LockGhostController m_instance;
        private float nearPlayerTime;
        private bool hasCollided;
        private bool questFailDashActive;
        private bool cachedShouldMoveNormally;
        private float dashChargeTime;
        public State currentState;
        public float DashTimerDistance;
        public float DashChargeTime;
        public float FadeTime;
        public float DashTime;
        public float DashSlowdownTime;
        public float DashSpeed;
        public float DashCooldownTime;
        public float PreferredDistance;
        public float MinSpeed;
        public float MaxSpeed;
        public float MinSpeedDistance;
        public float MaxSpeedDistance;
        public enum State
        {
            Chasing,
            Charging,
            Dashing,
            Fading
        }
    }
}
