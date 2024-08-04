using Alexandria.SoundAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Sepulchrifle
    {
        public static void Init()
        {
            var name = "Sepulchrifle";
            var shortdesc = "Somebody Call the Vatican";
            var longdesc = "Fires 4 projectiles from random owned guns.";
            var gun = EasyGunInit("sepulchrifle", name, shortdesc, longdesc, "sepulchrifle_idle_001", "sepulchrifle_idle_001", "gunsprites/sepulchrifle", 200, 1f, new(15, 7), OriguniObject.muzzleFlashEffects, "SPAPI_Sepulchrifle", PickupObject.ItemQuality.A, GunClass.RIFLE, out var finish);
            for(int i = 0; i < 4; i++)
            {
                gun.RawSourceVolley.projectiles.Add(new()
                {
                    numberOfShotsInClip = 5,
                    ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                    customAmmoType = "SPAPI_Holy",
                    projectiles = new()
                    {
                        KlobbeObject.GetProjectile()
                    },
                    shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                    cooldownTime = 1f,
                    angleVariance = 15f
                });
            }
            gun.AddComponent<RandomOwnedProjectiles>().baseModules = 4;
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_Sepulchrifle", "Play_WPN_Gun_Shot_01", "BrutalOrchestraSepulchreHurt");
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_Sepulchrifle", "Play_WPN_Gun_Reload_01");
            finish();
        }
    }
}
