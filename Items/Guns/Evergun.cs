using Alexandria.SoundAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Evergun : GunBehaviour
    {
        public static void Init()
        {
            string name = "Evergun";
            string shortdesc = "An Ineffable Tale";
            string longdesc = "";
            var gun = EasyGunInit("evergun", name, shortdesc, longdesc, "evergun_idle_001", "evergun_idle_001", "gunsprites/evergun", 100, 1f, new(17, 9), Empty, "SPAPI_Evergun", PickupObject.ItemQuality.B, GunClass.NONE, out var finish);
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", "EvergunFire");
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", "EvergunReload");
            var maintex = gun.sprite.CurrentSprite.material.mainTexture;
            gun.sprite.CurrentSprite.material = new(GunnerObject.sprite.CurrentSprite.material);
            gun.sprite.CurrentSprite.materialInst = gun.sprite.CurrentSprite.material;
            gun.sprite.CurrentSprite.material.mainTexture = maintex;
            (gun.sprite as tk2dSprite).GenerateUV2 = true;
            gun.PreventOutlines = true;
            gun.Volley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    LuxinCannonObject.GetProjectile()
                }
            });
            var evergun = gun.AddComponent<Evergun>();
            finish();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
