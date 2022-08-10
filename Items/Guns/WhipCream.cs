using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class WhipCream
    {
        public static void Init()
        {
            var name = "Whipped Cream";
            var shortdesc = "Dangerous food";
            var longdesc = "Some whipped cream, which can also act as a weapon.";
            var gun = GunBuilder.EasyGunInit("whipcream", name, shortdesc, longdesc, "whipcream_idle_001", "whipcream_idle_001", "gunsprites/whipcream/", 1000, 0f, new(8, 4), Empty, "EyeballGun",
                PickupObject.ItemQuality.B, GunClass.FULLAUTO, out var finish);
            var time = 0.25f;
            gun.MakeContinuous();
            gun.SetAnimationFPS(Mathf.CeilToInt(1 / time));
            gun.LocalInfiniteAmmo = true;
            var damage = 4.5f;
            var length = 5;
            var proj = GunBuilder.EasyProjectileInit<WhipProjectile>("WhipCreamProjectile", "whipped_cream_projectile_001", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj.Length = length;
            proj.time = time;
            proj.startAngle = 30f;
            proj.endAngle = -30f;
            proj.whipPrefab = "WhipCreamProjectile";
            var pierce = proj.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 9999;
            pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce.preventPenetrationOfActors = false;
            pierce.penetratesBreakables = true;
            proj.specRigidbody.CollideWithTileMap = false;
            var proj2 = GunBuilder.EasyProjectileInit<WhipProjectile>("WhipCreamProjectile2", "whipped_cream_projectile_001", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj2.Length = length;
            proj2.time = time;
            proj2.startAngle = -30f;
            proj2.endAngle = 30f;
            proj2.whipPrefab = "WhipCreamProjectile2";
            var pierce2 = proj2.GetOrAddComponent<PierceProjModifier>();
            pierce2.penetration = 9999;
            pierce2.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce2.preventPenetrationOfActors = false;
            pierce2.penetratesBreakables = true;
            proj2.specRigidbody.CollideWithTileMap = false;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                ammoType = GameUIAmmoType.AmmoType.BEAM,
                projectiles = new List<Projectile> { proj, proj2 },
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = time,
                numberOfShotsInClip = 100,
                sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered
            });
            finish();
        }
    }
}
