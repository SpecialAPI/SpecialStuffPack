using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class FrailStatusEffect : GameActorSpeedEffect
    {
        public static GameObject FrailVFX;

        public static FrailStatusEffect FrailGenerator(float duration)
        {
            return new()
            {
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesDeathTint = false,
                AppliesOutlineTint = false,
                AppliesTint = false,
                duration = duration,
                DeathTintColor = Color.clear,
                maxStackedDuration = -1f,
                OnlyAffectPlayerWhenGrounded = false,
                OutlineTintColor = Color.clear,
                OverheadVFX = FrailVFX,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = EffectStackingMode.Stack,
                TintColor = Color.clear
            };
        }

        public FrailStatusEffect()
        {
            SpeedMultiplier = 1f;
            CooldownMultiplier = 1f;
            effectIdentifier = "SPAPI_frail";
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            actor.healthHaver.ModifyDamage += DoubleDamage;
        }

        public void DoubleDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 2f;
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            actor.healthHaver.ModifyDamage -= DoubleDamage;
        }
    }
}
