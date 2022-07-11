using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.WizardKnight.Sword
{
    public class WizardKnightSwordSwipe1 : Script
    {
        public override IEnumerator Top()
        {
            float angleGap = 5f;
            int numBullets = 16;
            for(int i = -numBullets / 2; i < numBullets / 2; i++)
            {
                Fire(new Direction(i * angleGap, DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(6.5f, 9.5f), SpeedType.Absolute));
            }
            return null;
        }
    }
}
