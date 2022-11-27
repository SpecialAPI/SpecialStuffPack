using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    [HarmonyPatch]
    public static class SpecialAssets
    {
        [HarmonyPatch(typeof(BraveResources), nameof(BraveResources.Load), typeof(string), typeof(string))]
        [HarmonyPrefix]
        public static bool HandleNewAsset(ref Object __result, string path, string extension = ".prefab")
        {
            if(newBraveAssets.TryGetValue((path + extension).ToLowerInvariant(), out __result))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(BraveResources), nameof(BraveResources.Load), typeof(string), typeof(Type), typeof(string))]
        [HarmonyPrefix]
        public static bool HandleNewAsset2(ref Object __result, string path, Type type, string extension = ".prefab")
        {
            var fullPathLowered = (path + extension).ToLowerInvariant();
            if (newBraveAssets.ContainsKey(fullPathLowered))
            {
                var typed = newBraveAssets[fullPathLowered].GetType();
                if (typed.IsSubclassOf(type) || typed == type)
                {
                    __result = newBraveAssets[fullPathLowered];
                    return false;
                }
            }
            return true;
        }

        public static void AddToBraveAssets(string name, Object obj)
        {
            newBraveAssets.Add(name.ToLowerInvariant(), obj);
        }

        private static Dictionary<string, Object> newBraveAssets = new(); 
        public static List<Object> assets = new();
    }
}
