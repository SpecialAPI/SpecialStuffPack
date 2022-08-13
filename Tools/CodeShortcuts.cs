global using static SpecialStuffPack.CodeShortcuts;
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
    public static partial class CodeShortcuts
    {
        public static Gun AsGun(this PickupObject po) => po as Gun;
        public static PlayerItem AsActive(this PickupObject po) => po as PlayerItem;
        public static PassiveItem AsPassive(this PickupObject po) => po as PassiveItem;
        public static T As<T>(this object o)
        {
            if (o is T t)
            {
                return t;
            }
            return default;
        }

        public static StatModifier CreateStatMod(PlayerStats.StatType targetStat, StatModifier.ModifyMethod method, float amt, bool destroyOnHit = false)
        {
            var mod = StatModifier.Create(targetStat, method, amt);
            mod.isMeatBunBuff = destroyOnHit;
            return mod;
        }

        public static T RandomElement<T>(this IEnumerable<T> collection)
        {
            return BraveUtility.RandomElement(collection.ToArray());
        }

        public static void HandleStealth(PlayerController user, string reason)
        {
            user.ChangeSpecialShaderFlag(1, 1f);
            user.SetIsStealthed(true, reason);
            user.SetCapableOfStealing(true, reason, null);
            user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.PlayEffectOnActor(CoolEffects.smokePoofVFX, Vector3.zero, false, true, false);
            void BreakStealthOnStolen(PlayerController x, ShopItemController x2)
            {
                BreakStealth(x, reason, BreakStealthOnUnstealthy, BreakStealthOnStolen);
            }
            void BreakStealthOnUnstealthy(PlayerController x)
            {
                BreakStealth(x, reason, BreakStealthOnUnstealthy, BreakStealthOnStolen);
            }
            user.OnDidUnstealthyAction += BreakStealthOnUnstealthy;
            user.OnItemStolen += BreakStealthOnStolen;
        }

        public static void MakeContinuous(this Gun gun)
        {
            gun.usesContinuousFireAnimation = true;
            var clippy = gun.spriteAnimator?.GetClipByName(gun.shootAnimation);
            if(clippy != null)
            {
                clippy.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            }
        }

        public static void UnmakeContinuous(this Gun gun)
        {
            gun.usesContinuousFireAnimation = false;
            var clippy = gun.spriteAnimator?.GetClipByName(gun.shootAnimation);
            if (clippy != null)
            {
                clippy.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            }
        }

        public static PlayerController PlayerOwner(this Projectile proj)
        {
            return proj?.Owner as PlayerController;
        }

        public static void IncrementFlag<T>(this PlayerController p)
        {
            p.IncrementFlag(typeof(T));
        }

        public static void IncrementFlag(this PlayerController p, Type passiveFlag)
        {
            PassiveItem.IncrementFlag(p, passiveFlag);
        }

        public static void DecrementFlag<T>(this PlayerController p)
        {
            p.DecrementFlag(typeof(T));
        }

        public static void DecrementFlag(this PlayerController p, Type passiveFlag)
        {
            PassiveItem.DecrementFlag(p, passiveFlag);
        }

        public static int GetFlagCount<T>(this PlayerController p)
        {
            return p.GetFlagCount(typeof(T));
        }

        public static int GetFlagCount(this PlayerController p, Type passiveFlag)
        {
            if(!PassiveItem.IsFlagSetForCharacter(p, passiveFlag))
            {
                return 0;
            }
            return PassiveItem.ActiveFlagItems[p][passiveFlag];
        }

        public static int ToInt(this bool b, int onFalse = 0, int onTrue = 1)
        {
            return b ? onTrue : onFalse;
        }

        public static bool IsDefaultOrClone(this ProjectileModule mod, Gun g)
        {
            return mod == g.DefaultModule || mod.CloneSourceIndex == 0;
        }

        public static bool OwnerHasSynergy(this Projectile proj, string name)
        {
            return (proj?.PlayerOwner()?.PlayerHasActiveSynergy(name)).GetValueOrDefault();
        }

        public static Projectile GetProjectile(this Gun g)
        {
            return g.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged ?
                g?.DefaultModule?.chargeProjectiles?.Find(x => x.Projectile != null)?.Projectile ?? g?.DefaultModule?.projectiles.FirstOrDefault() :
                g?.DefaultModule?.projectiles.FirstOrDefault() ?? g?.DefaultModule?.chargeProjectiles?.Find(x => x.Projectile != null)?.Projectile;
        }

        public static Projectile GetProjectile(int id)
        {
            return GetGun(id).GetProjectile();
        }

        public static VFXPool GetMuzzleFlash(int id)
        {
            return GetGunById(id)?.muzzleFlashEffects;
        }

        public static void BreakStealth(PlayerController obj, string reason, Action<PlayerController> unstealthyActionDelegate, Action<PlayerController, ShopItemController> stolenDelegate)
        {
            obj.PlayEffectOnActor(CoolEffects.smokePoofVFX, Vector3.zero, false, true, false);
            obj.OnDidUnstealthyAction -= unstealthyActionDelegate;
            obj.OnItemStolen -= stolenDelegate;
            obj.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            obj.ChangeSpecialShaderFlag(1, 0f);
            obj.SetIsStealthed(false, reason);
            obj.SetCapableOfStealing(false, reason, null);
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", obj.gameObject);
        }

        public static bool PlayerHasSynergyCompletionGun(this AdvancedSynergyEntry self, PlayerController p)
        {
            if (p && p.inventory != null)
            {
                for (int i = 0; i < p.inventory.AllGuns.Count; i++)
                {
                    if (p.inventory.AllGuns[i].GetComponent<SynergyCompletionGun>() != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DecayingStatModifier(this PlayerController player, PlayerStats.StatType stat, float targetValue, float time, bool isMultiplicative = false)
        {
            if (player != null)
            {
                var modifier = StatModifier.Create(stat, isMultiplicative ? StatModifier.ModifyMethod.MULTIPLICATIVE : StatModifier.ModifyMethod.ADDITIVE, targetValue);
                player.ownerlessStatModifiers.Add(modifier);
                player.StartCoroutine(FadeAwayMod(player, modifier, time, isMultiplicative ? 1f : 0f));
            }
        }

        public static int GetClipShotsRemaining(this ProjectileModule module, Gun gun)
        {
            if (gun.RequiresFundsToShoot && gun.m_owner is PlayerController)
            {
                return Mathf.FloorToInt((float)(gun.m_owner as PlayerController).carriedConsumables.Currency / (float)gun.CurrencyCostPerShot);
            }
            int num = gun.ammo;
            if (gun.m_moduleData == null || !gun.m_moduleData.ContainsKey(gun.DefaultModule))
            {
                num = ((gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner) > 0) ? gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner) : gun.ammo);
            }
            else
            {
                num = ((gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner) > 0) ? (gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner) - gun.RuntimeModuleData[gun.DefaultModule].numberShotsFired) : gun.ammo);
            }
            if (num > gun.ammo)
            {
                gun.ClipShotsRemaining = gun.ammo;
            }
            return Mathf.Min(num, gun.ammo);
        }

        public static List<ProjectileModule> GetProjectileModules(this Gun gun)
        {
            if(gun?.Volley?.projectiles != null && (gun.Volley.projectiles.Count > 0 || gun.singleModule == null))
            {
                return gun.Volley.projectiles;
            }
            return new() { gun.singleModule };
        }

        public static IEnumerator FadeAwayMod(PlayerController player, StatModifier mod, float time, float target)
        {
            float ela = 0f;
            float startValue = mod.amount;
            while(ela < time)
            {
                if(mod == null)
                {
                    yield break;
                }
                mod.amount = Mathf.Lerp(startValue, target, ela / time);
                player?.stats?.RecalculateStats(player, false, false);
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            if (mod == null)
            {
                yield break;
            }
            player?.ownerlessStatModifiers.Remove(mod);
            player?.stats?.RecalculateStats(player, false, false);
            yield break;
        }

        public static Projectile OwnedShootProjectile(Projectile proj, Vector2 position, float angle, GameActor owner)
        {
            var obj = SpawnManager.SpawnProjectile(proj.gameObject, position, Quaternion.Euler(0f, 0f, angle), true);
            var bullet = obj.GetComponent<Projectile>();
            if(bullet != null)
            {
                bullet.Owner = owner;
                bullet.Shooter = owner.specRigidbody;
            }
            return bullet;
        }

        public static Vector2 GetRelativeAim(this PlayerController player)
        {
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
            Vector2 a = Vector2.zero;
            if (instanceForPlayer != null)
            {
                if (instanceForPlayer.IsKeyboardAndMouse(false))
                {
                    a = player.unadjustedAimPoint.XY() - player.CenterPosition;
                }
                else
                {
                    bool flag4 = instanceForPlayer.ActiveActions == null;
                    if (flag4)
                    {
                        return a;
                    }
                    a = instanceForPlayer.ActiveActions.Aim.Vector;
                }
            }
            return a;
        }

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
            return PickupObjectDatabase.GetById(id) as T;
        }

        public static T GetItem<T>(int id) where T : PickupObject
        {
            return GetItemById<T>(id);
        }

        public static PickupObject GetItemById(int id)
        {
            return PickupObjectDatabase.GetById(id);
        }

        public static PickupObject GetItem(int id)
        {
            return GetItemById(id);
        }

        public static Gun GetGunById(int id)
        {
            return GetItemById<Gun>(id);
        }

        public static Gun GetGun(int id)
        {
            return GetGunById(id);
        }

        public static T GetPassiveById<T>(int id) where T : PassiveItem
        {
            return GetItemById<T>(id);
        }

        public static T GetPassive<T>(int id) where T : PassiveItem
        {
            return GetPassiveById<T>(id);
        }

        public static PassiveItem GetPassiveById(int id)
        {
            return GetItemById<PassiveItem>(id);
        }

        public static PassiveItem GetPassive(int id)
        {
            return GetPassiveById(id);
        }

        public static T GetActiveById<T>(int id) where T : PlayerItem
        {
            return GetItemById<T>(id);
        }

        public static T GetActive<T>(int id) where T : PlayerItem
        {
            return GetActiveById<T>(id);
        }

        public static PlayerItem GetActiveById(int id)
        {
            return GetItemById<PlayerItem>(id);
        }

        public static PlayerItem GetActive(int id)
        {
            return GetActiveById(id);
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

        public static VFXPool Empty => new() { type = VFXPoolType.None, effects = new VFXComplex[0] };

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

        public static UnityEventHandler Events(this Component comp)
        {
            return comp.gameObject.Events();
        }

        public static UnityEventHandler Events(this GameObject obj)
        {
            return obj.GetOrAddComponent<UnityEventHandler>();
        }

        public static Dictionary<T, T2> ToDictionary<T, T2>(this IEnumerable<KeyValuePair<T, T2>> list)
        {
            var dict = new Dictionary<T, T2>();
            foreach (var kvp in list)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
            return dict;
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
            return s.ToID().Replace("'", "");
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
