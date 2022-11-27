using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class DefaultLootMods : MonoBehaviour
    {
        public void Start()
        {
            if(lootMods != null)
            {
                var play = GetComponent<PlayerController>();
                if (play != null)
                {
                    (play.lootModData ??= new()).AddRange(lootMods);
                }
            }
        }

        public List<LootModData> lootMods;
    }
}
