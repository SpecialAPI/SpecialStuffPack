using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class AkEpsilon
    {
        public static void Init()
        {
            var name = "ak-epsilon";
            var shortdesc = "accept no substitutes";
            var longdesc = "fires tiny bullets.\n\nthe result of an experiment on how small a gun can be, this gun is the smallest ak-47 possible. or at least with the current precision.";
            var gun = EasyGunInit("akepsilon", name, shortdesc, longdesc, "akepsilon_idle_001", "akepsilon_idle_001", "gunsprites/akepsilon", 500, 0.5f, new(12, 4), Empty, "ak47", PickupObject.ItemQuality.D, GunClass.FULLAUTO, out var finish);
            var proj = EasyProjectileInit<Projectile>("epsilonprojectile", "epsilon_projectile_001", 2f, 23, 1000f, 3f, false, false, false);
            gun.MakeContinuous();
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    proj
                },
                ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET,
                angleVariance = 3f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 30,
                shootStyle = ProjectileModule.ShootStyle.Automatic
            });
            finish();
        }
    }
}
