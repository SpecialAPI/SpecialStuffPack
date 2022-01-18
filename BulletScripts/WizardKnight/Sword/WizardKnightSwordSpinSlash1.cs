using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.WizardKnight.Sword
{
    public class WizardKnightSwordSpinSlash1 : Script
    {
        protected override IEnumerator Top()
        {
            PostWwiseEvent("Play_ENM_gunknight_shockwave_01");
            for (; ; )
            {
                Fire(new Direction(0f, DirectionType.Relative, -1), new Speed(6, SpeedType.Absolute));
                Fire(new Direction(0f, DirectionType.Relative, -1), new Speed(9, SpeedType.Absolute));
                yield return Wait(2);
            }
        }
    }
}
