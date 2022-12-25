using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class PlayerStatsExt : MonoBehaviour
    {
        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
        [HarmonyPrefix]
        public static void RecalculateStatsPrefix(PlayerStats __instance, PlayerController owner)
        {
            var ext = __instance.Ext();
            var mults = ext.trueMultiplicativeMults = new float[__instance.BaseStatValues.Count];
            for (int j = 0; j < mults.Length; j++)
            {
				mults[j] = 1f;
            }
			for (int k = 0; k < owner.ActiveExtraSynergies.Count; k++)
			{
				AdvancedSynergyEntry advancedSynergyEntry = GameManager.Instance.SynergyManager.synergies[owner.ActiveExtraSynergies[k]];
				if (advancedSynergyEntry.SynergyIsActive(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
				{
					for (int l = 0; l < advancedSynergyEntry.statModifiers.Count; l++)
					{
						StatModifier statModifier = advancedSynergyEntry.statModifiers[l];
						if (statModifier.modifyType == ModifyMethodE.TrueMultiplicative)
						{
							mults[(int)statModifier.statToBoost] *= statModifier.amount;
						}
					}
				}
			}
			for (int n = 0; n < owner.ownerlessStatModifiers.Count; n++)
			{
				StatModifier statModifier2 = owner.ownerlessStatModifiers[n];
				if (statModifier2.modifyType == ModifyMethodE.TrueMultiplicative)
				{
					mults[(int)statModifier2.statToBoost] *= statModifier2.amount;
				}
			}
		}

		[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.ApplyStatModifier))]
		[HarmonyPostfix]
		public static void ProcessTrueMultiplicativeStat(PlayerStats __instance, StatModifier modifier)
        {
			if(modifier.modifyType == ModifyMethodE.TrueMultiplicative)
            {
				var mults = __instance.Ext().trueMultiplicativeMults;
				if(mults != null)
                {
					int statToBoost = (int)modifier.statToBoost;
					if(mults.Length > statToBoost)
                    {
						mults[statToBoost] *= modifier.amount;
                    }
				}
			}
        }

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
        [HarmonyPostfix]
        public static void RecalculateStatsPostfix(PlayerStats __instance, PlayerController owner)
		{
			var mults = __instance.Ext().trueMultiplicativeMults;
			if(mults != null)
			{
				for (int i = 0; i < __instance.StatValues.Count && i < mults.Length; i++)
				{
					__instance.StatValues[i] *= mults[i];
				}
			}
			if(owner.GetFlagCount<BrokenCalculator>() > 0)
            {
				for (int i = 0; i < __instance.StatValues.Count && i < mults.Length; i++)
				{
                    if (BrokenCalculator.validStats.Contains((PlayerStats.StatType)i))
					{
						__instance.StatValues[i] *= Random.Range(1f / BrokenCalculator.randomRange, BrokenCalculator.randomRange);
					}
				}
			}
		}

        public float[] trueMultiplicativeMults;
    }
}
