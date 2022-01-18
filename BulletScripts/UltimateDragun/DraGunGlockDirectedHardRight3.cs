using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunGlockDirectedHardRight3 : DraGunGlockDirected3
    {
        protected override string BulletName
        {
            get
            {
                return "glockRight";
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
