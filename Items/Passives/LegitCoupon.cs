using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class LegitCoupon : PassiveItem
    {
        public static void Init()
        {
            string name = "Totally Legit Coupon";
            string shortdesc = "Free Stuff :)";
            string longdesc = "Makes all items from shops free.\n\nA crumpled and partially torn piece of paper that says \"-100%\". Seems fake.";
            LegitCoupon item = ItemBuilder.EasyInit<LegitCoupon>("items/legitcoupon", "sprites/legit_coupon_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, 353, null);
            item.priceIncreasePerPurchase = 0.2f;
            item.freeTakeChance = 0.5f;
            item.AddPassiveStatModifier(PlayerStats.StatType.GlobalPriceMultiplier, -1f, StatModifier.ModifyMethod.ADDITIVE);
            item.ShouldBeExcludedFromShops = true;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnItemPurchased += HandlePriceIncrease;
        }

        public void HandlePriceIncrease(PlayerController player, ShopItemController item)
        {
            if(!player.PlayerHasActiveSynergy("Take It Before They Notice") || UnityEngine.Random.value > freeTakeChance)
            {
                //CanBeDropped = false;
                player.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.GlobalPriceMultiplier, StatModifier.ModifyMethod.ADDITIVE, priceIncreasePerPurchase));
                player.stats.RecalculateStats(player, false, false);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnItemPurchased -= HandlePriceIncrease;
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            if(m_owner != null)
            {
                m_owner.OnItemPurchased -= HandlePriceIncrease;
            }
            base.OnDestroy();
        }

        public float priceIncreasePerPurchase;
        public float freeTakeChance;
    }
}
