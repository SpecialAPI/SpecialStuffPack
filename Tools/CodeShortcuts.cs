using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public static class CodeShortcuts
    {
        public static T AddComponent<T>(this Component self) where T : Component
        {
            return self.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            return self.gameObject.GetOrAddComponent<T>();
        }

        public static T GetItemById<T>(int id) where T : PickupObject
        {
            return (T)PickupObjectDatabase.GetById(id);
        }

        public static Delegate GetEventDelegate(this object self, string eventName)
        {
            Delegate result = null;
            if (self != null)
            {
                FieldInfo t = self.GetType().GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (t != null)
                {
                    object val = t.GetValue(self);
                    if (val != null && val is Delegate)
                    {
                        result = val as Delegate;
                    }
                }
            }
            return result;
        }

        public static T GetEventDelegate<T>(this object self, string eventName) where T : Delegate
        {
            return self.GetEventDelegate(eventName) as T;
        }

        public static void RaiseEvent(this object self, string eventName, params object[] args)
        {
            self.GetEventDelegate<Delegate>(eventName)?.DynamicInvoke(args);
        }

        public static object RaiseEventWithReturn(this object self, string eventName, params object[] args)
        {
            return self.GetEventDelegate<Delegate>(eventName)?.DynamicInvoke(args);
        }

        public static void RaiseEvent0(this object self, string eventName)
        {
            self.GetEventDelegate<Action>(eventName)?.Invoke();
        }

        public static void RaiseEvent1<T>(this object self, string eventName, T arg1)
        {
            self.GetEventDelegate<Action<T>>(eventName)?.Invoke(arg1);
        }

        public static void RaiseEvent2<T1, T2>(this object self, string eventName, T1 arg1, T2 arg2)
        {
            self.GetEventDelegate<Action<T1, T2>>(eventName)?.Invoke(arg1, arg2);
        }

        public static void RaiseEvent3<T1, T2, T3>(this object self, string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            self.GetEventDelegate<Action<T1, T2, T3>>(eventName)?.Invoke(arg1, arg2, arg3);
        }

        public static void RaiseEvent4<T1, T2, T3, T4>(this object self, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            self.GetEventDelegate<Action<T1, T2, T3, T4>>(eventName)?.Invoke(arg1, arg2, arg3, arg4);
        }

        public static Chest GenerationSpawnChestSetLootTable(Chest chest, RoomHandler targetRoom, IntVector2 positionInRoom, Vector2 offset, float overrideMimicChance, GenericLootTable genericLootTable)
        {
            GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(chest.gameObject, targetRoom, positionInRoom, true, AIActor.AwakenAnimationType.Default, false);
            gameObject.transform.position = gameObject.transform.position + offset.ToVector3ZUp(0f);
            Chest component = gameObject.GetComponent<Chest>();
            if (overrideMimicChance >= 0f)
            {
                component.overrideMimicChance = overrideMimicChance;
            }
            Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(IPlaceConfigurable));
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                IPlaceConfigurable placeConfigurable = componentsInChildren[i] as IPlaceConfigurable;
                if (placeConfigurable != null)
                {
                    placeConfigurable.ConfigureOnPlacement(targetRoom);
                }
            }
            if (component.specRigidbody)
            {
                component.specRigidbody.Reinitialize();
            }
            component.lootTable.lootTable = genericLootTable;
            if (component.lootTable.canDropMultipleItems && component.lootTable.overrideItemLootTables != null && component.lootTable.overrideItemLootTables.Count > 0)
            {
                component.lootTable.overrideItemLootTables[0] = genericLootTable;
            }
            targetRoom.RegisterInteractable(component);
            if (GameManager.Instance.RewardManager.SeededRunManifests.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
            {
                System.Random random = (!GameManager.Instance.IsSeeded) ? null : BraveRandom.GeneratorRandom;
                component.GenerationDetermineContents(GameManager.Instance.RewardManager.SeededRunManifests[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId], random);
            }
            return component;
        }

        public static string ToMTGId(this string s)
        {
            return s.ToLower().Replace(" ", "_").Replace("\"", "").Replace("'", "").Replace("-", "");
        }
    }
}
