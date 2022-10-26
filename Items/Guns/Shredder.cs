using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Shredder
    {
        public static void Init()
        {
            string name = "Shredder";
            string shortdesc = "For Shredding";
            string longdesc = "Projectiles have a chance to explode on hit.\n\nThese guns are usually used by potato people for handling large groups of aliens.";
            var gun = EasyGunInit("shredder", name, shortdesc, longdesc, "shredder_idle_001", "shredder_idle_001", "gunsprites/shredder", 200, 1f, new(18, 7), MagnumObject.muzzleFlashEffects, "Shotgun", PickupObject.ItemQuality.B,
                GunClass.EXPLOSIVE, out var finish);
            var projectile = EasyProjectileInit<Projectile>("ShredderProjectile", "shredder_projectile_001", 7, 23, 75, 10f, true, false, true, null, tk2dBaseSprite.Anchor.MiddleCenter);
            var pierce = projectile.AddComponent<PierceProjModifier>();
            pierce.penetration = 3;
            pierce.penetratesBreakables = true;
            projectile.AddComponent<DontLoseDamageOnPierce>();
            var explode = projectile.AddComponent<ChanceBasedExplodeOnEnemyHit>();
            explode.chance = 0.5f;
            explode.explosion = ExplosiveRoundsObject.ExplosionData.Copy();
            explode.explosion.damage = 7f;
            explode.explosion.doDestroyProjectiles = false;
            explode.increaseDamageSynergy = "Dynamite";
            explode.synergyDamageMultiplier = 1.15f;
            explode.increaseRangeSynergy = "Plastic Explosive";
            explode.synergyRangeMultiplier = 1.25f;
            explode.increaseChanceSynergy = "Shredder II";
            explode.synergyChanceOverride = 0.75f;
            gun.Volley.projectiles.Add(new()
            {
                cooldownTime = 0.35f,
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                numberOfShotsInClip = 6,
                angleVariance = 8f,
                projectiles = new()
                {
                    projectile
                }
            });
            finish();
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
        }
    }
}
