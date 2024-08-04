using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class BrokenCalculator : PassiveItem
    {
        public static void Init()
        {
            string name = "Broken Calculator";
            string shortdesc = "Miscalculated stats";
            string longdesc = "The owner's stats will no longer be properly calculated.\n\nA calculator that has been completely broken, becoming absolutely unusable.";
            EasyItemInit<BrokenCalculator>("items/brokencalculator", "sprites/broken_calculator_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            player.IncrementFlag<BrokenCalculator>();
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
			if(player == null)
            {
				return;
            }
            player.DecrementFlag<BrokenCalculator>();
        }

        public static float randomRange = 1.15f;
        public static List<PlayerStats.StatType> validStats = new()
		{
			PlayerStats.StatType.Damage,
			PlayerStats.StatType.MovementSpeed,
			PlayerStats.StatType.RateOfFire,
			PlayerStats.StatType.Accuracy,
			PlayerStats.StatType.ProjectileSpeed,
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
			PlayerStats.StatType.MoneyMultiplierFromEnemies,
			PlayerStats.StatType.Coolness,
			PlayerStats.StatType.ExtremeShadowBulletChance,
			PlayerStats.StatType.ShadowBulletChance
		};
	}
}
