using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class ShootingStar
    {
        public static void Init()
        {
            string name = "Shooting Star";
            string shortdesc = "Rapidfire Starlight";
            string longdesc = "Shoots starlight, rapidly.\n\nThe original idea behind the gun was a gun that could shoot meteors, but the gunsmith that was making the gun must've really misunderstood the idea...";
            Gun gun = EasyGunInit("guns/shooting_star", name, shortdesc, longdesc, "shooting_star_idle_001", "gunsprites/ammonomicon/shooting_star_idle_001", "gunsprites/shootingstar", 600, 0f, new(30, 7), 
                GetItemById<Gun>(15).muzzleFlashEffects, "ak47", PickupObject.ItemQuality.S, GunClass.FULLAUTO, out var finish, 15, null, null);
            ProjectileModule module = new()
            {
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "Star",
                projectiles = new List<Projectile>
                {
                    GetItemById<Gun>(52).DefaultModule.chargeProjectiles[0].Projectile
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = 4f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 600
            };
            gun.RawSourceVolley.projectiles.Add(module);
            ProjectileVolleyData synergyVolley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            synergyVolley.projectiles = new()
            {
                new()
                {
                    shootStyle = ProjectileModule.ShootStyle.Automatic,
                    ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                    customAmmoType = "Star",
                    projectiles = new List<Projectile>
                    {
                        GetItemById<Gun>(52).DefaultModule.chargeProjectiles[0].Projectile.GetComponent<SpawnProjModifier>().projectileToSpawnOnCollision,
                        GetItemById<Gun>(52).DefaultModule.chargeProjectiles[1].Projectile
                    },
                    orderedGroupCounts = new List<int>(),
                    numberOfFinalProjectiles = 0,
                    angleVariance = 4f,
                    cooldownTime = 0.11f,
                    numberOfShotsInClip = 1000,
                    sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered
                }
            };

            var rep = gun.AddComponent<VolleyReplacementSynergyProcessor>();

            rep.RequiredSynergy = ETGModCompatibility.ExtendEnum<CustomSynergyType>(SpecialStuffModule.GUID, "CelestialRhythm");
            rep.SynergyVolley = synergyVolley;

            finish();

            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
        }
    }
}
