using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Bait : PassiveItem
    {
        public static void Init()
        {
            var numMimicsToSpawn = 4;
            var name = "Bait";
            var shortdesc = "Something's fishy...";
            var longdesc = $"Increases damage, but {numMimicsToSpawn} empty mimics will be created when entering the next room.\n\nThe fish on the hook isn't the catch, but just the bait.";
            var item = EasyItemInit<Bait>("bait", "bait_idle_001", name, shortdesc, longdesc, ItemQuality.C, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.35f, StatModifier.ModifyMethod.ADDITIVE);
            item.numEnemiesToSpawn = numMimicsToSpawn;
            item.onlySpawnOneTime = true;
            item.enemyPool = new() { BrownChestMimicGuid, BlueChestMimicGuid, GreenChestMimicGuid };
            item.spawnDelay = 0.3f;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnEnteredCombat += SpawnEnemies;
        }

        public void SpawnEnemies()
        {
            if((!spawnedEnemies || !onlySpawnOneTime) && enemyPool != null && enemyPool.Count > 0 && Owner != null && Owner.CurrentRoom != null && (chanceToSpawnEnemies >= 1f || Random.value <= chanceToSpawnEnemies))
            {
                var actualNumToSpawn = numEnemiesToSpawn + Random.Range(-numEnemiesToSpawnVariance, numEnemiesToSpawnVariance);
                if(actualNumToSpawn > 0)
                {
                    if (spawnDelay > 0f)
                    {
                        GameManager.Instance.Dungeon.StartCoroutine(SpawnEnemiesCR(enemyPool, actualNumToSpawn, spawnDelay, Owner.CurrentRoom, this));
                    }
                    else
                    {
                        for (int i = 0; i < actualNumToSpawn; i++)
                        {
                            if(SpawnSingleEnemy(EnemyDatabase.GetOrLoadByGuid(enemyPool.RandomElement()), Owner.CurrentRoom) != null)
                            {
                                spawnedEnemies = true;
                            }
                        }
                    }
                }
            }
        }

        public static AIActor SpawnSingleEnemy(AIActor enemy, RoomHandler room)
        {
            if(enemy == null || room == null)
            {
                return null;
            }
            var pos = room.GetRandomAvailableCell(enemy.Clearance, CellTypes.FLOOR, false, x =>
            {
                for (int j = 0; j < enemy.Clearance.x; j++)
                {
                    for (int k = 0; k < enemy.Clearance.y; k++)
                    {
                        if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(x.x + j, x.y + k) || GameManager.Instance.Dungeon.data.cellData[x.x + j][x.y + k].type == CellType.WALL || GameManager.Instance.Dungeon.data.isTopWall(x.x + j, x.y + k))
                        {
                            return false;
                        }
                        foreach (var play in GameManager.Instance.AllPlayers)
                        {
                            if (play != null && Vector2.Distance(play.CenterPosition, new(x.x + j, x.y + k)) < 4f)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            });
            if(pos != null)
            {
                var e = AIActor.Spawn(enemy, pos.GetValueOrDefault(), room, false, AIActor.AwakenAnimationType.Default, true);
                e.HandleReinforcementFallIntoRoom();
                e.IgnoreForRoomClear = false;
                return e;
            }
            return null;
        }

        public static IEnumerator SpawnEnemiesCR(List<string> enemyPool, int actualNumToSpawn, float delay, RoomHandler room, Bait item)
        {
            for (int i = 0; i < actualNumToSpawn; i++)
            {
                if(SpawnSingleEnemy(EnemyDatabase.GetOrLoadByGuid(enemyPool.RandomElement()), room) != null && item != null)
                {
                    item.spawnedEnemies = true;
                }
                yield return new WaitForSeconds(delay);
            }
            yield break;
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player != null)
            {
                player.OnEnteredCombat -= SpawnEnemies;
            }
        }

        public float chanceToSpawnEnemies = 1f;
        public bool onlySpawnOneTime;
        public bool spawnedEnemies;
        public int numEnemiesToSpawn;
        public int numEnemiesToSpawnVariance;
        public float spawnDelay;
        public List<string> enemyPool;
    }
}
