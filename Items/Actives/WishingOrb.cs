using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SaveAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class WishingOrb : PlayerItem
    {
        public static void Init()
        {
            string name = "Wishing Orb";
            string shortdesc = "Grants Wishes... For a Cost";
            string longdesc = "Consumes a part of user's soul to grant a wish. Will get more and more unstable with each use. Are you sure you want to use it?\n\nA mysterious glowing orb. Powerful magic emanates from inside.";
            WishingOrb item = EasyInitItem<WishingOrb>("items/wishorb", "sprites/wishing_orb_idle_001", name, shortdesc, longdesc, ItemQuality.B, 814, null);
            item.SetCooldownType(CooldownType.Timed, 0.5f);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.FlashColor = new Color(0f, 1f, 1f);
            item.ErrorFlashColor = Color.red;
            item.ChestChances = new WeightedGameObjectCollection
            {
                elements = new List<WeightedGameObject>
                {
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.D_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 0.1f
                    },
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.C_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 0.35f
                    },
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.B_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 1f
                    },
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.A_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 1f
                    },
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.S_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 0.65f
                    },
                    new WeightedGameObject
                    {
                        rawGameObject = GameManager.Instance.RewardManager.Rainbow_Chest.gameObject,
                        pickupId = -1,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        forceDuplicatesPossible = true,
                        weight = 0.01f
                    }
                }
            };
            item.StarGunId = ItemIds["shooting_star"];
            item.AddToCursulaShop();
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            AkSoundEngine.PostEvent("Play_OBJ_time_bell_01", gameObject);
            RenderSettings.ambientLight = FlashColor;
            if (user.ForceZeroHealthState)
            {
                user.healthHaver.Armor -= 1;
            }
            else
            {
                user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, -1));
            }
            GameObject go = ChestChances.SelectByWeight();
            Chest chest = Chest.Spawn(go.GetComponent<Chest>(), user.CurrentRoom.GetRandomAvailableCell(new IntVector2(2, 1), Dungeonator.CellTypes.FLOOR, false, null).GetValueOrDefault(), user.CurrentRoom, true);
            chest.ForceUnlock();
            chest.lootTable.lootTable = BraveUtility.RandomBool() ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
            if(user.PlayerHasActiveSynergy("Double the Wish!"))
            {
                GameObject go2 = ChestChances.SelectByWeight();
                Chest chest2 = Chest.Spawn(go2.GetComponent<Chest>(), user.CurrentRoom.GetRandomAvailableCell(new IntVector2(2, 1), Dungeonator.CellTypes.FLOOR, false, null).GetValueOrDefault(), user.CurrentRoom, true);
                chest2.ForceUnlock();
                chest2.lootTable.lootTable = BraveUtility.RandomBool() ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
            }
            if(user.PlayerHasActiveSynergy("Wish of Power"))
            {
                foreach (PassiveItem p in user.passiveItems)
                {
                    if (p is SprenOrbitalItem)
                    {
                        MethodInfo m = typeof(SprenOrbitalItem).GetMethod("TransformSpren", BindingFlags.NonPublic | BindingFlags.Instance);
                        m.Invoke(p as SprenOrbitalItem, new object[0]);
                    }
                }
            }
            if(user.PlayerHasActiveSynergy("Wish of Reflection"))
            {
                foreach(Projectile p in StaticReferenceManager.AllProjectiles)
                {
                    if(p.Owner == null || !(p.Owner is PlayerController))
                    {
                        PassiveReflectItem.ReflectBullet(p, true, user, 10f, 1f, 1f, 0f);
                    }
                }
            }
            foreach(Gun g in user.inventory.AllGuns)
            {
                if(g.PickupObjectId == StarGunId)
                {
                    SaveTools.Add(ref g.currentGunStatModifiers, StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0.05f));
                }
            }
            user.stats.RecalculateStats(user, false, false);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.ForceZeroHealthState ? user.healthHaver.Armor > 2 : user.healthHaver.GetMaxHealth() > 1 && user.CurrentRoom != null;
        }

        public Color FlashColor;
        public Color ErrorFlashColor;
        public int StarGunId;
        public WeightedGameObjectCollection ChestChances;
    }
}
