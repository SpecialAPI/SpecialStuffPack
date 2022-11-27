using SpecialStuffPack.CursorAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    [HarmonyPatch]
    public static class CharacterBuilder
    {
        /// <summary>
        /// "collider" default x:5 y:0 width:14 height:4
        /// "hitbox" default x:7 y:5 width:8 height:10
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="consoleName"></param>
        /// <param name="braveAssetsName"></param>
        /// <param name="collectionName"></param>
        /// <param name="animationName"></param>
        /// <param name="spriteContainer"></param>
        /// <param name="identity"></param>
        /// <param name="collider">default x:5 y:0 width:14 height:4</param>
        /// <param name="hitbox">default x:7 y:5 width:8 height:10</param>
        /// <param name="portraitSprite"></param>
        /// <param name="bosscardSpriteContainer"></param>
        /// <param name="bosscardFps"></param>
        /// <param name="minimapPrefabName"></param>
        /// <param name="minimapSpriteName"></param>
        /// <param name="statsName"></param>
        /// <param name="defaultStats"></param>
        /// <param name="baseBlanksPerFloor"></param>
        /// <param name="maxItemsHeld"></param>
        /// <param name="startingItems"></param>
        /// <param name="altGuns"></param>
        /// <param name="startingMoney"></param>
        /// <param name="startingKeys"></param>
        /// <param name="armorInsteadOfHealth"></param>
        /// <param name="animationPrefix"></param>
        /// <param name="primaryHandSprite"></param>
        /// <param name="secondaryHandSprite"></param>
        /// <param name="animationsHaveHands"></param>
        /// <param name="armorlessAnimations"></param>
        /// <param name="voice"></param>
        /// <returns></returns>
        public static T BuildCharacter<T>(string assetName, string consoleName, string collectionName, string animationName, string spriteContainer, PlayableCharacters identity,
            IntRect collider, IntRect hitbox, string portraitSprite,
            string bosscardSpriteContainer, int bosscardFps, string minimapPrefabName, string minimapSpriteName, string statsName, Dictionary<PlayerStats.StatType, float> defaultStats, float startingHealth,
            int startingArmor, int baseBlanksPerFloor,
            int maxItemsHeld, List<int> startingItems, List<int> altGuns, int startingMoney, int startingKeys, bool armorInsteadOfHealth, string animationPrefix, string primaryHandSprite, string secondaryHandSprite, 
            bool animationsHaveHands, bool armorlessAnimations, string voice) where T : PlayerController
        {
            if (!spriteContainer.StartsWith("assets/"))
            {
                spriteContainer = "assets/" + spriteContainer;
            }
            if (!bosscardSpriteContainer.StartsWith("assets/"))
            {
                bosscardSpriteContainer = "assets/" + bosscardSpriteContainer;
            }
            var gameobject = AssetBundleManager.Load<GameObject>(assetName, null, null);
            if(gameobject == null)
            {
                return null;
            }
            var player = gameobject.AddComponent<T>();
            player.characterIdentity = identity;
            var uiPortraitTexture = AssetBundleManager.Load<Texture2D>(portraitSprite, null, null);
            player.uiPortraitName = AddToAtlas(uiPortraitTexture);
            player.BosscardSprites = new();
            foreach (string asset in AssetBundleManager.specialeverything.GetAllAssetNames())
            {
                if (asset.StartsWith(bosscardSpriteContainer) && asset.EndsWith(".png"))
                {
                    player.BosscardSprites.Add(AssetBundleManager.Load<Texture2D>(asset, null, null));
                }
            }
            player.BosscardSpriteFPS = bosscardFps;
            player.stats = AssetBundleManager.Load<GameObject>(statsName, null, null).GetOrAddComponent<PlayerStats>();
            if(defaultStats != null)
            {
                var highest = Mathf.Max(defaultStats.Keys.Select(x => (int)x).Max(), (int)PlayerStats.StatType.MoneyMultiplierFromEnemies);
                player.stats.BaseStatValues = new();
                for(int i = 0; i < highest + 1; i++)
                {
                    if(defaultStats.TryGetValue((PlayerStats.StatType)i, out var givenValue))
                    {
                        player.stats.BaseStatValues.Add(givenValue);
                    }
                    else
                    {
                        if(i <= (int)PlayerStats.StatType.MoneyMultiplierFromEnemies)
                        {
                            var value = 1f;
                            var statType = (PlayerStats.StatType)i;
                            if(statType is PlayerStats.StatType.MovementSpeed)
                            {
                                value = 7f;
                            }
                            else if(statType is PlayerStats.StatType.Health)
                            {
                                value = 3f;
                            }
                            else if(statType is PlayerStats.StatType.Coolness or PlayerStats.StatType.Curse or PlayerStats.StatType.AdditionalGunCapacity or PlayerStats.StatType.AdditionalItemCapacity
                                or PlayerStats.StatType.AdditionalShotPiercing or PlayerStats.StatType.AdditionalShotBounces or PlayerStats.StatType.AdditionalBlanksPerFloor or PlayerStats.StatType.ShadowBulletChance or 
                                PlayerStats.StatType.ExtremeShadowBulletChance)
                            {
                                value = 0f;
                            }
                            player.stats.BaseStatValues.Add(value);
                        }
                        else
                        {
                            player.stats.BaseStatValues.Add(-1f);
                        }
                    }
                }
            }
            player.stats.NumBlanksPerFloor = player.stats.NumBlanksPerFloorCoop = baseBlanksPerFloor;
            player.rollStats = new()
            {
                hasPreDodgeDelay = false,
                preDodgeDelay = 0f,
                time = 0.65f,
                distance = 5f,
                speed = new()
                {
                    keys = new Keyframe[]
                    {
                        new()
                        {
                            time = 0f,
                            value = 0.0032f,
                            inTangent = 1.249f,
                            outTangent = 1.3945f,
                            tangentMode = 0
                        },
                        new()
                        {
                            time = 0.4736f,
                            value = 0.6637f,
                            inTangent = 1.3945f,
                            outTangent = 1.2491f,
                            tangentMode = 0
                        },
                        new()
                        {
                            time = 0.6811f,
                            value = 0.8201f,
                            inTangent = 0.7533f,
                            outTangent = 0.5642f,
                            tangentMode = 0
                        },
                        new()
                        {
                            time = 1f,
                            value = 1f,
                            inTangent = 0.5642f,
                            outTangent = 0.7021f,
                            tangentMode = 0
                        }
                    },
                    postWrapMode = WrapMode.ClampForever,
                    preWrapMode = WrapMode.ClampForever
                }
            };
            player.pitHelpers = new()
            {
                Landing = new(7, 10),
                PreJump = new(3, 6)
            };
            player.MAX_GUNS_HELD = 8;
            player.MAX_ITEMS_HELD = maxItemsHeld;
            player.startingGunIds = startingItems.FindAll(x => GetItem(x) is Gun);
            player.startingActiveItemIds = startingItems.FindAll(x => GetItem(x) is PlayerItem);
            player.startingPassiveItemIds = startingItems.FindAll(x => GetItem(x) is PassiveItem);
            player.startingAlternateGunIds = altGuns;
            player.carriedConsumables = new()
            {
                StartCurrency = startingMoney,
                StartKeyBullets = startingKeys
            };
            player.randomStartingEquipmentSettings = new();
            player.AllowZeroHealthState = false;
            player.ForceZeroHealthState = armorInsteadOfHealth;
            player.gunAttachPoint = gameobject.transform.Find("GunAttachPoint");
            player.downwardAttachPointPosition = new(0.6875f, 0.75f, 0f);
            player.collisionKnockbackStrength = 10f;
            player.unadjustedAimPoint = new();
            player.outlineColor = Color.black;
            player.hasArmorlessAnimations = armorlessAnimations;
            player.animationAudioEvents = new(); // who needs these when you have ones built into the animator
            player.characterAudioSpeechTag = voice;
            player.usingForcedInput = false;
            player.forcedInput = Vector2.zero;
            player.forceAimPoint = null;
            player.forceFire = false;
            player.forceFireDown = false;
            player.forceFireUp = false;
            player.DrawAutoAim = false; //what even is this
            player.ForceMetalGearMenu = false;
            player.InfiniteAmmo = new(false);
            player.OnlyFinalProjectiles = new(false);
            player.IsTalking = false;
            player.TalkPartner = null;
            player.ownerlessStatModifiers = new();
            player.ForceHandless = !animationsHaveHands;
            player.lostAllArmorVFX = null; // todo: make this configurable
            player.lostAllArmorAltVfx = null; // todo:make this configurable
            player.CustomDodgeRollEffect = null; //todo: make this configurable
            player.knockbackComponent = new();
            player.immutableKnockbackComponent = new();
            player.ImmuneToPits = new(false);
            player.RealtimeEnteredCurrentRoom = 0f;
            player.previousMineCart = null;
            player.PlayerIDX = -1;
            player.FlatColorOverridden = false;
            player.IsCurrentlyCoopReviving = false;
            player.AdditionalCanDodgeRollWhileFlying = new(false);
            player.TablesDamagedThisSlide = new();
            player.gunReloadDisplayTimer = 0f;
            player.m_pettingTarget = null;
            player.minimapIconPrefab = AssetBundleManager.Load<GameObject>(minimapPrefabName);

            //setup rigidbody
            var rigidbody = gameobject.AddComponent<SpeculativeRigidbody>();
            rigidbody.PixelColliders = new()
            {
                new()
                {
                    CollisionLayer = CollisionLayer.PlayerCollider,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    ManualOffsetX = collider.Left,
                    ManualOffsetY = collider.Bottom,
                    ManualWidth = collider.Width,
                    ManualHeight = collider.Height
                },
                new()
                {
                    CollisionLayer = CollisionLayer.PlayerHitBox,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    ManualOffsetX = hitbox.Left,
                    ManualOffsetY = hitbox.Bottom,
                    ManualWidth = hitbox.Width,
                    ManualHeight = hitbox.Height
                }
            };
            rigidbody.CanPush = true;
            rigidbody.ForceAlwaysUpdate = true;
            rigidbody.PushSpeedModifier = 0.4f;

            //setup healthhaver
            var hh = gameobject.AddComponent<HealthHaver>();
            hh.quantizeHealth = true;
            hh.quantizedIncrement = 0.5f;
            hh.flashesOnDamage = true;
            hh.incorporealityOnDamage = true;
            hh.incorporealityTime = 1.4f;
            hh.currentHealth = Mathf.Min(hh.maximumHealth = player.stats.BaseStatValues[(int)PlayerStats.StatType.Health], startingHealth);
            hh.currentArmor = startingArmor;
            hh.usesInvulnerabilityPeriod = true;
            hh.invulnerabilityPeriod = 1f;
            hh.shakesCameraOnDamage = true;
            hh.cameraShakeOnDamage = new()
            {
                magnitude = 0.7f,
                speed = 8f,
                time = 0.15f,
                falloff = 0.1f,
                direction = Vector2.zero
            };
            hh.damageTypeModifiers = new();
            hh.healthIsNumberOfHits = false;
            hh.OnlyAllowSpecialBossDamage = false;
            hh.spawnBulletScript = false;
            hh.chanceToSpawnBulletScript = 0f;
            hh.overrideDeathAnimBulletScript = "";
            hh.noCorpseWhenBulletScriptDeath = false;
            hh.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
            hh.bulletScript = null;
            hh.bossHealthBar = HealthHaver.BossBarType.None;
            hh.overrideBossName = "";
            hh.forcePreventVictoryMusic = false;
            hh.GlobalPixelColliderDamageMultiplier = 1f;

            //setup knockback
            var kb = gameobject.AddComponent<KnockbackDoer>();
            kb.weight = 50f;
            kb.deathMultiplier = 5f;
            kb.knockbackWhileReflecting = false;
            kb.shouldBounce = true;
            kb.collisionDecay = 0.5f;

            //setup collection
            var collection = EasyCollectionSetup(collectionName);
            foreach (string asset in AssetBundleManager.specialeverything.GetAllAssetNames())
            {
                if (asset.StartsWith(spriteContainer) && !asset.StartsWith(bosscardSpriteContainer) && asset.EndsWith(".png"))
                {
                    int id = SpriteBuilder.AddSpriteToCollection(asset, collection, PlayerController.DefaultShaderName);
                    if (AssetBundleManager.specialeverything.Contains(asset.Replace(".png", ".json")))
                    {
                        AssetSpriteData assetSpriteData = default;
                        using (MemoryStream s = new(Encoding.UTF8.GetBytes(AssetBundleManager.Load<TextAsset>(asset.Replace(".png", ".json")).text)))
                        {
                            assetSpriteData = JSONHelper.ReadJSON<AssetSpriteData>(s);
                        }
                        collection.SetAttachPoints(id, assetSpriteData.attachPoints);
                    }
                }
            }

            tk2dSprite.AddComponent(player.minimapIconPrefab, collection, SpriteBuilder.AddSpriteToCollection(minimapSpriteName, collection, PlayerController.DefaultShaderName));

            //setup animation
            var animation = EasyAnimationSetup(animationName);
            List<string> animationsWithHands = new()
            {
                "idle_hand",
                "idle_backward_hand",
                "idle_bw_hand",
                "idle_forward_hand",
                "run_right_hand",
                "run_down_hand",
                "run_up_hand",
                "tablekick_down_hand",
                "tablekick_right_hand",
                "tablekick_up_hand",
                "jetpack_down_hand",
                "jetpack_right_hand",
                "idle_twohands",
                "idle_backward_twohands",
                "idle_bw_twohands",
                "idle_forward_twohands",
                "run_down_twohands",
                "run_right_twohands",
                "run_up_twohands",
                "run_right_bw_twohands",
                "tablekick_down_twohands",
                "tablekick_right_twohands",
                "tablekick_up_twohands",
            };
            List<string> animations = new()
            {
                "dodge",
                "dodge_bw",
                "dodge_left",
                "dodge_left_bw",
                "idle",
                "idle_bw",
                "idle_backward",
                "idle_forward",
                "item_get",
                "run_right",
                "run_right_bw",
                "run_down",
                "run_up",
                "tablekick_down",
                "tablekick_right",
                "tablekick_up",
                "pitfall",
                "pitfall_down",
                "pitfall_return",
                "select_idle",
                "select_choose",
                "death",
                "death_shot",
                "jetpack_down",
                "jetpack_up",
                "jetpack_right",
                "jetpack_right_bw",
                "doorway",
                "spinfall",
                "chest_recover",
                "death_coop",
                "timefall",
                "ghost_idle_right",
                "ghost_idle_left",
                "ghost_idle_front",
                "ghost_idle_back",
                "ghost_idle_back_right",
                "ghost_idle_back_left",
                "ghost_sneeze_right",
                "ghost_sneeze_left",
                "slide_right",
                "slide_up",
                "slide_down",
                "spit_out",
                "pet",
            };
            Dictionary<string, string[]> altAnimationNames = new()
            {
                { "run_down",                   new string[] { "run_front" } },
                { "run_up",                     new string[] { "run_back" } },
                { "run_right_bw",               new string[] { "run_back_right" } },
                { "run_right",                  new string[] { "run_right_front" } },
                { "idle",                       new string[] { "idle_right_front" } },
                { "idle_bw",                    new string[] { "idle_back_right" } },
                { "idle_backward",              new string[] { "idle_back" } },
                { "idle_forward",               new string[] { "idle_front" } },
                { "dodge",                      new string[] { "dodge_front" } },
                { "dodge_bw",                   new string[] { "dodge_back" } },
                { "dodge_left",                 new string[] { "dodge_front_right" } },
                { "dodge_left_bw",              new string[] { "dodge_back_right" } },
                { "spinfall",                   new string[] { "spin" } },
                { "tablekick_right",            new string[] { "tablekick_front_right" } },
                { "tablekick_up",               new string[] { "tablekick_back_right" } },
                { "doorway",                    new string[] { "run_back_doorway", "run_up_doorway", "pitfall" } },

                //i love hands
                { "run_down_twohands",          new string[] { "run_front_twohands",                "run_front_hands",              "run_front_hands2",             "run_front_hand",               "run_front" } },
                { "run_up_twohands",            new string[] { "run_back_twohands",                 "run_back_hands",               "run_back_hands2",              "run_back_hand",                "run_back" } },
                { "run_right_bw_twohands",      new string[] { "run_back_right_twohands",           "run_back_right_hands",         "run_back_right_hands2",        "run_back_right_hand",          "run_back_right" } },
                { "run_right_twohands",         new string[] { "run_right_front_twohands",          "run_right_front_hands",        "run_right_front_hands2",       "run_right_front_hand",         "run_right_front" } },
                { "idle_twohands",              new string[] { "idle_right_front_twohands",         "idle_right_front_hands",       "idle_right_front_hands2",      "idle_right_front_hand",        "idle_right_front" } },
                { "idle_bw_twohands",           new string[] { "idle_back_right_twohands",          "idle_back_right_hands",        "idle_back_right_hands2",       "idle_back_right_hand",         "idle_back_right" } },
                { "idle_backward_twohands",     new string[] { "idle_back_twohands",                "idle_back_hands",              "idle_back_hands2",             "idle_back_hand",               "idle_back" } },
                { "idle_forward_twohands",      new string[] { "idle_front_twohands",               "idle_front_hands",             "idle_front_hands2",            "idle_front_hand",              "idle_front" } },
                { "tablekick_right_twohands",   new string[] { "tablekick_front_right_twohands",    "tablekick_front_right_hands",  "tablekick_front_right_hands2", "tablekick_front_right_hand",   "tablekick_front_right" } },
                { "tablekick_up_twohands",      new string[] { "tablekick_back_right_twohands",     "tablekick_back_right_hands",   "tablekick_back_right_hands2",  "tablekick_back_right_hand",    "tablekick_back_right" } },
                { "tablekick_down_twohands",    new string[] {                                                                                                      "tablekick_down_hand",          "tablekick_down" } },
                { "jetpack_down_twohands",      new string[] {                                                                                                      "jetpack_down_hand",            "jetpack_down" } },
                { "jetpack_right_twohands",     new string[] {                                                                                                      "jetpack_right_hand",           "jetpack_right" } },
                { "jetpack_up_twohands",        new string[] {                                                                                                      "jetpack_up_hand",              "jetpack_up" } },

                //hands <3
                { "run_down_hand",              new string[] { "run_front_hand",                    "run_front" } },
                { "run_up_hand",                new string[] { "run_back_hand",                     "run_back" } },
                { "run_right_bw_hand",          new string[] { "run_back_right_hand",               "run_back_right" } },
                { "run_right_hand",             new string[] { "run_right_front_hand",              "run_right_front" } },
                { "idle_hand",                  new string[] { "idle_right_front_hand",             "idle_right_front" } },
                { "idle_bw_hand",               new string[] { "idle_back_right_hand",              "idle_back_right" } },
                { "idle_backward_hand",         new string[] { "idle_back_hand",                    "idle_back" } },
                { "idle_forward_hand",          new string[] { "idle_front_hand",                   "idle_front" } },
                { "tablekick_right_hand",       new string[] { "tablekick_front_right_hand",        "tablekick_front_right" } },
                { "tablekick_up_hand",          new string[] { "tablekick_back_right_hand",         "tablekick_back_right" } },
                { "tablekick_down_hand",        new string[] {                                      "tablekick_down" } },
                { "jetpack_down_hand",          new string[] {                                      "jetpack_down" } },
                { "jetpack_right_hand",         new string[] {                                      "jetpack_right" } },
                { "jetpack_up_hand",            new string[] {                                      "jetpack_up" } },
            };
            if (animationsHaveHands)
            {
                animations.AddRange(animationsWithHands);
            }
            if (armorlessAnimations)
            {
                animations.AddRange(animations.ConvertAll(x => x + "_armorless"));
            }
            foreach(var anim in animations)
            {
                var fullName = $"{animationPrefix}_{anim}";
                var ta = AssetBundleManager.Load<TextAsset>(fullName + ".txt");
                var fps = 8;
                if(ta != null)
                {
                    if(!int.TryParse(ta.text, out fps))
                    {
                        ETGModConsole.Log($"Unreadable animation fps for clip \"{fullName}\", text: \"{ta.text}\"");
                    }
                }
                altAnimationNames.TryGetValue(anim, out var optional);
                animation.AddClip(fullName, collection, fps, tk2dSpriteAnimationClip.WrapMode.Once, anim, optional != null && optional.Length > 0 ? optional.Select(x => $"{animationPrefix}_{x}").ToArray() : null);
            }
            string[] dodgeanims = new string[]
            {
                "dodge",
                "dodge_bw",
                "dodge_left",
                "dodge_left_bw",
            };
            foreach(var dodgeanim in dodgeanims)
            {
                var clippy = animation.GetClipByName(dodgeanim);
                if(clippy != null && clippy.frames != null && clippy.frames.Length > 0)
                {
                    var middleFrame = Mathf.FloorToInt((clippy.frames.Length + 1) / 2f) - 1;
                    for(int i = 0; i < clippy.frames.Length; i++)
                    {
                        var frame = clippy.frames[i];
                        if (i < middleFrame)
                        {
                            frame.groundedFrame = false;
                            frame.invulnerableFrame = true;
                        }
                        else if (i > middleFrame)
                        {
                            frame.groundedFrame = true;
                            frame.invulnerableFrame = false;
                        }
                        else
                        {
                            frame.groundedFrame = true;
                            frame.invulnerableFrame = true;
                        }
                    }
                }
            }
            string[] slideyAnimations = new string[]
            {
                "slide_right",
                "slide_up",
                "slide_down"
            };
            foreach(var slideyAnim in slideyAnimations)
            {
                var clippy = animation.GetClipByName(slideyAnim);
                if (clippy != null)
                {
                    clippy.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
                    if (clippy.frames != null && clippy.frames.Length > 0)
                    {
                        foreach(var frame in clippy.frames)
                        {
                            frame.invulnerableFrame = true;
                        }
                    }
                }
            }

            //player light
            var light = gameobject.transform.Find("PlayerLight").AddComponent<PlayerLightController>();
            light.resolution = 1000;
            light.maxDistance = 10f;
            light.distortionMax = 1f;
            light.shadowMaterial = LoadHelper.LoadAssetFromAnywhere<Material>("ShadowMaterial");

            //primary hand
            var hand1Obj = gameobject.transform.Find("PrimaryHand").gameObject;
            collection.GetSpriteDefinition(primaryHandSprite).ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
            tk2dSprite.AddComponent(hand1Obj, collection, primaryHandSprite);
            var hand1Controller = hand1Obj.AddComponent<PlayerHandController>();
            hand1Controller.ForceRenderersOff = false;
            hand1Controller.attachPoint = null;
            hand1Controller.handHeightFromGun = 0.05f;
            player.primaryHand = hand1Controller;

            //secondary hand
            var hand2Obj = gameobject.transform.Find("SecondaryHand").gameObject;
            if(secondaryHandSprite != primaryHandSprite)
            {
                collection.GetSpriteDefinition(secondaryHandSprite).ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
            }
            tk2dSprite.AddComponent(hand2Obj, collection, secondaryHandSprite);
            var hand2Controller = hand2Obj.AddComponent<PlayerHandController>();
            hand2Controller.ForceRenderersOff = false;
            hand2Controller.attachPoint = null;
            hand2Controller.handHeightFromGun = 0.05f;
            player.secondaryHand = hand2Controller;

            //setup alt collection and animation
            player.AlternateCostumeLibrary = animation; //TEMP
            player.altHandName = hand1Controller.sprite.CurrentSprite.name;
            player.HandsOnAltCostume = player.ForceHandless;
            player.SwapHandsOnAltCostume = false;

            //sprite
            var spriteObj = gameobject.transform.Find("PlayerSprite").gameObject;
            var sprite = tk2dSprite.AddComponent(spriteObj, collection, 
                collection.GetSpriteIdByName($"{animationPrefix}_idle_right_front_001", -1) >= 0 ? collection.GetSpriteIdByName($"{animationPrefix}_idle_right_front_001", -1) :
                collection.GetSpriteIdByName($"{animationPrefix}_idle_001", -1) >= 0 ? collection.GetSpriteIdByName($"{animationPrefix}_idle_001", -1) :
                1);
            var animator = spriteObj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animation;
            rigidbody.TK2DSprite = sprite;

            //coin
            var coinObj = gameobject.transform.Find("CoinSpot").Find("CollectedCoin").gameObject;
            var coinSprite = tk2dSprite.AddComponent(coinObj, ETGMod.Databases.Items.ItemCollection, 2);
            var coinAnimator = coinObj.AddComponent<tk2dSpriteAnimator>();
            coinAnimator.Library = LoadHelper.LoadAssetFromAnywhere<GameObject>("ItemAnimation").GetComponent<tk2dSpriteAnimation>();
            var blooper = coinObj.AddComponent<CoinBloop>();
            blooper.bloopWait = 0.5f;
            var coinanim = coinObj.AddComponent<Animation>();
            coinanim.clip = LoadHelper.LoadAssetFromAnywhere<AnimationClip>("CoinBloop");
            coinanim.playAutomatically = false;
            coinanim.wrapMode = WrapMode.Default;
            coinanim.animatePhysics = false;
            coinanim.cullingType = AnimationCullingType.AlwaysAnimate;
            coinanim.localBounds = new();
            coinanim.AddClip(coinanim.clip, "CoinBloop");

            var braveAssetName = gameobject.name;
            if (!braveAssetName.ToLowerInvariant().StartsWith("player"))
            {
                braveAssetName = $"Player{braveAssetName}";
            }
            var nonPlayerName = braveAssetName.Substring(braveAssetName.ToLowerInvariant().IndexOf("r") + 1);
            ETGModConsole.Characters.Add($"{SpecialStuffModule.globalPrefix}:{consoleName}", nonPlayerName);
            SpecialAssets.AddToBraveAssets($"{braveAssetName}.prefab", gameobject);
            return player;
        }

        public static FoyerCharacterSelectFlag AddToBreach(this PlayerController player, string assetPath, Vector3 breachPosition, List<DungeonPrerequisite> unlockPrereqs, IntRect obstacle, IntRect bulletblocker,
            IntVector2 additionalShadowOffset, string overheadElementPath, string breachName, string facecardAnimationPrefix, float facecardIdleFps, string itemsTexture, string description1Text = null, string description2Text = null,
            string overrideChangeCharacterString = null, string overrideYesResponse = null, string overrideNoResponse = null)
        {
            var go = AssetBundleManager.Load<GameObject>(assetPath, null, null);
            if (go == null)
            {
                return null;
            }
            var overheadgo = AssetBundleManager.Load<GameObject>(overheadElementPath, null, null);
            if (overheadgo == null)
            {
                return null;
            }
            var braveAssetName = player.name;
            if (!braveAssetName.ToLowerInvariant().StartsWith("player"))
            {
                braveAssetName = $"Player{braveAssetName}";
            }

            //setup sprite
            var sprite = tk2dSprite.AddComponent(go, player.transform.Find("PlayerSprite").GetComponent<tk2dBaseSprite>().Collection,
                player.transform.Find("PlayerSprite").GetComponent<tk2dSpriteAnimator>().GetClipByName("select_idle")?.frames?.FirstOrDefault()?.spriteId ?? player.transform.Find("PlayerSprite").GetComponent<tk2dBaseSprite>().spriteId);

            //setup animator
            var anim = go.AddComponent<tk2dSpriteAnimator>();
            anim.Library = player.transform.Find("PlayerSprite").GetComponent<tk2dSpriteAnimator>().Library;
            anim.defaultClipId = anim.GetClipIdByName("select_idle");
            anim.playAutomatically = anim.defaultClipId >= 0;

            //setup rigidbody
            var rigidbody = go.AddComponent<SpeculativeRigidbody>();
            rigidbody.PixelColliders = new()
            {
                new()
                {
                    CollisionLayer = CollisionLayer.LowObstacle,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    ManualOffsetX = obstacle.Left,
                    ManualOffsetY = obstacle.Bottom,
                    ManualWidth = obstacle.Width,
                    ManualHeight = obstacle.Height
                },
                new()
                {
                    CollisionLayer = CollisionLayer.BulletBlocker,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    ManualOffsetX = bulletblocker.Left,
                    ManualOffsetY = bulletblocker.Bottom,
                    ManualWidth = bulletblocker.Width,
                    ManualHeight = bulletblocker.Height
                }
            };
            rigidbody.TK2DSprite = sprite;

            //setup talkdoer
            var talkdoer = go.AddComponent<TalkDoerLite>();
            talkdoer.usesOverrideInteractionRegion = false;
            talkdoer.overrideRegionOffset = Vector2.zero;
            talkdoer.overrideRegionDimensions = new(10f, 10f);
            talkdoer.overrideInteractionRadius = 2f;
            talkdoer.PreventInteraction = false;
            talkdoer.AllowPlayerToPassEventually = true;
            talkdoer.speakPoint = go.transform.Find("TalkPoint");
            talkdoer.SpeaksGleepGlorpenese = false;
            talkdoer.audioCharacterSpeechTag = player.characterAudioSpeechTag;
            talkdoer.playerApproachRadius = 5f;
            talkdoer.conversationBreakRadius = 7f;
            talkdoer.echo1 = null;
            talkdoer.echo2 = null;
            talkdoer.PreventCoopInteraction = true;
            talkdoer.IsPaletteSwapped = false;
            talkdoer.PaletteTexture = null;
            talkdoer.teleportInSettings = talkdoer.teleportOutSettings = new() { anim = "", animDelay = 0f, timing = Teleport.Timing.Simultaneous, vfx = null, vfxAnchor = null, vfxDelay = 0f };
            talkdoer.itemsToLeaveBehind = new();
            talkdoer.shadow = go.transform.Find("DefaultShadowSprite").gameObject;
            talkdoer.DisableOnShortcutRun = false;
            talkdoer.OptionalMinimapIcon = null;
            talkdoer.OverheadUIElementDelay = 0.5f;
            talkdoer.OptionalCustomNotificationSprite = null;
            talkdoer.OutlineDepth = 0.4f;
            talkdoer.OutlineLuminanceCutoff = 0.05f;
            talkdoer.ReassignPrefabReferences = new();
            talkdoer.MovementSpeed = 3f;
            talkdoer.PathableTiles = CellTypes.FLOOR;
            talkdoer.IsDoingForcedSpeech = false;

            //setup playmaker fsm
            #region bad
            var playmakerfsm = go.AddComponent<PlayMakerFSM>();
            playmakerfsm.fsm = new()
            {
                dataVersion = 1,
                usedInTemplate = null,
                name = $"{player.name} FoyerFSM",
                startState = "Sitting Around",
                states = new FsmState[5],
                events = new FsmEvent[11],
                globalTransitions = new FsmTransition[2],
                variables = new()
                {
                    categories = new string[] { "" },
                    floatVariables = new FsmFloat[0],
                    intVariables = new FsmInt[0],
                    boolVariables = new FsmBool[0],
                    stringVariables = new FsmString[] { new("currentMode") { value = "modeBegin" } },
                    vector2Variables = new FsmVector2[0],
                    vector3Variables = new FsmVector3[0],
                    colorVariables = new FsmColor[0],
                    rectVariables = new FsmRect[0],
                    quaternionVariables = new FsmQuaternion[0],
                    gameObjectVariables = new FsmGameObject[0],
                    objectVariables = new FsmObject[0],
                    materialVariables = new FsmMaterial[0],
                    textureVariables = new FsmTexture[0],
                    arrayVariables = new FsmArray[0],
                    enumVariables = new FsmEnum[0],
                    variableCategoryIDs = new int[0]
                },
                description = "Convict\n", //???
                docUrl = "",
                showStateLabel = true,
                maxLoopCount = 0,
                watermark = "Spy.png",
                password = "",
                locked = false,
                manualUpdate = false,
                keepDelayedEventsOnStateExit = false,
                preprocessed = false,
                editorFlags = Fsm.EditorFlags.nameIsExpanded | Fsm.EditorFlags.controlsIsExpanded,
                activeStateName = "",
                mouseEvents = false,
                handleTriggerEnter2D = false,
                handleTriggerExit2D = false,
                handleTriggerStay2D = false,
                handleCollisionEnter2D = false,
                handleCollisionExit2D = false,
                handleCollisionStay2D = false,
                handleTriggerEnter = false,
                handleTriggerExit = false,
                handleTriggerStay = false,
                handleCollisionEnter = false,
                handleCollisionExit = false,
                handleCollisionStay = false,
                handleParticleCollision = false,
                handleControllerColliderHit = false,
                handleJointBreak = false,
                handleJointBreak2D = false,
                handleOnGUI = false,
                handleFixedUpdate = false,
                handleApplicationEvents = false,
                handleAnimatorMove = false,
                handleAnimatorIK = false
            };
            playmakerfsm.fsm.states = new FsmState[]
            {
                new((Fsm)null)
                {
                    name = "Sitting Around",
                    description = "",
                    colorIndex = 1,
                    position = new(63.75f, 105.5f, 115f, 32f),
                    isBreakpoint = false,
                    isSequence = false,
                    hideUnused = false,
                    transitions = new FsmTransition[]
                    {
                        new()
                        {
                            fsmEvent = new(FsmEvent.FindEvent("playerInteract")),
                            toState = "Mode Switchboard",
                            linkStyle = FsmTransition.CustomLinkStyle.Default,
                            linkConstraint = FsmTransition.CustomLinkConstraint.None,
                            colorIndex = 0
                        }
                    },
                    actionData = new()
                    {
                        actionNames = new()
                        {
                            "HutongGames.PlayMaker.Actions.EndConversation"
                        },
                        customNames = new()
                        {
                            ""
                        },
                        actionEnabled = new()
                        {
                            true
                        },
                        actionIsOpen = new()
                        {
                            true
                        },
                        actionStartIndex = new()
                        {
                            0
                        },
                        actionHashCodes = new()
                        {
                            68280249
                        },
                        unityObjectParams = new(),
                        fsmGameObjectParams = new(),
                        fsmOwnerDefaultParams = new(),
                        animationCurveParams = new(),
                        functionCallParams = new(),
                        fsmTemplateControlParams = new(),
                        fsmEventTargetParams = new(),
                        fsmPropertyParams = new(),
                        layoutOptionParams = new(),
                        fsmStringParams = new(),
                        fsmObjectParams = new(),
                        fsmVarParams = new(),
                        fsmArrayParams = new(),
                        fsmEnumParams = new(),
                        fsmFloatParams = new(),
                        fsmIntParams = new(),
                        fsmBoolParams = new(),
                        fsmVector2Params = new(),
                        fsmVector3Params = new(),
                        fsmColorParams = new(),
                        fsmRectParams = new(),
                        fsmQuaternionParams = new(),
                        stringParams = new(),
                        byteData = new()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        },
                        arrayParamSizes = new(),
                        arrayParamTypes = new(),
                        customTypeSizes = new(),
                        customTypeNames = new(),
                        paramDataType = new()
                        {
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                        },
                        paramName = new()
                        {
                            "killZombieTextBoxes",
                            "doNotLerpCamera",
                            "suppressReinteractDelay",
                            "suppressFurtherInteraction"
                        },
                        paramDataPos = new()
                        {
                            0,
                            2,
                            4,
                            6
                        },
                        paramByteDataSize = new()
                        {
                            2,
                            2,
                            2,
                            2
                        }
                    }
                },
                new((Fsm)null)
                {
                    name = "Mode Switchboard",
                    description = "",
                    colorIndex = 0,
                    position = new(231.5313f, 113.6172f, 142f, 16f),
                    isBreakpoint = false,
                    isSequence = false,
                    hideUnused = false,
                    transitions = new FsmTransition[0],
                    actionData = new()
                    {
                        actionNames = new()
                        {
                            "HutongGames.PlayMaker.Actions.ModeSwitchboard"
                        },
                        customNames = new()
                        {
                            ""
                        },
                        actionEnabled = new()
                        {
                            true
                        },
                        actionIsOpen = new()
                        {
                            true
                        },
                        actionStartIndex = new()
                        {
                            0
                        },
                        actionHashCodes = new()
                        {
                            0
                        },
                        unityObjectParams = new(),
                        fsmGameObjectParams = new(),
                        fsmOwnerDefaultParams = new(),
                        animationCurveParams = new(),
                        functionCallParams = new(),
                        fsmTemplateControlParams = new(),
                        fsmEventTargetParams = new(),
                        fsmPropertyParams = new(),
                        layoutOptionParams = new(),
                        fsmStringParams = new(),
                        fsmObjectParams = new(),
                        fsmVarParams = new(),
                        fsmArrayParams = new(),
                        fsmEnumParams = new(),
                        fsmFloatParams = new(),
                        fsmIntParams = new(),
                        fsmBoolParams = new(),
                        fsmVector2Params = new(),
                        fsmVector3Params = new(),
                        fsmColorParams = new(),
                        fsmRectParams = new(),
                        fsmQuaternionParams = new(),
                        stringParams = new(),
                        byteData = new(),
                        arrayParamSizes = new(),
                        arrayParamTypes = new(),
                        customTypeSizes = new(),
                        customTypeNames = new(),
                        paramDataType = new(),
                        paramName = new(),
                        paramDataPos = new(),
                        paramByteDataSize = new()
                    }
                },
                new((Fsm)null)
                {
                    name = "First Speech",
                    description = "",
                    colorIndex = 0,
                    position = new(240.1953f, 247.6641f, 102f, 64f),
                    isBreakpoint = false,
                    isSequence = false,
                    hideUnused = false,
                    transitions = new FsmTransition[]
                    {
                        new()
                        {
                            fsmEvent = new(FsmEvent.FindEvent("yes")),
                            toState = "Do Character Change",
                            linkStyle = FsmTransition.CustomLinkStyle.Default,
                            linkConstraint = FsmTransition.CustomLinkConstraint.None,
                            colorIndex = 0
                        },
                        new()
                        {
                            fsmEvent = new(FsmEvent.FindEvent("no")),
                            toState = "End",
                            linkStyle = FsmTransition.CustomLinkStyle.Default,
                            linkConstraint = FsmTransition.CustomLinkConstraint.None,
                            colorIndex = 0
                        },
                        new()
                        {
                            fsmEvent = new(FsmEvent.FindEvent("FINISHED")),
                            toState = "End",
                            linkStyle = FsmTransition.CustomLinkStyle.Default,
                            linkConstraint = FsmTransition.CustomLinkConstraint.None,
                            colorIndex = 0
                        },
                    },
                    actionData = new()
                    {
                        actionNames = new()
                        {
                            "HutongGames.PlayMaker.Actions.BeginConversation",
                            "HutongGames.PlayMaker.Actions.DialogueBox"
                        },
                        customNames = new()
                        {
                            "",
                            ""
                        },
                        actionEnabled = new()
                        {
                            true,
                            true
                        },
                        actionIsOpen = new()
                        {
                            true,
                            true
                        },
                        actionStartIndex = new()
                        {
                            0,
                            5
                        },
                        actionHashCodes = new()
                        {
                            89987726,
                            34040686
                        },
                        unityObjectParams = new()
                        {
                            null
                        },
                        fsmGameObjectParams = new(),
                        fsmOwnerDefaultParams = new(),
                        animationCurveParams = new(),
                        functionCallParams = new(),
                        fsmTemplateControlParams = new(),
                        fsmEventTargetParams = new(),
                        fsmPropertyParams = new(),
                        layoutOptionParams = new(),
                        fsmStringParams = new()
                        {
                            new FsmString("") { value = overrideChangeCharacterString ?? "#CHARACTER_SELECT_CHANGE_QUESTION" },
                            new FsmString("") { value = overrideYesResponse ?? "#YES" },
                            new FsmString("") { value = overrideNoResponse ?? "#NO" },
                            new FsmString("") { value = "" },
                        },
                        fsmObjectParams = new(),
                        fsmVarParams = new(),
                        fsmArrayParams = new(),
                        fsmEnumParams = new(),
                        fsmFloatParams = new(),
                        fsmIntParams = new(),
                        fsmBoolParams = new(),
                        fsmVector2Params = new(),
                        fsmVector3Params = new(),
                        fsmColorParams = new(),
                        fsmRectParams = new(),
                        fsmQuaternionParams = new(),
                        stringParams = new(),
                        byteData = new()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            128,
                            191,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            4,
                            0,
                            0,
                            0,
                            1,
                            0,
                            0,
                            0,
                            0,
                            121,
                            101,
                            115,
                            110,
                            111,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            1,
                            0,
                            0,
                            0,
                            0,
                            0,
                        },
                        arrayParamSizes = new()
                        {
                            1,
                            2,
                            2
                        },
                        arrayParamTypes = new()
                        {
                            "HutongGames.PlayMaker.FsmString",
                            "HutongGames.PlayMaker.FsmString",
                            "HutongGames.PlayMaker.FsmEvent"
                        },
                        customTypeSizes = new(),
                        customTypeNames = new(),
                        paramDataType = new()
                        {
                            ParamDataType.Enum,
                            ParamDataType.Enum,
                            ParamDataType.FsmFloat,
                            ParamDataType.Boolean,
                            ParamDataType.Vector2,
                            ParamDataType.Enum,
                            ParamDataType.Enum,
                            ParamDataType.FsmInt,
                            ParamDataType.Array,
                            ParamDataType.FsmString,
                            ParamDataType.Array,
                            ParamDataType.FsmString,
                            ParamDataType.FsmString,
                            ParamDataType.Array,
                            ParamDataType.FsmEvent,
                            ParamDataType.FsmEvent,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmFloat,
                            ParamDataType.FsmFloat,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmString,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.ObjectReference,
                        },
                        paramName = new()
                        {
                            "conversationType",
                            "locked",
                            "overrideNpcScreenHeight",
                            "UsesCustomScreenBuffer",
                            "CustomScreenBuffer",
                            "condition",
                            "sequence",
                            "persistentStringsToShow",
                            "dialogue",
                            "",
                            "responses",
                            "",
                            "",
                            "events",
                            "",
                            "",
                            "skipWalkAwayEvent",
                            "forceCloseTime",
                            "zombieTime",
                            "SuppressDefaultAnims",
                            "OverrideTalkAnim",
                            "PlayBoxOnInteractingPlayer",
                            "IsThoughtBubble",
                            "AlternativeTalker"
                        },
                        paramDataPos = new()
                        {
                            0,
                            4,
                            8,
                            13,
                            14,
                            22,
                            26,
                            30,
                            0,
                            0,
                            1,
                            1,
                            2,
                            2,
                            35,
                            38,
                            40,
                            42,
                            47,
                            52,
                            3,
                            54,
                            56,
                            0
                        },
                        paramByteDataSize = new()
                        {
                            4,
                            4,
                            5,
                            1,
                            8,
                            4,
                            4,
                            5,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            3,
                            2,
                            2,
                            5,
                            5,
                            2,
                            0,
                            2,
                            2,
                            0
                        }
                    }
                },
                new((Fsm)null)
                {
                    name = "Do Character Change",
                    description = "",
                    colorIndex = 0,
                    position = new(477.6328f, 256.0547f, 160f, 16f),
                    isBreakpoint = false,
                    isSequence = true,
                    hideUnused = false,
                    transitions = new FsmTransition[0],
                    actionData = new()
                    {
                        actionNames = new()
                        {
                            "HutongGames.PlayMaker.Actions.EndConversation",
                            "HutongGames.PlayMaker.Actions.ChangeToNewCharacter",
                            "HutongGames.PlayMaker.Actions.RestartWhenFinished"
                        },
                        customNames = new()
                        {
                            "",
                            "",
                            ""
                        },
                        actionEnabled = new()
                        {
                            true,
                            true,
                            true
                        },
                        actionIsOpen = new()
                        {
                            true,
                            true,
                            true
                        },
                        actionStartIndex = new()
                        {
                            0,
                            4,
                            5
                        },
                        actionHashCodes = new()
                        {
                            68280249,
                            19973496,
                            0
                        },
                        unityObjectParams = new(),
                        fsmGameObjectParams = new(),
                        fsmOwnerDefaultParams = new(),
                        animationCurveParams = new(),
                        functionCallParams = new(),
                        fsmTemplateControlParams = new(),
                        fsmEventTargetParams = new(),
                        fsmPropertyParams = new(),
                        layoutOptionParams = new(),
                        fsmStringParams = new(),
                        fsmObjectParams = new(),
                        fsmVarParams = new(),
                        fsmArrayParams = new(),
                        fsmEnumParams = new(),
                        fsmFloatParams = new(),
                        fsmIntParams = new(),
                        fsmBoolParams = new(),
                        fsmVector2Params = new(),
                        fsmVector3Params = new(),
                        fsmColorParams = new(),
                        fsmRectParams = new(),
                        fsmQuaternionParams = new(),
                        stringParams = new(),
                        byteData = new List<byte>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        }.Concat(FsmUtility.Encoding.GetBytes(braveAssetName)).ToList(),
                        arrayParamSizes = new(),
                        arrayParamTypes = new(),
                        customTypeSizes = new(),
                        customTypeNames = new(),
                        paramDataType = new()
                        {
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.FsmBool,
                            ParamDataType.String
                        },
                        paramName = new()
                        {
                            "killZombieTextBoxes",
                            "doNotLerpCamera",
                            "suppressReinteractDelay",
                            "suppressFurtherInteraction",
                            "PlayerPrefabPath"
                        },
                        paramDataPos = new()
                        {
                            0,
                            2,
                            4,
                            6,
                            8
                        },
                        paramByteDataSize = new()
                        {
                            2,
                            2,
                            2,
                            2,
                            braveAssetName.Length
                        }
                    }
                },
                new((Fsm)null)
                {
                    name = "End",
                    description = "",
                    colorIndex = 0,
                    position = new(498.5f, 315f, 100f, 16f),
                    isBreakpoint = false,
                    isSequence = true,
                    hideUnused = false,
                    transitions = new FsmTransition[0],
                    actionData = new()
                    {
                        actionNames = new()
                        {
                            "HutongGames.PlayMaker.Actions.RestartWhenFinished"
                        },
                        customNames = new()
                        {
                            ""
                        },
                        actionEnabled = new()
                        {
                            true
                        },
                        actionIsOpen = new()
                        {
                            true
                        },
                        actionStartIndex = new()
                        {
                            0
                        },
                        actionHashCodes = new()
                        {
                            0
                        },
                        unityObjectParams = new(),
                        fsmGameObjectParams = new(),
                        fsmOwnerDefaultParams = new(),
                        animationCurveParams = new(),
                        functionCallParams = new(),
                        fsmTemplateControlParams = new(),
                        fsmEventTargetParams = new(),
                        fsmPropertyParams = new(),
                        layoutOptionParams = new(),
                        fsmStringParams = new(),
                        fsmObjectParams = new(),
                        fsmVarParams = new(),
                        fsmArrayParams = new(),
                        fsmEnumParams = new(),
                        fsmFloatParams = new(),
                        fsmIntParams = new(),
                        fsmBoolParams = new(),
                        fsmVector2Params = new(),
                        fsmVector3Params = new(),
                        fsmColorParams = new(),
                        fsmRectParams = new(),
                        fsmQuaternionParams = new(),
                        stringParams = new(),
                        byteData = new(),
                        arrayParamSizes = new(),
                        arrayParamTypes = new(),
                        customTypeSizes = new(),
                        customTypeNames = new(),
                        paramDataType = new(),
                        paramName = new(),
                        paramDataPos = new(),
                        paramByteDataSize = new()
                    }
                }
            };
            playmakerfsm.fsm.events = new FsmEvent[]
            {
                new(FsmEvent.FindEvent("FINISHED")),
                new(FsmEvent.FindEvent("RESTART")),
                new(FsmEvent.FindEvent("modeBegin")),
                new(FsmEvent.FindEvent("modeCompletedTutorial")),
                new(FsmEvent.FindEvent("modeFirstSpeech")),
                new(FsmEvent.FindEvent("modeGatlingGull")),
                new(FsmEvent.FindEvent("modeNotCompletedTutorial")),
                new(FsmEvent.FindEvent("modeNothingElseToSay")),
                new(FsmEvent.FindEvent("no")),
                new(FsmEvent.FindEvent("playerInteract")),
                new(FsmEvent.FindEvent("yes"))
            };
            playmakerfsm.fsm.globalTransitions = new FsmTransition[]
            {
                new()
                {
                    fsmEvent = new(FsmEvent.FindEvent("RESTART")),
                    toState = "Sitting Around",
                    linkStyle = FsmTransition.CustomLinkStyle.Default,
                    linkConstraint = FsmTransition.CustomLinkConstraint.None,
                    colorIndex = 0
                },
                new()
                {
                    fsmEvent = new(FsmEvent.FindEvent("modeBegin")),
                    toState = "First Speech",
                    linkStyle = FsmTransition.CustomLinkStyle.Default,
                    linkConstraint = FsmTransition.CustomLinkConstraint.None,
                    colorIndex = 0
                }
            };
            playmakerfsm.fsmTemplate = null;
            #endregion

            //setup idle doer
            var idledoer = go.AddComponent<CharacterSelectIdleDoer>();
            idledoer.coreIdleAnimation = "select_idle";
            idledoer.onSelectedAnimation = "select_choose";
            idledoer.idleMin = 0f;
            idledoer.idleMax = 0f;
            idledoer.phases = new CharacterSelectIdlePhase[0];
            idledoer.IsEevee = false;
            idledoer.EeveeTex = null;
            idledoer.AnimationLibraries = new tk2dSpriteAnimation[0];

            //setup select flag
            var selectflag = go.AddComponent<FoyerCharacterSelectFlag>();
            selectflag.CharacterPrefabPath = braveAssetName;
            selectflag.OverheadElement = talkdoer.OverheadUIElementOnPreInteract = overheadgo;
            selectflag.IsCoopCharacter = false;
            selectflag.IsGunslinger = false;
            selectflag.IsEevee = false;
            selectflag.prerequisites = unlockPrereqs.ToArray();
            selectflag.AltCostumeLibrary = player.AlternateCostumeLibrary;

            //setup shadow sprite
            var defaultShadowSprite = ResourceCache.Acquire("DefaultShadowSprite").As<GameObject>().GetComponent<tk2dBaseSprite>();
            var shadowSprite = tk2dSprite.AddComponent(go.transform.Find("DefaultShadowSprite").gameObject, defaultShadowSprite.Collection, defaultShadowSprite.spriteId);
            shadowSprite.transform.position = (sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerCenter) - shadowSprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter)).Quantize(0.0625f) +
                additionalShadowOffset.ToVector2() / 16f;

            //setup panel
            var panel = overheadgo.AddComponent<dfPanel>();
            var techBlueSkin = LoadHelper.LoadAssetFromAnywhere<GameObject>("TechBlue Skin").GetComponent<dfAtlas>();
            var gameUiAtlas = LoadHelper.LoadAssetFromAnywhere<GameObject>("GameUIAtlas").GetComponent<dfAtlas>();
            var daikonBitmap = LoadHelper.LoadAssetFromAnywhere<GameObject>("OurFont_DaikonBitmap").GetComponent<dfFont>();
            panel.atlas = techBlueSkin;
            panel.backgroundSprite = "";
            panel.anchorStyle = dfAnchorStyle.All | dfAnchorStyle.Proportional;
            panel.layout = new(dfAnchorStyle.All | dfAnchorStyle.Proportional) { owner = panel, margins = new() { bottom = 0.5f, left = 0.4438f, right = 0.5563f, top = 0.3f } };
            panel.pivot = dfPivotPoint.BottomCenter;
            panel.renderOrder = 34;
            panel.size = new(162f, 162f);
            panel.zindex = 0;
            panel.tooltip = "";
            var infopanel = overheadgo.AddComponent<FoyerInfoPanelController>();
            infopanel.characterIdentity = player.characterIdentity;
            var textpanel = overheadgo.transform.Find("TextPanel");
            var panelcomp = textpanel.AddComponent<dfPanel>();
            infopanel.textPanel = panelcomp;
            panelcomp.atlas = techBlueSkin;
            panelcomp.backgroundSprite = "";
            panelcomp.anchorStyle = dfAnchorStyle.All | dfAnchorStyle.Proportional;
            panelcomp.layout = new(dfAnchorStyle.All | dfAnchorStyle.Proportional) { owner = panelcomp, margins = new() { bottom = 0.7037f, left = 0.8519f, right = 0.8519f, top = -0.4074f } };
            panelcomp.renderOrder = 35;
            panelcomp.size = new(0f, 202f);
            panelcomp.zindex = 0;
            panelcomp.tooltip = "";
            panelcomp.clipChildren = true;
            var nameLabel = textpanel.Find("NameLabel").AddComponent<dfLabel>();
            nameLabel.atlas = gameUiAtlas;
            nameLabel.backgroundSprite = "chamber_flash_letter_001";
            nameLabel.backgroundColor = new(0, 0, 0, 255);
            nameLabel.font = daikonBitmap;
            nameLabel.text = breachName;
            nameLabel.isLocalized = false;
            nameLabel.textScale = 3f;
            nameLabel.anchorStyle = dfAnchorStyle.Left;
            nameLabel.renderOrder = 36;
            nameLabel.size = Vector2.zero;
            nameLabel.autoSize = true;
            nameLabel.tooltip = "";
            nameLabel.zindex = 0;
            var gunLabel = textpanel.Find("GunLabel").AddComponent<dfLabel>();
            gunLabel.atlas = gameUiAtlas;
            gunLabel.backgroundSprite = "chamber_flash_letter_001";
            gunLabel.backgroundColor = new(0, 0, 0, 255);
            gunLabel.font = daikonBitmap;
            gunLabel.text = description2Text ?? "";
            gunLabel.isVisible = !string.IsNullOrEmpty(description2Text);
            gunLabel.isLocalized = false;
            gunLabel.textScale = 3f;
            gunLabel.anchorStyle = dfAnchorStyle.Left;
            gunLabel.renderOrder = 36;
            gunLabel.size = Vector2.zero;
            gunLabel.autoSize = true;
            gunLabel.tooltip = "";
            gunLabel.zindex = 0;
            var descLabel = textpanel.Find("DescLabel").AddComponent<dfLabel>();
            descLabel.atlas = gameUiAtlas;
            descLabel.backgroundSprite = "chamber_flash_letter_001";
            descLabel.backgroundColor = new(0, 0, 0, 255);
            descLabel.font = daikonBitmap;
            descLabel.text = description1Text ?? "";
            descLabel.isVisible = !string.IsNullOrEmpty(description1Text);
            descLabel.textScale = 3f;
            descLabel.anchorStyle = dfAnchorStyle.Left;
            descLabel.renderOrder = 39;
            descLabel.size = Vector2.zero;
            descLabel.autoSize = true;
            descLabel.tooltip = "";
            descLabel.zindex = 3;
            var pastKilledLabel = textpanel.Find("PastKilledLabel").AddComponent<dfLabel>();
            pastKilledLabel.atlas = gameUiAtlas;
            pastKilledLabel.backgroundSprite = "chamber_flash_letter_001";
            pastKilledLabel.backgroundColor = new(0, 0, 0, 255);
            pastKilledLabel.font = daikonBitmap;
            pastKilledLabel.text = "#CHARACTERSELECT_PASTKILLED";
            pastKilledLabel.isLocalized = true;
            pastKilledLabel.textScale = 3f;
            pastKilledLabel.anchorStyle = dfAnchorStyle.Left;
            pastKilledLabel.renderOrder = 37;
            pastKilledLabel.size = Vector2.zero;
            pastKilledLabel.autoSize = true;
            pastKilledLabel.tooltip = "";
            pastKilledLabel.zindex = 1;
            var itemsPanel = infopanel.itemsPanel = textpanel.Find("ItemsPanel").AddComponent<dfPanel>();
            itemsPanel.atlas = gameUiAtlas;
            itemsPanel.backgroundColor = new(0, 0, 0, 255);
            itemsPanel.backgroundSprite = "chamber_flash_letter_001";
            itemsPanel.anchorStyle = dfAnchorStyle.Top | dfAnchorStyle.Left;
            itemsPanel.renderOrder = 40;
            itemsPanel.zindex = 4;
            itemsPanel.tooltip = "";
            itemsPanel.size = new(850f, 75f);
            if(!string.IsNullOrEmpty(itemsTexture))
            {
                var itemsSprite = itemsPanel.transform.Find("ItemsSprite").AddComponent<dfSprite>();
                itemsSprite.atlas = gameUiAtlas;
                itemsSprite.spriteName = AddToAtlas(itemsTexture, out var info);
                itemsSprite.size = info.sizeInPixels * 3;
                itemsSprite.renderOrder = 41;
                itemsSprite.zindex = 5;
                itemsSprite.tooltip = "";
            }

            var arrow = overheadgo.transform.Find("Arrow");
            var facecardColl = LoadHelper.LoadAssetFromAnywhere<GameObject>("CHR_RoguePanel").transform.Find("Rogue Arrow").GetComponent<tk2dSprite>().Collection;
            var facecardAnimation = LoadHelper.LoadAssetFromAnywhere<GameObject>("Facecard_Animation").GetComponent<tk2dSpriteAnimation>();
            infopanel.scaledSprites = new tk2dSprite[] { infopanel.arrow = tk2dSprite.AddComponent(arrow.gameObject, facecardColl, facecardColl.GetSpriteIdByName("SelectionReticle")) };
            var arrowanim = arrow.AddComponent<tk2dSpriteAnimator>();
            arrowanim.defaultClipId = 22;
            arrowanim.playAutomatically = true;
            arrowanim.Library = facecardAnimation;
            var facecard = arrow.Find("Sprite FaceCard");
            facecard.gameObject.layer = LayerMask.NameToLayer("GUI");
            var specialFacecardCollection = EasyCollectionSetup("SpecialFacecardCollection");
            var specialFacecardAnimation = EasyAnimationSetup("SpecialFacecardAnimation");
            specialFacecardAnimation.AddClip($"{facecardAnimationPrefix}_facecard_appear", specialFacecardCollection, 17, tk2dSpriteAnimationClip.WrapMode.Once, overrideShaderName: "tk2d/BlendVertexColor").frames.Do(
                x => x.spriteCollection.spriteDefinitions[x.spriteId].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, null, false, false));
            specialFacecardAnimation.AddClip($"{facecardAnimationPrefix}_facecard_idle", specialFacecardCollection, facecardIdleFps, tk2dSpriteAnimationClip.WrapMode.Loop, overrideShaderName: "tk2d/BlendVertexColor").frames.Do(
                x => { var def = x.spriteCollection.spriteDefinitions[x.spriteId]; def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, null, false, false); def.AddOffset(new(0f, 0.125f)); });
            tk2dSprite.AddComponent(facecard.gameObject, specialFacecardCollection, $"{facecardAnimationPrefix}_facecard_idle_001");
            var facecardAnim = facecard.AddComponent<tk2dSpriteAnimator>();
            facecardAnim.Library = specialFacecardAnimation;
            facecardAnim.playAutomatically = true;
            facecardAnim.defaultClipId = specialFacecardAnimation.GetClipIdByName($"{facecardAnimationPrefix}_facecard_appear");
            var facecardIdleDoer = facecard.AddComponent<CharacterSelectFacecardIdleDoer>();
            facecardIdleDoer.appearAnimation = $"{facecardAnimationPrefix}_facecard_appear";
            facecardIdleDoer.coreIdleAnimation = $"{facecardAnimationPrefix}_facecard_idle";
            facecardIdleDoer.idleMin = 0f;
            facecardIdleDoer.idleMax = 0f;
            facecardIdleDoer.usesMultipleIdleAnimations = false;
            facecardIdleDoer.multipleIdleAnimations = new string[0];
            facecardIdleDoer.EeveeTex = null;

            breachCharacters[go] = breachPosition;
            return selectflag;
        }

        [HarmonyPatch(typeof(Foyer), nameof(Foyer.SetUpCharacterCallbacks))]
        [HarmonyPrefix]
        public static void PlaceBreachCharacters()
        {
            foreach(var kvp in breachCharacters)
            {
                var room = GameManager.Instance.Dungeon.data.GetRoomFromPosition(kvp.Value.IntXY(VectorConversions.Floor));
                if(room != null)
                {
                    var obj = Object.Instantiate(kvp.Key, room.hierarchyParent);
                    obj.transform.position = kvp.Value;
                    var interactables = obj.GetComponentsInChildren<IPlayerInteractable>();
                    foreach (var interactable in interactables)
                    {
                        room.RegisterInteractable(interactable);
                    }
                }
            }
        }

        public static Dictionary<GameObject, Vector3> breachCharacters = new();
    }
}