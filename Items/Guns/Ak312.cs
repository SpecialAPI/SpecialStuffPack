using Alexandria.SoundAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Ak312 : GunBehaviour
    {
        public static void Init()
        {
            var name = "AK-312";
            var shortdesc = "Tres Uno Dos";
            var longdesc = "Rewinds projectiles when reloading.\n\nA powerful artifact said to be able to bend time to the will of its owner.";
            var gun = EasyGunInit("ak312", name, shortdesc, longdesc, "ak312_idle_001", "ak312_idle_001", "gunsprites/ak312", 500, 1f, new(27, 5), Ak47Object.muzzleFlashEffects, "SPAPI_TresUndos", PickupObject.ItemQuality.S, GunClass.FULLAUTO, out var finish);
            gun.MakeContinuous();
            gun.AddComponent<Ak312>().undoes = 3;
            var projectile = EasyProjectileInit<Projectile>("Ak312Projectile", null, 5.5f, 23f, 1000f, 9f, false, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
            var undoProjectile = EasyProjectileInit<Projectile>("Ak312UndoProjectile", null, 5.5f, 23f, 1000f, 9f, false, false, true, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("bullet_variant_011"));
            undoProjectile.specRigidbody.CollideWithTileMap = false;
            undoProjectile.GetOrAddPierce().BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            undoProjectile.GetOrAddPierce().penetration = 1000;
            undoProjectile.GetOrAddPierce().penetratesBreakables = true;
            projectile.AddComponent<UndoProjectile>().projToFireOnUndo = undoProjectile;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    projectile
                },
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                angleVariance = 4f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 30
            });
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_TresUndos", "Play_WPN_Gun_Shot_01", "TresUndosMove");
            SoundManager.AddCustomSwitchData("WPN_Guns", "SPAPI_TresUndos", "Play_WPN_Gun_Reload_01", "TresUndosUndo");
            finish();
        }

        public int undoes;
    }
}
