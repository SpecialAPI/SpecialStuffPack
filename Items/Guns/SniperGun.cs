using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class SniperGun
    {
        public static void Init()
        {
            string name = "Sniper Gun";
            string shortdesc = "Sniper GUN, not Sniper RIFLE";
            string longdesc = "Fires pink energy bolts that split into smaller energy bolts on destruction.\n\nThe Sniper Rifle from the future, these guns are expensive and rare, used by only the most successful of the potato " +
                "hunters.";
            var gun = EasyGunInit("tatosniper", name, shortdesc, longdesc, "tatosniper_idle_001", "tatosniper_idle_001", "gunsprites/snipergun", 80, 1.5f, new(33, 11), TurboGunObject.muzzleFlashEffects, "heavylaser",
                PickupObject.ItemQuality.A, GunClass.RIFLE, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            var projectile2Collection = ETGMod.Assets.Collections.Find(x => x.spriteCollectionName == "Projectile2Collection");
            var proj = EasyProjectileInit<Projectile>("SniperGunProjectile_Big", null, 30f, 75f, 1000f, 20f, true, false, false, projectile2Collection.GetSpriteIdByName("recharge_gun_projectile_001"),
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null, projectile2Collection);
            var splitProj = EasyProjectileInit<Projectile>("SniperGunProjectile_Small", null, 7.5f, 35f, 1000f, 10f, true, false, false, projectile2Collection.GetSpriteIdByName("recharge_gun_projectile_005"),
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null, projectile2Collection);
            var split = proj.AddComponent<SpawnProjModifier>();
            split.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST;
            split.projectileToSpawnOnCollision = splitProj;
            split.PostprocessSpawnedProjectiles = true;
            split.numberToSpawnOnCollison = 5;
            split.spawnProjectilesInFlight = false;
            split.spawnProjectilesOnCollision = true;
            split.spawnProjecitlesOnDieInAir = false;
            gun.Volley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    proj
                },
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "Rifle",
                angleVariance = 1f,
                numberOfShotsInClip = 10,
                cooldownTime = 1f
            });
            finish();
        }
    }
}
