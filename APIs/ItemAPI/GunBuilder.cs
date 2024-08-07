﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;
using System.IO;
using System.Reflection;

namespace SpecialStuffPack.ItemAPI
{
    public static class GunBuilder
    {
        public static Gun SetupBasicGunComponents(GameObject obj)
        {
            obj.AddComponent<tk2dSprite>();
            Gun gun = obj.AddComponent<Gun>();
            gun.gunName = "gun";
            gun.gunSwitchGroup = string.Empty;
            gun.isAudioLoop = false;
            gun.lowersAudioWhileFiring = false;
            gun.gunClass = GunClass.NONE;
            gun.currentGunStatModifiers = new StatModifier[0];
            gun.passiveStatModifiers = new StatModifier[0];
            gun.currentGunDamageTypeModifiers = new DamageTypeModifier[0];
            gun.barrelOffset = obj.transform.Find("Barrel");
            gun.muzzleOffset = null;
            gun.chargeOffset = null;
            gun.reloadOffset = null;
            gun.carryPixelOffset = new IntVector2(2, 0);
            gun.carryPixelUpOffset = new IntVector2(0, 0);
            gun.carryPixelDownOffset = new IntVector2(0, 0);
            gun.UsesPerCharacterCarryPixelOffsets = false;
            gun.PerCharacterPixelOffsets = new CharacterCarryPixelOffset[0];
            gun.leftFacingPixelOffset = new IntVector2(0, 0);
            gun.overrideOutOfAmmoHandedness = GunHandedness.AutoDetect;
            gun.additionalHandState = AdditionalHandState.None;
            gun.gunPosition = GunPositionOverride.AutoDetect;
            gun.forceFlat = false;
            gun.RawSourceVolley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            gun.RawSourceVolley.projectiles = new List<ProjectileModule>();
            gun.singleModule = null;
            gun.rawOptionalReloadVolley = null;
            gun.OverrideFinaleAudio = false;
            gun.HasFiredHolsterShot = false;
            gun.HasFiredReloadSynergy = false;
            gun.modifiedVolley = null;
            gun.modifiedFinalVolley = null;
            gun.modifiedOptionalReloadVolley = null;
            gun.DuctTapeMergedGunIDs = null;
            gun.PreventNormalFireAudio = false;
            gun.OverrideNormalFireAudioEvent = string.Empty;
            gun.ammo = 100;
            gun.CanGainAmmo = true;
            gun.InfiniteAmmo = false;
            gun.UsesBossDamageModifier = false;
            gun.CustomBossDamageModifier = 1f;
            gun.reloadTime = 1f;
            gun.CanReloadNoMatterAmmo = false;
            gun.blankDuringReload = false;
            gun.blankReloadRadius = 0f;
            gun.reflectDuringReload = false;
            gun.blankKnockbackPower = 0f;
            gun.blankDamageToEnemies = 0f;
            gun.blankDamageScalingOnEmptyClip = 1f;
            gun.doesScreenShake = false;
            gun.gunScreenShake = new ScreenShakeSettings();
            gun.directionlessScreenShake = false;
            gun.damageModifier = 0;
            gun.thrownObject = null;
            gun.procGunData = null;
            gun.activeReloadData = null;
            gun.ClearsCooldownsLikeAWP = false;
            gun.AppliesHoming = false;
            gun.AppliedHomingAngularVelocity = 0f;
            gun.AppliedHomingDetectRadius = 0f;
            gun.shootAnimation = string.Empty;
            gun.usesContinuousFireAnimation = false;
            gun.reloadAnimation = string.Empty;
            gun.emptyReloadAnimation = string.Empty;
            gun.idleAnimation = string.Empty;
            gun.chargeAnimation = string.Empty;
            gun.dischargeAnimation = string.Empty;
            gun.emptyAnimation = string.Empty;
            gun.introAnimation = string.Empty;
            gun.finalShootAnimation = string.Empty;
            gun.enemyPreFireAnimation = string.Empty;
            gun.outOfAmmoAnimation = string.Empty;
            gun.criticalFireAnimation = string.Empty;
            gun.dodgeAnimation = string.Empty;
            gun.usesDirectionalIdleAnimations = false;
            gun.usesDirectionalAnimator = false;
            gun.preventRotation = false;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.usesContinuousMuzzleFlash = false;
            gun.finalMuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.reloadEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.emptyReloadEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.activeReloadSuccessEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.activeReloadFailedEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            gun.light = null;
            gun.baseLightIntensity = 0f;
            gun.shellCasing = null;
            gun.shellsToLaunchOnFire = 0;
            gun.shellCasingOnFireFrameDelay = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.reloadShellLaunchFrame = 0;
            gun.clipObject = null;
            gun.clipsToLaunchOnReload = 0;
            gun.reloadClipLaunchFrame = 0;
            gun.prefabName = string.Empty;
            gun.rampBullets = false;
            gun.rampStartHeight = 0f;
            gun.rampTime = 0f;
            gun.IgnoresAngleQuantization = false;
            gun.IsTrickGun = false;
            gun.TrickGunAlternatesHandedness = false;
            gun.PreventOutlines = false;
            gun.alternateVolley = null;
            gun.alternateShootAnimation = string.Empty;
            gun.alternateReloadAnimation = string.Empty;
            gun.alternateIdleAnimation = string.Empty;
            gun.alternateSwitchGroup = string.Empty;
            gun.IsHeroSword = false;
            gun.HeroSwordDoesntBlank = false;
            gun.StarterGunForAchievement = false;
            gun.CanSneakAttack = false;
            gun.SneakAttackDamageMultiplier = 1f;
            gun.SuppressLaserSight = false;
            gun.RequiresFundsToShoot = false;
            gun.CurrencyCostPerShot = 1;
            gun.weaponPanelSpriteOverride = null;
            gun.IsLuteCompanionBuff = false;
            gun.MovesPlayerForwardOnChargeFire = false;
            gun.LockedHorizontalOnCharge = false;
            gun.LockedHorizontalCenterFireOffset = 0f;
            gun.LockedHorizontalOnReload = false;
            gun.GoopReloadsFree = false;
            gun.IsUndertaleGun = false;
            gun.LocalActiveReload = false;
            gun.UsesRechargeLikeActiveItem = false;
            gun.ActiveItemStyleRechargeAmount = 0f;
            gun.CanAttackThroughObjects = false;
            gun.CanCriticalFire = false;
            gun.CriticalChance = 1f;
            gun.CriticalDamageMultiplier = 1f;
            gun.CriticalMuzzleFlashEffects = new VFXPool { type = VFXPoolType.None };
            gun.CriticalReplacementProjectile = null;
            gun.GainsRateOfFireAsContinueAttack = false;
            gun.RateOfFireMultiplierAdditionPerSecond = 0f;
            gun.OnlyUsesIdleInWeaponBox = false;
            gun.DisablesRendererOnCooldown = false;
            gun.ObjectToInstantiateOnReload = null;
            gun.AdditionalClipCapacity = 0;
            gun.LastShotIndex = -1;
            gun.DidTransformGunThisFrame = false;
            gun.CustomLaserSightDistance = 30f;
            gun.CustomLaserSightHeight = 0.25f;
            gun.LastLaserSightEnemy = null;
            gun.HasEverBeenAcquiredByPlayer = false;
            gun.HasBeenPickedUp = false;
            gun.OverrideAngleSnap = null;
            gun.SetBaseMaxAmmo(100);
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = LoadHelper.LoadAssetFromAnywhere<GameObject>("GunAnimation02").GetComponent<tk2dSpriteAnimation>();
            tk2dSpriteAttachPoint attachPoint = obj.AddComponent<tk2dSpriteAttachPoint>();
            attachPoint.attachPoints = new List<Transform>
            {
                obj.transform.Find("PrimaryHand"),
                obj.transform.Find("Casing"),
                obj.transform.Find("Clip")
            };
            if(obj.transform.Find("SecondaryHand") != null)
            {
                attachPoint.attachPoints.Add(obj.transform.Find("SecondaryHand"));
                gun.gunHandedness = GunHandedness.TwoHanded;
            }
            else
            {
                gun.gunHandedness = GunHandedness.OneHanded;
            }
            //gun.gunHandedness = GunHandedness.AutoDetect;
            attachPoint.deactivateUnusedAttachPoints = false;
            attachPoint.disableEmissionOnUnusedParticleSystems = false;
            attachPoint.ignorePosition = false;
            attachPoint.ignoreScale = false;
            attachPoint.ignoreRotation = false;
            attachPoint.centerUnusedAttachPoints = false;
            obj.AddComponent<EncounterTrackable>();
            return gun;
        }

