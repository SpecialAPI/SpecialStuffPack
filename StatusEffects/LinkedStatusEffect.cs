using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class LinkedStatusEffect : GameActorSpeedEffect
    {
        public static GameObject LinkVFX;

        public static LinkedStatusEffect LinkedGenerator(float duration = 10f)
        {
            return new()
            {
                AffectsEnemies = true,
                AffectsPlayers = true,
                AppliesDeathTint = false,
                AppliesOutlineTint = false,
                AppliesTint = false,
                duration = duration,
                DeathTintColor = Color.clear,
                maxStackedDuration = -1f,
                OnlyAffectPlayerWhenGrounded = false,
                OutlineTintColor = Color.clear,
                OverheadVFX = LinkVFX,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = EffectStackingMode.Stack,
                TintColor = Color.clear
            };
        }

        public LinkedStatusEffect()
        {
            SpeedMultiplier = 1f;
            CooldownMultiplier = 1f;
            effectIdentifier = "SPAPI_linked";
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            actor.healthHaver.Ext().OnDamagedContextLater += Link;
        }

        public void Link(HealthHaver hh, float damage, string source, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection, bool ignoreInvulnerabilityFrames = false, bool ignoreDamageCaps = false)
        {
            if(damageTypes != CoreDamageTypesE.Linked && hh != null && hh.gameActor != null)
            {
                LinkEffect(hh.gameActor, damage, source, damageDirection);
            }
        }

        public static void LinkEffect(GameActor a, float damage, string source, Vector2 damageDirection)
        {
            if(a == null)
            {
                return;
            }
            RoomHandler r = null;
            PlayerController exceptionPlayer = null;
            AIActor exceptionEnemy = null;
            if (a is AIActor ai)
            {
                exceptionEnemy = ai;
                r = ai.ParentRoom;
            }
            else if (a is PlayerController play)
            {
                exceptionPlayer = play;
                r = play.CurrentRoom;
            }
            else
            {
                //Application.Quit();
            }
            if (r != null)
            {
                var enemies = r.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                if (enemies != null)
                {
                    foreach (var e in enemies)
                    {
                        if (e != null && e != exceptionEnemy && e.GetEffect("SPAPI_linked") != null)
                        {
                            e.healthHaver.ApplyDamage(damage * (exceptionPlayer != null ? 20f : 1f), damageDirection, source, CoreDamageTypesE.Linked, DamageCategory.Normal, true, null, true);
                        }
                    }
                }
            }
            foreach (var play in GameManager.Instance.AllPlayers)
            {
                if (play != null && play != exceptionPlayer && play.GetEffect("SPAPI_linked") != null)
                {
                    play.healthHaver.ApplyDamage(0.5f, damageDirection, "Linked", CoreDamageTypesE.Linked, DamageCategory.Normal, true, null, true);
                }
            }
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            actor.healthHaver.Ext().OnDamagedContextLater += Link;
        }
    }
}
