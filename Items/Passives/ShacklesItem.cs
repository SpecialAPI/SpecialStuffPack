using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SaveAPI;
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
            string name = "Convict's Shackles";
            string shortdesc = "Unlocked... for now";
            string longdesc = "On use, the item is locked, granting a boost in damage but permanently capping and decreasing the owner's health. Becomes rusted when the owner goes down a floor. If locked while rusted, the damage boost" +
                " becomes significantly smaller and the health is further decreased.\n\nShackles such as these are usually used by potatoes to empower themself for fighting aliens. It is unknown why shackles would ever grant a " +
                "boost in power, but they do anyways.";
            ShacklesItem item = EasyItemInit<ShacklesItem>("items/shackles", "sprites/shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.A, 525, null);
            item.rustedSprite = AddSpriteToCollection("rusted_shackles_idle_001", item.sprite.Collection);
            item.consumable = true;
            item.IgnoredByRat = true;
            item.CanBeSold = false;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            item.SetupUnlockOnCustomCharacterSpecificFlag("BeatBossrush", PlayableCharacters.Convict);
            PassiveShackle.Init();
            PassiveShackle.InitRusted();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += OnNewFloor;
        }

        public void OnNewFloor(PlayerController p)
        {
            didRust = true;
            sprite.SetSprite(rustedSprite);
        }

        public override void OnPreDrop(PlayerController user)
        {
            user.OnNewFloorLoaded -= OnNewFloor;
            base.OnPreDrop(user);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            EncounterTrackable.SuppressNextNotification = true;
            if (didRust)
            {
                LootEngine.GivePrefabToPlayer(GetItemById(ItemIds["rustedpassiveshackle"]).gameObject, user);
            }
            else
            {
                LootEngine.GivePrefabToPlayer(GetItemById(ItemIds["passiveshackle"]).gameObject, user);
            }
        }

        public bool didRust;
        public int rustedSprite;
    }

    public class PassiveShackle : PassiveItem
    {
        public static void Init()
        {
            string name = "Convict's Shackles";
            string shortdesc = "Locked";
            string longdesc = "Grants a boost in damage but the owner's health is permanently capped and decreased.\n\nShackles such as these are usually used by potatoes to empower themself for fighting aliens. It is unknown why " +
                "shackles would ever grant a boost in power, but they do anyways.";
            PassiveShackle item = EasyItemInit<PassiveShackle>("items/passiveshackle", "sprites/shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.EXCLUDED, null, $"{SpecialStuffModule.globalPrefix}:convicts_shackles_passive");
            item.CanBeDropped = false;
            item.CanBeSold = false;
            item.DamageToAdd = 4.5f;
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, -1f, StatModifier.ModifyMethod.ADDITIVE);
        }

        public static void InitRusted()
        {
            string name = "Convict's Shackles";
            string shortdesc = "Rusted";
            string longdesc = "Grants a small boost in damage but the owner's health is permanently capped and decreased.\n\nShackles such as these are usually used by potatoes to empower themself for fighting aliens. It is unknown why " +
                "shackles would ever grant a boost in power, but they do anyways.";
            PassiveShackle item = EasyItemInit<PassiveShackle>("items/rustedpassiveshackle", "sprites/rusted_shackles_idle_001.png", name, shortdesc, longdesc, ItemQuality.EXCLUDED, null, $"{SpecialStuffModule.globalPrefix}:rusted_convicts_shackles_passive");
            item.CanBeDropped = false;
            item.CanBeSold = false;
            item.DamageToAdd = 1.5f;
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, -1.5f, StatModifier.ModifyMethod.ADDITIVE);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.CursedMaximum = player.healthHaver.GetMaxHealth();
            player.PostProcessProjectile += AddDamage;
        }

        public void AddDamage(Projectile p, float scale)
        {
            p.baseData.damage += DamageToAdd * scale;
        }

        public float DamageToAdd;
    }
}
