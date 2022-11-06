using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class RustyAmmoBox : PassiveItem
    {
        public static void Init()
        {
            string name = "Rusty Ammo Box";
            string shortdesc = "The Inevitability of Rust";
            string longdesc = "Guns without ammo are automatically destroyed, dropping items in the process.\n\nDefinitely the best Inventor episode.";
            var item = EasyItemInit<RustyAmmoBox>("rustyammobox", "rusty_ammo_box_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
            item.BadRewardTable = new()
            {
                name = "Rusty Ammo Box Bad Reward",
                includedLootTables = new(),
                defaultItemDrops = new()
                {
                    elements = new()
                    {
                        new()
                        {
                            pickupId = ArmorId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = ChaffGrenadeId,
                            weight = 0.5f
                        },
                        new()
                        {
                            pickupId = SupplyDropId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = ArmorId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = MeatbunId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = GoldCasingId,
                            weight = 0.5f
                        },
                        new()
                        {
                            pickupId = IronCoinId,
                            weight = 0.5f
                        },
                        new()
                        {
                            pickupId = MapId,
                            weight = 1f
                        },
                    },
                }
            };
            item.GoodRewardTable = new()
            {
                name = "Rusty Ammo Box Good Reward",
                includedLootTables = new(),
                defaultItemDrops = new()
                {
                    elements = new()
                    {
                        new()
                        {
                            pickupId = DuctTapeId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = MedkitId,
                            weight = 0.5f
                        },
                        new()
                        {
                            pickupId = RationId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = ShadowCloneId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = OldKnightsHelmId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = Plus1BulletsId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = BoxId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = HeartSynthesizerId,
                            weight = 1f
                        },
                        new()
                        {
                            pickupId = ArmorSynthesizerId,
                            weight = 1f
                        }
                    },
                }
            };
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                foreach (var gun in player.inventory.AllGuns)
                {
                    if (IsGunValid(gun))
                    {
                        gun.GainAmmo(1);
                    }
                }
            }
            else
            {
                foreach (var gun in player.inventory.AllGuns)
                {
                    if (IsGunValid(gun))
                    {
                        ScrapGun(gun);
                    }
                }
            }
            base.Pickup(player);
        }

        public override void Update()
        {
            base.Update();
            if (PickedUp && Owner != null && Owner.CurrentGun != null && IsGunValid(Owner.CurrentGun))
            {
                ScrapGun(Owner.CurrentGun);
            }
        }

        public void ScrapGun(Gun gun)
        {
            var table = GoodRewardTable;
            if(gun.quality < ItemQuality.B)
            {
                table = BadRewardTable;
            }
            LootEngine.SpawnItem(table.SelectByWeight(), Owner.CenterPosition, Random.insideUnitCircle, 2f, false, true, false);
            if(Owner.PlayerHasActiveSynergy("Recycling at its Finest"))
            {
                LootEngine.SpawnItem(AmmoObject.gameObject, Owner.CenterPosition, Random.insideUnitCircle, 2f, false, true, false);
            }
            Owner.inventory.DestroyGun(gun);
            AkSoundEngine.PostEvent("StatusRust", Owner.gameObject);
        }

        public bool IsGunValid(Gun gun)
        {
            return gun != null && gun.ammo == 0 && gun.CanGainAmmo && gun.CurrentOwner as PlayerController != null && gun.CanActuallyBeDropped(gun.CurrentOwner as PlayerController) && !gun.InfiniteAmmo;
        }

        public GenericLootTable BadRewardTable;
        public GenericLootTable GoodRewardTable;
    }
}
