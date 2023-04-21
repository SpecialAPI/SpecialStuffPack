using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class GoldenShotgun
    {
        public static void Init()
        {
            var name = "Golden Shotgun";
            var shortdesc = "Run";
            var longdesc = "An extremely powreful shotgun.\n\nThe Golden Shotgun is a thing of beauty, with a gleaming, golden finish that catches the light and draws the eye. Despite its elegant appearance, it can easily decimate even the toughest foes.";
            var gun = EasyGunInit("gshotgun", name, shortdesc, longdesc, "gshotgun_idle_001", "gshotgun_idle_001", "gunsprites/goldenshotgun", 10000, 1f, new(25, 6), new()
            {
                effects = new VFXComplex[]
                {
                    new()
                    {
                        effects = new VFXObject[]
                        {
                            new()
                            {
                                zHeight = 1f,
                                destructible = false,
                                alignment = VFXAlignment.Fixed,
                                attached = false,
                                orphaned = true,
                                effect = GatlingGullEnemy.bulletBank.GetBullet("bigBullet").MuzzleFlashEffects.effects[0].effects[0].effect,
                                persistsOnDeath = false,
                                usesZHeight = true
                            }
                        }
                    }
                },
                type = VFXPoolType.Single
            }, "GoldenShotgun", PickupObject.ItemQuality.SPECIAL, GunClass.SHOTGUN, out var finish, null, null, null);
            SoundManager.AddCustomSwitchData("WPN_Guns", "GoldenShotgun", "Play_WPN_Gun_Shot_01", "PizzaTowerShotgunShot");
            SoundManager.AddCustomSwitchData("WPN_Guns", "GoldenShotgun", "Play_WPN_Gun_Reload_01");
            SoundManager.AddCustomSwitchData("WPN_Guns", "GoldenShotgun", "IntroFakeReload", "PizzaTowerShotgunLoad");
            gun.spriteAnimator.GetClipByName(gun.introAnimation).frames[7].triggerEvent = true;
            gun.spriteAnimator.GetClipByName(gun.introAnimation).frames[7].eventAudio = "IntroFakeReload";
            gun.SetAnimationFPS(gun.introAnimation, 13);
            gun.reloadTime = 0f;
            var projectile = EasyProjectileInit<Projectile>("GoldenShotgunProjectile", null, 10f, 60f, 10f, 30f, true, false, true, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("yellow_blaster_bolt_001"), tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null, ETGMod.Databases.Items.ProjectileCollection);

            var pierce = projectile.GetOrAddPierce();
            pierce.penetratesBreakables = true;
            pierce.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            pierce.penetration = 3;
            projectile.AddComponent<DontLoseDamageOnPierce>();
            projectile.hitEffects = RegularShotgunObject.GetProjectile().hitEffects;
            for(int i = 0; i < 6; i++)
            {
                gun.RawSourceVolley.projectiles.Add(new()
                {
                    ammoType = GameUIAmmoType.AmmoType.SHOTGUN,
                    numberOfShotsInClip = 10,
                    projectiles = new()
                    {
                        projectile
                    },
                    angleVariance = 15f,
                    cooldownTime = 0.75f
                });
            }
            gun.RawSourceVolley.UsesShotgunStyleVelocityRandomizer = true;
            gun.RawSourceVolley.IncreaseFinalSpeedPercentMax = 150f;
            gun.RawSourceVolley.DecreaseFinalSpeedPercentMin = 75f;
            gun.InfiniteAmmo = true;
            gun.CanBeDropped = false;
            finish();
        }
    }
}
