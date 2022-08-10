using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class FishingReward : ScriptableObject
    {
        public FishingReward(bool isItem = false)
        {
            isRoomReward = !isItem;
            isRandomItem = isItem;
        }

        public FishingReward(string enemyGuid)
        {
            FishEnemyGUID = enemyGuid;
        }

        public FishingReward(int itemId)
        {
            FishItemId = itemId;
        }

        public bool isRandomItem;
        public bool isRoomReward;
        public string FishEnemyGUID;
        public int FishItemId;
    }
}
