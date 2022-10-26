using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BalancingPole : PassiveItem
    {
        public static PlayerStats.StatType[] STATS_THAT_DONT_MATTER = new PlayerStats.StatType[]
        {
            PlayerStats.StatType.Health,
            PlayerStats.StatType.Coolness,
            PlayerStats.StatType.Curse,
            PlayerStats.StatType.AdditionalGunCapacity,
            PlayerStats.StatType.AdditionalItemCapacity,
            PlayerStats.StatType.AdditionalShotPiercing,
            PlayerStats.StatType.AdditionalShotBounces,
            PlayerStats.StatType.AdditionalBlanksPerFloor,
            PlayerStats.StatType.TarnisherClipCapacityMultiplier,
            PlayerStats.StatType.ExtremeShadowBulletChance
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
            new Hook(typeof(PlayerStats).GetMethod(nameof(PlayerStats.RecalculateStatsInternal)), typeof(BalancingPole).GetMethod(nameof(BalancingPole.BalanceStats)));
        }

        public static void BalanceStats(Action<PlayerStats, PlayerController> orig, PlayerStats self, PlayerController player)
        {
            orig(self, player);
            if (IsFlagSetForCharacter(player, typeof(BalancingPole)))
            {
                List<float> stats = new();
                List<PlayerStats.StatType> types = new();
                var originalReloadSpeed = self.StatValues[(int)PlayerStats.StatType.ReloadSpeed];
                for (int i = 0; i < (int)PlayerStats.StatType.MoneyMultiplierFromEnemies + 1; i++)
                {
                    if (STATS_THAT_DONT_MATTER.Contains((PlayerStats.StatType)i))
                    {
                        continue;
                    }
                    float statmult = self.StatValues[i];
                    if (STATS_TO_REVERSE.Contains((PlayerStats.StatType)i))
                    {
                        statmult = 1 / Mathf.Max(statmult, 0.1f);
                    }
                    else if (i == (int)PlayerStats.StatType.MovementSpeed)
                    {
                        statmult /= 7;
                    }
                    else if ((PlayerStats.StatType)i is PlayerStats.StatType.ShadowBulletChance or PlayerStats.StatType.ExtremeShadowBulletChance)
                    {
                        statmult /= 100;
                    }
                    stats.Add(statmult);
                    types.Add((PlayerStats.StatType)i);
                }
                float avg = stats.Average();
                types.ForEach(x =>
                {
                    var val = avg;
                    if (STATS_TO_REVERSE.Contains(x))
                    {
                        if (val <= 0f)
                        {
                            val = 0.0025f;
                        }
                        val = 1 / val;
                    }
                    else if (x == PlayerStats.StatType.MovementSpeed)
                    {
                        val *= 7;
                    }
                    else if (x is PlayerStats.StatType.ShadowBulletChance or PlayerStats.StatType.ExtremeShadowBulletChance)
                    {
                        val *= 10;
                    }
                    self.StatValues[(int)x] = val;
                });
                if (originalReloadSpeed <= 0f)
                {
                    self.StatValues[(int)PlayerStats.StatType.ReloadSpeed] = originalReloadSpeed;
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            IncrementFlag(player, GetType());
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            DecrementFlag(disablingPlayer, GetType());
            base.DisableEffect(disablingPlayer);
        }
    }
}
