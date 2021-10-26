using Dungeonator;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SaveAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class BoxOfStuff : PlayerItem
    {
        public static void Init()
        {
            string name = "Special API's Stuff";
            string shortdesc = "Fill the room with whatever";
            string longdesc = "A box filled with various stuff. It refills itself over time.\n\nMade by a floating circle with eyes that hates something called \"encounter guids\". Truly a weird individual.";
            BoxOfStuff item = ItemBuilder.EasyInit<BoxOfStuff>("items/boxofstuff", "sprites/spapis_stuff_idle_001.png", name, shortdesc, longdesc, ItemQuality.B, SpecialStuffModule.globalPrefix, 644, null);
            item.SetCooldownType(ItemBuilder.CooldownType.Damage, 250f);
            item.Stuff = new List<GameObject>
            {
                CodeShortcuts.GetItemById<FoldingTableItem>(644).TableToSpawn.gameObject,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(71).objectToSpawn,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(438).objectToSpawn,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(108).objectToSpawn,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(109).objectToSpawn,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(66).objectToSpawn,
                CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(308).objectToSpawn,
                EnemyDatabase.GetOrLoadByGuid(CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(201).enemyGuidToSpawn).gameObject,
                CodeShortcuts.GetItemById<MagazineRackItem>(814).MagazineRackPrefab,
                LoadHelper.LoadAssetFromAnywhere<GameObject>("Red Barrel"),
                LoadHelper.LoadAssetFromAnywhere<GameObject>("Red Drum"),
                LoadHelper.LoadAssetFromAnywhere<GameObject>("Blue Drum"),
                LoadHelper.LoadAssetFromAnywhere<GameObject>("Purple Drum"),
                LoadHelper.LoadAssetFromAnywhere<GameObject>("Yellow Drum")
            };
            item.MinStuff = 25;
            item.MaxStuff = 35;
            item.MinStuffSynergy = 40;
            item.MaxStuffSynergy = 55;
        }

        protected override void DoEffect(PlayerController user)
        {
            user.StartCoroutine(SpawnStuffCR(user));
        }

        private IEnumerator SpawnStuffCR(PlayerController user)
        {
            int stuffToSpawn = UnityEngine.Random.Range(MinStuff, MaxStuff);
            if(user.PlayerHasActiveSynergy("v3.0 The Massive Update"))
            {
                stuffToSpawn = UnityEngine.Random.Range(MinStuffSynergy, MaxStuffSynergy);
            }
            for (int i = 0; i < stuffToSpawn; i++)
            {
                GameObject toSpawn = BraveUtility.RandomElement(Stuff);
                IntVector2? randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2(1, 1), CellTypes.FLOOR, false, null);
                if (randomAvailableCell != null)
                {
                    GameObject go = Instantiate(toSpawn, randomAvailableCell.Value.ToVector2(), Quaternion.identity);
                    PortableTurretController turret = go.GetComponent<PortableTurretController>();
                    if (turret)
                    {
                        turret.sourcePlayer = user;
                        SaveTools.Add(ref turret.GetComponent<AIActor>().EffectResistances, new ActorEffectResistance { resistType = EffectResistanceType.Freeze, resistAmount = 1 });
                    }
                    else
                    {
                        foreach (IPlaceConfigurable configurable in go.GetInterfacesInChildren<IPlaceConfigurable>())
                        {
                            configurable.ConfigureOnPlacement(user.CurrentRoom);
                        }
                    }
                    Projectile proj = go.GetComponentInChildren<Projectile>();
                    if (proj)
                    {
                        proj.Owner = user;
                        proj.TreatedAsNonProjectileForChallenge = true;
                    }
                    SpawnObjectItem objectItem = go.GetComponentInChildren<SpawnObjectItem>();
                    if (objectItem)
                    {
                        objectItem.SpawningPlayer = user;
                    }
                    foreach (SpeculativeRigidbody body in go.GetComponentsInChildren<SpeculativeRigidbody>())
                    {
                        body.Initialize();
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(body, null, false);
                    }
                    foreach (IPlayerInteractable interactable in go.GetInterfacesInChildren<IPlayerInteractable>())
                    {
                        user.CurrentRoom.RegisterInteractable(interactable);
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && user.CurrentRoom != null;
        }

        public List<GameObject> Stuff;
        public int MinStuff;
        public int MaxStuff;
        public int MinStuffSynergy;
        public int MaxStuffSynergy;
    }
}
