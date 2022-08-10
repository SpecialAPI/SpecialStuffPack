using SpecialStuffPack.GungeonAPI;
using SpecialStuffPack.ItemAPI;
using System;
using Dungeonator;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class ExtraChestItem : PassiveItem
    {
        public static void Init()
        {
            string name = "DLChest";
            string shortdesc = "Extra Chest Each Floor";
            string longdesc = "Adds an extra chest room each floor. The chest will be locked like the other chests.\n\nThis DLChest can be bought on Vapour (the popular video game digital distribution service by Faucet) for the low price of 999 " +
                "casings!";
            ExtraChestItem item = ItemBuilder.EasyInit<ExtraChestItem>("items/dlchest", "sprites/dlchest_idle_001", name, shortdesc, longdesc, ItemQuality.B, 314, null);
            RoomFactory.AddInjection(RoomFactory.BuildFromResource("SpecialStuffPack.Rooms.Room_DLChest.room").room, "Special DLChest Room", new List<ProceduralFlowModifierData.FlowModifierPlacementType> { 
                ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN }, 0f, new List<DungeonPrerequisite> { new SpecialDungeonPrerequisite { specialPrerequisiteType = SpecialDungeonPrerequisite.SpecialPrerequisiteType.ITEM_FLAG, 
                    flagToCheck = typeof(ExtraChestItem) } }, "DLChest Room Injection");
            item.AddToFlyntShop();
            item.AddToBlacksmithShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(ExtraChestItem));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(ExtraChestItem));
            return base.Drop(player);
        }
    }
}
