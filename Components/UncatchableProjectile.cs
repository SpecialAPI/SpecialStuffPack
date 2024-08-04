using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class UncatchableProjectile : MonoBehaviour
    {
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.CanBeCaught), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool Uncatch(Projectile __instance, ref bool __result)
        {
            if(__instance.GetComponent<UncatchableProjectile>() != null)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
