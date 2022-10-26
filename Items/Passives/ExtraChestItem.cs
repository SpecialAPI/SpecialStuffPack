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
                "hegemony credits!";
            ExtraChestItem item = EasyItemInit<ExtraChestItem>("items/dlchest", "sprites/dlchest_idle_001", name, shortdesc, longdesc, ItemQuality.B, 314, null);
            WeightedRoomCollection roomTable = new();
            roomTable.Add(new() { room = RoomFactory.BuildFromResource("SpecialStuffPack.Rooms.Room_DLChest.room").room, weight = 1f, additionalPrerequisites = new DungeonPrerequisite[] {
                        new SpecialDungeonPrerequisite() { specialPrerequisiteType = SpecialDungeonPrerequisite.SpecialPrerequisiteType.SYNERGY, synergyToCheck = "Enter the Gungeon 2", shouldHaveSynergy = false } } });
            foreach (var asset in AssetBundleManager.specialeverything.GetAllAssetNames())
            {
                if (asset.ToLowerInvariant().StartsWith("assets/rooms/dlchestsynergy"))
                {
                    var roomdata = RoomFactory.BuildFromTextAsset(AssetBundleManager.Load<TextAsset>(asset));
                    roomTable.Add(new() { room = roomdata.room, weight = roomdata.weight, additionalPrerequisites = new DungeonPrerequisite[] { 
                        new SpecialDungeonPrerequisite() { specialPrerequisiteType = SpecialDungeonPrerequisite.SpecialPrerequisiteType.SYNERGY, synergyToCheck = "Enter the Gungeon 2", shouldHaveSynergy = true } } });
                }
            }
            RoomFactory.AddInjection(new GenericRoomTable() { includedRooms = roomTable, includedRoomTables = new(), name = "DLChest Rooms" }, "Special DLChest Room", new List<ProceduralFlowModifierData.FlowModifierPlacementType> {
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

        public override void DisableEffect(PlayerController player)
        {
            DecrementFlag(player, typeof(ExtraChestItem));
        }
    }
}
