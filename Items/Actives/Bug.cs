using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class Bug : PlayerItem
    {
        public static void Init()
        {
            string name = "Bug";
            string shortdesc = "Yummy snack";
            string longdesc = "Heals and bugs out the user.\n\nThis bug probably escaped from someone's plate of spaghetti.";
            var item = EasyItemInit<Bug>("bug", "bug_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            item.consumable = true;
            item.multRange = 1.15f;
            item.SetCooldownType(CooldownType.None, 0f);
        }

        public override void DoEffect(PlayerController user)
        {
            user.healthHaver.ApplyHealing(0.5f);
            user.ownerlessStatModifiers.Add(CreateRandomStatMod(multRange, user));
            AkSoundEngine.PostEvent("EverhoodHandError", user.gameObject);
            base.DoEffect(user);
        }

        public static StatModifier CreateRandomStatMod(float multiplicativeRange, PlayerController owner = null)
        {
            List<PlayerStats.StatType> validStats = new()
            {
                PlayerStats.StatType.Damage,
                PlayerStats.StatType.MovementSpeed,
                PlayerStats.StatType.RateOfFire,
                PlayerStats.StatType.Accuracy,
                PlayerStats.StatType.ProjectileSpeed,
                PlayerStats.StatType.AmmoCapacityMultiplier,
                PlayerStats.StatType.ReloadSpeed,
                PlayerStats.StatType.KnockbackMultiplier,
                PlayerStats.StatType.GlobalPriceMultiplier,
                PlayerStats.StatType.PlayerBulletScale,
                PlayerStats.StatType.AdditionalClipCapacityMultiplier,
                PlayerStats.StatType.ThrownGunDamage,
                PlayerStats.StatType.DodgeRollDamage,
                PlayerStats.StatType.DamageToBosses,
                PlayerStats.StatType.EnemyProjectileSpeedMultiplier,
                PlayerStats.StatType.ChargeAmountMultiplier,
                PlayerStats.StatType.RangeMultiplier,
                PlayerStats.StatType.DodgeRollDistanceMultiplier,
                PlayerStats.StatType.DodgeRollSpeedMultiplier,
                PlayerStats.StatType.MoneyMultiplierFromEnemies
            };
            if(owner != null)
            {
                var stats = owner.stats;
                if (owner.healthHaver.GetMaxHealth() > 1)
                {
                    validStats.Add(PlayerStats.StatType.Health);
                }
                if (stats.GetStatValue(PlayerStats.StatType.Coolness) > 0)
                {
                    validStats.Add(PlayerStats.StatType.Coolness);
                }
                if(stats.GetStatValue(PlayerStats.StatType.AdditionalShotPiercing) > 0)
                {
                    validStats.Add(PlayerStats.StatType.AdditionalShotPiercing);
                }
                if (stats.GetStatValue(PlayerStats.StatType.Curse) > 0)
                {
                    validStats.Add(PlayerStats.StatType.Curse);
                }
                if (stats.GetStatValue(PlayerStats.StatType.AdditionalShotBounces) > 0)
                {
                    validStats.Add(PlayerStats.StatType.AdditionalShotBounces);
                }
                if (stats.GetStatValue(PlayerStats.StatType.AdditionalBlanksPerFloor) > 0)
                {
                    validStats.Add(PlayerStats.StatType.AdditionalBlanksPerFloor);
                }
            }
            var stat = validStats.RandomElement();
            List<PlayerStats.StatType> additiveStats = new()
            {
                PlayerStats.StatType.Health,
                PlayerStats.StatType.Coolness,
                PlayerStats.StatType.AdditionalShotPiercing,
                PlayerStats.StatType.Curse,
                PlayerStats.StatType.AdditionalShotBounces,
                PlayerStats.StatType.AdditionalBlanksPerFloor
            };
            var modifyMethod = ModifyMethodE.TrueMultiplicative;
            if (additiveStats.Contains(stat))
            {
                modifyMethod = StatModifier.ModifyMethod.ADDITIVE;
            }
            var amount = Random.Range(1f / multiplicativeRange, multiplicativeRange);
            if(modifyMethod == StatModifier.ModifyMethod.ADDITIVE)
            {
                var rand = BraveUtility.RandomBool();
                amount = stat == PlayerStats.StatType.Health ? (rand ? 0.5f : -0.5f) : (rand ? 1f : -1f);
            }
            return StatModifier.Create(stat, modifyMethod, amount);
        }

        public float multRange;
    }
}
