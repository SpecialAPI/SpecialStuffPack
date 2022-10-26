using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    [HarmonyPatch]
    public abstract class CustomBlankModificationItem : BlankModificationItem
    {
        [HarmonyPatch(typeof(SilencerInstance), nameof(SilencerInstance.ProcessBlankModificationItemAdditionalEffects))]
        [HarmonyPostfix]
        public static void Add(SilencerInstance __instance, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
        {
            if(bmi != null && bmi is CustomBlankModificationItem cbmi)
            {
                cbmi.DoCustomBlankEffect(__instance, centerPoint, user);
            }
        }

        public virtual void DoCustomBlankEffect(SilencerInstance silencer, Vector2 blankCenter, PlayerController user)
        {

        }
    }
}
