using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class RupturedStatusEffect : GameActorSpeedEffect
	{
		public static GameObject RupturedVFX;
		private float particleTime;

        public static RupturedStatusEffect RupturedGenerator(float duration, float dpsPerSpeed)
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
                OverheadVFX = RupturedVFX,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = EffectStackingMode.Stack,
                TintColor = Color.clear,
                damagePerSecondPer1Speed = dpsPerSpeed
            };
        }

        public RupturedStatusEffect()
        {
            SpeedMultiplier = 1f;
            CooldownMultiplier = 1f;
            effectIdentifier = "SPAPI_ruptured";
		}

		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			if (actor.specRigidbody != null && actor.specRigidbody.Velocity.magnitude > 0.05f)
			{
				actor.healthHaver.ApplyDamage(damagePerSecondPer1Speed * BraveTime.DeltaTime * actor.specRigidbody.Velocity.magnitude, Vector2.zero, effectIdentifier, CoreDamageTypes.None, DamageCategory.DamageOverTime);
				particleTime += BraveTime.DeltaTime * actor.specRigidbody.Velocity.magnitude;
				if (particleTime > 0.25f)
				{
					GlobalSparksDoer.DoRandomParticleBurst(1, actor.specRigidbody.UnitBottomLeft, actor.specRigidbody.UnitTopRight, Vector3.down, 120f, 0.5f, systemType: GlobalSparksDoer.SparksType.BLOODY_BLOOD);
					particleTime = 0f;
				}
			}
		}

		public float damagePerSecondPer1Speed;
	}
}
