using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class HyperDarkBlaster
    {
        public static void Init()
        {
            var name = "Hyper Dark Blaster";
            var shortdesc = "Don't Miss";
            var longdesc = "Consumes health to shoot, shots that hit enemies refund health.";
            var gun = EasyGunInit("hdb", name, shortdesc, longdesc, "hdb_idle_001", "hdb_idle_001", "gunsprites/hyperdarkblaster", 1000, 0f, new(17, 8), SuperMeatGunObject.muzzleFlashEffects, "HLD", PickupObject.ItemQuality.S, GunClass.PISTOL, out var finish);
            var proj = EasyProjectileInit<Projectile>("hyperdarkblasterprojectile", "", 40f, 26f, 1000f, 6f, true, false, true, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("blaster_bolt_001"));
            proj.AddComponent<BloodyHandProjectile>().autoDamagePlayer = true;
            proj.hitEffects = VoidMarshalObject.GetProjectile().hitEffects;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                projectiles = new()
                {
                    proj
                },
                numberOfShotsInClip = -1,
                cooldownTime = 0.15f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = CustomAmmoUtility.AddCustomAmmoType("SPAPI_HDB", "HyperDarkBlasterAmmoType", "HyperDarkBlasterAmmoTypeEmpty", "hdb_ammo", "hdb_ammo_empty"),
                angleVariance = 1f
            });
            gun.AddComponent<RequiresHealthToShoot>();
            finish();
        }
    }
}
