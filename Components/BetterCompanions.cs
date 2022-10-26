using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class BetterCompanions : BraveBehaviour
    {
        public void Start()
        {
            companion = GetComponent<CompanionController>();
            if(companion != null && companion.m_owner != null)
            {
                companion.m_owner.OnEnteredCombat += WarpToPlayer;
            }
            if(bulletBank != null)
            {
                bulletBank.OnProjectileCreated += MakeProjectilesDealIntendedDamage;
            }
            if (aiShooter != null)
            {
                aiShooter.PostProcessProjectile += MakeProjectilesDealIntendedDamage;
            }
            healthHaver.ModifyDamage += HalveTakenDamage;
        }

        [HarmonyPatch(typeof(CompanionController), nameof(CompanionController.HandleCompanionPostProcessProjectile))]
        [HarmonyPrefix]
        public static bool Override(CompanionController __instance, Projectile obj)
        {
            if (__instance.GetComponent<BetterCompanions>() != null)
            {
                if (obj)
                {
                    obj.collidesWithPlayer = false;
                    obj.TreatedAsNonProjectileForChallenge = true;
                }
                if (__instance.m_owner)
                {
                    if (PassiveItem.IsFlagSetForCharacter(__instance.m_owner, typeof(BattleStandardItem)))
                    {
                        obj.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
                    }
                    if (__instance.m_owner.CurrentGun && __instance.m_owner.CurrentGun.LuteCompanionBuffActive)
                    {
                        obj.baseData.damage *= 2f;
                        obj.RuntimeUpdateScale(1f / obj.AdditionalScaleMultiplier);
                        obj.RuntimeUpdateScale(1.75f);
                    }
                }
                return false;
            }
            return true;
        }

        public void HalveTakenDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage /= 2f;
        }

        public void MakeProjectilesDealIntendedDamage(Projectile obj)
        {
            obj.baseData.damage = 5;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (companion != null && companion.m_owner != null)
            {
                companion.m_owner.OnEnteredCombat -= WarpToPlayer;
            }
        }

        public void WarpToPlayer()
        {
            if (companion != null && companion.m_owner != null)
            {
                aiActor.CompanionWarp(companion.m_owner.transform.position);
            }
        }

        public void Update()
        {
            if(companion != null && aiActor != null)
            {
                aiActor.ParentRoom = companion.m_owner.CurrentRoom;
            }
        }

        public CompanionController companion;
    }
}
