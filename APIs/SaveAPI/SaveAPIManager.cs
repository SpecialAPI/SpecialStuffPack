using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.IO;
using Dungeonator;
using System.Reflection;
using System.Collections;

namespace SpecialStuffPack.SaveAPI
{
    /// <summary>
    /// The core class in SaveAPI
    /// </summary>
    public static class SaveAPIManager
    {
        /// <summary>
        /// Call this method in your <see cref="ETGModule.Init"/> method. Adds SaveAPI <see cref="Hook"/>s, loads <see cref="AdvancedGameStatsManager"/> and setups the custom <see cref="SaveManager.SaveType"/>s
        /// </summary>
        /// <param name="prefix">Mod prefix for SaveTypes</param>
        public static void Setup()
        {
            if (m_loaded)
            {
                return;
            }
            prerequisiteHook = new Hook(
                typeof(DungeonPrerequisite).GetMethod("CheckConditionsFulfilled", BindingFlags.Public | BindingFlags.Instance),
                typeof(SaveAPIManager).GetMethod("PrerequisiteHook")
            );
            m_loaded = true;
        }

        /// <summary>
        /// Disposes SaveAPI <see cref="Hook"/>s, unloads <see cref="AdvancedGameStatsManager"/> and nulls custom <see cref="SaveManager.SaveType"/>s
        /// </summary>
        public static void Unload()
        {
            if (!m_loaded)
            {
                return;
            }
            prerequisiteHook?.Dispose();
            m_loaded = false;
        }

        public static bool PrerequisiteHook(Func<DungeonPrerequisite, bool> orig, DungeonPrerequisite self)
        {
            if (self is CustomDungeonPrerequisite)
            {
                return (self as CustomDungeonPrerequisite).CheckConditionsFulfilled();
            }
            return orig(self);
        }

        private static Hook prerequisiteHook;
        private static bool m_loaded;
    }
}
