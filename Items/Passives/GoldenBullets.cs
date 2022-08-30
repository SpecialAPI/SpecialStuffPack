using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class GoldenBullets : PassiveItem
    {
        public static void Init()
        {
            string name = "Golden Bullets";
            string shortdesc = "Fair and Balanced";
            string longdesc = "Increases in power each kill. Doesn't ever decrease in power.\n\nThese bullets are constantly trying to improve themselves, never forgetting any past improvements made to them.";
        }
    }
}
