using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Chaos : PassiveItem
    {
        public static void Init()
        {
            string name = "CHAOS";
            string shortdesc = "More items, but less";
            string longdesc = "Items are tripled, but collecting an item will remove one of the owner's collected items";
            EasyInitItem<Chaos>("items/chaos", "sprites/chaos_idle_001", name, shortdesc, longdesc, ItemQuality.S, null, null).AddToGooptonShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, GetType());
        }

        public override void DisableEffect(PlayerController player)
        {
            DecrementFlag(player, GetType());
        }
    }
}
