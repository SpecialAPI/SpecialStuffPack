using Alexandria.SoundAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class BellRinger
    {
        public static void Init()
        {
            var name = "Bell Ringer";
            var shortdesc = "Heaven is close";
            var longdesc = "Shots apply Divine Protection to enemies before hitting them.";
            var gun = EasyGunInit("bellringer", name, shortdesc, longdesc, "bellringer_idle_001", "bellringer_idle_001", "gunsprites/bellringer", 150, 1f, new(16, 9), FaceMelterObject.muzzleFlashEffects, "SPAPI_BellRinger", PickupObject.ItemQuality.S, GunClass.PISTOL, out var finish);
            var proj = EasyProjectileInit<Projectile>("bellringerprojectile", null, 9f, 23f, 1000f, 10f, true, false, false, 136);
            proj.AddComponent<ApplyDivineProtection>();
            var anim = proj.transform.Find("Sprite").AddComponent<tk2dSpriteAnimator>();
            anim.Library = LoadHelper.LoadAssetFromAnywhere<GameObject>("ProjectileAnimation").GetComponent<tk2dSpriteAnimation>();
            anim.DefaultClipId = 21;
            anim.playAutomatically = true;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                cooldownTime = 0.15f,
                projectiles = new()
                {
                    proj
                },
                numberOfShotsInClip = 6,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = CustomAmmoUtility.AddCustomAmmoType("SPAPI_Holy", "holyammotype", "holyammotypeempty", "holy_ammo_type", "holy_ammo_type_empty"),
                angleVariance = 5f
            });
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_BellRinger", "Play_WPN_Gun_Shot_01", "BrutalOrchestraBellRing");
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_BellRinger", "Play_WPN_Gun_Reload_01");
            finish();
        }
    }
}
