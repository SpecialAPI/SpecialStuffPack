using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Collections;
using Dungeonator;
using SpecialStuffPack.Items;
using Gungeon;

namespace SpecialStuffPack.ItemAPI
{
    public static class ItemBuilder
    {
        /// <summary>
        /// Sets the base assembly of the ResourceExtractor, so 
        /// resources can be accessed
        /// </summary>
        public static void InitItemBuilder()
        {
            try
            {
                sharedAssets = ResourceManager.LoadAssetBundle("shared_auto_001");
                Shop_Key_Items_01 = sharedAssets.LoadAsset<GenericLootTable>("Shop_Key_Items_01");
                Shop_Truck_Items_01 = sharedAssets.LoadAsset<GenericLootTable>("Shop_Truck_Items_01");
                Shop_Curse_Items_01 = sharedAssets.LoadAsset<GenericLootTable>("Shop_Curse_Items_01");
                Shop_Goop_Items_01 = sharedAssets.LoadAsset<GenericLootTable>("Shop_Goop_Items_01");
                Shop_Blank_Items_01 = sharedAssets.LoadAsset<GenericLootTable>("Shop_Blank_Items_01");
                ForgeDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Forge");
                BlacksmithShop = ForgeDungeonPrefab.PatternSettings.flows[0].AllNodes[10].overrideExactRoom;
                BlackSmith_Items_01 = (BlacksmithShop.placedObjects[8].nonenemyBehaviour as BaseShopController).shopItemsGroup2;
                ForgeDungeonPrefab = null;
                ItemIds = new();
                Passive = new();
                Active = new();
                Guns = new();
                Item = new();
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }



        public enum CooldownType
        {
            Timed, Damage, PerRoom, None
        }

        /// <summary>
        /// Adds a tk2dSprite component to an object and adds that sprite to the 
        /// ammonomicon for later use. If obj is null, returns a new GameObject with the sprite
        /// </summary>
        public static GameObject AddSpriteToObject(string name, string resourcePath, GameObject obj = null)
        {
            GameObject spriteObject = SpriteFromResource(resourcePath, obj);
            spriteObject.name = name;
            return spriteObject;
        }

        public static PlayerOrbital EasyGuonInit(string objectPath, string spritePath, IntVector2 hitboxSize, float orbitRadius = 2.5f, float degreesPerSecond = 120f, int tier = 0, bool shouldRotate = false, 
            Vector2? additionalSpriteOffset = null, IntVector2? additionalHitboxPixelOffset = null, CollisionLayer? overrideCollisionLayer = null)
        {
            GameObject go = AssetBundleManager.Load<GameObject>(objectPath);
            var sprite = tk2dSprite.AddComponent(go.transform.Find("Sprite").gameObject, SpriteBuilder.itemCollection, AddSpriteToCollection(spritePath, SpriteBuilder.itemCollection));
            sprite.transform.localPosition = -sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter) + (additionalSpriteOffset ?? Vector2.zero);
            var rigidbody = go.AddComponent<SpeculativeRigidbody>();
            var hitboxOffset = additionalHitboxPixelOffset ?? IntVector2.Zero;
            rigidbody.PixelColliders = new()
            {
                new()
                {
                    CollisionLayer = overrideCollisionLayer ?? CollisionLayer.EnemyBulletBlocker,
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    ManualOffsetX = -hitboxSize.x / 2 + hitboxOffset.x,
                    ManualOffsetY = -hitboxSize.y / 2 + hitboxOffset.y,
                    ManualWidth = hitboxSize.x,
                    ManualHeight = hitboxSize.y
                }
            };
            var guon = go.AddComponent<PlayerOrbital>();
            guon.orbitRadius = orbitRadius;
            guon.orbitDegreesPerSecond = degreesPerSecond;
            guon.SetOrbitalTier(tier);
            guon.shouldRotate = shouldRotate;
            return guon;
        }

        public static GameObject SpriteFromResource(string spriteName, GameObject obj = null)
        {
            return SpriteBuilder.SpriteFromResource(spriteName, "tk2d/CutoutVertexColorTintableTilted", obj);
        }

