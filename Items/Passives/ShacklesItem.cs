using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class ShacklesItem : PassiveItem
    {
        public static void Init()
        {
            string name = "Convict's Shackles";
            string shortdesc = "Slowing Protection";
            string longdesc = "Grants armor, but slows the owner down. Losing armor speeds the owner up.\n\nHeavy iron shackles brought by the convict to the gungeon.";
            ShacklesItem item = ItemBuilder.EasyInit<ShacklesItem>("items/shackles", "sprites/shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.C, 525, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, -2f, StatModifier.ModifyMethod.ADDITIVE);
            item.CanBeDropped = false;
            item.AddToOldRedShop();
            item.AddToBlacksmithShop();
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.ITEMSPECIFIC_CONVICTS_SHACKLES, true);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.Armor += ArmorToGainOnPickup;
            player.LostArmor += SpeedUpOnDamage;
        }

        public void SpeedUpOnDamage()
        {
            m_owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, SpeedBoostOnDamage));
            m_owner.stats.RecalculateStats(m_owner, false, false);
        }

        public int ArmorToGainOnPickup;
        public float SpeedBoostOnDamage;
    }
}
