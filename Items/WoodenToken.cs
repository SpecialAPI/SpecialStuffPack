using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialStuffPack.SaveAPI;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class WoodenToken : PlayerItem
    {
        public static void Init()
        {
            string name = "Wooden Token";
            string shortdesc = "You get FREE guns!";
            string longdesc = "Can be traded with Resourceful Rat for guns. Perhaps it's worth keeping it around for longer to get more value?\n\nA wooden token with Resourceful Rat's head engraved into it.";
            WoodenToken item = ItemBuilder.EasyInit<WoodenToken>("items/woodentoken", "sprites/wooden_token_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, 435, null);
            item.SetCooldownType(ItemBuilder.CooldownType.Timed, 0.1f);
            item.IgnoredByRat = true;
            item.consumable = true;
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.ITEMSPECIFIC_WOODEN_TOKEN, true);
        }

        public override void Pickup(PlayerController player)
        {
            if(!m_pickedUpThisRun && (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON))
            {
                m_turnsIntoMoney = true;
            }
            if(m_currentRatShop != null && m_currentRatShop.GetComponent<BaseShopController>() != null)
            {
                List<ShopItemController> items = m_currentRatShop.GetComponent<BaseShopController>().ItemControllers();
                if(items != null)
                {
                    foreach(ShopItemController item in items)
                    {
                        item.Locked = false;
                    }
                    player.ForceRefreshInteractable = true;
                }
            }
            player.OnItemPurchased += HandleItemExchange;
            player.OnItemStolen += HandleItemExchange;
            player.OnNewFloorLoaded += SpawnShop;
            base.Pickup(player);
        }

        public void HandleItemExchange(PlayerController player, ShopItemController item)
        {
            if(m_currentRatShop != null && !player.PlayerHasActiveSynergy("Fufufufufu"))
            {
                if (item.BaseParentShop() == m_currentRatShop.GetComponent<BaseShopController>())
                {
                    player.RemoveActiveItem(PickupObjectId);
                }
            }
        }

        public void SpawnShop(PlayerController player)
        {
            if(GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON && m_currentRatShop == null)
            {
                RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
                foreach (BaseShopController shop in entrance.GetComponentsInRoom<BaseShopController>())
                {
                    if(shop.baseShopType == BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
                    {
                        return;
                    }
                }
                DungeonPlaceable dungeonPlaceable = BraveResources.Load("Global Prefabs/Merchant_Rat_Placeable", ".asset") as DungeonPlaceable;
                GameObject gObj = dungeonPlaceable.InstantiateObject(entrance, new IntVector2(1, 3), false, false);
                m_currentRatShop = gObj;
                IPlayerInteractable[] interfacesInChildren = gObj.GetInterfacesInChildren<IPlayerInteractable>();
                for (int j = 0; j < interfacesInChildren.Length; j++)
                {
                    entrance.RegisterInteractable(interfacesInChildren[j]);
                }
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            user.OnItemPurchased -= HandleItemExchange;
            user.OnItemStolen -= HandleItemExchange;
            user.OnNewFloorLoaded -= SpawnShop;
            if (m_currentRatShop != null && m_currentRatShop.GetComponent<BaseShopController>() != null)
            {
                List<ShopItemController> items = m_currentRatShop.GetComponent<BaseShopController>().ItemControllers();
                if (items != null)
                {
                    foreach (ShopItemController item in items)
                    {
                        item.Locked = true;
                    }
                    user.ForceRefreshInteractable = true;
                }
            }
            base.OnPreDrop(user);
        }

        protected override void OnDestroy()
        {
            if(LastOwner != null)
            {
                LastOwner.OnNewFloorLoaded -= SpawnShop;
                LastOwner.OnItemPurchased -= HandleItemExchange;
                LastOwner.OnItemStolen -= HandleItemExchange;
            }
            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();
            if (m_turnsIntoMoney && m_pickedUp && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
            {
                m_turnsIntoMoney = false;
            }
        }

        protected override void DoEffect(PlayerController user)
        {
            LootEngine.SpawnCurrency(user.sprite.WorldCenter, 115, false);
            LootEngine.SpawnItem(LootEngine.GetItemOfTypeAndQuality<Gun>(ItemQuality.D, GameManager.Instance.RewardManager.GunsLootTable, false).gameObject, user.sprite.WorldCenter, Vector2.down, 1f, true, true, false);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return m_turnsIntoMoney;
        }

        [SerializeField]
        private GameObject m_currentRatShop;
        [SerializeField]
        private bool m_turnsIntoMoney;
    }
}
