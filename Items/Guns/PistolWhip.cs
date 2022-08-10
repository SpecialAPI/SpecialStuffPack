using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class PistolWhip
    {
        public static void Init()
        {
            var name = "Pistol Whip";
            var shortdesc = "Whip made out of pistols";
            var longdesc = "A whip made out of pistols, quite literally. The pistols will fire when an enemy is hit with them.";
            var gun = EasyGunInit("pistolwhip", name, shortdesc, longdesc, "pistolwhip_idle_001", "pistolwhip_idle_001", "gunsprites/pistolwhip/", 100, 0f, new(12, 10), Empty, "Baseball", 
                PickupObject.ItemQuality.C, GunClass.PISTOL, out var finish);
            var time = 0.25f;
            gun.SetAnimationFPS(Mathf.CeilToInt(1 / time));
            gun.LocalInfiniteAmmo = true;
            var damage = 6;
            var length = 7;
            var bullet = GetProjectile(378);
            var proj = EasyProjectileInit<WhipProjectile>("PistolWhipProjectile", "pistolwhip_projectile_001", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj.Length = length;
            proj.time = time;
            proj.startAngle = 30f;
            proj.endAngle = -30f;
            proj.whipPrefab = "PistolWhipProjectile2";
            proj.maxScale = proj.minScale = 1f;
            var pwhip = proj.AddComponent<PistolWhipProjectile>();
            pwhip.isUp = false;
            pwhip.projToSpawn = bullet;
            var pierce = proj.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 9999;
            pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce.preventPenetrationOfActors = false;
            pierce.penetratesBreakables = true;
            proj.specRigidbody.CollideWithTileMap = false;
            var proj2 = EasyProjectileInit<WhipProjectile>("PistolWhipProjectile2", "pistolwhip_projectile_002", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj2.Length = length;
            proj2.time = time;
            proj2.startAngle = 30f;
            proj2.endAngle = -30f;
            proj2.maxScale = proj2.minScale = 1f;
            var pwhip2 = proj2.AddComponent<PistolWhipProjectile>();
            pwhip2.isUp = true;
            pwhip2.projToSpawn = bullet;
            proj2.whipPrefab = "PistolWhipProjectile";
            var pierce2 = proj2.GetOrAddComponent<PierceProjModifier>();
            pierce2.penetration = 9999;
            pierce2.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce2.preventPenetrationOfActors = false;
            pierce2.penetratesBreakables = true;
            proj2.specRigidbody.CollideWithTileMap = false;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                ammoType = GameUIAmmoType.AmmoType.BEAM,
                projectiles = new List<Projectile> { proj },
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = 0.75f,
                numberOfShotsInClip = 25
            });
            finish();
        }
    }
}
