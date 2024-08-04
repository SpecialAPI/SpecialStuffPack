using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class RequiresHealthToShoot : MonoBehaviour
    {
        [HarmonyPatch(typeof(Gun), nameof(Gun.AdjustedMaxAmmo), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool UseHealth(Gun __instance, ref int __result)
        {
            if(__instance.GetComponent<RequiresHealthToShoot>() != null && !__instance.InfiniteAmmo && __instance.CurrentOwner != null && __instance.CurrentOwner.healthHaver != null && __instance.CurrentOwner is PlayerController)
            {
                __result = Mathf.Max(0, Mathf.RoundToInt(__instance.CurrentOwner.healthHaver.AdjustedMaxHealth * 2) - 1);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.CurrentAmmo), MethodType.Getter)]
        [HarmonyPatch(typeof(Gun), nameof(Gun.ClipShotsRemaining), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool UseHealth2(Gun __instance, ref int __result)
        {
            if(__instance.GetComponent<RequiresHealthToShoot>() != null && __instance.CurrentOwner != null && __instance.CurrentOwner.healthHaver != null && __instance.CurrentOwner is PlayerController)
            {
                __result = Mathf.Clamp(Mathf.RoundToInt(__instance.CurrentOwner.healthHaver.currentHealth * 2) - 1, 0, Mathf.Max(0, Mathf.RoundToInt(__instance.CurrentOwner.healthHaver.AdjustedMaxHealth * 2) - 1));
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.IsGunBlocked))]
        [HarmonyPrefix]
        public static bool GunBlockedHealth(Gun __instance, ref bool __result)
        {
            if (__instance.GetComponent<RequiresHealthToShoot>() != null && __instance.CurrentOwner != null && __instance.CurrentOwner.healthHaver != null && __instance.CurrentOwner is PlayerController && __instance.CurrentOwner.healthHaver.currentHealth <= 0.5f)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.CheckHasLoadedModule), typeof(ProjectileModule))]
        [HarmonyPrefix]
        public static bool LoadedModule1(Gun __instance, ref bool __result, ProjectileModule module)
        {
            if (__instance.GetComponent<RequiresHealthToShoot>() != null && __instance.CurrentOwner != null && __instance.CurrentOwner.healthHaver != null && __instance.CurrentOwner is PlayerController)
            {
                __instance.RuntimeModuleData[module].numberShotsFired = 0;
                __instance.RuntimeModuleData[module].needsReload = false;
                __result = __instance.CurrentOwner.healthHaver.currentHealth > 0.5f;
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(Gun), nameof(Gun.CheckHasLoadedModule), typeof(ProjectileVolleyData))]
        [HarmonyPrefix]
        public static bool LoadedModule2(Gun __instance, ref bool __result, ProjectileVolleyData Volley)
        {
            if (__instance.GetComponent<RequiresHealthToShoot>() != null && __instance.CurrentOwner != null && __instance.CurrentOwner.healthHaver != null && __instance.CurrentOwner is PlayerController)
            {
                for (int i = 0; i < Volley.projectiles.Count; i++)
                {
                    __instance.RuntimeModuleData[Volley.projectiles[i]].numberShotsFired = 0;
                    __instance.RuntimeModuleData[Volley.projectiles[i]].needsReload = false;
                }
                __result = __instance.CurrentOwner.healthHaver.currentHealth > 0.5f;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(AmmoPickup), nameof(AmmoPickup.Interact))]
        [HarmonyPrefix]
        public static bool DontRefillHealthGuns(AmmoPickup __instance, PlayerController interactor)
        {
            if(__instance != null && interactor.CurrentGun != null && interactor.CurrentGun.GetComponent<RequiresHealthToShoot>() != null)
            {
                GameUIRoot.Instance.InformNeedsReload(interactor, new Vector3(interactor.specRigidbody.UnitCenter.x - interactor.transform.position.x, 1.25f, 0f), 1f, "#RELOAD_FULL");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GameUIAmmoController), nameof(GameUIAmmoController.UpdateAmmoUIForModule))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AmmoCount(IEnumerable<CodeInstruction> instructions)
        {
            var firstTryLoadingArg1 = true;
            foreach(var instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Ldarg_1 && firstTryLoadingArg1)
                {
                    firstTryLoadingArg1 = false;
                    yield return new CodeInstruction(OpCodes.Ldarg, 8);
                    yield return new CodeInstruction(OpCodes.Ldarg, 7);
                    yield return new CodeInstruction(OpCodes.Ldloca, 0);
                    yield return new CodeInstruction(OpCodes.Ldloca, 1);
                    yield return new CodeInstruction(OpCodes.Call, modcount);
                }
            }
            yield break;
        }

        public static MethodInfo modcount = AccessTools.Method(typeof(RequiresHealthToShoot), nameof(RequiresHealthToShoot.ModModuleAmmoCounts));

        public static void ModModuleAmmoCounts(Gun gun, ProjectileModule mod, ref int currentModuleAmmo, ref int maxModuleAmmo)
        {
            if (gun.GetComponent<RequiresHealthToShoot>())
            {
                maxModuleAmmo = Mathf.Max(0, Mathf.RoundToInt(gun.CurrentOwner.healthHaver.AdjustedMaxHealth * 2) - 1);
                currentModuleAmmo = gun.CurrentAmmo;
            }
        }
    }
}
