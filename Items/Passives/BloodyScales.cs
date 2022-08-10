using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class BloodyScales : PassiveItem
    {
        public static bool ACTIVATING;

        public static void Init()
        {
            string name = "Bloody Scales";
            string shortdesc = "A gamble...";
            string description = "All items are doubled, but taking damage has a low chance to be lethal. Having more items increases that chance";
            var item = ItemBuilder.EasyInit<BloodyScales>("items/bloodyscales", "sprites/bloody_scales_idle_001", name, shortdesc, description, ItemQuality.A, null, null);
            item.CanBeDropped = false;
            item.AddToCursulaShop();
            item.AddToFlyntShop();
            new Hook(typeof(PassiveItem).GetMethod(nameof(PassiveItem.Pickup)), typeof(BloodyScales).GetMethod(nameof(BloodyScales.DoubleItem)));
            new Hook(typeof(Gun).GetMethod(nameof(Gun.Pickup)), typeof(BloodyScales).GetMethod(nameof(BloodyScales.DoubleGun)));

        }

        public static void DoubleItem(Action<PassiveItem, PlayerController> orig, PassiveItem self, PlayerController player)
        {
            var pickedUp = self.m_pickedUpThisRun;
            List<PassiveItem> playerItems = new(player.passiveItems);
            playerItems.RemoveAll(x => x.quality == ItemQuality.COMMON && x.quality == ItemQuality.SPECIAL && x.quality == ItemQuality.EXCLUDED);
            playerItems.AddRange(StaticReferenceManager.AllDebris.FindAll(x => x.GetComponent<PassiveItem>() != null).ConvertAll(x => x.GetComponent<PassiveItem>()).Where(x => x != null && x.m_pickedUpThisRun && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL && 
                self.quality != ItemQuality.EXCLUDED && !x.PickedUp));
            playerItems.RemoveAll(x => x == null || x is Chaos || x.m_isBeingEyedByRat || (x.Owner != null && !x.CanActuallyBeDropped(x.Owner)));
            playerItems.Remove(self);
            orig(self, player);
            if (IsFlagSetForCharacter(player, typeof(BloodyScales)) && !pickedUp && !ACTIVATING && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL && self.quality != ItemQuality.EXCLUDED)
            {
                ACTIVATING = true;
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<PassiveItem>(self.quality, GameManager.Instance.RewardManager.ItemsLootTable, false).gameObject, player);
                ACTIVATING = false;
            }
            if(IsFlagSetForCharacter(player, typeof(Chaos)) && !pickedUp && !ACTIVATING && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL && self.quality != ItemQuality.EXCLUDED)
            {
                ACTIVATING = true;
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<PassiveItem>(self.quality, GameManager.Instance.RewardManager.ItemsLootTable, false).gameObject, player);
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<PassiveItem>(self.quality, GameManager.Instance.RewardManager.ItemsLootTable, false).gameObject, player);
                playerItems.RemoveAll(x => x == null || (x.Owner != null && !x.CanActuallyBeDropped(x.Owner)));
                if(playerItems.Count > 0)
                {
                    var randomitem = playerItems.RandomElement();
                    if(randomitem != null)
                    {
                        if (randomitem.PickedUp && randomitem.Owner != null)
                        {
                            var drop = randomitem.Owner.DropPassiveItem(randomitem).GetComponent<PassiveItem>();
                            drop.m_pickedUp = true;
                            Destroy(drop.gameObject, 1f);
                        }
                        else
                        {
                            LootEngine.DoDefaultItemPoof(randomitem.sprite.WorldCenter, false, false);
                            Destroy(randomitem.gameObject);
                        }
                    }
                }
                ACTIVATING = false;
            }
        }

        public static void DoubleGun(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            var pickedUp = self.HasEverBeenAcquiredByPlayer;
            List<Gun> guns = new(player.inventory.AllGuns);
            guns.RemoveAll(x => x.quality == ItemQuality.COMMON && x.quality == ItemQuality.SPECIAL && x.quality == ItemQuality.EXCLUDED);
            guns.AddRange(StaticReferenceManager.AllDebris.FindAll(x => x.GetComponentInChildren<Gun>() != null).ConvertAll(x => x.GetComponentInChildren<Gun>()).Where(x => x != null && x.HasEverBeenAcquiredByPlayer && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL &&
                self.quality != ItemQuality.EXCLUDED && x.CurrentOwner == null));
            guns.RemoveAll(x => x == null || x.m_isBeingEyedByRat || (x.CurrentOwner != null && x.CurrentOwner is PlayerController && !x.CanActuallyBeDropped(x.CurrentOwner as PlayerController)));
            guns.Remove(self);
            orig(self, player);
            if (IsFlagSetForCharacter(player, typeof(BloodyScales)) && !pickedUp && !ACTIVATING && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL && self.quality != ItemQuality.EXCLUDED)
            {
                ACTIVATING = true;
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<Gun>(self.quality, GameManager.Instance.RewardManager.GunsLootTable, false).gameObject, player);
                ACTIVATING = false;
            }
            if (IsFlagSetForCharacter(player, typeof(Chaos)) && !pickedUp && !ACTIVATING && self.quality != ItemQuality.COMMON && self.quality != ItemQuality.SPECIAL && self.quality != ItemQuality.EXCLUDED)
            {
                ACTIVATING = true;
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<Gun>(self.quality, GameManager.Instance.RewardManager.GunsLootTable, false).gameObject, player);
                LootEngine.GivePrefabToPlayer(LootEngine.GetItemOfTypeAndQuality<Gun>(self.quality, GameManager.Instance.RewardManager.GunsLootTable, false).gameObject, player);
                guns.RemoveAll(x => x == null || (x.CurrentOwner != null && x.CurrentOwner is PlayerController && !x.CanActuallyBeDropped(x.CurrentOwner as PlayerController)));
                if (guns.Count > 0)
                {
                    var randomitem = guns.RandomElement();
                    if (randomitem != null)
                    {
                        if (randomitem.CurrentOwner != null && randomitem.CurrentOwner is PlayerController)
                        {
                            player.inventory.DestroyGun(randomitem);
                        }
                        else
                        {
                            LootEngine.DoDefaultItemPoof(randomitem.sprite.WorldCenter, false, false);
                            if (randomitem.transform.parent != null)
                            {
                                Destroy(randomitem.transform.parent.gameObject);
                            }
                            else
                            {
                                Destroy(randomitem.gameObject);
                            }
                        }
                    }
                }
                ACTIVATING = false;
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += ChanceToKill;
            IncrementFlag(player, GetType());
        }

        public static void ChanceToKill(PlayerController player)
        {
            var chance = 1f + (player.passiveItems.Count + player.activeItems.Count + player.inventory.AllGuns.Count) / 2f;
            if(Random.value <= chance / 100f)
            {
                player.healthHaver.ForceSetCurrentHealth(0f);
                player.healthHaver.Armor = 0;
                if (!player.healthHaver.SuppressDeathSounds)
                {
                    AkSoundEngine.PostEvent("Play_ENM_death", player.healthHaver.gameObject);
                    AkSoundEngine.PostEvent(string.IsNullOrEmpty(player.healthHaver.overrideDeathAudioEvent) ? "Play_CHR_general_death_01" : player.healthHaver.overrideDeathAudioEvent, player.healthHaver.gameObject);
                }
                player.healthHaver.Die(Vector3.zero);
            }
        }

        public override void OnDestroy()
        {
            if(Owner != null)
            {
                Owner.OnReceivedDamage -= ChanceToKill;
                DecrementFlag(Owner, GetType());
            }
        }
    }
}
