using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class HelloWorld
    {

        public static void Init()
        {
            var gun = EasyGunInit("helloworld", "Hello World Gun", "A coder's first gun", "what", "helloworld_idle_001", "helloworld_idle_001", "gunsprites/helloworldgun/", 25, 1f, new(24, 10), Empty, 
                "", PickupObject.ItemQuality.EXCLUDED, GunClass.NONE, out var finish);
            //var proj = EasyProjectileInit<WhipProjectile>("WhipProjectile", "melon_projectile_001", 10, 0f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            //proj.Length = 5;
            //proj.time = 0.5f;
            //proj.startAngle = 30f;
            //proj.endAngle = -30f;
            //proj.whipPrefab = "WhipProjectile";
            //var pierce = proj.GetOrAddComponent<PierceProjModifier>();
            //pierce.penetration = 9999;
            //pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            //pierce.preventPenetrationOfActors = false;
            //pierce.penetratesBreakables = true;
            //proj.specRigidbody.CollideWithTileMap = false;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                ammoType = GameUIAmmoType.AmmoType.BEAM,
            //    projectiles = new List<Projectile> { proj },
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = 0.75f,
                numberOfShotsInClip = 25
            });
            finish();
        }
    }
}
