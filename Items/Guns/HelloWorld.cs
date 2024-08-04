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
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                ammoType = GameUIAmmoType.AmmoType.BEAM,
                projectiles = new List<Projectile> { },
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = 0.75f,
                numberOfShotsInClip = 25
            });
            finish();
        }
    }
}
