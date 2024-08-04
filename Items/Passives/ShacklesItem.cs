using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class ShacklesItem : PlayerItem
    {
        public static void Init()
        {
            var name = "Convict's Handcuffs";
            var shortdesc = "Unlocked... for now";
            var longdesc = "On use, the item is locked, granting a boost in damage but permanently capping and decreasing the owner's health.\n\nHandcuffs such as these are usually used by potatoes to empower themself for fighting aliens. It is unknown why handcuffs would ever grant a boost in power, but they do anyways.";
            var item = EasyItemInit<ShacklesItem>("items/shackles", "sprites/shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.A, 525, null);
            //item.rustedSprite = AddSpriteToCollection("rusted_shackles_idle_001", item.sprite.Collection);
            item.consumable = true;
            item.IgnoredByRat = true;
            item.CanBeSold = false;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            HealthCapItem.Init();
        }

        /*public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            //player.OnNewFloorLoaded += OnNewFloor;
        }*/

        /*public void OnNewFloor(PlayerController p)
        {
            //didRust = true;
            //sprite.SetSprite(rustedSprite);
        }*/

        /*public override void OnPreDrop(PlayerController user)
        {
            //user.OnNewFloorLoaded -= OnNewFloor;
            base.OnPreDrop(user);
        }*/

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            EncounterTrackable.SuppressNextNotification = true;
            LootEngine.GivePrefabToPlayer(GetItemById(ItemIds["passiveshackle"]).gameObject, user);
            /*if (didRust)
            {
                LootEngine.GivePrefabToPlayer(GetItemById(ItemIds["rustedpassiveshackle"]).gameObject, user);
            }
            else
            {
            }*/
        }

        //public bool didRust;
        //public int rustedSprite;
    }

    public class HealthCapItem : PassiveItem
    {
        public static void Init()
        {
            var name = "Convict's Shackles";
            var shortdesc = "Locked";
            var longdesc = "Grants a boost in damage but the owner's health is permanently capped and decreased.\n\nShackles such as these are usually used by potatoes to empower themself for fighting aliens. It is unknown why shackles would ever grant a boost in power, but they do anyways.";
            var item = EasyItemInit<HealthCapItem>("items/passiveshackle", "sprites/shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.EXCLUDED, null, $"{SpecialStuffModule.globalPrefix}:convicts_shackles_passive");
            //item.CanBeDropped = false;
            item.CanBeSold = false;
            item.AddPassiveStatModifier("FlatDamage", 5f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, -1f, StatModifier.ModifyMethod.ADDITIVE);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.CursedMaximum = player.healthHaver.GetMaxHealth();
        }

        public float DamageToAdd;
    }
}
