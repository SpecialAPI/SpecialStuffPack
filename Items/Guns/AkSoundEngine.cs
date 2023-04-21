using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AkSoundShitgine = AkSoundEngine;

namespace SpecialStuffPack.Items.Guns
{
    public class AkSoundEngineGun : GunBehaviour
    {
        public static void Init()
        {
            string name = "AK-SoundEngine";
            string shortdesc = "The worst of the worst";
            string longdesc = "Fires garbage. Very rapidly.\n\n" +
                "STOP DOING WWISE\n" +
                " * SOUNDS WERE NOT SUPPOSED TO BE SWITCHED\n" +
                " * YEARS OF USING WWISE yet NO REAL-WORLD USE FOUND FOR DOING ANYTHING BEYOND UNITY'S BUILTIN SOUND SYSTEM\n" +
                " * Wanted to use audio switches anyways? We had a tool for that: It was called \"custom code\"\n" +
                " * \"Yes please give me rtpc of something. Please give me Init.bnk of it\" - Statements dreamed up by the utterly Deranged.\n" +
                "LOOK at what Wwise developers have been demanding your Respect for all this time, with all the sounds and events we built for them\n" +
                "(This is REAL Wwise, done by REAL Wwise users):\n" +
                "{Wwise Screenshot}\n" +
                "     ?????\n" +
                "{Wwise Screenshot}\n" +
                "     ???????\n" +
                "{Wwise Screenshot}\n" +
                "??????????????????????\n" +
                "\"Hello I would like {Wwise logo} apples please\"\n" +
                "They have played us for absolute fools";
            var gun = EasyGunInit("soundengine", name, shortdesc, longdesc, "soundengine_idle_001", "soundengine_idle_001", "gunsprites/aksoundengine/", 500, 1f, new(27, 5), Empty,
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
            AkSoundShitgine.SetSwitch("WPN_Guns", gun.gunSwitchGroup, gun.gameObject);
            if (PlayerOwner.PlayerHasActiveSynergy("#ADVANCED_FEATURE-RICH_INTERACTIVE"))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(gun.barrelOffset.position, 1f, 0.5f, false);
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (gun.IsReloading)
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(player.specRigidbody.UnitBottomCenter, PlayerOwner.PlayerHasActiveSynergy("#ADVANCED_FEATURE-RICH_INTERACTIVE") ? 4.5f : 2f, 0.5f, false);
            }
        }

        public List<string> switches;
    }
}
