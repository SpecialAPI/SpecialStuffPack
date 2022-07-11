using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class ActiveOverchargeItem : PassiveItem
    {
        public static void Init()
        {
            string name = "Infinice";
            string shortdesc = "Infinitely Cool";
            string longdesc = "Active items can be infinitely overcharged.";
            ActiveOverchargeItem item = ItemBuilder.EasyInit<ActiveOverchargeItem>("items/infinice", "sprites/infinice_idle_001", name, shortdesc, longdesc, ItemQuality.B, SpecialStuffModule.globalPrefix, null, null);
            item.MaxOverchargeAmount = int.MaxValue;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }

        public int MaxOverchargeAmount;
    }
}
