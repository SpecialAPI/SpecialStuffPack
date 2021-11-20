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
        public static void Init()
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
                ItemIds = new Dictionary<string, int>();
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

        public static T EasyInit<T>(string objectPath, string spritePath, string name, string shortDesc, string longDesc, PickupObject.ItemQuality quality, string consolePrefix, int? ammonomiconPlacement = null, string overrideConsoleID = null) 
            where T : PickupObject
        {
            if (!objectPath.StartsWith("assets/"))
            {
                objectPath = "assets/" + objectPath;
            }
            if (!objectPath.EndsWith(".prefab"))
            {
                objectPath += ".prefab";
            }
            if (!spritePath.StartsWith("assets/"))
            {
                spritePath = "assets/" + spritePath;
            }
            if (!spritePath.EndsWith(".png"))
            {
                spritePath += ".png";
            }
            GameObject go = AssetBundleManager.Load<GameObject>(objectPath);
            tk2dSprite.AddComponent(go, SpriteBuilder.itemCollection, AddSpriteToCollection(AssetBundleManager.Load<Texture2D>(spritePath), SpriteBuilder.itemCollection));
            T item = go.AddComponent<T>();
            SetupItem(item, name, shortDesc, longDesc, consolePrefix);
            item.quality = quality;
            if(ammonomiconPlacement != null)
            {
                item.PlaceItemInAmmonomiconAfterItemById(ammonomiconPlacement.Value);
            }
            if(overrideConsoleID != null)
            {
                Game.Items.Rename(consolePrefix + ":" + name.ToMTGId(), overrideConsoleID);
            }
            return item;
        }

        public static void PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
        {
            item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
        }

        /// <summary>
        /// Finishes the item setup, adds it to the item databases, adds an encounter trackable 
        /// blah, blah, blah
        /// </summary>
        public static void SetupItem(PickupObject item, string name, string shortDesc, string longDesc, string idPool)
        {
            try
            {
                item.encounterTrackable = null;
                ETGMod.Databases.Items.SetupItem(item, item.name);
                SpriteBuilder.AddToAmmonomicon(item.sprite.GetCurrentSpriteDef());
                item.encounterTrackable.journalData.AmmonomiconSprite = item.sprite.GetCurrentSpriteDef().name;
                item.SetName(name);
                item.SetShortDescription(shortDesc);
                item.SetLongDescription(longDesc);
                if (item is PlayerItem)
                    (item as PlayerItem).consumable = false;
                Game.Items.Add(idPool + ":" + name.ToMTGId(), item);
                ETGMod.Databases.Items.Add(item);
                item.RemovePeskyQuestionmark();
                ItemIds.Add(item.name.ToLower(), item.PickupObjectId);
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        public static void RemovePeskyQuestionmark(this PickupObject item)
        {
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).journalData.SuppressKnownState = false;
        }

        /// <summary>
        /// Sets the cooldown type and length of a PlayerItem, and resets all other cooldown types
        /// </summary>
        public static void SetCooldownType(this PlayerItem item, CooldownType cooldownType, float value)
        {
            item.damageCooldown = -1;
            item.roomCooldown = -1;
            item.timeCooldown = -1;

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
        }

        public static void AddToFlyntShop(this PickupObject po)
        {
            Shop_Key_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        public static void AddToTrorkShop(this PickupObject po)
        {
            Shop_Truck_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        public static void AddToGooptonShop(this PickupObject po)
        {
            Shop_Goop_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        public static void AddToCursulaShop(this PickupObject po)
        {
            Shop_Curse_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        public static void AddToOldRedShop(this PickupObject po)
        {
            Shop_Blank_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        public static void AddToBlacksmithShop(this PickupObject po)
        {
            BlackSmith_Items_01.defaultItemDrops.Add(new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = 1f,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }

        /// <summary>
        /// Adds a passive player stat modifier to a PlayerItem or PassiveItem
        /// </summary>
        public static void AddPassiveStatModifier(this PickupObject po, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
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
        }

        public static IEnumerator HandleDuration(PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
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
        public static Dictionary<string, int> ItemIds;
    }
}
