using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class LeatherWhip
    {
        public static void Init()
        {
            var name = "Leather Whip";
            var shortdesc = "4 summon tag damage";
            var longdesc = "Just a leather whip.\n\nIn another world, this whip might have made your companions better at attacking, unfortunately that's not the case in the Gungeon.";
            var gun = GunBuilder.EasyGunInit("whip", name, shortdesc, longdesc, "whip_idle_001", "whip_idle_001", "gunsprites/whip/", 100, 0f, new(8, 8), Empty, "Baseball", PickupObject.ItemQuality.D, GunClass.BEAM,
                out var finish);
            var time = 0.25f;
            gun.SetAnimationFPS(Mathf.CeilToInt(1 / time));
            gun.LocalInfiniteAmmo = true;
            var damage = 9;
            var length = 6;
            var proj = GunBuilder.EasyProjectileInit<WhipProjectile>("WhipProjectile", "whip_projectile_001", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj.Length = length;
            proj.time = time;
            proj.startAngle = 30f;
            proj.endAngle = -30f;
            proj.whipPrefab = "WhipProjectile";
            proj.whipEndPrefab = "WhipEndProjectile";
            var pierce = proj.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 9999;
            pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce.preventPenetrationOfActors = false;
            pierce.penetratesBreakables = true;
            proj.specRigidbody.CollideWithTileMap = false;
            var proj2 = GunBuilder.EasyProjectileInit<WhipProjectile>("WhipEndProjectile", "whip_projectile_002", damage, 1f, 99999f, 0f, true, false, true, anchor: tk2dBaseSprite.Anchor.MiddleLeft);
            proj2.Length = length;
            proj2.time = time;
            proj2.startAngle = 30f;
            proj2.endAngle = -30f;
            proj2.maxScale = proj2.minScale = 1f;
            proj2.whipPrefab = "WhipProjectile";
            proj2.whipEndPrefab = "WhipEndProjectile";
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