        public static int SetupProjectileSpriteDefinition(Texture2D texture, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.LowerLeft, int offsetX = 0, int offsetY = 0, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null)
        {
            int id = SpriteBuilder.AddSpriteToCollection(texture, ETGMod.Databases.Items.ProjectileCollection, "tk2d/CutoutVertexColorTilted");
            if (overrideColliderPixelWidth == null)
            {
                overrideColliderPixelWidth = texture.width;
            }
            if (overrideColliderPixelHeight == null)
            {
                overrideColliderPixelHeight = texture.height;
            }
            if (overrideColliderOffsetX == null)
            {
                overrideColliderOffsetX = 0;
            }
            if (overrideColliderOffsetY == null)
            {
                overrideColliderOffsetY = 0;
            }
            overrideColliderOffsetX += offsetX;
            overrideColliderOffsetY += offsetY;
            float colliderWidth = overrideColliderPixelWidth.Value / 16f;
            float colliderHeight = overrideColliderPixelHeight.Value / 16f;
            float colliderOffsetX = overrideColliderOffsetX.Value / 16f;
            float colliderOffsetY = overrideColliderOffsetY.Value / 16f;
            tk2dSpriteDefinition def = ETGMod.Databases.Items.ProjectileCollection.spriteDefinitions[id];
            def.colliderVertices = new Vector3[2];
            def.colliderVertices[0] = new Vector3(colliderOffsetX, colliderOffsetY, 0f);
            def.colliderVertices[1] = new Vector3(colliderWidth / 2, colliderHeight / 2);
            def.ConstructOffsetsFromAnchor(anchor, null, false, true);
            def.AddOffset(new Vector2(offsetX / 16f, offsetY / 16f), false);
            return id;
        }

