using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class ScarStatusEffect : GameActorSpeedEffect
    {
        public const float BONUS_DMG_PER_SCAR = 0.05f;
        public static GameObject ScarVFX;
        public static ScarStatusEffect GlobalScarStatus = new()
        {
            AffectsEnemies = true,
            AffectsPlayers = false,
            AppliesDeathTint = false,
            AppliesOutlineTint = false,
            AppliesTint = false,
            duration = 10f,
            DeathTintColor = Color.clear,
            maxStackedDuration = -1f,
            OnlyAffectPlayerWhenGrounded = false,
            OutlineTintColor = Color.clear,
            OverheadVFX = ScarVFX,
            PlaysVFXOnActor = false,
            resistanceType = EffectResistanceType.None,
            stackMode = EffectStackingMode.Stack,
            TintColor = Color.clear
        };

        public ScarStatusEffect()
        {
            SpeedMultiplier = 1f;
            CooldownMultiplier = 1f;
            effectIdentifier = "SPAPI_scar";
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            if (actor is AIActor ai)
            {
                ai.Ext().scarCount = 1;
                ai.healthHaver.ModifyDamage += ScarDamage;
            }
        }

        public override void OnDarkSoulsAccumulate(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1, Projectile sourceProjectile = null)
        {
            if(actor is AIActor ai)
            {
                ai.Ext().scarCount++;
            }
        }

        public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
        {
            return false;
        }

        public void ScarDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if(hh.aiActor != null)
            {
                args.ModifiedDamage *= 1 + (hh.aiActor.Ext().scarCount * BONUS_DMG_PER_SCAR);
            }
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if(actor is AIActor ai)
            {
                ai.Ext().scarCount = 0;
                ai.healthHaver.ModifyDamage -= ScarDamage;
            }
        }
    }
}
