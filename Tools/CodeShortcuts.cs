using Dungeonator;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using SpecialStuffPack.Components;
using SpecialStuffPack.Controls;

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

        public static List<AIActor> GetActiveEnemiesUnreferenced(this RoomHandler room, RoomHandler.ActiveEnemyType type)
        {
            List<AIActor> enemies = new List<AIActor>();
            room.GetActiveEnemies(type, ref enemies);
            return enemies;
        }

        public static List<T> GetComponentsInRoom<T>(this RoomHandler room) where T : Component
        {
            T[] array = UnityEngine.Object.FindObjectsOfType<T>();
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (GameManager.Instance.Dungeon.GetRoomFromPosition(array[i].transform.position.IntXY(VectorConversions.Floor)) == room)
                {
                    list.Add(array[i]);
                }
            }
            return list;
        }

        public static void BecomeTerrifyingDarkRoomNoEnemies(this RoomHandler self, float duration = 1f, float goalIntensity = 0.1f, float lightIntensity = 1f, string wwiseEvent = "Play_ENM_darken_world_01")
        {
            if (self.IsDarkAndTerrifying)
            {
                return;
            }
            self.OnChangedTerrifyingDarkState?.Invoke(true);
            GameManager.Instance.StartCoroutine(self.HandleBecomeTerrifyingDarkRoom(duration, goalIntensity, lightIntensity, false));
            AkSoundEngine.PostEvent(wwiseEvent, GameManager.Instance.PrimaryPlayer.gameObject);
        }

        public static IEnumerator HandleBecomeTerrifyingDarkRoom(this RoomHandler room, float duration, float goalIntensity, float lightIntensity = 1f, bool reverse = false)
        {
            float elapsed = 0f;
            room.IsDarkAndTerrifying = !reverse;
            while (elapsed < duration || duration == 0f)
            {
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                float t = (duration != 0f) ? Mathf.Clamp01(elapsed / duration) : 1f;
                if (reverse)
                {
                    t = 1f - t;
                }
                float num = (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW) ? 1f : 1.25f;
                RenderSettings.ambientIntensity = num;
                float targetAmbient = num;
                RenderSettings.ambientIntensity = Mathf.Lerp(targetAmbient, goalIntensity, t);
                if (!GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms)
                {
                    GameManager.Instance.Dungeon.PlayerIsLight = true;
                    GameManager.Instance.Dungeon.PlayerLightColor = Color.white;
                    GameManager.Instance.Dungeon.PlayerLightIntensity = Mathf.Lerp(0f, lightIntensity * 4.25f, t);
                    GameManager.Instance.Dungeon.PlayerLightRadius = Mathf.Lerp(0f, lightIntensity * 7.25f, t);
                }
                Pixelator.Instance.pointLightMultiplier = Mathf.Lerp(1f, 0f, t);
                if (duration == 0f)
                {
                    break;
                }
                yield return null;
            }
            if (!GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms && reverse)
            {
                GameManager.Instance.Dungeon.PlayerIsLight = false;
            }
            yield break;
        }

        public static void UnsealOneWayDoors(this RoomHandler self)
        {
            for (int i = 0; i < self.connectedDoors.Count; i++)
            {
                if (self.connectedDoors[i].IsSealed || (self.connectedDoors[i].subsidiaryBlocker != null && self.connectedDoors[i].subsidiaryBlocker.isSealed) || (self.connectedDoors[i].subsidiaryDoor != null && self.connectedDoors[i].subsidiaryDoor.IsSealed))
                {
                    if (self.connectedDoors[i].OneWayDoor)
                    {
                        if (self.npcSealState == RoomHandler.NPCSealState.SealNone)
                        {
                            self.connectedDoors[i].DoUnseal(self);
                        }
                        else if (self.npcSealState == RoomHandler.NPCSealState.SealPrior)
                        {
                            RoomHandler roomHandler = (self.connectedDoors[i].upstreamRoom != self) ? self.connectedDoors[i].upstreamRoom : self.connectedDoors[i].downstreamRoom;
                            if (roomHandler.distanceFromEntrance >= self.distanceFromEntrance)
                            {
                                self.connectedDoors[i].DoUnseal(self);
                            }
                        }
                        else if (self.npcSealState == RoomHandler.NPCSealState.SealNext)
                        {
                            RoomHandler roomHandler2 = (self.connectedDoors[i].upstreamRoom != self) ? self.connectedDoors[i].upstreamRoom : self.connectedDoors[i].downstreamRoom;
                            if (roomHandler2.distanceFromEntrance < self.distanceFromEntrance)
                            {
                                self.connectedDoors[i].DoUnseal(self);
                            }
                        }
                    }
                }
            }
        }

        public static SpecialPlayerController SpecialPlayer(this PlayerController player)
        {
            if(player == null)
            {
                return null;
            }
            return player.GetOrAddComponent<SpecialPlayerController>();
        }

        public static SpecialInput SpecialInput(this BraveInput input)
        {
            if(input == null)
            {
                return null;
            }
            return input.GetOrAddComponent<SpecialInput>();
        }

        public static GenericFieldInfo<T> GetField<T>(this Type type, string name, BindingFlags flags)
        {
            return new GenericFieldInfo<T>(type.GetField(name, flags));
        }
    }
}
