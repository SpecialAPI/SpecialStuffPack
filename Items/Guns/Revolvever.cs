﻿using System;
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
            var gun = EasyGunInit("revolvever", name, shortdesc, longdesc, "revolvever_idle_001", "revolvever_idle_001", "gunsprites/revolvever/", 200, 1f, new(22, 9),
                GetItemById<Gun>(38).muzzleFlashEffects, "SAA", PickupObject.ItemQuality.C, GunClass.PISTOL, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 44);
            var projectile = EasyProjectileInit<Projectile>("RevolveverProjectile", null, 13f, 23, 60f, 15f, false, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
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
