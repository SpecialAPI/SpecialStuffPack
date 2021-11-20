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
            Gun gun = GunBuilder.EasyGunInit("guns/shooting_star", name, shortdesc, longdesc, "shooting_star_idle_001", "gunsprites/ammonomicon/shooting_star_idle_001", "gunsprites/shootingstar", 600, 0f, new Vector3(1.875f, 0.4375f), 
                CodeShortcuts.GetItemById<Gun>(15).muzzleFlashEffects, "ak47", PickupObject.ItemQuality.S, GunClass.FULLAUTO, SpecialStuffModule.globalPrefix, out var finish, 15, null, null);
            ProjectileModule module = new ProjectileModule
            {
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "Star",
                projectiles = new List<Projectile>
                {
                    CodeShortcuts.GetItemById<Gun>(52).DefaultModule.chargeProjectiles[0].Projectile
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = 4f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 600
            };
            gun.RawSourceVolley.projectiles.Add(module);
            ProjectileVolleyData synergyVolley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            synergyVolley.projectiles = new List<ProjectileModule>
            {
                new ProjectileModule
                {
                    shootStyle = ProjectileModule.ShootStyle.Automatic,
                    ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                    customAmmoType = "Star",
                    projectiles = new List<Projectile>
                    {
                        CodeShortcuts.GetItemById<Gun>(52).DefaultModule.chargeProjectiles[0].Projectile.GetComponent<SpawnProjModifier>().projectileToSpawnOnCollision,
                        CodeShortcuts.GetItemById<Gun>(52).DefaultModule.chargeProjectiles[1].Projectile
                    },
                    orderedGroupCounts = new List<int>(),
                    numberOfFinalProjectiles = 0,
                    angleVariance = 4f,
                    cooldownTime = 0.11f,
                    numberOfShotsInClip = 1000,
                    sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered
                }
            };
            gun.AddVolleyReplacementSynergyProcessor("Celestial Rhythm", synergyVolley);
            finish();
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
        }
    }
}
