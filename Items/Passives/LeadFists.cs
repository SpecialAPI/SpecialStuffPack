using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    [HarmonyPatch]
    public class LeadFists : PassiveItem
    {
        public static void Init()
        {
            string name = "Lead Fists";
            string shortdesc = "Cheater...";
            string longdesc = "Made for stinky people who want to cheat in punchout.";
            EasyItemInit<LeadFists>("LeadFists", "solid_lead_fists_idle_001", name, shortdesc, longdesc, ItemQuality.EXCLUDED, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            player.IncrementFlag<LeadFists>();
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            player.DecrementFlag<LeadFists>();
            base.DisableEffect(player);
        }

        [HarmonyPatch(typeof(PunchoutPlayerController.PlayerPunchState), nameof(PunchoutPlayerController.PlayerPunchState.CanHitOpponent))]
        [HarmonyPostfix]
        public static void HitOpponentAnyways(ref bool __result, PunchoutGameActor.State state)
        {
            if (IsFlagSetAtAll(typeof(LeadFists)))
            {
                if (state is PunchoutGameActor.BlockState)
                {
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(PunchoutAIActor), nameof(PunchoutAIActor.Hit))]
        [HarmonyPrefix]
        public static void CheatTheGame(ref float damage, ref int starsUsed)
        {
            if (IsFlagSetAtAll(typeof(LeadFists)))
            {
                damage = 999999f;
                starsUsed = 3;
            }
        }
    }
}
