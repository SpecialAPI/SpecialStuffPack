using Dungeonator;
using SpecialStuffPack.ItemAPI;
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
            BoxOfStuff item = EasyItemInit<BoxOfStuff>("items/boxofstuff", "sprites/spapis_stuff_idle_001.png", name, shortdesc, longdesc, ItemQuality.B, 644, null);
            item.SetCooldownType(CooldownType.Damage, 250f);
            item.Stuff = new List<GameObject>
            {
                GetItemById<FoldingTableItem>(644).TableToSpawn.gameObject,
                GetItemById<SpawnObjectPlayerItem>(71).objectToSpawn,
                GetItemById<SpawnObjectPlayerItem>(438).objectToSpawn,
                GetItemById<SpawnObjectPlayerItem>(108).objectToSpawn,
                GetItemById<SpawnObjectPlayerItem>(109).objectToSpawn,
                GetItemById<SpawnObjectPlayerItem>(66).objectToSpawn,
                GetItemById<SpawnObjectPlayerItem>(308).objectToSpawn,
                EnemyDatabase.GetOrLoadByGuid(GetItemById<SpawnObjectPlayerItem>(201).enemyGuidToSpawn).gameObject,
                GetItemById<MagazineRackItem>(814).MagazineRackPrefab,
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
            item.TrashSynergyAmount = 3;
            item.TrashSynergyProjectile = TrashcannonObject.GetProjectile();
            item.ArmorChance = 0.01f;
            item.AmmoChance = 0.04f;
        }

        public override void DoEffect(PlayerController user)
        {
            user.StartCoroutine(SpawnStuffCR(user));
        }

        private IEnumerator SpawnStuffCR(PlayerController user)
        {
            int stuffToSpawn = Random.Range(MinStuff, MaxStuff);
            if(user.PlayerHasActiveSynergy("v3.0 The Massive Update"))
            {
                stuffToSpawn = Random.Range(MinStuffSynergy, MaxStuffSynergy);
            }
            for (int i = 0; i < stuffToSpawn; i++)
            {
                IntVector2? spawnPos = user.CurrentRoom.GetRandomAvailableCell(new IntVector2(1, 1), CellTypes.FLOOR, false, null);
                if (!spawnPos.HasValue)
                {
                    continue;
                }
                int overrideItemId = -1;
                if (user.PlayerHasActiveSynergy("QoL"))
                {
                    var chance = Random.value;
                    if(chance < ArmorChance)
                    {
                        overrideItemId = ArmorId;
                    }
                    else if(chance < ArmorChance + AmmoChance)
                    {
                        overrideItemId = BraveUtility.RandomBool() ? AmmoId : SpreadAmmoId;
                    }
                }
                if(overrideItemId >= 0)
                {
                    LootEngine.SpawnItem(GetItemById(overrideItemId).gameObject, spawnPos.Value.ToVector3(), Vector2.down, 0f, false, true, false);
                }
                else
                {
                    GameObject toSpawn = BraveUtility.RandomElement(Stuff);
                    GameObject go = Instantiate(toSpawn, spawnPos.Value.ToVector2(), Quaternion.identity);
                    PortableTurretController turret = go.GetComponent<PortableTurretController>();
                    if (turret)
                    {
                        turret.sourcePlayer = user;
                        turret.GetComponent<AIActor>().EffectResistances = turret.GetComponent<AIActor>().EffectResistances.AddToArray(new ActorEffectResistance { resistType = EffectResistanceType.Freeze, resistAmount = 1 });
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
            if (user.PlayerHasActiveSynergy("SpecialUtils"))
            {
                for(int i = 0; i < TrashSynergyAmount; i++)
                {
                    IntVector2? randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2(1, 1), CellTypes.FLOOR, false, null);
                    if (randomAvailableCell.HasValue)
                    {
                        OwnedShootProjectile(TrashSynergyProjectile, randomAvailableCell.Value.ToCenterVector2(), 0f, user).baseData.speed = 0f;
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(randomAvailableCell.Value.ToCenterVector2(), 1.5f, 0.5f, false);
                        yield return new WaitForSeconds(0.1f);
                    }
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
        public int TrashSynergyAmount;
        public Projectile TrashSynergyProjectile;
        public float ArmorChance;
        public float AmmoChance;
    }
}
