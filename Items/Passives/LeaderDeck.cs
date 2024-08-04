using SpecialStuffPack.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class LeaderDeck : PassiveItem
    {
        public static void Init()
        {
            string name = "Leader's Deck";
            string shortdesc = "Draw Your Cards";
            string longdesc = "Tarot Cards now appear, empowering the owner in various ways when picked up.\n\nThese cards were once used by the mighty Blam to gain power on crusades.";
            EasyItemInit<LeaderDeck>("leaderdeck", "leader_deck_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            var repeat = RoomFactory.AddInjection<ItemFlagBasedRepeatingFlowModifier>(RoomFactory.BuildFromTextAsset("leaderdeck_twocards").room, "Leader's Deck Two Cards", new List<ProceduralFlowModifierData.FlowModifierPlacementType> {
                ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD }, 0f, new List<DungeonPrerequisite> { new SpecialDungeonPrerequisite { 
                    specialPrerequisiteType = SpecialDungeonPrerequisite.SpecialPrerequisiteType.ITEM_FLAG, flagToCheck = typeof(LeaderDeck) } }, "Leader's Deck Twocard Injection");
            repeat.itemFlag = typeof(LeaderDeck);
            RoomFactory.AddInjection(RoomFactory.BuildFromTextAsset("leaderdeck_onecard").room, "Leader's Deck One Card", new List<ProceduralFlowModifierData.FlowModifierPlacementType> {
                ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN }, 0f, new List<DungeonPrerequisite> { new SpecialDungeonPrerequisite {
                    specialPrerequisiteType = SpecialDungeonPrerequisite.SpecialPrerequisiteType.ITEM_FLAG, flagToCheck = typeof(LeaderDeck) } }, "Leader's Deck Onecard Injection", 0.3f);
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                LootEngine.SpawnItem(TarotCards.GetTarotCardForPlayer().gameObject, player.specRigidbody.UnitBottomCenter, Vector2.down, 0f, true, false, false);
            }
            player.IncrementFlag<LeaderDeck>();
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player == null)
            {
                return;
            }
            player.DecrementFlag<LeaderDeck>();
            base.DisableEffect(player);
        }
    }
}
