using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.StatusEffects
{
    public class RustStatusEffect : GameActorEffect
    {
        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            if(actor is AIActor aiactor)
            {
                if(aiactor.healthHaver != null)
                {
                    aiactor.healthHaver.ModifyDamage += ApplyDamageMultipliers;
                }
                aiactor.LocalTimeScale *= speedMultiplier;
                if (!string.IsNullOrEmpty(onApplyEvent))
                {
                    AkSoundEngine.PostEvent(onApplyEvent, aiactor.gameObject);
                }
                if(effectData.vfxObjects == null)
                {
                    effectData.vfxObjects = new();
                }
                var indicator = ((GameObject)Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), aiactor.CenterPosition, Quaternion.identity, aiactor.transform)).GetComponent<HeatIndicatorController>();
                indicator.CurrentRadius = auraRadius;
                indicator.CurrentColor = auraColor;
                indicator.IsFire = false;
                effectData.vfxObjects.Add(new(indicator.gameObject, 0f));
            }
        }

        public void ApplyDamageMultipliers(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= damageMultiplier;
        }

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if(actor is AIActor aiactor)
            {
                if(aiactor.ParentRoom != null)
                {
                    aiactor.ParentRoom.ApplyActionToNearbyEnemies(aiactor.CenterPosition, auraRadius, (enemy, distance) =>
                    {
                        if (enemy != aiactor)
                        {
                            enemy.ApplyEffect(this, 1f, null);
                        }
                    });
                }
            }
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if (actor is AIActor aiactor)
            {
                if (aiactor.healthHaver != null)
                {
                    aiactor.healthHaver.ModifyDamage -= ApplyDamageMultipliers;
                }
                aiactor.LocalTimeScale /= speedMultiplier;
                if(effectData.vfxObjects != null)
                {
                    foreach(var tup in effectData.vfxObjects)
                    {
                        if (tup.First.GetComponent<HeatIndicatorController>() != null)
                        {
                            tup.First.GetComponent<HeatIndicatorController>().EndEffect();
                        }
                    }
                }
            }
        }

        public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
        {
            return actor != null && actor.healthHaver != null && actor.healthHaver.IsBoss && base.IsFinished(actor, effectData, elapsedTime);
        }

        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float auraRadius;
        public string onApplyEvent;
        public Color auraColor;
    }
}
