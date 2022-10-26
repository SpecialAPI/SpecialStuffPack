using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class HotSauce : PlayerItem
    {
        public static void Init()
        {
            var name = "Hot Sauce";
            var shortdesc = "Spicy!";
            var longdesc = "Burns the user. Grants various benefits while on fire.\n\nA simple bottle of hot sauce. Spicy but also delicious!";
            var item = EasyItemInit<HotSauce>("HotSauce", "hot_sauce_idle_001", name, shortdesc, longdesc, ItemQuality.C, null, null);
            item.SetCooldownType(CooldownType.Timed, 0.5f);
            item.FireVFX = PhoenixObject.muzzleFlashEffects;
            item.fireResistanceMultiplier = 0.5f;
            item.fireGoopRadius = 3f;
            item.fireModifier = HotLeadObject.FireModifierEffect;
            item.tint = HotLeadObject.TintColor;
        }

        public override void Pickup(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Add(modifier = new()
            {
                damageMultiplier = fireResistanceMultiplier,
                damageType = CoreDamageTypes.Fire
            });
            player.PostProcessProjectile += PostProcessProjectile;
            base.Pickup(player);
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if (LastOwner.IsOnFire)
            {
                proj.statusEffectsToApply.Add(fireModifier);
                proj.baseData.damage *= 1 + Mathf.Clamp01(LastOwner.CurrentFireMeterValue);
                proj.AdjustPlayerProjectileTint(tint, 1, 0f);
            }
        }

        public override void OnPreDrop(PlayerController user)
        {
            if(modifier != null)
            {
                user.healthHaver.damageTypeModifiers.Remove(modifier);
                modifier = null;
            }
            user.PostProcessProjectile -= PostProcessProjectile;
            base.OnPreDrop(user);
        }

        public override void DoEffect(PlayerController user)
        {
            if(user.healthHaver.GetDamageModifierForType(CoreDamageTypes.Fire) <= 0f)
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultFireGoop).TimedAddGoopCircle(user.sprite.WorldBottomCenter, fireGoopRadius, 0.5f, false);
            }
            else
            {
                user.IsOnFire = true;
            }
            FireVFX.SpawnAtPosition(user.sprite.WorldBottomCenter, 90f, null, null, null, null, false, null, null, false);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return !user.IsOnFire;
        }

        public VFXPool FireVFX;
        public float fireResistanceMultiplier;
        public float fireGoopRadius;
        public Color tint;
        public GameActorFireEffect fireModifier;
        private DamageTypeModifier modifier;
    }
}
