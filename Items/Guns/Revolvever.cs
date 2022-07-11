using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Revolvever
    {
        public static void Init()
        {
            string name = "Revolve-ver";
            string shortdesc = "SPIN";
            string longdesc = "Shoots projectiles that spin around the shooter.";
            var gun = GunBuilder.EasyGunInit("revolvever", name, shortdesc, longdesc, "revolvever_idle_001", "revolvever_idle_001", "gunsprites/revolvever/", 200, 1f, new Vector3(1.375f, 0.5625f),
                GetItemById<Gun>(38).muzzleFlashEffects, "SAA", PickupObject.ItemQuality.C, GunClass.PISTOL, SpecialStuffModule.globalPrefix, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 44);
            var projectile = GunBuilder.EasyProjectileInit<Projectile>("RevolveverProjectile", null, 13f, 23, 60f, 15f, false, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
            projectile.AddComponent<OrbitProjectile>().expandTime = 0.25f;
            projectile.specRigidbody.CollideWithTileMap = false;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                numberOfShotsInClip = 6,
                projectiles = new() { projectile },
                cooldownTime = 0.15f,
                ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET
            });
            finish();
        }
    }
}