        public static int AddSpriteToCollection(string name, tk2dSpriteCollectionData collection)
        {
            return SpriteBuilder.AddSpriteToCollection(name, collection, "tk2d/CutoutVertexColorTintableTilted");
        }

        public static int AddSpriteToCollection(Texture2D tex, tk2dSpriteCollectionData collection)
        {
            return SpriteBuilder.AddSpriteToCollection(tex, collection, "tk2d/CutoutVertexColorTintableTilted");
        }

        public static T EasyItemInit<T>(string objectPath, string spritePath, string itemName, string itemShortDesc, string itemLongDesc, PickupObject.ItemQuality quality, int? ammonomiconPlacement = null, string overrideConsoleID = null) 
            where T : PickupObject
        {
            GameObject go = AssetBundleManager.Load<GameObject>(objectPath);
            tk2dSprite.AddComponent(go, SpriteBuilder.itemCollection, AddSpriteToCollection(AssetBundleManager.Load<Texture2D>(spritePath), SpriteBuilder.itemCollection));
            T item = go.AddComponent<T>();
            SetupItem(item, itemName, itemShortDesc, itemLongDesc, SpecialStuffModule.globalPrefix, overrideConsoleID);
            item.quality = quality;
            if(ammonomiconPlacement != null)
            {
                item.PlaceItemInAmmonomiconAfterItemById(ammonomiconPlacement.Value);
            }
            if(quality == PickupObject.ItemQuality.SPECIAL || quality == PickupObject.ItemQuality.EXCLUDED)
            {
                item.RemovePickupFromLootTables();
            }
            return item;
        }

        public static PickupObject RemovePickupFromLootTables(this PickupObject po)
        {
            WeightedGameObject go1 = GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
            if (go1 != null)
            {
                GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements.Remove(go1);
            }
            WeightedGameObject go2 = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
            if (go2 != null)
            {
                GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements.Remove(go2);
            }
            return po;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, PickupObject po)
        {
            WeightedGameObject go = collection.FindWeightedGameObjectInCollection(po.PickupObjectId);
            if (go == null)
            {
                go = collection.FindWeightedGameObjectInCollection(po.gameObject);
            }
            return go;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, int id)
        {
            foreach (WeightedGameObject go in collection.elements)
            {
                if (go.pickupId == id)
                {
                    return go;
                }
            }
            return null;
        }

        public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, GameObject obj)
        {
            foreach (WeightedGameObject go in collection.elements)
            {
                if (go.gameObject == obj)
                {
                    return go;
                }
            }
            return null;
        }

        public static PickupObject PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
        {
            item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
            return item;
        }

