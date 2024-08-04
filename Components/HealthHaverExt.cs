using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class HealthHaverExt : MonoBehaviour
    {
		[HarmonyPatch(typeof(HealthHaver), nameof(HealthHaver.ApplyDamageDirectional))]
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> StatMods(IEnumerable<CodeInstruction> instructions)
		{
			var firstTimeGettingModifyDamage = true;
			var firstTimeGettingPlayer = true;
			var firstTimeGettingOnDamage = true;
			var firstTimeGettingHealthChanged = true;
			foreach (var instruction in instructions)
			{
                if (firstTimeGettingModifyDamage && instruction.LoadsField(modifydamage))
                {
					firstTimeGettingModifyDamage = false;
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, hhext);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarga, 1);
					yield return new CodeInstruction(OpCodes.Callvirt, extmodify);
					//yield return new CodeInstruction(OpCodes.Starg_S, "damage");
					yield return new CodeInstruction(OpCodes.Ldarg_0);
				}
				else if(!firstTimeGettingModifyDamage && firstTimeGettingPlayer && instruction.LoadsField(hhplayer))
				{
					firstTimeGettingPlayer = false;
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, hhext);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarga, 1);
					yield return new CodeInstruction(OpCodes.Callvirt, extmodify2);
					//yield return new CodeInstruction(OpCodes.Starg_S, "damage");
					yield return new CodeInstruction(OpCodes.Ldarg_0);
				}
				if (firstTimeGettingOnDamage && instruction.LoadsField(ondmg))
				{
					firstTimeGettingOnDamage = false;
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, hhext);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarg, 1);
					yield return new CodeInstruction(OpCodes.Callvirt, extdmg);
					//yield return new CodeInstruction(OpCodes.Starg_S, "damage");
					yield return new CodeInstruction(OpCodes.Ldarg_0);
				}
				if (firstTimeGettingHealthChanged && instruction.LoadsField(hchanged))
				{
					firstTimeGettingHealthChanged = false;
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, hhext);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarg, 1);
					yield return new CodeInstruction(OpCodes.Callvirt, extdmg2);
					//yield return new CodeInstruction(OpCodes.Starg_S, "damage");
					yield return new CodeInstruction(OpCodes.Ldarg_0);
				}
				yield return instruction;
				if (instruction.opcode == OpCodes.Stfld && instruction.operand == initdmg)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_1);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, hhext);
					yield return new CodeInstruction(OpCodes.Ldfld, initdmgext);
					yield return new CodeInstruction(OpCodes.Stfld, initdmg);
				}
			}
			yield break;
		}

		[HarmonyPatch(typeof(HealthHaver), nameof(HealthHaver.ApplyDamageDirectional))]
		[HarmonyPrefix]
		public static void SetValues(HealthHaver __instance, Vector2 direction, string damageSource, CoreDamageTypes damageTypes, DamageCategory damageCategory, bool ignoreInvulnerabilityFrames, bool ignoreDamageCaps)
		{
			var ext = __instance.Ext();
			ext.dir = direction;
			ext.source = damageSource;
			ext.t = damageTypes;
			ext.c = damageCategory;
			ext.ignoreInv = ignoreInvulnerabilityFrames;
			ext.ignoreDPS = ignoreDamageCaps;
		}

		public void ModifyDamageAdvanced(HealthHaver hh, ref float dmg)
        {
			initialDmg = dmg;
            var args = new HealthHaver.ModifyDamageEventArgs
            {
                InitialDamage = dmg,
                ModifiedDamage = dmg
            };
            ModifyDamageContext?.Invoke(hh, args, dir, source, t, c, ignoreInv, ignoreDPS);
			dmg = args.ModifiedDamage;
		}

		public void ModifyDamageAdvancedLater(HealthHaver hh, ref float dmg)
		{
			initialDmg = dmg;
			var args = new HealthHaver.ModifyDamageEventArgs
			{
				InitialDamage = initialDmg,
				ModifiedDamage = dmg
			};
			ModifyDamageContextLater?.Invoke(hh, args, dir, source, t, c, ignoreInv, ignoreDPS);
			dmg = args.ModifiedDamage;
		}

		public void OnDamagedAdvanced(HealthHaver hh, float dmg)
		{
			OnDamagedContext?.Invoke(hh, dmg, source, hh.currentHealth, hh.AdjustedMaxHealth, t, c, dir, ignoreInv, ignoreDPS);
		}

		public void ChessBattleAdvanced(HealthHaver hh, float dmg)
		{
			OnDamagedContextLater?.Invoke(hh, dmg, source, hh.currentHealth, hh.AdjustedMaxHealth, t, c, dir, ignoreInv, ignoreDPS);
		}

		public static FieldInfo modifydamage = AccessTools.Field(typeof(HealthHaver), nameof(HealthHaver.ModifyDamage));
		public static FieldInfo hhplayer = AccessTools.Field(typeof(HealthHaver), nameof(HealthHaver.m_player));
		public static FieldInfo initdmg = AccessTools.Field(typeof(HealthHaver.ModifyDamageEventArgs), nameof(HealthHaver.ModifyDamageEventArgs.InitialDamage));
		public static FieldInfo ondmg = AccessTools.Field(typeof(HealthHaver), nameof(HealthHaver.OnDamaged));
		public static FieldInfo hchanged = AccessTools.Field(typeof(HealthHaver), nameof(HealthHaver.OnHealthChanged));
		public static FieldInfo initdmgext = AccessTools.Field(typeof(HealthHaverExt), nameof(initialDmg));
		public static MethodInfo extmodify = AccessTools.Method(typeof(HealthHaverExt), nameof(HealthHaverExt.ModifyDamageAdvanced));
		public static MethodInfo extmodify2 = AccessTools.Method(typeof(HealthHaverExt), nameof(HealthHaverExt.ModifyDamageAdvancedLater));
		public static MethodInfo extdmg = AccessTools.Method(typeof(HealthHaverExt), nameof(HealthHaverExt.OnDamagedAdvanced));
		public static MethodInfo extdmg2 = AccessTools.Method(typeof(HealthHaverExt), nameof(HealthHaverExt.ChessBattleAdvanced));
		public static MethodInfo hhext = AccessTools.Method(typeof(CodeShortcuts), nameof(CodeShortcuts.Ext), new Type[] { typeof(HealthHaver) });

		public Vector2 dir;
		public string source;
		public CoreDamageTypes t;
		public DamageCategory c;
		public bool ignoreInv;
		public bool ignoreDPS;
		public float initialDmg;
		public ModifyDamageContextDelegate ModifyDamageContext;
		public ModifyDamageContextDelegate ModifyDamageContextLater;
		public OnDamagedContextDelegate OnDamagedContext;
		public OnDamagedContextDelegate OnDamagedContextLater;

		public delegate void ModifyDamageContextDelegate(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args, Vector2 direction, string source, CoreDamageTypes type, DamageCategory category, bool ignoreInvulnerability, bool ignoreDPSCaps);
		public delegate void OnDamagedContextDelegate(HealthHaver hh, float damage, string source, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection, bool ignoreInvulnerabilityFrames, bool ignoreDamageCaps);
    }
}
