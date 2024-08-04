using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BalancingPole : PassiveItem
    {

        public static PlayerStats.StatType[] VALID_STATS = new PlayerStats.StatType[]
        {
            PlayerStats.StatType.MovementSpeed,
            PlayerStats.StatType.RateOfFire,
            PlayerStats.StatType.Accuracy,
            PlayerStats.StatType.Damage,
            PlayerStats.StatType.ProjectileSpeed,
            PlayerStats.StatType.AmmoCapacityMultiplier,
            PlayerStats.StatType.ReloadSpeed,
            PlayerStats.StatType.KnockbackMultiplier,
            PlayerStats.StatType.GlobalPriceMultiplier,
            PlayerStats.StatType.PlayerBulletScale,
            PlayerStats.StatType.AdditionalClipCapacityMultiplier,
            PlayerStats.StatType.ShadowBulletChance,
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

        public static PlayerStats.StatType[] STATS_TO_REVERSE = new PlayerStats.StatType[]
        {
            PlayerStats.StatType.Accuracy,
            PlayerStats.StatType.ReloadSpeed,
            PlayerStats.StatType.GlobalPriceMultiplier,
            PlayerStats.StatType.EnemyProjectileSpeedMultiplier
        };

        public static void Init()
        {
            string name = "Balancing Pole";
            string shortdesc = "Fair and balanced";
            string longdesc = "Balances the owner's stats.";
            EasyItemInit<BalancingPole>("items/balancingpole", "sprites/balancing_pole_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            IncrementFlag(player, GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            DecrementFlag(player, GetType());
            base.DisableEffect(player);
        }
    }
}
