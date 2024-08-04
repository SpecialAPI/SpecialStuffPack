using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class ReverseAK
    {
        public static void Init()
        {
            var name = "AK -47";
            var shortdesc = "Setutitsbus On Tpecca";
            var longdesc = "Retawrednu erif neve nac ti. Ngised sselemit sti ni rof detnuocca lla erew noegnud dna ,wons ,elgnuj ,tresed. Noitautis ro niarret yna ylraen ni flesti nevorp sah erawdrah fo eceip elbailer dna elbadroffa siht. Degduj era snug rehto lla hcihw tsniaga nug eht syaw ynam ni si 74-KA eht.";
            var gun = EasyGunInit("reverseak", name, shortdesc, longdesc, "reverseak_idle_001", "reverseak_idle_001", "gunsprites/reverseak", 500, 0.5f, new(0, 5), Ak47Object.muzzleFlashEffects, "ak47", PickupObject.ItemQuality.C, GunClass.FULLAUTO, out var finish, null, $"{SpecialStuffModule.globalPrefix}:ak_-47");
            gun.MakeContinuous();
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    Ak47Object.GetProjectile()
                },
                angleFromAim = 180f,
                ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET,
                angleVariance = 4f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 30,
                shootStyle = ProjectileModule.ShootStyle.Automatic
            });
            finish();
        }
    }
}
