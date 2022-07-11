using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.WizardKnight.Sword
{
    public class WizardKnightSwordSpin1 : Script
    {
        public override IEnumerator Top()
        {
            for(; ;)
            {
                PostWwiseEvent("Play_WPN_earthwormgun_shot_01");
                Fire(new Direction(-90, DirectionType.Relative, -1), new Speed(14f, SpeedType.Absolute), new SpeedChangingBullet(-9, 90, -1));
                Fire(new Direction(90, DirectionType.Relative, -1), new Speed(14f, SpeedType.Absolute), new SpeedChangingBullet(-9, 90, -1));
                yield return Wait(4);
            }
        }
    }
}
