using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;
using SpecialStuffPack.Components;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class BadLuckClover : PassiveItem
    {
        public static void Init()
        {
            string name = "Withered Clover";
            string shortdesc = "Bad Luck";
            string longdesc = "A withered clover, seems to affect the bearer's luck negatively.";
            EasyInitItem<BadLuckClover>("items/badluckclover", "sprites/withered_clover_idle_001", name, shortdesc, longdesc, ItemQuality.D, 289, null);
            new Hook(typeof(RewardManager).GetMethod("GenerationSpawnRewardChestAt", BindingFlags.Public | BindingFlags.Instance), typeof(BadLuckClover).GetMethod("DoBadLuck", BindingFlags.Public | BindingFlags.Static));
            ETGMod.Chest.OnPreOpen += HandleEncounterableBadLuck;
        }
        
        public static bool HandleEncounterableBadLuck(bool b, Chest chest, PlayerController p)
        {
            if (chest.GetComponent<BadLuckAffectedChest>())
            {
                List<PickupObject> contents = chest.PredictContents(p);
                foreach(PickupObject content in contents)
                {
                    if(content.encounterTrackable != null)
                    {
                        EncounterTrackable.SuppressNextNotification = true;
                        content.encounterTrackable.HandleEncounter();
                    }
                }
            }
            return true;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(BadLuckClover));
        }

        public static Chest DoBadLuck(Func<RewardManager, IntVector2, RoomHandler, ItemQuality?, float, Chest> orig, RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, ItemQuality? targetQuality, float
            overrideMimicChance)
        {
            if(targetQuality != null)
            {
                return orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
            }
            else if (AnyoneHasActiveSynergy("Just Your Normal Luck"))
            {
                Chest toReturn = null;
                ItemQuality quality = (ItemQuality)UnityEngine.Random.Range(1, 6);
                int numToSpawn;
                switch (quality)
                {
                    case ItemQuality.D:
                    case ItemQuality.C:
                        numToSpawn = 4;
                        break;
                    case ItemQuality.B:
                        numToSpawn = 2;
                        break;
                    case ItemQuality.A:
                    case ItemQuality.S:
                        numToSpawn = 1;
                        break;
                    default:
                        toReturn = orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
                        return toReturn;
                }
                if(AnyoneHasActiveSynergy("Somehow... Luckier?"))
                {
                    numToSpawn += 1;
                }
                if(numToSpawn > 1)
                {
                    int dchests = StaticReferenceManager.DChestsSpawnedOnFloor;
                    StaticReferenceManager.DChestsSpawnedOnFloor = 0;
                    List<IntVector2> spawnOffsets = new List<IntVector2>();
                    if(numToSpawn == 2)
                    {
                        spawnOffsets.Add(new IntVector2(2, 0));
                        spawnOffsets.Add(new IntVector2(-2, 0));
                    }
                    else if(numToSpawn == 3)
                    {
                        spawnOffsets.Add(new IntVector2(2, 1));
                        spawnOffsets.Add(new IntVector2(-2, 1));
                        spawnOffsets.Add(new IntVector2(0, -1));
                    }
                    else if(numToSpawn == 4)
                    {
                        spawnOffsets.Add(new IntVector2(2, 1));
                        spawnOffsets.Add(new IntVector2(-2, 1));
                        spawnOffsets.Add(new IntVector2(2, -1));
                        spawnOffsets.Add(new IntVector2(-2, -1));
                    }
                    else if (numToSpawn == 5)
                    {
                        spawnOffsets.Add(IntVector2.Zero);
                        spawnOffsets.Add(new IntVector2(2, 1));
                        spawnOffsets.Add(new IntVector2(-2, 1));
                        spawnOffsets.Add(new IntVector2(2, -1));
                        spawnOffsets.Add(new IntVector2(-2, -1));
                    }
                    else
                    {
                        spawnOffsets = new List<IntVector2>(numToSpawn);
                    }
                    List<Chest> spawnedChests = new List<Chest>();
                    Chest original = orig(self, positionInRoom + spawnOffsets[0], targetRoom, quality, 0f);
                    spawnedChests.Add(original);
                    Chest toSpawn = PayToWin.GetChestFromQuality(quality);
                    bool isSynergy = false;
                    if (original.lootTable.CompletesSynergy)
                    {
                        toSpawn = GameManager.Instance.RewardManager.Synergy_Chest;
                        isSynergy = true;
                    }
                    GenericLootTable table = GameManager.Instance.RewardManager.ItemsLootTable;
                    if (original.ChestType == Chest.GeneralChestType.WEAPON)
                    {
                        table = GameManager.Instance.RewardManager.GunsLootTable;
                    }
                    for(int i = 1; i < numToSpawn; i++)
                    {
                        spawnedChests.Add(CodeShortcuts.GenerationSpawnChestSetLootTable(toSpawn, targetRoom, positionInRoom + spawnOffsets[i], isSynergy ? new Vector2(-0.1875f, 0f) : Vector2.zero, 0f, table));
                    }
                    foreach (Chest c in spawnedChests)
                    {
                        c.ForceUnlock();
                        c.AddComponent<BadLuckAffectedChest>();
                    }
                    StaticReferenceManager.DChestsSpawnedOnFloor += dchests;
                }
                else
                {
                    toReturn = orig(self, positionInRoom, targetRoom, quality, overrideMimicChance);
                    toReturn.ForceUnlock();
                }
                return toReturn;
            }
            else if (IsFlagSetAtAll(typeof(BadLuckClover)))
            {
                int dchests = StaticReferenceManager.DChestsSpawnedOnFloor;
                StaticReferenceManager.DChestsSpawnedOnFloor = 0;
                ItemQuality quality = BraveUtility.RandomBool() ? ItemQuality.D : ItemQuality.C;
                List<Chest> spawnedChests = new List<Chest>();
                Chest original = orig(self, positionInRoom + new IntVector2(2, 1), targetRoom, quality, 0f);
                spawnedChests.Add(original);
                Chest toSpawn = PayToWin.GetChestFromQuality(quality);
                bool isSynergy = false;
                if (original.lootTable.CompletesSynergy)
                {
                    toSpawn = GameManager.Instance.RewardManager.Synergy_Chest;
                    isSynergy = true;
                }
                GenericLootTable table = GameManager.Instance.RewardManager.ItemsLootTable;
                if(original.ChestType == Chest.GeneralChestType.WEAPON)
                {
                    table = GameManager.Instance.RewardManager.GunsLootTable;
                }
                spawnedChests.Add(CodeShortcuts.GenerationSpawnChestSetLootTable(toSpawn, targetRoom, positionInRoom + new IntVector2(-2, 1), isSynergy ? new Vector2(-0.1875f, 0f) : Vector2.zero, 0f, table));
                spawnedChests.Add(CodeShortcuts.GenerationSpawnChestSetLootTable(toSpawn, targetRoom, positionInRoom + new IntVector2(2, -1), isSynergy ? new Vector2(-0.1875f, 0f) : Vector2.zero, 0f, table));
                spawnedChests.Add(CodeShortcuts.GenerationSpawnChestSetLootTable(toSpawn, targetRoom, positionInRoom + new IntVector2(-2, -1), isSynergy ? new Vector2(-0.1875f, 0f) : Vector2.zero, 0f, table));
                if(AnyoneHasActiveSynergy("Somehow... Luckier?"))
                {
                    spawnedChests.Add(CodeShortcuts.GenerationSpawnChestSetLootTable(toSpawn, targetRoom, positionInRoom, isSynergy ? new Vector2(-0.1875f, 0f) : Vector2.zero, 0f, table));
                }
                foreach (Chest c in spawnedChests)
                {
                    c.ForceUnlock();
                    c.AddComponent<BadLuckAffectedChest>();
                }
                StaticReferenceManager.DChestsSpawnedOnFloor += dchests;
                return original;
            }
            else
            {
                return orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(BadLuckClover));
            return base.Drop(player);
        }
    }
}
