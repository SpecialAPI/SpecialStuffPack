using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class BulletShootingGun
    {
        public static void Init()
        {
            var name = "Bullet-Shooting Gun";
            var shortdesc = "Shoot Bullet?";
            var longdesc = "It's a bullet-shooting gun! Shoot bullet? Shoot bullet! Bullet-shooting! Shoot! Bullet! Shoot! Bullet! Shoot! Shoot!";
            var gun = EasyGunInit("iabsg", name, shortdesc, longdesc, "iabsg_idle_001", "iabsg_idle_001", "gunsprites/bulletshootinggun", 220, 1f, new(26, 13), Empty, "Quad", PickupObject.ItemQuality.B, GunClass.PISTOL, out var finish);
            var bullet = EasyProjectileInit<Projectile>("iabsgprojectile", "iabsg_projectile_001", 9f, 23f, 1000f, 10f, false, false, false);
            bullet.AddComponent<BulletShootingGunProjectile>();
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    bullet
                },
                numberOfShotsInClip = 6,
                cooldownTime = 0.15f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = CustomAmmoUtility.AddCustomAmmoType("iabsg", "iabsgammotype", "iabsgammotypeempty", "iabsg_ammo_type", "iabsg_ammo_type_empty"),
                angleVariance = 4f
            });
            finish();
        }
    }
}
