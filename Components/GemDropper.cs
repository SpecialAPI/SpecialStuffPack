using SpecialStuffPack.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class GemDropper : BraveBehaviour
    {
        public GemDropper()
        {
            AdditionalItems = new List<int>();
        }

        public void Start()
        {
            healthHaver.OnDeath += DropGem;
        }

        public void DropGem(Vector2 v)
        {
            if (UseDeathPosition)
            {
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(GemId).gameObject, healthHaver.specRigidbody.UnitCenter, Vector2.down, 0f, false, false, false);
                return;
            }
            Vector3 spawnPosition = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomAvailableCell(new IntVector2(1, 1), Dungeonator.CellTypes.FLOOR, false, null).GetValueOrDefault().ToVector3();
            if (IsRNGBossRoom)
            {
                spawnPosition = new IntVector2(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetCenterCell().x, GameManager.Instance.PrimaryPlayer.CurrentRoom.area.basePosition.y + ((GameManager.Instance.PrimaryPlayer.CurrentRoom.GetCenterCell().y - 
                    GameManager.Instance.PrimaryPlayer.CurrentRoom.area.basePosition.y) / 2)).ToVector3();
            }
            if(AdditionalItems != null && AdditionalItems.Count > 0)
            {
                if (IsRNGBossRoom)
                {
                    List<int> itemsToDrop = new List<int>(AdditionalItems);
                    itemsToDrop.Add(GemId);
                    for(int i = 0; i < itemsToDrop.Count; i++)
                    {
                        int itemToDrop = itemsToDrop[i];
                        Vector2 actualSpawnPosition = spawnPosition + new Vector3((i + 1 - ((itemsToDrop.Count + 1) / 2f)) * 1.5f, 0f, 0f);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(itemToDrop).gameObject, actualSpawnPosition, Vector3.down, 0f, true, true, false);
                    }
                }
                else
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(GemId).gameObject, spawnPosition, Vector3.down, 0f, true, true, false);
                    foreach (int additionalItem in AdditionalItems)
                    {
                        Vector3 spawnPosition2 = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomAvailableCell(new IntVector2(1, 1), Dungeonator.CellTypes.FLOOR, false, null).GetValueOrDefault().ToVector3();
                        DebrisObject d = LootEngine.SpawnItem(PickupObjectDatabase.GetById(additionalItem).gameObject, spawnPosition2, Vector3.down, 0f, true, true, false);
                        GoldKey key;
                        if((key = d.GetComponent<GoldKey>()) != null)
                        {
                            key.RecreateGhostOnPickup();
                        }
                    }
                }
            }
            else
            {
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(GemId).gameObject, spawnPosition, Vector3.down, 0f, true, true, false);
            }
        }

        public int GemId;
        public bool UseDeathPosition;
        public bool IsRNGBossRoom;
        [NonSerialized]
        public List<int> AdditionalItems;
    }
}
