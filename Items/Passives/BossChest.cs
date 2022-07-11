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
            ItemBuilder.EasyInit<BossChest>("items/bosschest", "sprites/boss_chest_idle_001.png", name, shortdesc, longdesc, ItemQuality.S, SpecialStuffModule.globalPrefix, null, null).AddToFlyntShop();
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
            GameManager.Instance.RewardManager.SpawnTotallyRandomItem(lastBossKilledPos);
        }

        public override void OnDestroy()
        {
            if(Owner != null)
            {
                Owner.OnAnyEnemyReceivedDamage -= StoreLastDamagePosition;
                Owner.OnRoomClearEvent -= BossRoomCleared;
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage -= StoreLastDamagePosition;
            player.OnRoomClearEvent -= BossRoomCleared;
            return base.Drop(player);
        }

        public Vector2 lastBossKilledPos;
    }
}
