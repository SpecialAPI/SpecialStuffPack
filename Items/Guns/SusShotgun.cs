using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class SusShotgun
    {
        public static void Init()
        {
            var name = "Suspicious Shotgun";
            var shortdesc = "The Train of Rain";
            var longdesc = "A weirdly familiar shotgun. The words \"Bow-Bow\" are engraved on the barrel.";
            var gun = EasyGunInit("sus_shotgun", name, shortdesc, longdesc, "sus_shotgun_idle_001", "sus_shotgun_idle_001", "gunsprites/suspiciousshotgun/", 200, 1f, new(23, 6), RegularShotgunObject.muzzleFlashEffects,
                "Shotgun", PickupObject.ItemQuality.B, GunClass.SHOTGUN, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);
            for(int i = 0; i < 6; i++)
            {
                gun.RawSourceVolley.projectiles.Add(new()
                {
                    projectiles = new() { RegularShotgunObject.GetProjectile() },
                    angleVariance = 20f,
                    cooldownTime = 0.6f,
                    numberOfShotsInClip = 8,
                    ammoCost = i == 0 ? 1 : 0
                });
            }
            finish();
            var reloadFire = gun.AddComponent<FireVolleyOnReload>();
            reloadFire.volley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            reloadFire.volley.projectiles = new()
            {
                new()
                {
                    projectiles = new()
                    {
                        MagnumObject.GetProjectile()
                    },
                    angleVariance = 0f,
                    numberOfShotsInClip = 1,
                    cooldownTime = 0f,
                }
            };
            reloadFire.switchGroup = "DesertEagle";
            reloadFire.sfx = "Play_WPN_Gun_Shot_01";
            reloadFire.shootOffset = new(15f / 16f, 3 / 16f);
            gun.AddTransformGunSynergyProcessor(reloadFire.synergy = "When the shotgun is sus!", InitSynergy(), false, -1);
        }

        public static int InitSynergy()
        {
            var name = "Sus Amongun";
            var shortdesc = "sussy";
            var longdesc = "amogus";
            var gun = EasyGunInit("sus_amongun", name, shortdesc, longdesc, "sus_amongun_idle_001", "sus_shotgun_idle_001", "gunsprites/susamongun/", 200, 1f, new(23, 6), RegularShotgunObject.muzzleFlashEffects,
                "Shotgun", PickupObject.ItemQuality.EXCLUDED, GunClass.NONE, out var finish, overrideConsoleId:$"{SpecialStuffModule.globalPrefix}:suspicious_shotgun+when_the_shotgun_is_sus");
            var proj = EasyProjectileInit<Projectile>("KnifeProjectile", "knife_projectile_001", 5, 23, 15, 30, true, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            proj.GetOrAddComponent<PierceProjModifier>().penetration += 1;
            proj.pierceMinorBreakables = true;
            for (int i = 0; i < 6; i++)
            {
                gun.RawSourceVolley.projectiles.Add(new()
                {
                    projectiles = new() { proj },
                    angleVariance = 20f,
                    cooldownTime = 0.6f,
                    numberOfShotsInClip = 8,
                    ammoCost = i == 0 ? 1 : 0
                });
            }
            finish();
            return gun.PickupObjectId;
        }
    }
}
