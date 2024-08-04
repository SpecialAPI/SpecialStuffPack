using Alexandria.SoundAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Slingshot
    {
        public static void Init()
        {
            var name = "Slingshot";
            var shortdesc = "Primitive";
            var longdesc = "Projectiles ricochet off of enemies.\n\nThis weapon was used by a one-armed potato to obliterate an army of purple aliens before being hit by a rhino and dying.";
            var gun = EasyGunInit("slingshot", name, shortdesc, longdesc, "slingshot_idle_001", "slingshot_idle_001", "gunsprites/slingshot", 100, 1f, new(16, 6), Empty, "SPAPI_slingshot", PickupObject.ItemQuality.B, GunClass.CHARGE, out var finish);
            var proj = EasyProjectileInit<Projectile>("projectiles/slingshotprojectile", string.Empty, 8f, 23f, 1000f, 0f, true, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("rock_projectile_001"), tk2dBaseSprite.Anchor.LowerLeft, 0, 0, null, null, null, null);
            proj.hitEffects = SlingObject.GetProjectile().hitEffects;
            proj.AddComponent<DontLoseDamageOnPierce>();
            proj.AddComponent<RicochetProjectile>();
            proj.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 3;
            var anim = gun.spriteAnimator.GetClipByName(gun.chargeAnimation);
            anim.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            anim.loopStart = 4;
            gun.carryPixelOffset = new(6, 0);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                chargeProjectiles = new()
                {
                    new()
                    {
                        ChargeTime = 0.35f,
                        Projectile = proj
                    }
                },
                numberOfShotsInClip = 3,
                cooldownTime = 0.2f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "rock",
                shootStyle = ProjectileModule.ShootStyle.Charged
            });
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_slingshot", "Play_WPN_Gun_Charge_01", "BowChargeLessDumb");
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_slingshot", "Play_WPN_Gun_Shot_01", new SwitchedEvent("Play_WPN_Gun_Shot_01", "WPN_Guns", "Sling"));
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_slingshot", "Play_WPN_Gun_Reload_01", new SwitchedEvent("Play_WPN_Gun_Reload_01", "WPN_Guns", "Sling"));
            finish();
        }
    }
}
