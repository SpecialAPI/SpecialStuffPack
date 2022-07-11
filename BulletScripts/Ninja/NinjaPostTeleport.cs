using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.Ninja
{
    public class NinjaPostTeleport : Script
    {
        public override IEnumerator Top()
        {
            Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new TombstoneCrossAttack1.CrossBullet(new Vector2(0f, -0.35f), 0, 20));
            Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new TombstoneCrossAttack1.CrossBullet(new Vector2(0f, 0.35f), 0, 20));
            Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new TombstoneCrossAttack1.CrossBullet(new Vector2(-0.7f, 0f), 0, 20));
            Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new TombstoneCrossAttack1.CrossBullet(new Vector2(0.7f, 0f), 18, 15));
            Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new TombstoneCrossAttack1.CrossBullet(new Vector2(1.4f, 0.175f), 18, 15));
            yield break;
        }
    }
}
