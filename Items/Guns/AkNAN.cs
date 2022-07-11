using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class AkNAN : AdvancedGunBehaviour
    {
        public static void Init()
        {
            string name = "Ak-NAN";
            string shortdesc = "0f / 0f";
            string longdesc = "s Shotgun fires a si||???()NLOCK: Unlock this weapon by de</>**d Lord - Single target, semi-automots in a V patail of water. Clip this weapon by reaching Bullng a new room while under!&&!!";
            var gun = GunBuilder.EasyGunInit("guns/nank", name, shortdesc, longdesc, "nank_idle_001", "gunsprites/ammonomicon/nank_idle_001", "gunsprites/aknan", 500, 0.5f, Vector3.zero,
                CodeShortcuts.GetItemById<Gun>(537).CriticalMuzzleFlashEffects, "ak47", PickupObject.ItemQuality.A, GunClass.SILLY, SpecialStuffModule.globalPrefix, out var finish);
            gun.SetAnimationFPS("nank_reload", 16);
            gun.usesContinuousFireAnimation = true;
            gun.AddComponent<AkNAN>();
            gun.RawSourceVolley.projectiles.Add(new()
            {
                numberOfShotsInClip = 30,
                cooldownTime = 0.11f,
                angleVariance = 0f,
                projectiles = new()
                {
                    CodeShortcuts.GetItemById<Gun>(15).DefaultModule.projectiles[0]
                },
                shootStyle = ProjectileModule.ShootStyle.Automatic
            });
            finish();
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.shouldRotate = BraveUtility.RandomBool();
            projectile.baseData.damage *= Random.Range(-1f, 3f);
            projectile.baseData.range *= Random.Range(0.1f, 10f);
            projectile.baseData.speed *= Random.Range(-25f, 25f);
            projectile.AdditionalScaleMultiplier = Random.Range(0.1f, 5f);
            projectile.transform.position = projectile.transform.position + (Random.insideUnitCircle * Random.Range(0f, 3f)).ToVector3ZUp(0f);
            projectile.specRigidbody.Reinitialize();
            projectile.SendInDirection(Random.insideUnitCircle, false, true);
            projectile.GetOrAddComponent<BounceProjModifier>().numberOfBounces += Random.Range(-25, 25);
            projectile.GetOrAddComponent<PierceProjModifier>().penetration += Random.Range(-50, 50);
            var homing = projectile.GetOrAddComponent<HomingModifier>();
            homing.HomingRadius = Random.Range(0f, 20f);
            homing.AngularVelocity = Random.Range(-50, 420);
        }
    }
}
