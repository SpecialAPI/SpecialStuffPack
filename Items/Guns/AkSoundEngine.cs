using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AkSHITengine = AkSoundEngine;

namespace SpecialStuffPack.Items.Guns
{
    public class AkSoundEngineGun : GunBehaviour
    {
        public static void Init()
        {
            string name = "AK-SoundEngine";
            string shortdesc = "The worst of the worst";
            string longdesc = "Fires garbage. Very rapidly.\n\nThis version of the AK-47 was created by a complete idiot. It was initially intended to be a speaker but the creator was so dumb that it turned into a gun instead. " +
                "Instead of firing bullets, this gun makes random noises. The pure trashiness of this gun attracts garbage so quickly that it seems like it shoots it.";
            var gun = GunBuilder.EasyGunInit("soundengine", name, shortdesc, longdesc, "soundengine_idle_001", "soundengine_idle_001", "gunsprites/aksoundengine/", 500, 1f, new(27, 5), Empty,
                "audioshitnetic", PickupObject.ItemQuality.C, GunClass.POISON, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 16);
            gun.AddComponent<AkSoundEngineGun>().switches = PickupObjectDatabase.Instance.Objects.OfType<Gun>().Select(x => x.gunSwitchGroup).ToList();
            var trashbag = GetProjectile(154);
            var trash = trashbag.GetComponent<SpawnProjModifier>().projectileToSpawnOnCollision;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                angleVariance = 10f,
                projectiles = new()
                {
                    trashbag,
                    trash,
                    trash
                },
                numberOfShotsInClip = 30,
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                cooldownTime = 0.11f
            });
            finish();
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            gun.gunSwitchGroup = switches.RandomElement();
            AkSHITengine.SetSwitch("WPN_Guns", gun.gunSwitchGroup, gun.gameObject);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (gun.IsReloading)
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(player.specRigidbody.UnitBottomCenter, 2f, 0.5f, false);
            }
        }

        public List<string> switches;
    }
}
