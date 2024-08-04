using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
			ext.unprocessedHealthDecrement = 0f;
			ext.unprocessedHealthMult = 1f;
            var mults = ext.trueMultiplicativeMults = new float[__instance.BaseStatValues.Count];
			var exps = ext.exponentMods = new float[__instance.BaseStatValues.Count];
			for (int j = 0; j < __instance.BaseStatValues.Count; j++)
            {
				mults[j] = 1f;
				exps[j] = 1f;
			}
			foreach(var s in owner.ActiveExtraSynergies)
			{
				AdvancedSynergyEntry advancedSynergyEntry = GameManager.Instance.SynergyManager.synergies[s];
				if (advancedSynergyEntry.SynergyIsActive(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
				{
					foreach(var mod in advancedSynergyEntry.statModifiers)
					{
						if (mod.modifyType == ModifyMethodE.TrueMultiplicative)
						{
							mults[(int)mod.statToBoost] *= mod.amount;
						}
						else if(mod.modifyType == ModifyMethodE.Exponent)
						{
							exps[(int)mod.statToBoost] *= mod.amount;
						}
					}
				}
			}
			foreach(var mod in owner.ownerlessStatModifiers)
			{
				if (mod.modifyType == ModifyMethodE.TrueMultiplicative)
				{
					mults[(int)mod.statToBoost] *= mod.amount;
				}
				else if (mod.modifyType == ModifyMethodE.Exponent)
				{
					exps[(int)mod.statToBoost] *= mod.amount;
				}
                if (!mod.hasBeenOwnerlessProcessed)
                {
					ext.AdditionalModifierProcessing(mod);
                }
			}
			foreach (var passive in owner.passiveItems)
			{
				if (passive.passiveStatModifiers != null && passive.passiveStatModifiers.Length > 0)
				{
					foreach (var mod in passive.passiveStatModifiers)
					{
						if (mod != null && !passive.HasBeenStatProcessed)
						{
							ext.AdditionalModifierProcessing(mod);
						}
					}
				}
				if (passive is BasicStatPickup statpickup)
				{
					foreach (var mod in statpickup.modifiers)
					{
						if (mod != null && !passive.HasBeenStatProcessed)
						{
							ext.AdditionalModifierProcessing(mod);
						}
					}
				}
			}
			foreach (var active in owner.activeItems)
			{
				if (active.passiveStatModifiers != null && active.passiveStatModifiers.Length > 0)
				{
					foreach (var mod in active.passiveStatModifiers)
					{
						if (mod != null && !active.HasBeenStatProcessed)
						{
							ext.AdditionalModifierProcessing(mod);
						}
					}
				}
				var holder = active.GetComponent<StatHolder>();
				if (holder && (!holder.RequiresPlayerItemActive || active.IsCurrentlyActive))
				{
					foreach (var mod in holder.modifiers)
					{
						if (mod != null && !active.HasBeenStatProcessed)
						{
							ext.AdditionalModifierProcessing(mod);
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(PickupObject), nameof(PickupObject.HandlePickupCurseParticles), new Type[0])]
		[HarmonyPrefix]
		public static bool BetterCurseCheck(PickupObject __instance)
        {
			if(__instance == null || __instance.sprite == null)
            {
				return false;
            }
			if (__instance is Gun g)
			{
				if(g.passiveStatModifiers == null)
                {
					return false;
                }
				foreach (var mod in g.passiveStatModifiers)
				{
					if (mod != null && mod.statToBoost == PlayerStats.StatType.Curse && mod.IncreasesStat())
					{
						return true;
                    }
				}
			}
			else if (__instance is PlayerItem a)
			{
				if (a.passiveStatModifiers == null)
				{
					return false;
				}
				foreach (var mod in a.passiveStatModifiers)
				{
					if (mod != null && mod.statToBoost == PlayerStats.StatType.Curse && mod.IncreasesStat())
					{
						return true;
					}
				}
			}
			else if (__instance is PassiveItem p)
			{
				if (p.passiveStatModifiers == null)
				{
					return false;
				}
				foreach (var mod in p.passiveStatModifiers)
				{
					if (mod != null && mod.statToBoost == PlayerStats.StatType.Curse && mod.IncreasesStat())
					{
						return true;
					}
				}
			}
			return false;
		}

        public void AdditionalModifierProcessing(StatModifier mod)
        {
			if (mod.statToBoost == PlayerStats.StatType.Health && (mod.modifyType == ModifyMethodE.TrueMultiplicative || mod.modifyType == ModifyMethodE.Exponent))
			{
				unprocessedHealthDecrement += mod.amount;
				if (mod.amount > 1f && mod.modifyType == ModifyMethodE.TrueMultiplicative)
				{
					unprocessedHealthMult *= mod.amount;
				}
				if (mod.modifyType == ModifyMethodE.Exponent)
				{
					unprocessedHealthExp *= mod.amount;
				}
			}
        }

		public void InternalRecalculateStats(PlayerController owner, PlayerStats ogstats, ref float healAmount)
        {
			if(baseValueKeys == null)
            {
				Awake();
            }
            Dictionary<string, float> additive = new();
			Dictionary<string, float> multiplicative = new();
			Dictionary<string, float> tmultiplicative = new();
			Dictionary<string, float> exponent = new();
			foreach (var s in owner.ActiveExtraSynergies)
			{
				if(s >= GameManager.Instance.SynergyManager.synergies.Length)
                {
					continue;
                }
				var entry = GameManager.Instance.SynergyManager.synergies[s];
				if (entry == null || entry.statModifiers == null || !entry.SynergyIsActive(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
				{
					continue;
				}
				foreach (var mod in entry.statModifiers)
				{
					if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
					{
						ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
					}
				}
			}
			foreach(var mod in owner.ownerlessStatModifiers)
            {
				if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
				{
					ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
				}
			}
			foreach(var passive in owner.passiveItems)
			{
				if (passive.passiveStatModifiers != null && passive.passiveStatModifiers.Length > 0)
				{
					foreach(var mod in passive.passiveStatModifiers)
					{
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
				if (passive is BasicStatPickup statpickup)
				{
					foreach(var mod in statpickup.modifiers)
                    {
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
				if (passive is CoopPassiveItem coop && (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || (GameManager.Instance.PrimaryPlayer.healthHaver && GameManager.Instance.PrimaryPlayer.healthHaver.IsDead) || owner.HasActiveBonusSynergy(CustomSynergyType.THE_TRUE_HERO)))
				{
					foreach(var mod in coop.modifiers)
                    {
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
			}
			if (owner.inventory != null && owner.inventory.AllGuns != null)
			{
				if (owner.inventory.CurrentGun != null && owner.inventory.CurrentGun.currentGunStatModifiers != null && owner.inventory.CurrentGun.currentGunStatModifiers.Length > 0)
				{
					foreach(var mod in owner.inventory.CurrentGun.currentGunStatModifiers)
					{
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
				foreach(var g in owner.inventory.AllGuns)
                {
					if(g && g.passiveStatModifiers != null && g.passiveStatModifiers.Length > 0)
                    {
						foreach(var mod in g.passiveStatModifiers)
						{
							if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
							{
								ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
							}
						}
                    }
                }
			}
			foreach(var active in owner.activeItems)
			{
				if (active.passiveStatModifiers != null && active.passiveStatModifiers.Length > 0)
				{
					foreach(var mod in active.passiveStatModifiers)
					{
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
				var holder = active.GetComponent<StatHolder>();
				if (holder && (!holder.RequiresPlayerItemActive || active.IsCurrentlyActive))
				{
					foreach(var mod in holder.modifiers)
					{
						if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
						{
							ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
						}
					}
				}
			}
			var currentItem = owner.CurrentItem;
			if (currentItem && currentItem is ActiveBasicStatItem activestat && currentItem.IsActive)
			{
				foreach(var mod in activestat.modifiers)
				{
					if (mod != null && newStatsForMods.ContainsKey(mod.statToBoost))
					{
						ProcessMod(mod, additive, multiplicative, tmultiplicative, exponent);
					}
				}
			}
			var tarots = owner.Ext().tarotCards;
			var synergyActive = owner.PlayerHasActiveSynergy("A Better Fate");
			foreach (var card in tarots)
			{
				switch (card)
				{
					case Items.Pickups.TarotCards.TarotCardType.TheArachnid:
						additive["LambPoisonChance"] += synergyActive ? 0.5f : 0.3f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.TrueSight:
						additive["LambCritChance"] += synergyActive ? 0.15f : 0.1f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.WeepingMoon:
						if (additive.ContainsKey("EvenFloorDamageBonus"))
						{
							additive["EvenFloorDamageBonus"] += synergyActive ? 0.45f : 0.35f;
						}
						else
						{
							additive["EvenFloorDamageBonus"] = synergyActive ? 0.45f : 0.35f;
						}
						break;
					case Items.Pickups.TarotCards.TarotCardType.AllSeeingSun:
						if (additive.ContainsKey("OddFloorDamageBonus"))
						{
							additive["OddFloorDamageBonus"] +=  synergyActive ? 0.35f : 0.25f;
						}
						else
						{
							additive["OddFloorDamageBonus"] = synergyActive ? 0.35f : 0.25f;
						}
						break;
					case Items.Pickups.TarotCards.TarotCardType.ConsecratedOil:
					case Items.Pickups.TarotCards.TarotCardType.FervoursHarvest:
						if (additive.ContainsKey("ActiveChargeMultiplier"))
						{
							additive["ActiveChargeMultiplier"] += synergyActive ? 0.35f : 0.25f;
						}
						else
						{
							additive["ActiveChargeMultiplier"] = synergyActive ? 0.35f : 0.25f;
						}
						break;
					case Items.Pickups.TarotCards.TarotCardType.FervoursHost:
						additive["ActiveChargeOnRoomEnter"] += synergyActive ? 150f : 100f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.StrengthFromWithout:
						additive["ActiveChargeOnDamage"] += synergyActive ? 300f : 200f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.StrengthFromWithin:
						additive["ActiveChargePerSecond"] += synergyActive ? 7.5f : 5f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.DivineCurse:
						if (multiplicative.ContainsKey("ActiveChargeMultiplier"))
						{
							multiplicative["ActiveChargeMultiplier"] *= synergyActive ? 0.66f : 0.75f;
						}
						else
						{
							multiplicative["ActiveChargeMultiplier"] = synergyActive ? 0.66f : 0.75f;
						}
						break;
					case Items.Pickups.TarotCards.TarotCardType.RabbitsFoot:
						if (multiplicative.ContainsKey("BulletChanceMultiplier"))
						{
							multiplicative["BulletChanceMultiplier"] *= synergyActive ? 1.35f : 1.25f;
						}
						else
						{
							multiplicative["BulletChanceMultiplier"] = synergyActive ? 1.35f : 1.25f;
						}
						ogstats.StatValues[(int)PlayerStats.StatType.Coolness] += synergyActive ? 2f : 1f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.TheHeartsI:
						ogstats.StatValues[(int)PlayerStats.StatType.Health] += synergyActive ? 1f : 0.5f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.TheHeartsII:
						ogstats.StatValues[(int)PlayerStats.StatType.Health] += synergyActive ? 1.5f : 1f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.TheHeartsIII:
						ogstats.StatValues[(int)PlayerStats.StatType.Health] += synergyActive ? 2.5f : 2f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.DivineStrength:
						ogstats.StatValues[(int)PlayerStats.StatType.RateOfFire] += synergyActive ? 0.35f : 0.25f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.MasterOfTheArt:
						ogstats.StatValues[(int)PlayerStats.StatType.Damage] += synergyActive ? 0.3f : 0.2f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.ThePath:
						ogstats.StatValues[(int)PlayerStats.StatType.MovementSpeed] += synergyActive ? 3f : 2f;
						break;
					case Items.Pickups.TarotCards.TarotCardType.TheCollector:
						ogstats.StatValues[(int)PlayerStats.StatType.AdditionalItemCapacity] += synergyActive ? 2f : 1f;
						break;
				}
			}
			if (statKeys == null)
            {
				statKeys = new();
				statValues = new();
            }
			statValues.Clear();
			statKeys.Clear();
			for (int i = 0; i < baseValueKeys.Count; i++)
            {
				statKeys.Add(baseValueKeys[i]);
				statValues.Add(baseValueValues[i]);
            }
			foreach(var kvp in multiplicative)
            {
				if (statKeys.Contains(kvp.Key))
                {
					statValues[statKeys.IndexOf(kvp.Key)] *= kvp.Value;
                }
                else
                {
					statKeys.Add(kvp.Key);
					baseValueKeys.Add(kvp.Key);
					statValues.Add(kvp.Value);
					baseValueValues.Add(1f);
                }
			}
			foreach (var kvp in additive)
			{
				if (statKeys.Contains(kvp.Key))
				{
					statValues[statKeys.IndexOf(kvp.Key)] += kvp.Value;
				}
				else
				{
					statKeys.Add(kvp.Key);
					baseValueKeys.Add(kvp.Key);
					statValues.Add(1f + kvp.Value);
					baseValueValues.Add(1f);
				}
			}
			foreach (var kvp in tmultiplicative)
			{
				if (statKeys.Contains(kvp.Key))
				{
					statValues[statKeys.IndexOf(kvp.Key)] *= kvp.Value;
				}
				else
				{
					statKeys.Add(kvp.Key);
					baseValueKeys.Add(kvp.Key);
					statValues.Add(kvp.Value);
					baseValueValues.Add(1f);
				}
			}
			foreach (var kvp in exponent)
			{
				if (statKeys.Contains(kvp.Key))
				{
					statValues[statKeys.IndexOf(kvp.Key)] = Mathf.Pow(statValues[statKeys.IndexOf(kvp.Key)], kvp.Value);
				}
				else
				{
					statKeys.Add(kvp.Key);
					baseValueKeys.Add(kvp.Key);
					statValues.Add(1f);
					baseValueValues.Add(1f);
				}
			}
			var mults = trueMultiplicativeMults;
			var exps = exponentMods;
			if (owner.GetFlagCount<BrokenCalculator>() > 0)
			{
				for (int i = 0; i < ogstats.StatValues.Count && i < mults.Length; i++)
				{
					if (BrokenCalculator.validStats.Contains((PlayerStats.StatType)i))
					{
						ogstats.StatValues[i] *= Mathf.Pow(Random.Range(1f / BrokenCalculator.randomRange, BrokenCalculator.randomRange), owner.GetFlagCount<BrokenCalculator>());
					}
				}
			}
			healAmount -= unprocessedHealthDecrement;
			var oghealth = ogstats.StatValues[(int)PlayerStats.StatType.Health];
			if (mults != null)
			{
				for (int i = 0; i < ogstats.StatValues.Count && i < mults.Length; i++)
				{
					ogstats.StatValues[i] *= mults[i];
					if(i == (int)PlayerStats.StatType.Health)
                    {
						healAmount *= mults[i];
                    }
				}
				if (GameManager.HasInstance)
				{
					if (GameManager.Instance.nextLevelIndex % 2 == 0)
					{
						ogstats.StatValues[(int)PlayerStats.StatType.Damage] *= ogstats.Ext().GetStatValue("EvenFloorDamageMult");
					}
					else
					{
						ogstats.StatValues[(int)PlayerStats.StatType.Damage] *= ogstats.Ext().GetStatValue("OddFloorDamageMult");
					}
				}
			}
			if (exps != null)
			{
				for (int i = 0; i < ogstats.StatValues.Count && i < exps.Length; i++)
				{
					ogstats.StatValues[i] = Mathf.Pow(ogstats.StatValues[i], exps[i]);
				}
			}
			healAmount += oghealth * (unprocessedHealthMult - 1) + Mathf.Max(Mathf.Pow(oghealth, unprocessedHealthExp) - oghealth, 0f);
			if (owner.GetFlagCount<BalancingPole>() > 0)
			{
				List<float> stats = new();
				List<PlayerStats.StatType> types = new();
				var originalReloadSpeed = ogstats.StatValues[(int)PlayerStats.StatType.ReloadSpeed];
				for (int i = 0; i < (int)PlayerStats.StatType.MoneyMultiplierFromEnemies + 1; i++)
				{
					if (!BalancingPole.VALID_STATS.Contains((PlayerStats.StatType)i))
					{
						continue;
					}
					float statmult = ogstats.StatValues[i];
					if (BalancingPole.STATS_TO_REVERSE.Contains((PlayerStats.StatType)i))
					{
						statmult = 1 / Mathf.Max(statmult, 0.0025f);
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
					if (BalancingPole.STATS_TO_REVERSE.Contains(x))
					{
						val = 1 / Mathf.Max(val, 0.0025f);
					}
					else if (x == PlayerStats.StatType.MovementSpeed)
					{
						val *= 7;
					}
					else if (x is PlayerStats.StatType.ShadowBulletChance or PlayerStats.StatType.ExtremeShadowBulletChance)
					{
						val *= 10;
					}
					ogstats.StatValues[(int)x] = val;
				});
				if (originalReloadSpeed <= 0f)
				{
					ogstats.StatValues[(int)PlayerStats.StatType.ReloadSpeed] = originalReloadSpeed;
				}
			}
			ogstats.StatValues[(int)PlayerStats.StatType.Health] = Mathf.Round(ogstats.StatValues[(int)PlayerStats.StatType.Health] * 2) / 2;
			healAmount = Mathf.Round(healAmount * 2) / 2;
		}

		[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.ApplyStatModifier))]
		[HarmonyPrefix]
		internal static bool IgnoreCustomStatMods(StatModifier modifier)
		{
			return !newStatsForMods.ContainsKey(modifier.statToBoost);
		}

		public void Awake()
        {
			if(baseValueKeys == null)
            {
				baseValueKeys = new();
				baseValueValues = new();
				foreach(var kvp in overrideBaseStatValues)
                {
					baseValueKeys.Add(kvp.Key);
					baseValueValues.Add(kvp.Value);
                }
            }
        }

		public float GetStatValue(string name)
        {
            if (!statKeys.Contains(name))
            {
				return 1f;
            }
			return statValues[statKeys.IndexOf(name)];
        }

		public float GetBaseStatValue(string name)
        {
            if (!baseValueKeys.Contains(name))
            {
				baseValueKeys.Add(name);
				baseValueValues.Add(1f);
            }
			return baseValueValues[baseValueKeys.IndexOf(name)];
        }

		public void SetBaseStatValue(string name, float value, PlayerController owner = null)
        {
            if (!baseValueKeys.Contains(name))
            {
				baseValueKeys.Add(name);
				baseValueValues.Add(value);
            }
            else
            {
				baseValueValues[baseValueKeys.IndexOf(name)] = value;
            }
			if(owner != null)
            {
				owner.RecalculateStats();
            }
        }

		public void ProcessMod(StatModifier mod, Dictionary<string, float> additive, Dictionary<string, float> multiplicative, Dictionary<string, float> tmultiplicative, Dictionary<string, float> exponent)
        {
			var actualkey = newStatsForMods[mod.statToBoost];
			if (mod.modifyType == StatModifier.ModifyMethod.ADDITIVE)
            {
                if (additive.ContainsKey(actualkey))
				{
					additive[actualkey] += mod.amount;
				}
                else
                {
					additive[actualkey] = mod.amount;
                }
            }
			else if(mod.modifyType == StatModifier.ModifyMethod.MULTIPLICATIVE)
			{
				if (multiplicative.ContainsKey(actualkey))
				{
					multiplicative[actualkey] *= mod.amount;
				}
				else
				{
					multiplicative[actualkey] = mod.amount;
				}
			}
			else if(mod.modifyType == ModifyMethodE.TrueMultiplicative)
			{
				if (tmultiplicative.ContainsKey(actualkey))
				{
					tmultiplicative[actualkey] *= mod.amount;
				}
				else
				{
					tmultiplicative[actualkey] = mod.amount;
				}
			}
			else if(mod.modifyType == ModifyMethodE.Exponent)
			{
				if (exponent.ContainsKey(actualkey))
				{
					exponent[actualkey] *= mod.amount;
				}
				else
				{
					exponent[actualkey] = mod.amount;
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
			else if(modifier.modifyType == ModifyMethodE.Exponent)
            {
				var exps = __instance.Ext().exponentMods;
				if (exps != null)
				{
					int statToBoost = (int)modifier.statToBoost;
					if (exps.Length > statToBoost)
					{
						exps[statToBoost] *= modifier.amount;
					}
				}
			}
        }

		[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> StatMods(IEnumerable<CodeInstruction> instructions)
        {
			foreach (var instruction in instructions)
			{
                if (instruction.LoadsField(allowzerohealth))
                {
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, statsext);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldloca_S, 5);
					yield return new CodeInstruction(OpCodes.Callvirt, extrecalculate);
				}
				yield return instruction;
			}
			yield break;
        }

		public static MethodInfo inventorymaxgunsmethod = typeof(PlayerController).GetMethod(nameof(PlayerController.UpdateInventoryMaxGuns));
		public static MethodInfo statsext = typeof(CodeShortcuts).GetMethod(nameof(CodeShortcuts.Ext), new Type[] { typeof(PlayerStats) });
		public static MethodInfo extrecalculate = typeof(PlayerStatsExt).GetMethod(nameof(PlayerStatsExt.InternalRecalculateStats));
		public static FieldInfo allowzerohealth = typeof(PlayerController).GetField("AllowZeroHealthState");

		public float[] trueMultiplicativeMults;
		public float unprocessedHealthMult;
		public float unprocessedHealthExp;
		public float unprocessedHealthDecrement;
		public float[] exponentMods;
		public List<string> statKeys;
		public List<float> statValues;
		public List<string> baseValueKeys;
		public List<float> baseValueValues;
		public static Dictionary<PlayerStats.StatType, string> newStatsForMods = new();

		public static Dictionary<string, float> overrideBaseStatValues = new()
		{
			{ "LambPoisonChance", 0f },
			{ "LambCritChance", 0f },
			{ "ActiveChargeOnRoomEnter", 0f },
			{ "ActiveChargeOnDamage", 0f },
			{ "ActiveChargePerSecond", 0f },
			{ "FlatDamage", 0f },
			{ "UnscaledFlatDamage", 0f }
		};
	}
}
