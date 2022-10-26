using Dungeonator;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class PayToWin : PlayerItem
    {
        public static void Init()
        {
            string name = "PAY 2 WIN";
            string shortdesc = "Upgrade Chests for $$$";
            string longdesc = "Consumes casings to upgrade chests. Upgrading better chests costs more.";
            PayToWin item = EasyItemInit<PayToWin>("items/paytowin", "sprites/ptw_idle_001", name, shortdesc, longdesc, ItemQuality.B, 494, null);
            item.SetCooldownType(CooldownType.Timed, 0.5f);
            item.dUpgradePrice = 25;
            item.cUpgradePrice = 35;
            item.bUpgradePrice = 50;
            item.aUpgradePrice = 85;
            item.sUpgradePrice = 215;
            item.rainbowUpgradePrice = 1000;
            ETGMod.Databases.Strings.Core.Set("#PTW_UPGRADE_CHEST", "Upgrade chest for %PRICE%CURRENCY_SYMBOL?");
            item.AddToFlyntShop();
        }

        public override void Update()
        {
            base.Update();
            if(m_pickedUp && LastOwner != null)
            {
                if(LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) != null && LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) is Chest && 
                    (GetPriceForQuality(GameManager.Instance.RewardManager.GetQualityFromChest(LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest)) != null))
                {
                    if(m_currentChest != null && m_currentChest != LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest)
                    {
                        GameUIRoot.Instance.DeregisterDefaultLabel(m_currentChest.transform);
                        Chest chest = LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest;
                        Vector3 offset = new Vector3(chest.sprite.GetBounds().max.x + 0.1875f, chest.sprite.GetBounds().min.y, 0f);
                        string text = StringTableManager.GetString("#PTW_UPGRADE_CHEST").Replace("%PRICE", GetPriceForQuality(GameManager.Instance.RewardManager.GetQualityFromChest(chest)).GetValueOrDefault().ToString());
                        GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(chest.transform, offset, text);
                        dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
                        componentInChildren.ColorizeSymbols = false;
                        componentInChildren.ProcessMarkup = true;
                        m_currentChest = chest;
                    }
                    else if(m_currentChest == null)
                    {
                        Chest chest = LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest;
                        Vector3 offset = new Vector3(chest.sprite.GetBounds().max.x + 0.1875f, chest.sprite.GetBounds().min.y, 0f);
                        string text = StringTableManager.GetString("#PTW_UPGRADE_CHEST").Replace("%PRICE", GetPriceForQuality(GameManager.Instance.RewardManager.GetQualityFromChest(chest)).GetValueOrDefault().ToString());
                        GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(chest.transform, offset, text);
                        dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
                        componentInChildren.ColorizeSymbols = false;
                        componentInChildren.ProcessMarkup = true;
                        m_currentChest = chest;
                    }
                }
                else if(LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) != null && LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) is Chest &&
                    (LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest).IsRainbowChest)
                {
                    if (m_currentChest != null && m_currentChest != LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest)
                    {
                        GameUIRoot.Instance.DeregisterDefaultLabel(m_currentChest.transform);
                        Chest chest = LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest;
                        Vector3 offset = new Vector3(chest.sprite.GetBounds().max.x + 0.1875f, chest.sprite.GetBounds().min.y, 0f);
                        string text = StringTableManager.GetString("#PTW_UPGRADE_CHEST").Replace("%PRICE", rainbowUpgradePrice.ToString());
                        GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(chest.transform, offset, text);
                        dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
                        componentInChildren.ColorizeSymbols = false;
                        componentInChildren.ProcessMarkup = true;
                        m_currentChest = chest;
                    }
                    else if (m_currentChest == null)
                    {
                        Chest chest = LastOwner.CurrentRoom.GetNearestInteractable(LastOwner.CenterPosition, 1f, LastOwner) as Chest;
                        Vector3 offset = new Vector3(chest.sprite.GetBounds().max.x + 0.1875f, chest.sprite.GetBounds().min.y, 0f);
                        string text = StringTableManager.GetString("#PTW_UPGRADE_CHEST").Replace("%PRICE", rainbowUpgradePrice.ToString());
                        GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(chest.transform, offset, text);
                        dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
                        componentInChildren.ColorizeSymbols = false;
                        componentInChildren.ProcessMarkup = true;
                        m_currentChest = chest;
                    }
                }
                else
                {
                    if(m_currentChest != null)
                    {
                        GameUIRoot.Instance.DeregisterDefaultLabel(m_currentChest.transform);
                        m_currentChest = null;
                    }
                }
            }
        }

        public override void DoEffect(PlayerController user)
        {
            Chest chest = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as Chest;
            if (chest.IsRainbowChest)
            {
                chest.majorBreakable.Break(Vector2.zero);
                chest.sprite.HeightOffGround -= 0.5f;
                Exploder.DoDefaultExplosion(chest.sprite.WorldCenter, Vector2.zero, null, true, CoreDamageTypes.None, false);
                user.carriedConsumables.Currency -= rainbowUpgradePrice;
                return;
            }
            Vector3 pos = chest.transform.position;
            RoomHandler room = chest.GetAbsoluteParentRoom();
            GenericLootTable lootTable = chest.ChestType == Chest.GeneralChestType.WEAPON ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
            bool isLocked = chest.IsLocked;
            ItemQuality quality = GameManager.Instance.RewardManager.GetQualityFromChest(chest);
            ItemQuality? nextQuality = GetNextQuality(quality);
            int? price = GetPriceForQuality(quality);
            user.carriedConsumables.Currency -= price.GetValueOrDefault();
            if (!chest.pickedUp)
            {
                chest.pickedUp = true;
                chest.GetAbsoluteParentRoom().DeregisterInteractable(chest);
                chest.majorBreakable.Break(Vector2.zero);
                chest.sprite.HeightOffGround -= 0.5f;
                chest.sprite.UpdateZDepth();
            }
            Chest toSpawn = quality == ItemQuality.S ? GameManager.Instance.RewardManager.Rainbow_Chest : GetChestFromQuality(nextQuality.GetValueOrDefault());
            Chest newChest = Chest.Spawn(toSpawn, pos, room, false);
            if (!isLocked)
            {
                newChest.ForceUnlock();
            }
            newChest.lootTable.lootTable = lootTable;
        }

        public static ItemQuality? GetNextQuality(ItemQuality quality)
        {
            ItemQuality? newQuality = null;
            switch (quality)
            {
                case ItemQuality.D:
                    newQuality = ItemQuality.C;
                    break;
                case ItemQuality.C:
                    newQuality = ItemQuality.B;
                    break;
                case ItemQuality.B:
                    newQuality = ItemQuality.A;
                    break;
                case ItemQuality.A:
                    newQuality = ItemQuality.S;
                    break;
                case ItemQuality.S:
                    newQuality = ItemQuality.S;
                    break;
            }
            return newQuality;
        }

        public static Chest GetChestFromQuality(ItemQuality quality)
        {
            Chest chest = null;
            switch (quality)
            {
                case ItemQuality.D:
                    chest = GameManager.Instance.RewardManager.D_Chest;
                    break;
                case ItemQuality.C:
                    chest = GameManager.Instance.RewardManager.C_Chest;
                    break;
                case ItemQuality.B:
                    chest = GameManager.Instance.RewardManager.B_Chest;
                    break;
                case ItemQuality.A:
                    chest = GameManager.Instance.RewardManager.A_Chest;
                    break;
                case ItemQuality.S:
                    chest = GameManager.Instance.RewardManager.S_Chest;
                    break;
            }
            return chest;
        }

        private int? GetPriceForQuality(ItemQuality quality)
        {
            int? price = null;
            switch (quality)
            {
                case ItemQuality.D:
                    price = dUpgradePrice;
                    break;
                case ItemQuality.C:
                    price = cUpgradePrice;
                    break;
                case ItemQuality.B:
                    price = bUpgradePrice;
                    break;
                case ItemQuality.A:
                    price = aUpgradePrice;
                    break;
                case ItemQuality.S:
                    price = sUpgradePrice;
                    break;
            }
            float pricemult = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
            if(price != null)
            {
                if (LastOwner.PlayerHasActiveSynergy("50% OFF ON ALL DLCHESTS!"))
                {
                    if (LastOwner.CurrentRoom != null && LastOwner.CurrentRoom.area != null && !string.IsNullOrEmpty(LastOwner.CurrentRoom.area.PrototypeRoomName) && LastOwner.CurrentRoom.area.PrototypeRoomName.ToLower().Contains("dlchest"))
                    {
                        pricemult *= 0.5f;
                    }
                }
                if(LastOwner.PlayerHasActiveSynergy("25% OFF ON ALL IN-GAME PURCHASES!"))
                {
                    pricemult *= 0.75f;
                }
                price = Mathf.RoundToInt(price.Value * pricemult);
            }
            return price;
        }

        public override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if(m_currentChest != null)
            {
                GameUIRoot.Instance.DeregisterDefaultLabel(m_currentChest.transform);
                m_currentChest = null;
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.CurrentRoom != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) is Chest &&
                ((GetPriceForQuality(GameManager.Instance.RewardManager.GetQualityFromChest(user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as Chest)) != null && user.carriedConsumables.Currency >= 
                GetPriceForQuality(GameManager.Instance.RewardManager.GetQualityFromChest(user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as Chest)).GetValueOrDefault() && 
                !(user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as Chest).pickedUp) || ((user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user) as Chest).IsRainbowChest && user.carriedConsumables.Currency >= 
                rainbowUpgradePrice));
        }

        public int dUpgradePrice;
        public int cUpgradePrice;
        public int bUpgradePrice;
        public int aUpgradePrice;
        public int sUpgradePrice;
        public int rainbowUpgradePrice;
        private Chest m_currentChest;
    }
}