        public static T SetupBasicProjectileComponents<T>(GameObject obj) where T : Projectile
        {
            T proj = obj.AddComponent<T>();
            proj.baseData = new ProjectileData();
            proj.persistTime = 0f;
            proj.AdditionalBurstLimits = new SynergyBurstLimit[0];
            SpeculativeRigidbody body = obj.AddComponent<SpeculativeRigidbody>();
            body.PixelColliders = new List<PixelCollider>()
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon,
                    CollisionLayer = CollisionLayer.Projectile
                }
            };
            obj.transform.Find("Sprite")?.AddComponent<tk2dSprite>();
            return proj;
        }

        public static T EasyProjectileInit<T>(string assetPath, string spritePath, float damage, float speed, float range, float knockback, bool shouldRotate, bool ignoreDamageCaps, bool pierceMinorBreakables, int? overrideSpriteId = null, 
            tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter, int offsetX = 0, int offsetY = 0, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, 
            int? overrideColliderOffsetY = null, tk2dSpriteCollectionData overrideSpriteCollection = null) where T : Projectile
        {
            T proj = SetupBasicProjectileComponents<T>(AssetBundleManager.Load<GameObject>(assetPath));
            if (overrideSpriteId != null)
            {
                proj.GetComponentInChildren<tk2dBaseSprite>()?.SetSprite(overrideSpriteCollection ?? ETGMod.Databases.Items.ProjectileCollection, overrideSpriteId.Value);
            }
            else
            {
                proj.GetComponentInChildren<tk2dBaseSprite>()?.SetSprite(ETGMod.Databases.Items.ProjectileCollection, SetupProjectileSpriteDefinition(AssetBundleManager.Load<Texture2D>(spritePath), anchor, offsetX, offsetY, overrideColliderPixelWidth,
                    overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY));
            }
            if (proj.GetComponentInChildren<tk2dBaseSprite>() == null)
            {
                int xOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
                {
                    xOffset = -(overrideColliderPixelHeight.GetValueOrDefault() / 2);
                }
                else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    xOffset = -overrideColliderPixelHeight.GetValueOrDefault();
                }
                int yOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
                {
                    yOffset = -(overrideColliderPixelHeight.GetValueOrDefault() / 2);
                }
                else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    yOffset = -overrideColliderPixelHeight.GetValueOrDefault();
                }
                proj.specRigidbody.PixelColliders = new List<PixelCollider>()
                {
                    new PixelCollider()
                    {
                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.Projectile,
                        ManualHeight = overrideColliderPixelHeight.GetValueOrDefault(),
                        ManualWidth = overrideColliderPixelWidth.GetValueOrDefault(),
                        ManualOffsetX = overrideColliderOffsetX.GetValueOrDefault() + xOffset,
                        ManualOffsetY = overrideColliderOffsetY.GetValueOrDefault() + yOffset
                    }
                };
            }
            proj.baseData.damage = damage;
            proj.baseData.speed = speed;
            proj.baseData.range = range;
            proj.baseData.force = knockback;
            proj.shouldRotate = shouldRotate;
            proj.shouldFlipVertically = true;
            proj.ignoreDamageCaps = ignoreDamageCaps;
            proj.pierceMinorBreakables = pierceMinorBreakables;
            SpecialAssets.assets.Add(proj.gameObject);
            return proj;
        }

        public static GameObject EasyCasingSetup(string assetPath, string spriteName)
        {
            var go = AssetBundleManager.Load<GameObject>(assetPath);
            var coll = EasyCollectionSetup("SpecialVFXCollection");
            var id = SpriteBuilder.AddSpriteToCollection(spriteName, coll, "Brave/LitTk2dCustomFalloffTiltedCutoutFast");
            tk2dSprite.AddComponent(go, coll, id);
            var def = coll.spriteDefinitions[id];
            def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
            var deb = go.AddComponent<DebrisObject>();
            deb.ACCURATE_DEBRIS_THRESHOLD = 0.25f;
            deb.accurateDebris = false;
            deb.additionalBounceEnglish = 0f;
            deb.additionalHeightBoost = 0f;
            deb.angularVelocity = 900f;
            deb.angularVelocityVariance = 180f;
            deb.animatePitFall = false;
            deb.AssignedGoop = null;
            deb.audioEventName = "Play_WPN_magnum_shells_01";
            deb.bounceCount = 1;
            deb.breakOnFallChance = 1f;
            deb.breaksOnFall = true;
            deb.canRotate = true;
            deb.changesCollisionLayer = false;
            deb.collisionStopsBullets = false;
            deb.decayOnBounce = 0.5f;
            deb.detachedParticleSystems = new();
            deb.directionalAnimationData = new();
            deb.doesDecay = false;
            deb.DoesGoopOnRest = false;
            deb.followupBehavior = DebrisObject.DebrisFollowupAction.None;
            deb.followupIdentifier = "";
            deb.ForceUpdateIfDisabled = false;
            deb.GoopRadius = 1f;
            deb.groundedCollisionLayer = CollisionLayer.LowObstacle;
            deb.groupManager = null;
            deb.inertialMass = 1f;
            deb.IsCorpse = false;
            deb.isFalling = false;
            deb.isPitFalling = false;
            deb.isStatic = false;
            deb.killTranslationOnBounce = false;
            deb.lifespanMax = 1f;
            deb.lifespanMin = 1f;
            deb.motionMultiplier = 1f;
            deb.onGround = false;
            deb.optionalBounceVFX = null;
            deb.pitFallSplash = false;
            deb.placementOptions = new();
            deb.playAnimationOnTrigger = false;
            deb.PreventAbsorption = false;
            deb.PreventFallingInPits = false;
            deb.removeSRBOnGrounded = false;
            deb.shadowSprite = null;
            deb.shouldUseSRBMotion = false;
            deb.usesDirectionalFallAnimations = false;
            deb.usesLifespan = false;
            deb.Priority = EphemeralObject.EphemeralPriority.Ephemeral;
            return go;
        }

        public static GameObject EasyMuzzleSetup(string assetPath, string animationPrefix, int fps, string shader, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleLeft, List<IntVector2> manualOffsets = null, Action<Material> postprocessMaterial = null)
        {
            var go = AssetBundleManager.Load<GameObject>(assetPath);
            var coll = EasyCollectionSetup("SpecialVFXCollection");
            var anim = EasyAnimationSetup("SpecialVFXAnimation");
            var clippy = anim.AddClip(animationPrefix, coll, fps, tk2dSpriteAnimationClip.WrapMode.Once, null, null, shader);
            var id = clippy.frames[0].spriteId;
            tk2dSprite.AddComponent(go, coll, id);
            for (int i = 0; i < clippy.frames.Length; i++)
            {
                var frame = clippy.frames[i];
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                def.ConstructOffsetsFromAnchor(anchor, null, false, true);
                if (manualOffsets != null && manualOffsets.Count > i)
                {
                    def.AddOffset(manualOffsets[i].ToVector2() / 16f);
                }
                postprocessMaterial?.Invoke(def.material);
                postprocessMaterial?.Invoke(def.materialInst);
            }
            var animator = go.AddComponent<tk2dSpriteAnimator>();
            animator.Library = anim;
            animator.DefaultClipId = anim.GetClipIdByName(clippy.name);
            animator.playAutomatically = true;
            go.AddComponent<SpriteAnimatorKiller>();
            return go;
        }

        public static Gun EasyGunInit(string assetPath, string gunName, string gunShortDesc, string gunLongDesc, string defaultSprite, string ammonomiconSprite, string gunSpriteContainer, int maxAmmo, float realoadTime, IntVector2 barrelOffset, 
            VFXPool muzzleflash, string gunSwitchGroup, PickupObject.ItemQuality quality, GunClass gunClass, out Action finish, int? ammonomiconPlacement = null, string overrideConsoleId = null, GameObject baseObject = null)
        {
            if (!gunSpriteContainer.StartsWith("assets/"))
            {
                gunSpriteContainer = "assets/" + gunSpriteContainer;
            }
            if (!ammonomiconSprite.StartsWith("gunsprites/ammonomicon/"))
            {
                ammonomiconSprite = "gunsprites/ammonomicon/" + ammonomiconSprite;
            }
            GameObject gameObject = baseObject ?? AssetBundleManager.Load<GameObject>(assetPath);
            Gun gun = SetupBasicGunComponents(gameObject);
            gun.gunName = gunName;
            Game.Items.Add(overrideConsoleId ?? (SpecialStuffModule.globalPrefix + ":" + gunName.ToMTGId()), gun);
            int id2 = SpriteBuilder.AddSpriteToCollection(ammonomiconSprite, SpriteBuilder.ammonomiconCollection, "tk2d/CutoutVertexColorTilted");
            List<int> ids = new();
            foreach (string asset in AssetBundleManager.specialeverything.GetAllAssetNames())
            {
                if (asset.StartsWith(gunSpriteContainer) && asset.EndsWith(".png"))
                {
                    int id = SpriteBuilder.AddSpriteToCollection(asset, ETGMod.Databases.Items.WeaponCollection, "tk2d/CutoutVertexColorTilted");
                    ids.Add(id);
                    if (AssetBundleManager.specialeverything.Contains(asset.Replace(".png", ".json")))
                    {
                        AssetSpriteData assetSpriteData = default;
                        using (MemoryStream s = new(Encoding.UTF8.GetBytes(AssetBundleManager.Load<TextAsset>(asset.Replace(".png", ".json")).text)))
                        {
                            assetSpriteData = JSONHelper.ReadJSON<AssetSpriteData>(s);
                        }
                        ETGMod.Databases.Items.WeaponCollection.SetAttachPoints(id, assetSpriteData.attachPoints);
                    }
                }
            }
            var defaultId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(defaultSprite);
            var defaultDefinition = ETGMod.Databases.Items.WeaponCollection.spriteDefinitions[defaultId];
            var globalOffset = new Vector2(-defaultDefinition.position0.x, -defaultDefinition.position0.y);
            foreach(var x in ids)
            {
                ETGMod.Databases.Items.WeaponCollection.spriteDefinitions[x].AddOffset(globalOffset);
                var attach = ETGMod.Databases.Items.WeaponCollection.GetAttachPoints(x);
                if(attach == null)
                {
                    continue;
                }
                foreach (var attachPoint in attach)
                {
                    attachPoint.position += globalOffset.ToVector3ZUp(0f);
                }
            };

            gun.sprite.SetSprite(ETGMod.Databases.Items.WeaponCollection, defaultId);
            gun.UpdateAnimations();
            var notSpapiName = gun.name;
            gun.gameObject.name = $"spapi_{gun.gameObject.name}";
            gun.encounterTrackable.journalData = new();
            gun.encounterTrackable.journalData.AmmonomiconSprite = SpriteBuilder.ammonomiconCollection.spriteDefinitions[id2].name;
            gun.encounterTrackable.EncounterGuid = gun.gameObject.name.RemoveUnacceptableCharactersForGUID();
            gun.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
            gun.encounterTrackable.journalData.SuppressKnownState = false;
            string keyName = "#" + gun.name.Replace(" ", "").ToUpperInvariant();
            gun.encounterTrackable.journalData.PrimaryDisplayName = keyName + "_ENCNAME";
            gun.encounterTrackable.journalData.NotificationPanelDescription = keyName + "_SHORTDESC";
            gun.encounterTrackable.journalData.AmmonomiconFullEntry = keyName + "_LONGDESC";
            gun.SetName(gunName);
            gun.SetShortDescription(gunShortDesc);
            gun.SetLongDescription(gunLongDesc);
            gun.SetBaseMaxAmmo(maxAmmo);
            gun.ammo = maxAmmo;
            gun.quality = quality;
            gun.gunClass = gunClass;
            gun.reloadTime = realoadTime;
            gun.gunSwitchGroup = gunSwitchGroup;
            gun.barrelOffset.position = barrelOffset.ToVector3() / 16f;
            if (muzzleflash != null)
            {
                gun.muzzleFlashEffects = muzzleflash;
            }
            if(ammonomiconPlacement != null)
            {
                gun.PlaceItemInAmmonomiconAfterItemById(ammonomiconPlacement.Value);
            }
            finish = delegate ()
            {
                ETGMod.Databases.Items.AddSpecific(gun, false, "ANY");
                ItemIds.Add(notSpapiName.ToLowerInvariant(), gun.PickupObjectId);
                Guns.Add(notSpapiName.ToLowerInvariant(), gun);
                Item.Add(notSpapiName.ToLowerInvariant(), gun);
            };
            return gun;
        }
    }
}
