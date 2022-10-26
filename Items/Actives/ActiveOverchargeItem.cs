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
            ActiveOverchargeItem item = EasyItemInit<ActiveOverchargeItem>("items/infinice", "sprites/infinice_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
            item.MaxOverchargeAmount = int.MaxValue;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            base.DisableEffect(disablingPlayer);
        }

        public int MaxOverchargeAmount;
    }
}
