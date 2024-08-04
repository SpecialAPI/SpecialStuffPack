using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Tools
{
    public class ItemFlagBasedRepeatingFlowModifier : RepeatingFlowModifier
    {
        public override int NumAdditionalRepeats => GetFlagCountAllPlayers(itemFlag) + additional;

        public Type itemFlag;
        public int additional = -1;
    }
}
