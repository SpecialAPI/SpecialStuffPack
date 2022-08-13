using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BossChest : PassiveItem
    {
        public static void Init()
        {
            string name = "Boss Chest";
            string shortdesc = "Your enemies are chests!";
            string longdesc = "Bosses drop an item on defeat.\n\nIf only there was a boss key for this chest...";
            EasyInitItem<BossChest>("items/bosschest", "sprites/boss_chest_idle_001.png", name, shortdesc, longdesc, ItemQuality.S, null, null).AddToFlyntShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnAnyEnemyReceivedDamage += StoreLastDamagePosition;
            player.OnRoomClearEvent += BossRoomCleared;
        }

        public void StoreLastDamagePosition(float damage, bool fatal, HealthHaver hh)
        {
            if (fatal)
            {
                lastBossKilledPos = hh.specRigidbody.UnitCenter;
            }
        }

        public void BossRoomCleared(PlayerController player)
        {
            if(player.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
            {
                return;
            }
            if (Owner.PlayerHasActiveSynergy("Fully Unlocked"))
            {
                Chest.Spawn(GameManager.Instance.RewardManager.Rainbow_Chest, lastBossKilledPos, player.CurrentRoom, true);
                Owner.RemovePassiveItem(PickupObjectId);
            }
            else
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomItem(lastBossKilledPos);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage -= StoreLastDamagePosition;
            player.OnRoomClearEvent -= BossRoomCleared;
        }

        public Vector2 lastBossKilledPos;
    }
}
