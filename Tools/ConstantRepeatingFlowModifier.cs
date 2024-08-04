using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Tools
{
    public class ConstantRepeatingFlowModifier : RepeatingFlowModifier
    {
        public override int NumAdditionalRepeats => additionalRepeats;

        public int additionalRepeats;
    }
}
