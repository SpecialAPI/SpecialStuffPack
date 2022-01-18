using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunGlockDirectedHardLeft3 : DraGunGlockDirected3
    {
        protected override string BulletName
        {
            get
            {
                return "glockLeft";
            }
        }

        protected override bool IsHard
        {
            get
            {
                return true;
            }
        }
    }
}
