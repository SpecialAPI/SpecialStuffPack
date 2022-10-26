using SpecialStuffPack.SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FullInspector;
using Gungeon;
using SpecialStuffPack.Behaviors.WizardKnight.Sword;
using SpecialStuffPack.BulletScripts;
using SpecialStuffPack.BulletScripts.WizardKnight.Sword;
using SpecialStuffPack.Components;
using SpecialStuffPack.Behaviors.WizardKnight;
using SpecialStuffPack.Behaviors.Ninja;
using SpecialStuffPack.BulletScripts.Ninja;

namespace SpecialStuffPack.Enemies
{
    public static class SpecialEnemies
    {
        public static void InitLockGhostPrefab()
        {
            GameObject go = AssetBundleManager.Load<GameObject>("enemies/lockghost");
            GameObject coll = AssetBundleManager.Load<GameObject>("spritecollections/lockghostcollection");
            GameObject anim = AssetBundleManager.Load<GameObject>("spriteanimations/lockghostanimation");
            tk2dSpriteCollectionData collection = coll.AddComponent<tk2dSpriteCollectionData>();
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            collection.spriteCollectionName = "LockGhostCollection";
            collection.assetName = "LockGhostCollection";
            int spriteId = SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_001", collection, "Brave/LitBlendUber");
            tk2dSprite.AddComponent(go, collection, spriteId).HeightOffGround = 10f;
            collection.spriteDefinitions[spriteId].material.SetFloat("_VertexColor", 1f);
            LockGhostPrefab = go;
            SpeculativeRigidbody body = go.AddComponent<SpeculativeRigidbody>();
            body.CollideWithOthers = true;
            body.CollideWithTileMap = false;
            body.PixelColliders = new List<PixelCollider>
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = true,
                    ManualOffsetX = -5,
                    ManualOffsetY = -9,
                    ManualWidth = 12,
                    ManualHeight = 19
                },
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyHitBox,
                    IsTrigger = true,
                    ManualOffsetX = -5,
                    ManualOffsetY = -9,
                    ManualWidth = 12,
                    ManualHeight = 19
                }
            };
            LockGhostController ghostController = go.AddComponent<LockGhostController>();
            ghostController.PreferredDistance = 3f;
            ghostController.MaxSpeed = 15f;
            ghostController.MinSpeed = 2f;
            ghostController.MinSpeedDistance = 3f;
            ghostController.MaxSpeedDistance = 12f;
            ghostController.DashCooldownTime = 1f;
            ghostController.DashChargeTime = 1f;
            ghostController.DashSpeed = 20f;
            ghostController.DashTime = 1f;
            ghostController.FadeTime = 0.5f;
            ghostController.DashSlowdownTime = 0.25f;
            ghostController.DashTimerDistance = 4.5f;
            body.UpdateCollidersOnRotation = true;
            tk2dSpriteAnimator animator = go.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = anim.AddComponent<tk2dSpriteAnimation>();
            animator.Library = animation;
            List<int> idleFrontIds = new List<int>
            {
                spriteId,
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_004", collection, "Brave/LitBlendUber")
            };
            List<int> idleLeftIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_left_001", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_left_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_left_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_left_004", collection, "Brave/LitBlendUber")
            };
            List<int> idleRightIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_right_001", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_right_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_right_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_right_004", collection, "Brave/LitBlendUber")
            };
            List<IntVector2> idleOffsets = new List<IntVector2>
            {
                new IntVector2(0, 0),
                new IntVector2(0, -1),
                new IntVector2(0, -1),
                new IntVector2(0, 0)
            }; 
            List<int> chargeFrontIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_001", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_004", collection, "Brave/LitBlendUber")
            };
            List<int> chargeLeftIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_left_001", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_left_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_left_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_left_004", collection, "Brave/LitBlendUber")
            };
            List<int> chargeRightIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_right_001", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_right_002", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_right_003", collection, "Brave/LitBlendUber"),
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_charge_right_004", collection, "Brave/LitBlendUber")
            };
            List<IntVector2> horizontalChargeOffsets = new List<IntVector2>
            {
                new IntVector2(1, 0),
                new IntVector2(-1, 0),
                new IntVector2(1, 0),
                new IntVector2(-1, 0)
            };
            List<IntVector2> verticalChargeOffsets = new List<IntVector2>
            {
                new IntVector2(0, 1),
                new IntVector2(0, -1),
                new IntVector2(0, 1),
                new IntVector2(0, -1)
            };
            List<int> dashIds = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_dash_001", collection, "Brave/LitBlendUber")
            };
            Vector2 scale = collection.spriteDefinitions[spriteId].position3;
            foreach (int id in idleFrontIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, scale, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in idleRightIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, scale, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in idleLeftIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, scale, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in chargeFrontIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in chargeRightIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in chargeLeftIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            foreach (int id in dashIds)
            {
                collection.spriteDefinitions[id].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, null, false, false);
                collection.spriteDefinitions[id].material.SetFloat("_VertexColor", 1f);
            }
            tk2dSpriteAnimationClip front = new tk2dSpriteAnimationClip { fps = 6, frames = idleFrontIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "idle_front" };
            front.ApplyOffsetsToAnimation(idleOffsets);
            tk2dSpriteAnimationClip left = new tk2dSpriteAnimationClip { fps = 6, frames = idleLeftIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "idle_left" };
            left.ApplyOffsetsToAnimation(idleOffsets);
            tk2dSpriteAnimationClip right = new tk2dSpriteAnimationClip { fps = 6, frames = idleRightIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "idle_right" };
            right.ApplyOffsetsToAnimation(idleOffsets);
            tk2dSpriteAnimationClip chargefront = new tk2dSpriteAnimationClip { fps = 12, frames = chargeFrontIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "charge_front" };
            chargefront.ApplyOffsetsToAnimation(horizontalChargeOffsets);
            tk2dSpriteAnimationClip chargeleft = new tk2dSpriteAnimationClip { fps = 12, frames = chargeLeftIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "charge_left" };
            chargeleft.ApplyOffsetsToAnimation(verticalChargeOffsets);
            tk2dSpriteAnimationClip chargeright = new tk2dSpriteAnimationClip { fps = 12, frames = chargeRightIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "charge_right" };
            chargeright.ApplyOffsetsToAnimation(verticalChargeOffsets);
            tk2dSpriteAnimationClip dash = new tk2dSpriteAnimationClip { fps = 1, frames = dashIds.ConvertAll((int id) => new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = collection }).ToArray(), name = "dash" };
            animation.clips = new tk2dSpriteAnimationClip[]
            {
                front,
                left,
                right,
                chargefront,
                chargeleft,
                chargeright,
                dash
            };
            AIAnimator aianimator = go.AddComponent<AIAnimator>();
            aianimator.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.SixWay,
                AnimNames = new string[] { "idle_front", "idle_right", "idle_right", "idle_front", "idle_left", "idle_left" },
                Prefix = string.Empty,
                Flipped = new DirectionalAnimation.FlipType[6]
            };
            aianimator.MoveAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.FlightAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.HitAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.facingType = AIAnimator.FacingType.Movement;
            aianimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
            {
                new AIAnimator.NamedDirectionalAnimation
                {
                    name = "charge",
                    anim = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.SixWay,
                        AnimNames = new string[] { "charge_front", "charge_right", "charge_right", "charge_front", "charge_left", "charge_left" },
                        Prefix = string.Empty,
                        Flipped = new DirectionalAnimation.FlipType[6]
                    }
                },
                new AIAnimator.NamedDirectionalAnimation
                {
                    name = "dash",
                    anim = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        AnimNames = new string[] { string.Empty },
                        Prefix = "dash",
                        Flipped = new DirectionalAnimation.FlipType[1]
                    }
                }
            };
            EncounterTrackable track = go.AddComponent<EncounterTrackable>();
            track.ProxyEncounterGuid = string.Empty;
            track.EncounterGuid = "lock_ghost";
            track.journalData = new JournalEntry
            {
                AmmonomiconFullEntry = "#LOCK_GHOST_DESCRIPTION",
                PrimaryDisplayName = "#LOCK_GHOST_NAME",
                NotificationPanelDescription = "#LOCK_GHOST_SHORTDESC",
                enemyPortraitSprite = AssetBundleManager.Load<Texture2D>("enemysprites/ammonomicon/lock_ghost_portrait_001"),
                AmmonomiconSprite = "lock_ghost_idle_001",
                DisplayOnLoadingScreen = false,
                IsEnemy = true,
                RequiresLightBackgroundInLoadingScreen = false,
                SpecialIdentifier = JournalEntry.CustomJournalEntryType.NONE,
                SuppressInAmmonomicon = false,
                SuppressKnownState = false
            };
            ETGMod.Databases.Strings.Enemies.Set("#LOCK_GHOST_NAME", "???");
            ETGMod.Databases.Strings.Enemies.Set("#LOCK_GHOST_SHORTDESC", "Vengeful Spirit");
            ETGMod.Databases.Strings.Enemies.Set("#LOCK_GHOST_DESCRIPTION", "Once a very important lock, now a mere shadow of the past. Will haunt anyone who dares disturb it. Can only be seen and interacted by it's target, causing a very weird sight " +
                "for any possible observers.");
            track.IgnoreDifferentiator = false;
            track.prerequisites = new DungeonPrerequisite[0];
            track.UsesPurpleNotifications = false;
            EncounterDatabase.Instance.Entries.Add(new EncounterDatabaseEntry(track) { path = "enemies/lockghost", myGuid = "lock_ghost", unityGuid = "lock_ghost" });
            EnemyDatabase.Instance.Entries.Add(new EnemyDatabaseEntry
            {
                difficulty = DungeonPlaceableBehaviour.PlaceableDifficulty.HARD,
                encounterGuid = "lock_ghost",
                ForcedPositionInAmmonomicon = 1000000000,
                isInBossTab = false,
                isNormalEnemy = false,
                myGuid = "lock_ghost",
                path = "enemies/lockghost",
                placeableHeight = 2,
                placeableWidth = 2,
                unityGuid = "lock_ghost"
            });
            SpriteBuilder.AddSpriteToCollection("enemysprites/lockghost/lock_ghost_idle_001", SpriteBuilder.ammonomiconCollection, "Brave/LitBlendUber");
            SpecialAssets.assets.Add(go);
            SpecialAssets.assets.Add(coll);
            SpecialAssets.assets.Add(anim);
        }

        public static void InitFriendlyLockGhostPrefab()
        {
            GameObject go = AssetBundleManager.Load<GameObject>("enemies/friendlylockghost");
            GameObject coll = AssetBundleManager.Load<GameObject>("spritecollections/lockghostcollection");
            GameObject anim = AssetBundleManager.Load<GameObject>("spriteanimations/lockghostanimation");
            tk2dSpriteCollectionData collection = coll.GetComponent<tk2dSpriteCollectionData>();
            int spriteId = collection.GetSpriteIdByName("lock_ghost_idle_001");
            tk2dSprite.AddComponent(go, collection, spriteId).HeightOffGround = 10f;
            FriendlyLockGhostPrefab = go;
            SpeculativeRigidbody body = go.AddComponent<SpeculativeRigidbody>();
            body.CollideWithOthers = true;
            body.CollideWithTileMap = false;
            body.PixelColliders = new List<PixelCollider>
            {
                new PixelCollider()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyBlocker,
                    IsTrigger = true,
                    ManualOffsetX = -7,
                    ManualOffsetY = -10,
                    ManualWidth = 16,
                    ManualHeight = 21
                }
            };
            FriendlyLockGhostController ghostController = go.AddComponent<FriendlyLockGhostController>();
            ghostController.DashChargeTime = 1f;
            ghostController.DashSpeed = 20f;
            ghostController.DashTime = 1f;
            ghostController.FadeTime = 0.5f;
            ghostController.HitDamage = 50f;
            ghostController.StunTime = 1f;
            body.UpdateCollidersOnRotation = true;
            tk2dSpriteAnimator animator = go.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = anim.GetComponent<tk2dSpriteAnimation>();
            animator.Library = animation;
            AIAnimator aianimator = go.AddComponent<AIAnimator>();
            aianimator.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.SixWay,
                AnimNames = new string[] { "idle_front", "idle_right", "idle_right", "idle_front", "idle_left", "idle_left" },
                Prefix = string.Empty,
                Flipped = new DirectionalAnimation.FlipType[6]
            };
            aianimator.MoveAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.FlightAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.HitAnimation = new DirectionalAnimation { Type = DirectionalAnimation.DirectionType.None };
            aianimator.facingType = AIAnimator.FacingType.Movement;
            aianimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
            {
                new AIAnimator.NamedDirectionalAnimation
                {
                    name = "charge",
                    anim = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.SixWay,
                        AnimNames = new string[] { "charge_front", "charge_right", "charge_right", "charge_front", "charge_left", "charge_left" },
                        Prefix = string.Empty,
                        Flipped = new DirectionalAnimation.FlipType[6]
                    }
                },
                new AIAnimator.NamedDirectionalAnimation
                {
                    name = "dash",
                    anim = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        AnimNames = new string[] { string.Empty },
                        Prefix = "dash",
                        Flipped = new DirectionalAnimation.FlipType[1]
                    }
                }
            };
            SpecialAssets.assets.Add(go);
        }

        public static void AddAdvancedDragunAmmonomiconEntry()
        {
            AIActor dragun = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec");
            if (dragun.GetComponent<EncounterTrackable>() != null)
            {
                UnityEngine.Object.Destroy(dragun.GetComponent<EncounterTrackable>());
            }
            SpriteBuilder.AddToAmmonomicon("enemysprites/ammonomicon/boss_icon_dragun_advanced_001");
            EncounterTrackable track = dragun.AddComponent<EncounterTrackable>();
            track.ProxyEncounterGuid = string.Empty;
            track.EncounterGuid = "05b8afe0b6cc4fffa9dc6036fa24c8ec";
            track.journalData = new JournalEntry
            {
                AmmonomiconFullEntry = "#DRAGUNGOLD_DESCRIPTION",
                PrimaryDisplayName = "#DRAGUNGOLD_NAME",
                NotificationPanelDescription = "#DRAGUNGOLD_SHORTDESC",
                enemyPortraitSprite = AssetBundleManager.Load<Texture2D>("enemysprites/ammonomicon/dragungold_portrait_001"),
                AmmonomiconSprite = "boss_icon_dragun_advanced_001",
                DisplayOnLoadingScreen = false,
                IsEnemy = true,
                RequiresLightBackgroundInLoadingScreen = false,
                SpecialIdentifier = JournalEntry.CustomJournalEntryType.NONE,
                SuppressInAmmonomicon = false,
                SuppressKnownState = false
            };
            EncounterDatabase.Instance.Entries.Add(new EncounterDatabaseEntry(track) { path = "enemies/dragungold", myGuid = "05b8afe0b6cc4fffa9dc6036fa24c8ec", unityGuid = "05b8afe0b6cc4fffa9dc6036fa24c8ec" });
            EnemyDatabase.GetEntry("05b8afe0b6cc4fffa9dc6036fa24c8ec").isInBossTab = true;
            EnemyDatabase.GetEntry("05b8afe0b6cc4fffa9dc6036fa24c8ec").encounterGuid = "05b8afe0b6cc4fffa9dc6036fa24c8ec";
            ETGMod.Databases.Strings.Enemies.Set("#DRAGUNGOLD_NAME", "Advanced Dragun");
            ETGMod.Databases.Strings.Enemies.Set("#DRAGUNGOLD_SHORTDESC", "Advanced Beast");
            ETGMod.Databases.Strings.Enemies.Set("#DRAGUNGOLD_DESCRIPTION", "The High Dragun, fully restored and powered up by the serpent's power. Even harder to slay than normal draguns.");
            track.IgnoreDifferentiator = false;
            track.prerequisites = new DungeonPrerequisite[0];
            track.UsesPurpleNotifications = false;
        }

        public static AIActor SetupAIActorDummyFromAssetBundle(string path, IntVector2 colliderOffset, IntVector2 colliderDimensions, bool noSprite = false)
        {
            GameObject obj = AssetBundleManager.specialeverything.LoadAsset<GameObject>(path);
            SpecialAssets.assets.Add(obj);
            return ProcessAIActorDummy(obj, colliderOffset, colliderDimensions, noSprite);
        }

        public static AIActor ProcessAIActorDummy(GameObject obj, IntVector2 colliderOffset, IntVector2 colliderDimensions, bool noSprite = false)
        {
            tk2dSprite sprite = null;
            if (!noSprite)
            {
                sprite = obj.AddComponent<tk2dSprite>();
                sprite.IsPerpendicular = true;
            }
            SpeculativeRigidbody body = obj.GetOrAddComponent<SpeculativeRigidbody>();
            body.PixelColliders = new List<PixelCollider>();
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            pixelCollider.ManualWidth = colliderDimensions.x;
            pixelCollider.ManualHeight = colliderDimensions.y;
            pixelCollider.ManualOffsetX = colliderOffset.x;
            pixelCollider.ManualOffsetY = colliderOffset.y;
            body.PixelColliders.Add(pixelCollider);
            PixelCollider pixelCollider2 = new PixelCollider();
            pixelCollider2.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider2.CollisionLayer = CollisionLayer.EnemyHitBox;
            pixelCollider2.ManualWidth = colliderDimensions.x;
            pixelCollider2.ManualHeight = colliderDimensions.y;
            pixelCollider2.ManualOffsetX = colliderOffset.x;
            pixelCollider2.ManualOffsetY = colliderOffset.y;
            body.PixelColliders.Add(pixelCollider2);
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            AIActor aiactor = obj.AddComponent<AIActor>();
            HealthHaver hh = obj.AddComponent<HealthHaver>();
            if(sprite != null)
            {
                hh.bodySprites.Add(sprite);
            }
            hh.SetHealthMaximum(10f);
            hh.ForceSetCurrentHealth(10f);
            aiactor.gameObject.AddComponent<KnockbackDoer>();
            obj.AddComponent<ObjectVisibilityManager>();
            BehaviorSpeculator spec = obj.AddComponent<BehaviorSpeculator>();
            ((ISerializedObject)spec).SerializedObjectReferences = new List<UnityEngine.Object>(0);
            ((ISerializedObject)spec).SerializedStateKeys = new List<string> { "OverrideBehaviors", "OtherBehaviors", "TargetBehaviors", "AttackBehaviors", "MovementBehaviors" };
            ((ISerializedObject)spec).SerializedStateValues = new List<string> { "", "", "", "", "" };
            return aiactor;
        }

        public static void AddEnemyToDatabase(GameObject aiactorPrefab, bool isInBossTab = false, bool isNormalEnemy = true, bool addToConsole = true, bool addToAmmonomicon = true, string guid = null)
        {
            guid ??= aiactorPrefab?.GetComponentInChildren<AIActor>()?.EnemyGuid;
            AdvancedEnemyDatabaseEntry item = new AdvancedEnemyDatabaseEntry
            {
                myGuid = guid,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = isNormalEnemy,
                path = "assets/enemies/" + aiactorPrefab.name.ToLower() + ".prefab",
                isInBossTab = isInBossTab,
                encounterGuid = guid
            };
            EnemyDatabase.Instance.Entries.Add(item);
            if (addToAmmonomicon)
            {
                EncounterDatabaseEntry encounterDatabaseEntry = new EncounterDatabaseEntry(aiactorPrefab.GetComponent<AIActor>().encounterTrackable)
                {
                    path = "assets/enemies/" + aiactorPrefab.name.ToLower() + ".prefab",
                    myGuid = guid
                };
                EncounterDatabase.Instance.Entries.Add(encounterDatabaseEntry);
            }
            if (addToConsole && !string.IsNullOrEmpty(aiactorPrefab.GetComponent<AIActor>().ActorName))
            {
                string EnemyName = SpecialStuffModule.globalPrefix + ":" + aiactorPrefab.GetComponent<AIActor>().ActorName.Replace(" ", "_").Replace("(", "_").Replace(")", string.Empty).Replace("-", "_").ToLower();
                if (!Game.Enemies.ContainsID(EnemyName)) { Game.Enemies.Add(EnemyName, aiactorPrefab.GetComponent<AIActor>()); }
            }
        }

        public static int AddSpriteToCollection(string spriteName, tk2dSpriteCollectionData collection)
        {
            int result = SpriteBuilder.AddSpriteToCollection(spriteName, collection, "Brave/LitCutoutUber");
            tk2dSpriteDefinition def = collection.spriteDefinitions[result];
            List<Material> materials = new List<Material> { def.material, def.materialInst };
            foreach (Material mat in materials)
            {
                mat.EnableKeyword("TINTING_ON");
                mat.DisableKeyword("TINTING_OFF");
                mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
            }
            return result;
        }

        public static tk2dSpriteAnimationFrame[] ToFrames(this IEnumerable<string> names, tk2dSpriteCollectionData collection)
        {
            return names.Select(x => new tk2dSpriteAnimationFrame() { spriteId = AddSpriteToCollection(x, collection), spriteCollection = collection }).ToArray();
        }

        public static void SetupAk47VeteranPrefab()
        {
            AIActor aiactor = SetupAIActorDummyFromAssetBundle("ak47veteran", new IntVector2(0, 0), new IntVector2(0, 0));
            aiactor.ActorName = "Ak47 Veteran";
            aiactor.EnemyGuid = "recursive_bullet_man";
            AIActor bulletkin = EnemyDatabase.GetOrLoadByGuid("70216cae6c1346309d86d4a0b4603045");
            AIActor ak47bulletkin = EnemyDatabase.GetOrLoadByGuid("db35531e66ce41cbb81d507a34366dfe");
            aiactor.healthHaver.SetHealthMaximum(15, null, true);
            aiactor.specRigidbody.PixelColliders = bulletkin.specRigidbody.PixelColliders.ToList();
            aiactor.ActorShadowOffset = new Vector3(bulletkin.ActorShadowOffset.x, bulletkin.ActorShadowOffset.y, bulletkin.ActorShadowOffset.z);
            aiactor.CorpseObject = bulletkin.CorpseObject;
            aiactor.optionalPalette = bulletkin.optionalPalette;
            aiactor.sprite.SetSprite(bulletkin.sprite.Collection, bulletkin.sprite.spriteId);
            tk2dSpriteAnimator animator = aiactor.gameObject.AddComponent<tk2dSpriteAnimator>();
            animator.Library = bulletkin.spriteAnimator.Library;
            AIAnimator aianimator = aiactor.gameObject.AddComponent<AIAnimator>();
            aianimator.IdleAnimation = bulletkin.aiAnimator.IdleAnimation;
            aianimator.MoveAnimation = bulletkin.aiAnimator.MoveAnimation;
            aianimator.FlightAnimation = bulletkin.aiAnimator.FlightAnimation;
            aianimator.OtherAnimations = bulletkin.aiAnimator.OtherAnimations.ToList();
            aianimator.spriteAnimator = animator;
            aiactor.aiAnimator = aianimator;
            AIBulletBank bb = aiactor.gameObject.AddComponent<AIBulletBank>();
            bb.Bullets = bulletkin.bulletBank.Bullets.ToList();
            AIShooter aishoot = aiactor.gameObject.AddComponent<AIShooter>();
            aishoot.volley = aiactor.aiShooter.volley;
            aishoot.equippedGunId = 15;
            aishoot.shouldUseGunReload = aiactor.aiShooter.shouldUseGunReload;
            aishoot.volleyShootPosition = null;
            aishoot.volleyShellCasing = null;
            aishoot.volleyShellTransform = null;
            aishoot.volleyShootVfx = null;
            aishoot.usesOctantShootVFX = aiactor.aiShooter.usesOctantShootVFX;
            aishoot.bulletName = aiactor.aiShooter.bulletName;
            aishoot.customShootCooldownPeriod = aiactor.aiShooter.customShootCooldownPeriod;
            aishoot.doesScreenShake = aiactor.aiShooter.doesScreenShake;
            aishoot.rampBullets = aiactor.aiShooter.rampBullets;
            aishoot.rampStartHeight = aiactor.aiShooter.rampStartHeight;
            aishoot.rampTime = aiactor.aiShooter.rampTime;
            aishoot.gunAttachPoint = aiactor.transform.Find("ShootPoint");
            aishoot.gunAttachPoint.localPosition = bulletkin.aiShooter.gunAttachPoint.localPosition;
            aishoot.bulletScriptAttachPoint = null;
            aishoot.overallGunAttachOffset = bulletkin.aiShooter.overallGunAttachOffset;
            aishoot.flippedGunAttachOffset = bulletkin.aiShooter.flippedGunAttachOffset;
            aishoot.handObject = bulletkin.aiShooter.handObject;
            aishoot.AllowTwoHands = true;
            aishoot.ForceGunOnTop = false;
            aishoot.IsReallyBigBoy = false;
            aishoot.BackupAimInMoveDirection = false;
            aishoot.PostProcessProjectile = null;
            KnockbackDoer kd = aiactor.gameObject.GetComponent<KnockbackDoer>();
            kd.weight = bulletkin.knockbackDoer.weight;
            kd.deathMultiplier = bulletkin.knockbackDoer.deathMultiplier;
            kd.knockbackWhileReflecting = bulletkin.knockbackDoer.knockbackWhileReflecting;
            kd.shouldBounce = bulletkin.knockbackDoer.shouldBounce;
            kd.collisionDecay = bulletkin.knockbackDoer.collisionDecay;
            aiactor.MovementSpeed = bulletkin.aiActor.MovementSpeed;
            BehaviorSpeculator spec = aiactor.behaviorSpeculator;
            spec.OverrideBehaviors = bulletkin.behaviorSpeculator.OverrideBehaviors.ToList();
            spec.OtherBehaviors = bulletkin.behaviorSpeculator.OtherBehaviors.ToList();
            spec.TargetBehaviors = bulletkin.behaviorSpeculator.TargetBehaviors.ToList();
            ShootGunBehavior behav = (ShootGunBehavior)ak47bulletkin.behaviorSpeculator.AttackBehaviors.Find((AttackBehaviorBase at) => at is ShootGunBehavior);
            ShootGunBehavior behavlead = (ShootGunBehavior)bulletkin.behaviorSpeculator.AttackBehaviors.Find((AttackBehaviorBase at) => at is ShootGunBehavior);
            spec.AttackBehaviors = new List<AttackBehaviorBase>
            {
                new ShootGunBehavior
                {
                    AccumulateHealthThresholds = behav.AccumulateHealthThresholds,
                    AimAtFacingDirectionWhenSafe = behav.AimAtFacingDirectionWhenSafe,
                    FixTargetDuringAttack = behav.FixTargetDuringAttack,
                    MinWallDistance = behav.MinWallDistance,
                    OverrideDirectionalAnimation = behav.OverrideDirectionalAnimation,
                    resetCooldownOnDamage = behav.resetCooldownOnDamage,
                    StopDuringAttack = behav.StopDuringAttack,
                    AttackCooldown = behav.AttackCooldown,
                    BulletScript = behav.BulletScript,
                    Cooldown = behav.Cooldown,
                    CooldownVariance = behav.CooldownVariance,
                    EmptiesClip = behav.EmptiesClip,
                    GlobalCooldown = behav.GlobalCooldown,
                    GroupCooldown = behav.GroupCooldown,
                    GroupCooldownVariance = behav.GroupCooldownVariance,
                    GroupName = behav.GroupName,
                    HealthThresholds = behav.HealthThresholds,
                    HideGun = behav.HideGun,
                    InitialCooldown = behav.InitialCooldown,
                    InitialCooldownVariance = behav.InitialCooldownVariance,
                    IsBlackPhantom = behav.IsBlackPhantom,
                    LeadAmount = behavlead.LeadAmount,
                    LeadChance = behavlead.LeadChance,
                    LineOfSight = behav.LineOfSight,
                    MagazineCapacity = behav.MagazineCapacity,
                    MaxEnemiesInRoom = behav.MaxEnemiesInRoom,
                    MaxHealthThreshold = behav.MaxHealthThreshold,
                    MaxUsages = behav.MaxUsages,
                    MinHealthThreshold = behav.MinHealthThreshold,
                    MinRange = behav.MinRange,
                    OverrideAnimation = behav.OverrideAnimation,
                    OverrideBulletName = behavlead.OverrideBulletName,
                    PreFireLaserTime = behav.PreFireLaserTime,
                    PreventTargetSwitching = behav.PreventTargetSwitching,
                    Range = behav.Range,
                    ReloadSpeed = behav.ReloadSpeed,
                    RequiresLineOfSight = behav.RequiresLineOfSight,
                    RespectReload = behav.RespectReload,
                    SuppressReloadAnim = behav.SuppressReloadAnim,
                    targetAreaStyle = behav.targetAreaStyle,
                    TimeBetweenShots = behav.TimeBetweenShots,
                    UseGreenLaser = behav.UseGreenLaser,
                    UseLaserSight = behav.UseLaserSight,
                    WeaponType = behav.WeaponType
                }
            };
            spec.MovementBehaviors = bulletkin.behaviorSpeculator.MovementBehaviors.ToList();
            spec.InstantFirstTick = bulletkin.behaviorSpeculator.InstantFirstTick;
            spec.TickInterval = bulletkin.behaviorSpeculator.TickInterval;
            spec.PostAwakenDelay = bulletkin.behaviorSpeculator.PostAwakenDelay;
            spec.RemoveDelayOnReinforce = bulletkin.behaviorSpeculator.RemoveDelayOnReinforce;
            spec.OverrideStartingFacingDirection = bulletkin.behaviorSpeculator.OverrideStartingFacingDirection;
            spec.SkipTimingDifferentiator = bulletkin.behaviorSpeculator.SkipTimingDifferentiator;
            AddEnemyToDatabase(aiactor.gameObject, false, true, true, false);
        }

        public static void AddNinja()
        {
            AIActor aiactor = SetupAIActorDummyFromAssetBundle("ninja", new IntVector2(2, 2), new IntVector2(12, 15));
            GameObject coll = AssetBundleManager.Load<GameObject>("spritecollections/ninjacollection");
            GameObject anim = AssetBundleManager.Load<GameObject>("spriteanimations/ninjaanimation");
            tk2dSpriteCollectionData collection = coll.AddComponent<tk2dSpriteCollectionData>();
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            collection.spriteCollectionName = "NinjaCollection";
            collection.assetName = "NinjaCollection";
            aiactor.ActorName = "Ninja";
            aiactor.EnemyGuid = "ninja";
            aiactor.MovementSpeed = 6f;
            aiactor.AddComponent<PoofOnDeath>();
            //aiactor.ActorShadowOffset = new(0.5f, 0f, 0f);
            aiactor.healthHaver.SetHealthMaximum(30, null, true);
            aiactor.sprite.SetSprite(collection, AddSpriteToCollection("enemysprites/ninja/ninja_idle_front_hand1", collection));
            tk2dSpriteAnimator animator = aiactor.gameObject.AddComponent<tk2dSpriteAnimator>();
            animator.playAutomatically = true;
            animator.Library = anim.AddComponent<tk2dSpriteAnimation>();
            int idlefps = 10;
            int runfps = 10;
            animator.Library.clips = new tk2dSpriteAnimationClip[]
            {
                new()
                {
                    name = "ninja_idle_front",
                    fps = idlefps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_idle_front_hand1",
                        "enemysprites/ninja/ninja_idle_front_hand2",
                        "enemysprites/ninja/ninja_idle_front_hand3",
                        "enemysprites/ninja/ninja_idle_front_hand4"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_idle_back",
                    fps = idlefps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_idle_back_001",
                        "enemysprites/ninja/ninja_idle_back_002",
                        "enemysprites/ninja/ninja_idle_back_003",
                        "enemysprites/ninja/ninja_idle_back_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_idle_left",
                    fps = idlefps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_idle_left_001",
                        "enemysprites/ninja/ninja_idle_left_002",
                        "enemysprites/ninja/ninja_idle_left_003",
                        "enemysprites/ninja/ninja_idle_left_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_idle_right",
                    fps = idlefps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_idle_right_001",
                        "enemysprites/ninja/ninja_idle_right_002",
                        "enemysprites/ninja/ninja_idle_right_003",
                        "enemysprites/ninja/ninja_idle_right_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_run_front",
                    fps = runfps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_run_front_001",
                        "enemysprites/ninja/ninja_run_front_002",
                        "enemysprites/ninja/ninja_run_front_003",
                        "enemysprites/ninja/ninja_run_front_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_run_back",
                    fps = runfps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_run_back_001",
                        "enemysprites/ninja/ninja_run_back_002",
                        "enemysprites/ninja/ninja_run_back_003",
                        "enemysprites/ninja/ninja_run_back_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_run_left",
                    fps = runfps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_run_left_001",
                        "enemysprites/ninja/ninja_run_left_002",
                        "enemysprites/ninja/ninja_run_left_003",
                        "enemysprites/ninja/ninja_run_left_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "ninja_run_right",
                    fps = runfps,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_run_right_001",
                        "enemysprites/ninja/ninja_run_right_002",
                        "enemysprites/ninja/ninja_run_right_003",
                        "enemysprites/ninja/ninja_run_right_004"
                    }.ToFrames(collection)
                },
                new()
                {
                    name = "spawn",
                    fps = 12,
                    frames = new List<string>()
                    {
                        "enemysprites/ninja/ninja_spawn_001",
                        "enemysprites/ninja/ninja_spawn_002",
                        "enemysprites/ninja/ninja_spawn_003",
                        "enemysprites/ninja/ninja_spawn_004",
                        "enemysprites/ninja/ninja_spawn_005",
                        "enemysprites/ninja/ninja_spawn_006",
                        "enemysprites/ninja/ninja_spawn_007",
                        "enemysprites/ninja/ninja_spawn_008",
                        "enemysprites/ninja/ninja_spawn_009",
                        "enemysprites/ninja/ninja_spawn_010",
                        "enemysprites/ninja/ninja_spawn_011"
                    }.ToFrames(collection)
                }
            };
            AIAnimator aianimator = aiactor.gameObject.AddComponent<AIAnimator>();
            aianimator.IdleAnimation = new()
            {
                Type = DirectionalAnimation.DirectionType.FourWayCardinal,
                AnimNames = new string[]
                {
                    "ninja_idle_back",
                    "ninja_idle_right",
                    "ninja_idle_front",
                    "ninja_idle_left"
                },
                Prefix = string.Empty,
                Flipped = new DirectionalAnimation.FlipType[4]
            };
            aianimator.MoveAnimation = new()
            {
                Type = DirectionalAnimation.DirectionType.FourWayCardinal,
                AnimNames = new string[]
                {
                    "ninja_run_back",
                    "ninja_run_right",
                    "ninja_run_front",
                    "ninja_run_left"
                },
                Prefix = string.Empty,
                Flipped = new DirectionalAnimation.FlipType[4]
            };
            aianimator.FlightAnimation = new() { AnimNames = new string[0], Type = DirectionalAnimation.DirectionType.None, Flipped = new DirectionalAnimation.FlipType[0] };
            aianimator.OtherAnimations = new();
            aianimator.spriteAnimator = animator;
            aiactor.aiAnimator = aianimator;
            var behav = aiactor.behaviorSpeculator;
            behav.AttackBehaviors = new()
            {
                new AttackBehaviorGroup()
                {
                    AttackBehaviors = new()
                    {
                        new()
                        {
                            NickName = "Teleport",
                            Behavior = new NinjaTeleportBehavior()
                            {
                                GoneTime = 0.25f,
                                PreTeleportTime = 0.5f,
                                Cooldown = 3f,
                                BulletScript = new CustomBulletScriptSelector(typeof(NinjaPostTeleport)),
                                ShootPoint = aiactor.transform.Find("ShootPoint").gameObject
                            },
                            Probability = 1f
                        }
                    },
                    ShareCooldowns = true
                }
            };
            behav.MovementBehaviors = new()
            {
                new SeekTargetBehavior()
                {
                    LineOfSight = false,
                    CustomRange = 0f,
                    ExternalCooldownSource = false,
                    MaxActiveRange = 25f,
                    MinActiveRange = 0f,
                    PathInterval = 0.2f,
                    ReturnToSpawn = false,
                    StopWhenInRange = false,
                    SpecifyRange = false
                }
            };
            behav.TargetBehaviors = new()
            {
                new TargetPlayerBehavior()
                {
                    LineOfSight = false,
                    SearchInterval = 0.2f,
                    PauseOnTargetSwitch = false,
                    PauseTime = 0f,
                    Radius = 25f
                }
            };
            behav.OverrideBehaviors = new();
            behav.OtherBehaviors = new();
            behav.PostAwakenDelay = 0.5f;
            AIBulletBank bb = aiactor.gameObject.AddComponent<AIBulletBank>();
            bb.Bullets = new()
            {
                EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet()
            };
            AddEnemyToDatabase(aiactor.gameObject, addToAmmonomicon: false);
        }

        public static GameObject LockGhostPrefab;
        public static GameObject FriendlyLockGhostPrefab;
    }
}
