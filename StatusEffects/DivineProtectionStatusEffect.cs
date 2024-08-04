using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class DivineProtectionStatusEffect : GameActorSpeedEffect
    {
        public static GameObject DivineVFX;

        public static DivineProtectionStatusEffect DivineProtectionGenerator(float duration)
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
                OverheadVFX = DivineVFX,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = EffectStackingMode.Stack,
                TintColor = Color.clear
            };
        }

        public DivineProtectionStatusEffect()
        {
            SpeedMultiplier = 1f;
            CooldownMultiplier = 1f;
            effectIdentifier = "SPAPI_divineprotection";
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            actor.healthHaver.Ext().ModifyDamageContextLater += DivineProtect;
        }

        public void DivineProtect(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args, Vector2 direction, string source, CoreDamageTypes type, DamageCategory category, bool ignoreInvulnerability, bool ignoreDPSCaps)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if(hh.aiActor != null)
            {
                if(DivineProtectionEffect(hh.aiActor, args.ModifiedDamage, direction, source, type))
                {
                    args.ModifiedDamage = 0f;
                }
            }
        }

        public static bool DivineProtectionEffect(AIActor enemy, float damage, Vector2 direction, string source, CoreDamageTypes type)
        {
            if (damage > 0f && enemy != null && enemy.ParentRoom != null)
            {
                var activeEnemies = enemy.ParentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null)
                {
                    var activeEnemiesThatAreNotDivineProtected = activeEnemies.FindAll(x => x.healthHaver != null && !x.healthHaver.IsDead && x.healthHaver.IsVulnerable && !IsDivineProtected(x));
                    if (activeEnemiesThatAreNotDivineProtected.Count > 0)
                    {
                        var dmg = damage / activeEnemiesThatAreNotDivineProtected.Count;
                        foreach (var en in activeEnemiesThatAreNotDivineProtected)
                        {
                            if (en != null)
                            {
                                en.healthHaver.ApplyDamage(dmg, direction, source, type, DamageCategory.Unstoppable, true, null, true);
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsDivineProtected(AIActor ai)
        {
            return ai.GetEffect("SPAPI_divineprotection") != null || ai.GetComponent<Immortal>() != null;
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            actor.healthHaver.Ext().ModifyDamageContextLater -= DivineProtect;
        }
    }
}