        /// <summary>
        /// Finishes the item setup, adds it to the item databases, adds an encounter trackable 
        /// blah, blah, blah
        /// </summary>
        public static void SetupItem(PickupObject item, string name, string shortDesc, string longDesc, string idPool, string overrideConsoleId = null)
        {
            try
            {
                item.encounterTrackable = null;
                var notSpapiName = item.name;
                item.gameObject.name = $"spapi_{item.gameObject.name}";
                ETGMod.Databases.Items.SetupItem(item, item.name);
                SpriteBuilder.AddToAmmonomicon(item.sprite.GetCurrentSpriteDef());
                item.encounterTrackable.journalData.AmmonomiconSprite = item.sprite.GetCurrentSpriteDef().name;
                item.SetName(name);
                item.SetShortDescription(shortDesc);
                item.SetLongDescription(longDesc);
                if (item is PlayerItem)
                    (item as PlayerItem).consumable = false;
                if(!string.IsNullOrEmpty(overrideConsoleId))
                {
                    Game.Items.Add(overrideConsoleId, item);
                }
                else
                {
                    Game.Items.Add(idPool + ":" + name.ToMTGId(), item);
                }
                ETGMod.Databases.Items.AddSpecific(false, item);
                ItemIds.Add(notSpapiName.ToLowerInvariant(), item.PickupObjectId);
                if(item is PassiveItem passive)
                {
                    Passive.Add(notSpapiName.ToLowerInvariant(), passive);
                }
                else if(item is PlayerItem active)
                {
                    Active.Add(notSpapiName.ToLowerInvariant(), active);
                }
                Item.Add(notSpapiName.ToLowerInvariant(), item);
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        /// <summary>
        /// Sets the cooldown type and length of a PlayerItem, and resets all other cooldown types
        /// </summary>
        public static PlayerItem SetCooldownType(this PlayerItem item, CooldownType cooldownType, float value)
        {
            item.damageCooldown = 0;
            item.roomCooldown = 0;
            item.timeCooldown = 0;

            switch (cooldownType)
            {
                case CooldownType.Timed:
                    item.timeCooldown = value;
                    break;
                case CooldownType.Damage:
                    item.damageCooldown = value;
                    break;
                case CooldownType.PerRoom:
                    item.roomCooldown = (int)value;
                    break;
            }
            return item;
        }

        public static PickupObject AddToFlyntShop(this PickupObject po)
        {
            Shop_Key_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        public static PickupObject AddToTrorkShop(this PickupObject po)
        {
            Shop_Truck_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        public static PickupObject AddToGooptonShop(this PickupObject po)
        {
            Shop_Goop_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        public static PickupObject AddToCursulaShop(this PickupObject po)
        {
            Shop_Curse_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        public static PickupObject AddToOldRedShop(this PickupObject po)
        {
            Shop_Blank_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        public static PickupObject AddToBlacksmithShop(this PickupObject po)
        {
            BlackSmith_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
            return po;
        }

        /// <summary>
        /// Adds a passive player stat modifier to a PlayerItem or PassiveItem
        /// </summary>
        public static PickupObject AddPassiveStatModifier(this PickupObject po, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            if (po is PlayerItem)
            {
                var item = (po as PlayerItem);
                if (item.passiveStatModifiers == null)
                    item.passiveStatModifiers = new StatModifier[] { modifier };
                else
                    item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
            }
            else if (po is PassiveItem)
            {
                var item = (po as PassiveItem);
                if (item.passiveStatModifiers == null)
                    item.passiveStatModifiers = new StatModifier[] { modifier };
                else
                    item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
            }
            else if (po is Gun)
            {
                var item = (po as Gun);
                if (item.passiveStatModifiers == null)
                    item.passiveStatModifiers = new StatModifier[] { modifier };
                else
                    item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
            }
            else
            {
                throw new NotSupportedException("Object must be of type PlayerItem, PassiveItem or Gun!");
            }
            return po;
        }

        public static IEnumerator HandleDuration(this PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
        {
            if (item.IsCurrentlyActive)
            {
                yield break;
            }

            SetPrivateType(item, "m_isCurrentlyActive", true);
            SetPrivateType(item, "m_activeElapsed", 0f);
            SetPrivateType(item, "m_activeDuration", duration);
            item.OnActivationStatusChanged?.Invoke(item);

            float elapsed = GetPrivateType<PlayerItem, float>(item, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(item, "m_activeDuration");

            while (GetPrivateType<PlayerItem, float>(item, "m_activeElapsed") < GetPrivateType<PlayerItem, float>(item, "m_activeDuration") && item.IsCurrentlyActive)
            {
                yield return null;
            }
            SetPrivateType(item, "m_isCurrentlyActive", false);
            item.OnActivationStatusChanged?.Invoke(item);

            OnFinish?.Invoke(user);
            yield break;
        }

        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }

        public static AssetBundle sharedAssets;
        public static Dungeon ForgeDungeonPrefab;
        public static PrototypeDungeonRoom BlacksmithShop;
        public static GenericLootTable Shop_Key_Items_01;
        public static GenericLootTable Shop_Truck_Items_01;
        public static GenericLootTable Shop_Curse_Items_01;
        public static GenericLootTable Shop_Goop_Items_01;
        public static GenericLootTable Shop_Blank_Items_01;
        public static GenericLootTable BlackSmith_Items_01;
        public static Dictionary<string, Gun> Guns;
        public static Dictionary<string, PassiveItem> Passive;
        public static Dictionary<string, PlayerItem> Active;
        public static Dictionary<string, PickupObject> Item;
        public static Dictionary<string, int> ItemIds;
    }
}
