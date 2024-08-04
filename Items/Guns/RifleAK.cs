using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class RifleAK
    {
        public static void Init()
        {
            var name = "AK-1/47";
            var shortdesc = "Accept All Substitutes";
            var longdesc = "Slowly fires powerful bullets.\n\nThe AK-1/47 is in many ways the gun which is judged against all other guns. This unaffordable and unreliable piece of hardware hasn't proven itself in nearly any terrain or situation. Desert, jungle, snow, and dungeon were all not accounted for in its timeless design. It can't even fire underwater.";
            var gun = EasyGunInit("rifleak", name, shortdesc, longdesc, "rifleak_idle_001", "rifleak_idle_001", "gunsprites/rifleak", 100, 0.9f, new(31, 6), Ak47Object.muzzleFlashEffects, "ak47", PickupObject.ItemQuality.B, GunClass.RIFLE, out var finish);
            var proj = EasyProjectileInit<Projectile>("rifleakprojectile", null, 27.5f, 40, 1000f, 9f, false, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    proj
                },
                angleFromAim = 0f,
                ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET,
                angleVariance = 3f,
                cooldownTime = 0.55f,
                numberOfShotsInClip = 6
            });
            finish();
        }
    }
}
