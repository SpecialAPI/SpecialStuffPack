using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;

namespace SpecialStuffPack.Items.Passives
{
    public class Anvil : PassiveItem
    {
        public static void Init()
        {
            var name = "Anvil";
            var shortdesc = "Weapon Upgrade Every Shop";
            var longdesc = "Upgrades a random gun when entering a shop for the first time, destroying it and giving a random gun that's 1 tier higher.";
            var item = EasyItemInit<Anvil>("anvil", "anvil_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.Ext().OnEnteredRoom += MaybeUpgrade;
            player.Ext().ModifyPriceMult += ArmsDealerSynergy;
        }

        public float ArmsDealerSynergy(PlayerController p, ShopItemController item)
        {
            return item.item != null && item.item is Gun && p.PlayerHasActiveSynergy("Arms Dealer") ? 0.5f : 1f;
        }

        public void MaybeUpgrade(PlayerController p, RoomHandler r, RoomHandler.VisibilityStatus visibility)
        {
            if(visibility == RoomHandler.VisibilityStatus.OBSCURED && r.area != null && r.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL && r.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP)
            {
                var upgradeMessages = new List<string> { "Upgrade complete!" };
                var upgradeCount = p.PlayerHasActiveSynergy("Double Anvil") ? 2 : 1;
                var tierIncrement = p.PlayerHasActiveSynergy("Expert Blacksmith") ? 2 : 1;
                if (p.CharacterUsesRandomGuns)
                {
                    p.ChangeToRandomGun();
                }
                for (int i = 0; i < upgradeCount; i++)
                {
                    if (p.CharacterUsesRandomGuns)
                    {
                        var armorcount = 2 * tierIncrement;
                        for (int j = 0; j < armorcount; j++)
                        {
                            LootEngine.GivePrefabToPlayer(ArmorObject.gameObject, p);
                        }
                        upgradeMessages.Add($"+{armorcount} Armor");
                        continue;
                    }
                    var possibleGuns =
                            p.inventory.AllGuns
                            .Concat(
                                StaticReferenceManager.AllDebris
                                .Where(x => x != null && x.GetComponent<Gun>() != null && x.GetComponent<Gun>().HasBeenPickedUp && x.GetComponent<Gun>().CurrentOwner == null)
                                .Select(x => x.GetComponent<Gun>())
                            ).Where(x =>
                                x != null &&
                                (x.CurrentOwner == null || x.CanActuallyBeDropped(p)) &&
                                x.quality < ItemQuality.S && x.quality >= ItemQuality.D &&
                                !x.IsBeingEyedByRat);
                    if (possibleGuns.Any())
                    {
                        var random = possibleGuns.RandomElement();
                        var tier = random.quality + tierIncrement;
                        var givenArmor = false;
                        if (tier > ItemQuality.S)
                        {
                            tier = ItemQuality.S;
                            LootEngine.GivePrefabToPlayer(ArmorObject.gameObject, p);
                            LootEngine.GivePrefabToPlayer(ArmorObject.gameObject, p);
                            givenArmor = true;
                        }
                        var ammoPercentage = random.InfiniteAmmo ? 1f : (float)random.ammo / random.AdjustedMaxAmmo;
                        var gun1name = random.DisplayName;
                        if (random.CurrentOwner != null)
                        {
                            p.inventory.RemoveGunFromInventory(random);
                        }
                        Destroy(random.gameObject);
                        var g = GameManager.Instance.RewardManager.GetItemForPlayer(p, GameManager.Instance.RewardManager.GunsLootTable, tier, null, false, null, false, null, false, RewardManager.RewardSource.UNSPECIFIED);
                        if (g != null)
                        {
                            string gun2name = null;
                            if (g.GetComponent<Gun>())
                            {
                                var gun = LootEngine.TryGiveGunToPlayer(g, p, false);
                                if (gun != null)
                                {
                                    if (!gun.InfiniteAmmo)
                                    {
                                        gun.ammo = Mathf.Max(0, Mathf.FloorToInt(gun.AdjustedMaxAmmo * ammoPercentage));
                                    }
                                    gun2name = gun.DisplayName;
                                }
                            }
                            else if (g.GetComponent<PickupObject>() != null) //spice
                            {
                                LootEngine.GivePrefabToPlayer(g, p);
                                if (g.GetComponent<EncounterTrackable>() != null)
                                {
                                    gun2name = $"{g.GetComponent<EncounterTrackable>().GetModifiedDisplayName()}..?";
                                }
                            }
                            if (gun2name != null)
                            {
                                upgradeMessages.Add($"{gun1name} -> {gun2name}");
                            }
                        }
                        if (givenArmor)
                        {
                            upgradeMessages.Add("+2 Armor");
                        }
                    }
                    else
                    {
                        var armorcount = 2 * tierIncrement;
                        for (int j = 0; j < armorcount; j++)
                        {
                            LootEngine.GivePrefabToPlayer(ArmorObject.gameObject, p);
                        }
                        upgradeMessages.Add($"+{armorcount} Armor");
                    }
                }
                if (upgradeMessages.Count > 1)
                {
                    DoEpicAnnouncementChain(p.sprite.WorldTopCenter + Vector2.up * 0.5f, upgradeMessages, null, 0.5f, 4f, 2f);
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player != null)
            {
                player.Ext().OnEnteredRoom -= MaybeUpgrade;
                player.Ext().ModifyPriceMult -= ArmsDealerSynergy;
            }
        }
    }
}
