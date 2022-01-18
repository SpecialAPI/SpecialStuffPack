using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.BulletScripts.AdvancedRatMetalGear
{
    public abstract class MetalGearRatSidePound2 : Script
    {
        public abstract float StartAngle { get; }
        public abstract float EndAngle { get; }

        protected override IEnumerator Top()
        {
            for (int i = 0; i < 30; i++)
            {
                Fire(new Direction(i * 12, DirectionType.Absolute, -1), new Speed(7f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(i * 12, DirectionType.Absolute, -1), new Speed(10f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(i * 12, DirectionType.Absolute, -1), new Speed(13f, SpeedType.Absolute), new Bullet("default_noramp"));
            }
            for(int i = 0; i < 30; i++)
            {
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(7f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(8.5f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(10f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(11.5f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(13f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(14.5f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(16f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(17.5f, SpeedType.Absolute), new Bullet("default_noramp"));
                Fire(new Direction(Mathf.Lerp(StartAngle, EndAngle, i / 29f), DirectionType.Absolute, -1), new Speed(19f, SpeedType.Absolute), new Bullet("default_noramp"));
            }
            return null;
        }
    }
}
