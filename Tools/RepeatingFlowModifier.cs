using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Tools
{
    [HarmonyPatch]
    public abstract class RepeatingFlowModifier : ProceduralFlowModifierData
    {
        public abstract int NumAdditionalRepeats { get; }

        [HarmonyPatch(typeof(LoopFlowBuilder), nameof(LoopFlowBuilder.ProcessSingleNodeInjection))]
        [HarmonyPostfix]
        public static void Repeat(LoopFlowBuilder __instance, bool __result, ProceduralFlowModifierData currentInjectionData, BuilderFlowNode root, RuntimeInjectionFlags injectionFlags, FlowCompositeMetastructure metastructure, RuntimeInjectionMetadata optionalMetadata)
        {
            if (__result)
            {
                if(currentInjectionData is RepeatingFlowModifier repeat)
                {
					var repeats = repeat.NumAdditionalRepeats;
					if(repeats > 0)
                    {
						for(int i = 0; i < repeats; i++)
						{
							PrototypeDungeonRoom prototypeDungeonRoom = currentInjectionData.exactRoom;
							if (currentInjectionData.roomTable != null && currentInjectionData.exactRoom == null)
							{
								WeightedRoom weightedRoom = currentInjectionData.roomTable.SelectByWeight();
								if (weightedRoom != null)
								{
									prototypeDungeonRoom = weightedRoom.room;
								}
							}
							if (prototypeDungeonRoom == null)
							{
								break;
							}
							var flowModifierPlacementType = currentInjectionData.GetPlacementRule();
							var flag3 = true;
							switch (flowModifierPlacementType)
							{
								case FlowModifierPlacementType.BEFORE_ANY_COMBAT_ROOM:
									flag3 = __instance.InjectNodeBefore(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_RandomCombatRoom, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.END_OF_CHAIN:
									flag3 = __instance.InjectNodeAfter(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_EndOfChain, metastructure, optionalMetadata);
									if (!flag3)
									{
										flag3 = __instance.InjectNodeAfter(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_RandomNodeChild, metastructure, optionalMetadata);
									}
									break;
								case FlowModifierPlacementType.HUB_ADJACENT_CHAIN_START:
									flag3 = __instance.InjectNodeBefore(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_HubAdjacentChainStart, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.HUB_ADJACENT_NO_LINK:
									flag3 = __instance.InjectNodeAfter(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_HubAdjacentNoLink, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.RANDOM_NODE_CHILD:
									flag3 = __instance.InjectNodeAfter(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_RandomNodeChild, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.COMBAT_FRAME:
									__instance.HandleInjectionFrame(currentInjectionData, root, optionalMetadata, metastructure);
									break;
								case FlowModifierPlacementType.NO_LINKS:
									__instance.InjectNodeNoLinks(currentInjectionData, prototypeDungeonRoom, root, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.AFTER_BOSS:
									flag3 = __instance.InjectNodeBefore(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_AfterBoss, metastructure, optionalMetadata);
									break;
								case FlowModifierPlacementType.BLACK_MARKET:
									flag3 = __instance.InjectNodeAfter(currentInjectionData, prototypeDungeonRoom, root, __instance.InjectValidator_BlackMarket, metastructure, optionalMetadata);
									break;
							}
							if (flag3 && prototypeDungeonRoom.requiredInjectionData != null)
							{
								RuntimeInjectionMetadata sourceMetadata = new RuntimeInjectionMetadata(prototypeDungeonRoom.requiredInjectionData);
								__instance.HandleNodeInjection(root, sourceMetadata, injectionFlags, metastructure);
							}
						}
                    }
				}
            }
        }
    }
}
