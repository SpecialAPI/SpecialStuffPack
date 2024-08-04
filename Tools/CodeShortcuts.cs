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
using System.IO;
using SpecialStuffPack.CursorAPI;

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

        public static StatModifier CreateCustomStatModifier(string statname, float amount, ModifyMethod modifyMethod)
        {
            var d = ETGModCompatibility.ExtendEnum<PlayerStats.StatType>(SpecialStuffModule.GUID, statname);
            if (!PlayerStatsExt.newStatsForMods.ContainsKey(d))
            {
                PlayerStatsExt.newStatsForMods.Add(d, statname);
            }
            return StatModifier.Create(d, modifyMethod, amount);
        }

        public static string EraseUserName(string original)
        {
            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            userFolder = userFolder.Substring(0, userFolder.LastIndexOf(Path.DirectorySeparatorChar));
            userFolder = userFolder.Substring(0, userFolder.LastIndexOf(Path.DirectorySeparatorChar));
            return original.Replace(userFolder, "C:" + Path.DirectorySeparatorChar + Path.Combine("Users", "USER")).Replace(userFolder.Replace(Path.DirectorySeparatorChar, '/'), "C:/Users/USER");
        }

        public static StatModifier CreateStatMod(PlayerStats.StatType targetStat, StatModifier.ModifyMethod method, float amt, bool destroyOnHit = false)
        {
            var mod = StatModifier.Create(targetStat, method, amt);
            mod.isMeatBunBuff = destroyOnHit;
            return mod;
        }

        public static EncounterDatabaseEntry DatabaseEntry(this EncounterTrackable self)
        {
            return EncounterDatabase.GetEntry(self.EncounterGuid);
        }

        public static ExplosionData Copy(this ExplosionData original)
        {
            ExplosionData data = new();
            data.CopyFrom(original);
            return data;
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

        public static string AddToAtlas(string textureName, string overrideName = null)
        {
            return AddToAtlas(AssetBundleManager.Load<Texture2D>(textureName), out _, overrideName);
        }

        public static string AddToAtlas(string textureName, out dfAtlas.ItemInfo info, string overrideName = null)
        {
            return AddToAtlas(AssetBundleManager.Load<Texture2D>(textureName), out info, overrideName);
        }

        public static string AddToAtlas(Texture2D tex, string overrideName = null)
        {
            return AddToAtlas(tex, out _, overrideName);
        }

        public static string AddToAtlas(Texture2D tex, out dfAtlas.ItemInfo info, string overrideName = null)
        {
            var name = overrideName ?? $"spapi_{tex.name}";
            info = CursorMaker.UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(tex, name);
            return name;
        }

        public static void LogFrames(this tk2dSpriteAnimationClip clippy)
        {
            if(clippy.frames.Length <= 0)
            {
                ETGModConsole.Log($"The clip {clippy.name} (fps = {clippy.fps}, wrap mode = {clippy.wrapMode}) doesn't have any frames");
            }
            else
            {
                ETGModConsole.Log($"Animation frames of clip {clippy.name} (fps = {clippy.fps}, wrap mode = {clippy.wrapMode}):");
            }
            int i = 1;
            foreach(var frame in clippy.frames)
            {
                var stuffInBrackets = new List<string>();
                if (!string.IsNullOrEmpty(frame.eventAudio))
                {
                    stuffInBrackets.Add("Audio: " + frame.eventAudio);
                }
                if (!string.IsNullOrEmpty(frame.eventInfo))
                {
                    stuffInBrackets.Add("Info: " + frame.eventInfo);
                }
                var message = $"{i}: {frame.spriteCollection.spriteDefinitions[frame.spriteId].name}";
                if(stuffInBrackets.Count > 0)
                {
                    message += $" ({string.Join(", ", stuffInBrackets.ToArray())})";
                }
                ETGModConsole.Log(message);
                i++;
            }
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

        public static int GetFlagCountAllPlayers<T>()
        {
            return GetFlagCountAllPlayers(typeof(T));
        }

        public static void TriggerStuffedStarInvulnerability(this PlayerController player, float duration)
        {
            player.StartCoroutine(HandleStuffedStarInvulnerability(player, duration));
        }

        public static IEnumerator HandleStuffedStarInvulnerability(PlayerController user, float dura)
        {
            AkSoundEngine.PostEvent("Play_OBJ_powerstar_use_01", user.gameObject);
            Material[] array = user.SetOverrideShader(ShaderCache.Acquire("Brave/Internal/RainbowChestShader"));
            for (int i = 0; i < array.Length; i++)
            {
                if (!(array[i] == null))
                {
                    array[i].SetFloat("_AllColorsToggle", 1f);
                }
            }
            float ela = 0f;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                user.healthHaver.IsVulnerable = false;
                yield return null;
            }
            user.ClearOverrideShader();
            user.healthHaver.IsVulnerable = true;
            AkSoundEngine.PostEvent("Stop_SND_OBJ", user.gameObject);
            yield break;
        }

        public static int GetFlagCountAllPlayers(Type flagType)
        {
            if(!GameManager.HasInstance || GameManager.Instance.AllPlayers == null)
            {
                return 0;
            }
            int c = 0;
            foreach(var play in GameManager.Instance.AllPlayers)
            {
                if(play == null)
                {
                    continue;
                }
                if (PassiveItem.IsFlagSetForCharacter(play, flagType))
                {
                    c += play.GetFlagCount(flagType);
                }
            }
            return c;
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

        public static string SetString(string key, string value)
        {
            ETGMod.Databases.Strings.Core.Set(key, value);
            return key;
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

        public static float Scale(this float self, float scale)
        {
            if(self < 1)
            {
                return self * scale;
            }
            return self;
        }

        public static bool ScaledRandom(this float self, float scale)
        {
            if(self <= 0f)
            {
                return false;
            }
            if(self >= 1f)
            {
                return true;
            }
            return Random.value < self.Scale(scale);
        }

        public static bool RandomChance(this float self)
        {
            if (self <= 0f)
            {
                return false;
            }
            if (self >= 1f)
            {
                return true;
            }
            return Random.value < self;
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

        public static tk2dSpriteCollectionData EasyCollectionSetup(string name)
        {
            var go = AssetBundleManager.Load<GameObject>(name, null, null);
            if(go == null)
            {
                return null;
            }
            var coll = go.GetComponent<tk2dSpriteCollectionData>();
            if(coll != null)
            {
                return coll;
            }
            coll = go.AddComponent<tk2dSpriteCollectionData>();
            coll.spriteDefinitions = new tk2dSpriteDefinition[0];
            coll.spriteDefinitions = new tk2dSpriteDefinition[0];
            coll.spriteCollectionName = coll.assetName = go.name;
            return coll;
        }

        public static tk2dSpriteAnimation EasyAnimationSetup(string name)
        {
            var go = AssetBundleManager.Load<GameObject>(name, null, null);
            if (go == null)
            {
                return null;
            }
            var anim = go.GetComponent<tk2dSpriteAnimation>();
            if (anim != null)
            {
                return anim;
            }
            anim = go.AddComponent<tk2dSpriteAnimation>();
            anim.clips = new tk2dSpriteAnimationClip[0];
            return anim;
        }

        public static PlayerStatsExt Ext(this PlayerStats pstats)
        {
            if(pstats == null)
            {
                return null;
            }
            return pstats.GetOrAddComponent<PlayerStatsExt>();
        }

        public static HealthHaverExt Ext(this HealthHaver hh)
        {
            if (hh == null)
            {
                return null;
            }
            return hh.GetOrAddComponent<HealthHaverExt>();
        }

        public static ProjectileExt Ext(this Projectile proj)
        {
            if (proj == null)
            {
                return null;
            }
            return proj.GetOrAddComponent<ProjectileExt>();
        }

        public static tk2dSpriteAnimationClip AddClipWithExistingFrames(this tk2dSpriteAnimation library, string animationName, tk2dSpriteCollectionData collection, float fps = 15f,
            tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Once, string overrideClipName = null, string[] optionalAnimationNames = null)
        {
            var clip = library.GetClipByName(animationName);
            if (clip != null)
            {
                return clip;
            }
            List<tk2dSpriteAnimationFrame> frames = new();
            for (int i = 1; i < 1000; i++)
            {
                var tostring = i.ToString();
                string text;
                if (tostring.Length < 2)
                {
                    text = $"{animationName}_00{tostring}";
                }
                else if (tostring.Length < 3)
                {
                    text = $"{animationName}_0{tostring}";
                }
                else
                {
                    text = $"{animationName}_{tostring}";
                }
                var id = collection.GetSpriteIdByName(text, -1);
                if (id < 0)
                {
                    if (optionalAnimationNames != null)
                    {
                        foreach (var name in optionalAnimationNames)
                        {
                            if (tostring.Length < 2)
                            {
                                text = $"{name}_00{tostring}";
                            }
                            else if (tostring.Length < 3)
                            {
                                text = $"{name}_0{tostring}";
                            }
                            else
                            {
                                text = $"{name}_{tostring}";
                            }
                            id = collection.GetSpriteIdByName(text, -1);
                            if (id >= 0)
                            {
                                break;
                            }
                        }
                        if (id < 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (id >= 0)
                {
                    tk2dSpriteAnimationFrame frame = new()
                    {
                        spriteCollection = collection,
                        spriteId = id
                    };
                    frames.Add(frame);
                }
            }
            if (frames.Count <= 0)
            {
                return null;
            }
            clip = new()
            {
                fps = fps,
                wrapMode = wrapMode,
                loopStart = 0,
                name = overrideClipName ?? animationName,
                maxFidgetDuration = 0f,
                minFidgetDuration = 0f,
                frames = frames.ToArray()
            };
            library.clips = library.clips.AddToArray(clip);
            return clip;
        }

        public static tk2dSpriteAnimationClip AddClip(this tk2dSpriteAnimation library, string animationName, tk2dSpriteCollectionData collection, float fps = 15f, 
            tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Once, string overrideClipName = null, string[] optionalAnimationNames = null, string overrideShaderName = null)
        {
            var clip = library.GetClipByName(animationName);
            if (clip != null)
            {
                return clip;
            }
            List<tk2dSpriteAnimationFrame> frames = new();
            for(int i = 1; i < 1000; i++)
            {
                var tostring = i.ToString();
                string text;
                if(tostring.Length < 2)
                {
                    text = $"{animationName}_00{tostring}";
                }
                else if(tostring.Length < 3)
                {
                    text = $"{animationName}_0{tostring}";
                }
                else
                {
                    text = $"{animationName}_{tostring}";
                }
                var tex = AssetBundleManager.Load<Texture2D>(text);
                if(tex == null)
                {
                    if(optionalAnimationNames != null)
                    {
                        foreach(var name in optionalAnimationNames)
                        {
                            if (tostring.Length < 2)
                            {
                                text = $"{name}_00{tostring}";
                            }
                            else if (tostring.Length < 3)
                            {
                                text = $"{name}_0{tostring}";
                            }
                            else
                            {
                                text = $"{name}_{tostring}";
                            }
                            tex = AssetBundleManager.Load<Texture2D>(text);
                            if (tex != null)
                            {
                                break;
                            }
                        }
                        if(tex == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if(tex != null)
                {
                    tk2dSpriteAnimationFrame frame = new()
                    {
                        spriteCollection = collection,
                        spriteId = SpriteBuilder.AddSpriteToCollection(tex, collection, overrideShaderName ?? "tk2d/CutoutVertexColorTintableTilted", true, false)
                    };
                    var animationMetadata = AssetBundleManager.Load<TextAsset>(text);
                    if (animationMetadata != null)
                    {
                        var lines = animationMetadata.text.Split('\n');
                        foreach (var line in lines)
                        {
                            if (!string.IsNullOrEmpty(line) && line.Length > 2 && line.Contains(":"))
                            {
                                var key = line.Substring(0, line.IndexOf(":")).Trim().ToLowerInvariant();
                                var value = line.Substring(line.IndexOf(":") + 1);
                                if (key == "audio")
                                {
                                    frame.eventAudio = value.Trim();
                                    frame.triggerEvent = true;
                                }
                                else if (key == "info")
                                {
                                    frame.eventInfo = value.TrimStart();
                                    frame.triggerEvent = true;
                                }
                                else if (key == "vfx")
                                {
                                    frame.eventVfx = value;
                                }
                                else if (key == "stopvfx")
                                {
                                    frame.eventStopVfx = value;
                                }
                            }
                        }
                    }
                    frames.Add(frame);
                }
            }
            if(frames.Count <= 0)
            {
                return null;
            }
            clip = new()
            {
                fps = fps,
                wrapMode = wrapMode,
                loopStart = 0,
                name = overrideClipName ?? animationName,
                maxFidgetDuration = 0f,
                minFidgetDuration = 0f,
                frames = frames.ToArray()
            };
            library.clips = library.clips.AddToArray(clip);
            return clip;
        }

        public static LilChest GenerationSpawnLilChestAt(this RewardManager man, IntVector2 positionInRoom, RoomHandler targetRoom, PickupObject.ItemQuality? targetQuality = null)
        {
            System.Random random = (!GameManager.Instance.IsSeeded) ? null : BraveRandom.GeneratorRandom;
            FloorRewardData rewardDataForFloor = man.GetRewardDataForFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
            bool forceDChanceZero = StaticReferenceManager.DChestsSpawnedInTotal >= 2;
            if (targetQuality == null)
            {
                targetQuality = new PickupObject.ItemQuality?(rewardDataForFloor.GetRandomTargetQuality(true, forceDChanceZero));
                if (PassiveItem.IsFlagSetAtAll(typeof(SevenLeafCloverItem)))
                {
                    targetQuality = new PickupObject.ItemQuality?((((random == null) ? UnityEngine.Random.value : ((float)random.NextDouble())) >= 0.5f) ? PickupObject.ItemQuality.S : PickupObject.ItemQuality.A);
                }
            }
            if (targetQuality == PickupObject.ItemQuality.D && StaticReferenceManager.DChestsSpawnedOnFloor >= 1 && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                targetQuality = new PickupObject.ItemQuality?(PickupObject.ItemQuality.C);
            }
            Vector2 zero = Vector2.zero;
            if (targetQuality == PickupObject.ItemQuality.A || targetQuality == PickupObject.ItemQuality.S)
            {
                zero = new Vector2(-0.5f, 0f);
            }
            Chest chest = LilChest.GetLilChestForQuality(targetQuality.GetValueOrDefault());
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.SYNERGRACE_UNLOCKED) && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                float num = (random == null) ? UnityEngine.Random.value : ((float)random.NextDouble());
                if (num < man.GlobalSynerchestChance)
                {
                    chest = LilChest.LilSynergy;
                    zero = new Vector2(-0.1875f, 0f);
                }
            }
            Chest.GeneralChestType generalChestType = (BraveRandom.GenerationRandomValue() >= rewardDataForFloor.GunVersusItemPercentChance) ? Chest.GeneralChestType.ITEM : Chest.GeneralChestType.WEAPON;
            if (StaticReferenceManager.ItemChestsSpawnedOnFloor > 0 && StaticReferenceManager.WeaponChestsSpawnedOnFloor == 0)
            {
                generalChestType = Chest.GeneralChestType.WEAPON;
            }
            else if (StaticReferenceManager.WeaponChestsSpawnedOnFloor > 0 && StaticReferenceManager.ItemChestsSpawnedOnFloor == 0)
            {
                generalChestType = Chest.GeneralChestType.ITEM;
            }
            GenericLootTable genericLootTable = (generalChestType != Chest.GeneralChestType.WEAPON) ? man.ItemsLootTable : man.GunsLootTable;
            GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(chest.gameObject, targetRoom, positionInRoom, true, AIActor.AwakenAnimationType.Default, false);
            gameObject.transform.position = gameObject.transform.position + zero.ToVector3ZUp(0f);
            Chest component = gameObject.GetComponent<Chest>();
            Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(IPlaceConfigurable));
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                IPlaceConfigurable placeConfigurable = componentsInChildren[i] as IPlaceConfigurable;
                if (placeConfigurable != null)
                {
                    placeConfigurable.ConfigureOnPlacement(targetRoom);
                }
            }
            if (targetQuality == PickupObject.ItemQuality.A)
            {
                GameManager.Instance.Dungeon.GeneratedMagnificence += 1f;
                component.GeneratedMagnificence += 1f;
            }
            else if (targetQuality == PickupObject.ItemQuality.S)
            {
                GameManager.Instance.Dungeon.GeneratedMagnificence += 1f;
                component.GeneratedMagnificence += 1f;
            }
            component.ChestType = generalChestType;
            component.lootTable.lootTable = genericLootTable;
            if (component.lootTable.canDropMultipleItems && component.lootTable.overrideItemLootTables != null && component.lootTable.overrideItemLootTables.Count > 0)
            {
                component.lootTable.overrideItemLootTables[0] = genericLootTable;
            }
            if (targetQuality == PickupObject.ItemQuality.D && !component.IsMimic)
            {
                StaticReferenceManager.DChestsSpawnedOnFloor++;
                StaticReferenceManager.DChestsSpawnedInTotal++;
                component.IsLocked = true;
                if (component.LockAnimator)
                {
                    component.LockAnimator.renderer.enabled = true;
                }
            }
            if (man.SeededRunManifests.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId))
            {
                component.GenerationDetermineContents(man.SeededRunManifests[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId], random);
            }
            return component as LilChest;
        }

        public static void DoCustomTextPopup(Vector3 worldPosition, float heightOffGround, string whatDoesItSay, Color whatColorIsIt)
        {
            var self = GameUIRoot.Instance;
            if(self == null)
            {
                return;
            }
            if (self.m_inactiveDamageLabels.Count == 0)
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DamagePopupLabel", ".prefab"), self.transform);
                self.m_inactiveDamageLabels.Add(gameObject.GetComponent<dfLabel>());
            }
            dfLabel dfLabel = self.m_inactiveDamageLabels[0];
            self.m_inactiveDamageLabels.RemoveAt(0);
            dfLabel.gameObject.SetActive(true);
            dfLabel.Text = whatDoesItSay;
            dfLabel.Color = whatColorIsIt;
            dfLabel.Opacity = 1f;
            dfLabel.transform.position = dfFollowObject.ConvertWorldSpaces(worldPosition, GameManager.Instance.MainCameraController.Camera, self.m_manager.RenderCamera).WithZ(0f);
            dfLabel.transform.position = dfLabel.transform.position.QuantizeFloor(dfLabel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
            dfLabel.StartCoroutine(self.HandleDamageNumberCR(worldPosition, worldPosition.y - heightOffGround, dfLabel));
        }

        public static void DoCustomRisingTextPopup(Vector3 worldPosition, string whatDoesItSay, Color whatColorIsIt, float vel, float duration)
        {
            var self = GameUIRoot.Instance;
            if (self == null)
            {
                return;
            }
            if (self.m_inactiveDamageLabels.Count == 0)
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DamagePopupLabel", ".prefab"), self.transform);
                self.m_inactiveDamageLabels.Add(gameObject.GetComponent<dfLabel>());
            }
            dfLabel dfLabel = self.m_inactiveDamageLabels[0];
            self.m_inactiveDamageLabels.RemoveAt(0);
            dfLabel.gameObject.SetActive(true);
            dfLabel.Text = whatDoesItSay;
            dfLabel.Color = whatColorIsIt;
            dfLabel.Opacity = 1f;
            dfLabel.transform.position = dfFollowObject.ConvertWorldSpaces(worldPosition, GameManager.Instance.MainCameraController.Camera, self.m_manager.RenderCamera).WithZ(0f);
            dfLabel.transform.position = dfLabel.transform.position.QuantizeFloor(dfLabel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
            dfLabel.StartCoroutine(HandleRisingPopup(dfLabel, worldPosition, vel, duration));
        }

        public static string FormatStatMessage(this StatModifier mod, out Color c)
        {
            if (mod.IsPositiveForPlayer())
            {
                c = Color.green;
            }
            else
            {
                c = Color.red;
            }
            if (!StatNameDict.ContainsKey(mod.statToBoost))
            {
                return "Error: unknown stat?";
            }
            var statname = StatNameDict[mod.statToBoost];
            if (mod.modifyType == StatModifier.ModifyMethod.ADDITIVE)
            {
                if (WeirdPercentageStats.Contains(mod.statToBoost))
                {
                    return $"{(mod.amount >= 0f ? "+" : "")}{mod.amount.Quantize(0.01f)}% {statname}";
                }
                else if (PercentageStats.Contains(mod.statToBoost))
                {
                    return $"{(mod.amount >= 0f ? "+" : "")}{(mod.amount * 100f).Quantize(0.01f)}% {statname}";
                }
                return $"{(mod.amount >= 0f ? "+" : "")}{(mod.amount).Quantize(0.01f)} {statname}";
            }
            else if(mod.modifyType == StatModifier.ModifyMethod.MULTIPLICATIVE || mod.modifyType == ModifyMethodE.TrueMultiplicative)
            {
                return $"{mod.amount.Quantize(0.01f)}x {statname}";
            }
            else if(mod.modifyType == ModifyMethodE.Exponent)
            {
                return $"{statname}^{mod.amount.Quantize(0.01f)}";
            }
            return "Error: unknown modify method?";
        }

        public static void DoEpicAnnouncementChain(Vector3 worldPosition, List<string> messages, List<Color> messageColors = null, float messageVel = 0.5f, float messageDur = 4f, float delayBetweenMessages = 2f)
        {
            if(messages == null || messages.Count <= 0)
            {
                return;
            }
            GameManager.Instance.Dungeon.StartCoroutine(EpicAnnouncementChainCR(worldPosition, messages, messageColors, messageVel, messageDur, delayBetweenMessages));
        }

        public static IEnumerator EpicAnnouncementChainCR(Vector3 worldPosition, List<string> messages, List<Color> messageColors = null, float messageVel = 0.5f, float messageDur = 4f, float delayBetweenMessages = 2f)
        {
            for(int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                if (!string.IsNullOrEmpty(msg))
                {
                    DoCustomRisingTextPopup(worldPosition, msg, messageColors != null && i < messageColors.Count ? messageColors[i] : Color.white, messageVel, messageDur);
                }
                yield return new WaitForSeconds(delayBetweenMessages);
            }
            yield break;
        }

        public static void ApplyHealingUnmodified(this HealthHaver hh, float healing)
        {
            if (!hh.isPlayerCharacter || !hh.m_player.IsGhost)
            {
                hh.currentHealth += healing;
                if (hh.quantizeHealth)
                {
                    hh.currentHealth = BraveMathCollege.QuantizeFloat(hh.currentHealth, hh.quantizedIncrement);
                }
                if (hh.currentHealth > hh.AdjustedMaxHealth)
                {
                    hh.currentHealth = hh.AdjustedMaxHealth;
                }
                hh.RaiseEvent("OnHealthChanged", hh.currentHealth, hh.AdjustedMaxHealth);
            }
        }

        public static bool IncreasesStat(this StatModifier mod)
        {
            if (mod.modifyType == StatModifier.ModifyMethod.MULTIPLICATIVE || mod.modifyType == ModifyMethodE.TrueMultiplicative || mod.modifyType == ModifyMethodE.Exponent)
            {
                return mod.amount > 1f;
            }
            else if (mod.modifyType == StatModifier.ModifyMethod.ADDITIVE)
            {
                return mod.amount > 0f;
            }
            return false;
        }

        public static bool IsPositiveForPlayer(this StatModifier mod)
        {
            return mod.IncreasesStat() == !ReverseBadStats.Contains(mod.statToBoost);
        }

        public static readonly List<PlayerStats.StatType> ReverseBadStats = new()
        {
            PlayerStats.StatType.Accuracy,
            PlayerStats.StatType.ReloadSpeed,
            PlayerStats.StatType.GlobalPriceMultiplier,
            PlayerStats.StatType.EnemyProjectileSpeedMultiplier,
            PlayerStats.StatType.Curse
        };

        public static readonly List<PlayerStats.StatType> PercentageStats = new()
        {
            PlayerStats.StatType.RateOfFire,
            PlayerStats.StatType.Accuracy,
            PlayerStats.StatType.Damage,
            PlayerStats.StatType.ProjectileSpeed,
            PlayerStats.StatType.AmmoCapacityMultiplier,
            PlayerStats.StatType.ReloadSpeed,
            PlayerStats.StatType.KnockbackMultiplier,
            PlayerStats.StatType.GlobalPriceMultiplier,
            PlayerStats.StatType.PlayerBulletScale,
            PlayerStats.StatType.AdditionalClipCapacityMultiplier,
            PlayerStats.StatType.ThrownGunDamage,
            PlayerStats.StatType.DodgeRollDamage,
            PlayerStats.StatType.DamageToBosses,
            PlayerStats.StatType.EnemyProjectileSpeedMultiplier,
            PlayerStats.StatType.ChargeAmountMultiplier,
            PlayerStats.StatType.RangeMultiplier,
            PlayerStats.StatType.DodgeRollDistanceMultiplier,
            PlayerStats.StatType.DodgeRollSpeedMultiplier,
            PlayerStats.StatType.TarnisherClipCapacityMultiplier,
            PlayerStats.StatType.MoneyMultiplierFromEnemies
        };

        public static readonly List<PlayerStats.StatType> WeirdPercentageStats = new()
        {
            PlayerStats.StatType.ShadowBulletChance,
            PlayerStats.StatType.ExtremeShadowBulletChance
        };

        public static readonly Dictionary<PlayerStats.StatType, string> StatNameDict = new()
        {
            { PlayerStats.StatType.MovementSpeed, "Speed" },
            { PlayerStats.StatType.RateOfFire, "Firerate" },
            { PlayerStats.StatType.Accuracy, "Spread" },
            { PlayerStats.StatType.Health, "Health" },
            { PlayerStats.StatType.Coolness, "Coolness" },
            { PlayerStats.StatType.Damage, "Damage" },
            { PlayerStats.StatType.ProjectileSpeed, "Bullet Speed" },
            { PlayerStats.StatType.AdditionalGunCapacity, "d" },
            { PlayerStats.StatType.AdditionalItemCapacity, "Item Slots" },
            { PlayerStats.StatType.AmmoCapacityMultiplier, "Ammo" },
            { PlayerStats.StatType.ReloadSpeed, "Reload Time" },
            { PlayerStats.StatType.AdditionalShotPiercing, "Bullet Pierces" },
            { PlayerStats.StatType.KnockbackMultiplier, "Knockback" },
            { PlayerStats.StatType.GlobalPriceMultiplier, "Shop Price" },
            { PlayerStats.StatType.Curse, "Curse" },
            { PlayerStats.StatType.PlayerBulletScale, "Bullet Scale" },
            { PlayerStats.StatType.AdditionalClipCapacityMultiplier, "Clip Size" },
            { PlayerStats.StatType.AdditionalShotBounces, "Bullet Bounces" },
            { PlayerStats.StatType.AdditionalBlanksPerFloor, "Blanks Per Floor" },
            { PlayerStats.StatType.ShadowBulletChance, "Shadow Bullet Chance" },
            { PlayerStats.StatType.ThrownGunDamage, "Thrown Gun Damage" },
            { PlayerStats.StatType.DodgeRollDamage, "Roll Damage" },
            { PlayerStats.StatType.DamageToBosses, "Damage To Bosses" },
            { PlayerStats.StatType.EnemyProjectileSpeedMultiplier, "Enemy Bullet Speed" },
            { PlayerStats.StatType.ExtremeShadowBulletChance, "Pop-Pop Chance" },
            { PlayerStats.StatType.ChargeAmountMultiplier, "Charge Speed" },
            { PlayerStats.StatType.RangeMultiplier, "Range" },
            { PlayerStats.StatType.DodgeRollDistanceMultiplier, "Roll Distance" },
            { PlayerStats.StatType.DodgeRollSpeedMultiplier, "Roll Speed" },
            { PlayerStats.StatType.TarnisherClipCapacityMultiplier, "Temporary Clip Size" },
            { PlayerStats.StatType.MoneyMultiplierFromEnemies, "Money Drops" }
        };

        public static IEnumerator HandleRisingPopup(dfLabel label, Vector3 initialPos, float vel, float duration)
        {
            var ela = 0f;
            while(ela < duration)
            {
                initialPos += Vector3.up * vel * BraveTime.DeltaTime;
                ela += BraveTime.DeltaTime;
                label.transform.position = dfFollowObject.ConvertWorldSpaces(initialPos, GameManager.Instance.MainCameraController.Camera, GameUIRoot.Instance.Manager.renderCamera);
                label.Opacity = 1 - ela / duration;
                yield return null;
            }
            label.gameObject.SetActive(value: false);
            GameUIRoot.Instance.m_inactiveDamageLabels.Add(label);
            yield break;
        }

        public static bool ScaledChance(float chance, float scale)
        {
            if(chance >= 1f)
            {
                return true;
            }
            return Random.value < chance * scale;
        }

        public static float GetAimDirection(this PlayerController player)
        {
            return player.GetRelativeAim().ToAngle();
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

        public static void StealthPlayer(this PlayerController player, string reason, bool allowStealing = true, bool disableEnemyCollision = true, GameObject poof = null, string stealthUnstealthSound = null, 
            bool enableStealthShader = true)
        {
            void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
            {
                BreakStealth(arg1);
            }

            void BreakStealth(PlayerController obj)
            {
                if (poof != null)
                {
                    obj.PlayEffectOnActor(poof, Vector3.zero, false, true, false);
                }
                obj.OnDidUnstealthyAction -= BreakStealth;
                if (allowStealing)
                {
                    obj.SetCapableOfStealing(false, reason, null);
                    obj.OnItemStolen -= BreakStealthOnSteal;
                }
                if (disableEnemyCollision)
                {
                    obj.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
                }
                if (enableStealthShader)
                {
                    obj.ChangeSpecialShaderFlag(1, 0f);
                }
                obj.SetIsStealthed(false, reason);
                if (!string.IsNullOrEmpty(stealthUnstealthSound))
                {
                    AkSoundEngine.PostEvent(stealthUnstealthSound, obj.gameObject);
                }
            }

            if (enableStealthShader)
            {
                player.ChangeSpecialShaderFlag(1, 1f);
            }
            player.SetIsStealthed(true, reason);
            if (disableEnemyCollision)
            {
                player.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            }
            if (poof != null)
            {
                player.PlayEffectOnActor(poof, Vector3.zero, false, true, false);
            }
            if (allowStealing)
            {
                player.SetCapableOfStealing(true, reason, null);
                player.OnItemStolen += BreakStealthOnSteal;
            }
            player.OnDidUnstealthyAction += BreakStealth;
            if (!string.IsNullOrEmpty(stealthUnstealthSound))
            {
                AkSoundEngine.PostEvent(stealthUnstealthSound, player.gameObject);
            }
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
            return s.ToID().Replace("'", "").Replace("/", "_");
        }

        public static List<AIActor> GetActiveEnemiesUnreferenced(this RoomHandler room, RoomHandler.ActiveEnemyType type)
        {
            List<AIActor> enemies = new List<AIActor>();
            room.GetActiveEnemies(type, ref enemies);
            return enemies;
        }

        public static AIActorExt Ext(this AIActor enemy)
        {
            return enemy.GetOrAddComponent<AIActorExt>();
        }

        public static float DiminishingReturnsChance(float stack, float m, float a)
        {
            return 1 - 1 / (stack * m + a);
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

        public static PierceProjModifier GetOrAddPierce(this Projectile proj)
        {
            if (proj.GetComponent<PierceProjModifier>() == null)
            {
                proj.AddComponent<PierceProjModifier>().penetration = 0;
            }
            return proj.GetComponent<PierceProjModifier>();
        }

        public static Vector2 PredictGameActorPosition(Vector2 firingOrigin, SpeculativeRigidbody body, float speed)
        {
            Vector2 unitCenter = body.GetUnitCenter(ColliderType.HitBox);
            Vector2 targetVelocity = body.Velocity;
            return BraveMathCollege.GetPredictedPosition(unitCenter, targetVelocity, firingOrigin, speed);
        }

        public static T[] AddRangeToArray<T>(this T[] array, ICollection<T> range)
        {
            return CollectionExtensions.AddRangeToArray(array, range.ToArray());
        }

        public static T SetupDfSpriteFromTexture<T>(this GameObject obj, Texture2D texture, Shader shader) where T : dfSprite
        {
            T sprite = obj.GetOrAddComponent<T>();
            dfAtlas atlas = obj.GetOrAddComponent<dfAtlas>();
            atlas.Material = new(shader);
            atlas.Material.mainTexture = texture;
            atlas.Items.Clear();
            dfAtlas.ItemInfo info = new()
            {
                border = new RectOffset(),
                deleted = false,
                name = "main_sprite",
                region = new Rect(Vector2.zero, new Vector2(1, 1)),
                rotated = false,
                sizeInPixels = new Vector2(texture.width, texture.height),
                texture = null,
                textureGUID = "main_sprite"
            };
            atlas.AddItem(info);
            sprite.Atlas = atlas;
            sprite.SpriteName = "main_sprite";
            sprite.zindex = 0;
            return sprite;
        }

        public static T NC<T>(this T yes) where T : Object
        {
            if(yes == null)
            {
                return null;
            }
            return yes;
        }

        public static void DropReward(this PunchoutAIActor self, bool isLeft, int exactItemId)
        {
            self.StartCoroutine(self.DropRewardCR(isLeft, exactItemId));
        }

        public static IEnumerator DropRewardCR(this PunchoutAIActor self, bool isLeft, int exactItemId)
        {
            if (exactItemId >= 0)
            {
                self.DroppedRewardIds.Add(exactItemId);
                while (self.state is PunchoutAIActor.ThrowAmmoState)
                {
                    yield return null;
                }
                GameObject droppedItem = SpawnManager.SpawnVFX(self.DroppedItemPrefab, self.transform.position + new Vector3(-0.25f, 2.5f), Quaternion.identity);
                tk2dSprite droppedItemSprite = droppedItem.GetComponent<tk2dSprite>();
                tk2dSprite rewardSprite = PickupObjectDatabase.GetById(exactItemId).GetComponent<tk2dSprite>();
                droppedItemSprite.SetSprite(rewardSprite.Collection, rewardSprite.spriteId);
                droppedItem.GetComponent<PunchoutDroppedItem>().Init(isLeft);
            }
            yield break;
        }

        public static tk2dSprite AddSprite(this GameObject go, string path, string coll, string shader = "tk2d/CutoutVertexColorTintableTilted")
        {
            return tk2dSprite.AddComponent(go, EasyCollectionSetup(coll), SpriteBuilder.AddSpriteToCollection(path, EasyCollectionSetup(coll), shader));
        }

        public static bool HasExt(this PlayerController player)
        {
            if (player == null)
            {
                return false;
            }
            return player.GetComponent<Components.PlayerControllerExt>();
        }

        public static Components.PlayerControllerExt Ext(this PlayerController player)
        {
            if(player == null)
            {
                return null;
            }
            return player.GetOrAddComponent<Components.PlayerControllerExt>();
        }

        public static void RecalculateStats(this PlayerController play)
        {
            play.stats.RecalculateStats(play, false, false);
        }

        public static StatModifier AddOwnerlessModifier(this PlayerController player, PlayerStats.StatType stat, float amount, StatModifier.ModifyMethod modifyMethod = StatModifier.ModifyMethod.ADDITIVE)
        {
            var thingy = StatModifier.Create(stat, modifyMethod, amount);
            player.ownerlessStatModifiers.Add(thingy);
            player.RecalculateStats();
            return thingy;
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

        public static void InitStatics()
        {
            stealthyPoof = SmokeBombObject.poofVfx;
            defaultPoison = IrradiatedLeadObject.HealthModifierEffect;
            defaultFire = HotLeadObject.FireModifierEffect;
            defaultFreeze = FrostBulletsObject.FreezeModifierEffect;
            defaultCheese = ElimentalerObject.GetProjectile().cheeseEffect;
            defaultCharm = CharmingRoundsObject.CharmModifierEffect;
            permanentCharm = YellowChamberObject.CharmEffect;
            defaultGreenFire = PitchPerfectSynergyObject.GetProjectile().fireEffect;
            bombExplosionData = new();
            bombExplosionData.CopyFrom(ExplosiveRoundsObject.ExplosionData);
            bombExplosionData.preventPlayerForce = true;
            bombExplosionData.damageRadius *= 1.25f;
        }

        public static GameObject stealthyPoof;
        public static GameActorHealthEffect defaultPoison;
        public static GameActorFireEffect defaultFire;
        public static GameActorFreezeEffect defaultFreeze;
        public static GameActorCheeseEffect defaultCheese;
        public static GameActorCharmEffect defaultCharm;
        public static GameActorCharmEffect permanentCharm;
        public static GameActorFireEffect defaultGreenFire;
        public static ExplosionData bombExplosionData;
    }
}
