using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class AkPI
    {
        public static void Init()
        {
            string name = "AK-3.14";
            string shortdesc = "C = gun * d";
            string longdesc = "Quickly shoots projectiles that spin around the shooter.";
            var gun = GunBuilder.EasyGunInit("akpi", name, shortdesc, longdesc, "akpi_idle_001", "akpi_idle_001", "gunsprites/akpi", 600, 0.5f, new(15, 22), GetItemById<Gun>(15).muzzleFlashEffects, "ak47", 
                PickupObject.ItemQuality.C, GunClass.FULLAUTO, out var finish);
            gun.MakeContinuous();
            var projectile = GunBuilder.EasyProjectileInit<Projectile>("AkPIProjectile", null, 5.5f, 23, 60f, 6, false, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
            projectile.AddComponent<OrbitProjectile>().expandTime = 0.25f;
            projectile.specRigidbody.CollideWithTileMap = false;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                numberOfShotsInClip = 30,
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                projectiles = new() { projectile },
                cooldownTime = 0.11f,
            });
            finish();
        }
    }
}
