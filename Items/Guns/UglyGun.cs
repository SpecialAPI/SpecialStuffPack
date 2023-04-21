using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class UglyGun
    {
        public static void Init()
        {
            var name = "Ugly Gun";
            var shortdesc = "Average SpecialUtils Gun";
            var longdesc = "A very ugly gun.\n\nThe projectiles are ugly too.";
            var gun = EasyGunInit("ugly_gun", name, shortdesc, longdesc, "ugly_gun_idle_001", "ugly_gun_idle_001", "gunsprites/uglygun", 513, 1.34871023948710239481f, new(27, 10), PeaShooterObject.muzzleFlashEffects, "", PickupObject.ItemQuality.C, GunClass.SHITTY, out var finish);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                angleVariance = 0f,
                ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET,
                cooldownTime = 0.369817263493402345209387f,
                numberOfShotsInClip = 7,
                projectiles = new()
                {
                    EasyProjectileInit<Projectile>("uglygunprojectile", "ugly_projectile_001", 7.123412342348102398f, 5.2125976049335586695989981580569f, 60f, 30f, false, false, false)
                }
            });
            var reloadFire = gun.AddComponent<FireVolleyOnReload>();
            reloadFire.volley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            reloadFire.volley.projectiles = new()
            {
                new()
                {
                    projectiles = new()
                    {
                        GetProjectile(154).GetComponent<SpawnProjModifier>().projectileToSpawnOnCollision
                    },
                    angleVariance = 0f,
                    numberOfShotsInClip = 1,
                    cooldownTime = 0f,
                }
            };
            reloadFire.switchGroup = "";
            reloadFire.sfx = "Play_WPN_Gun_Shot_01";
            reloadFire.shootOffset = Vector2.zero;
            reloadFire.synergy = "#ADVANCED_FEATURE-RICH_INTERACTIVE";
            finish();
        }
    }
}
