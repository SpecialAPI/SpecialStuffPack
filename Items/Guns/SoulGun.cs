using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class SoulGun : GunBehaviour
    {
        public static void Init()
        {
            string name = "Soul Gun";
            string shortdesc = "";
            string longdesc = "";
            var gun = EasyGunInit("guns/soulgun/soulgun_soul_base", name, shortdesc, longdesc, "soulgun_soul_base_idle_001", "gunsprites/ammonomicon/soulgun_soul_base_idle_001", "gunsprites/soulgun", 
                100, 0f, new(13, 6), GetItemById<Gun>(223).muzzleFlashEffects, "Skullgun", PickupObject.ItemQuality.S, GunClass.NONE, out var finish, null, null, null);
            ProjectileModule module = new()
            {
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                projectiles = new List<Projectile>
                {
                    EasyProjectileInit<Projectile>("projectiles/soulgun/projectile_soul_base", "", 10f, 11, 9, 10, true, false, false, 
                        ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("floating_eye_projectile_003"), tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null)
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = 10f,
                cooldownTime = 0.55f,
                numberOfShotsInClip = 1
            };
            gun.InfiniteAmmo = true;
            gun.RawSourceVolley.projectiles.Add(module);
            gun.AddComponent<SoulGun>();
            gun.gunHandedness = GunHandedness.NoHanded;
            gun.carryPixelOffset = new(16, 0);
            gun.IgnoresAngleQuantization = true;
            finish();
            for (int i = 1; i < (int)SoulGunForm.Rocket_Mine + 1; i++)
            {
                var form = (SoulGunForm)i;
                var stats = GunStats[i];
                InitForm(form, 
                    stats[0] / 100, stats[1] / 100, stats[2] / 100, stats[3] / 100, stats[4], 
                    form is SoulGunForm.Sniper_Reload, 
                    form is SoulGunForm.Shotgun_Base or SoulGunForm.Shotgun_Death,
                    form is SoulGunForm.Shotgun_Cluster,
                    form is SoulGunForm.Rocket_Base or SoulGunForm.Rocket_Twin or SoulGunForm.Rocket_Mine,
                    form is SoulGunForm.Rocket_Twin, 
                    form is SoulGunForm.Machine_Burst,
                    form is SoulGunForm.Laser_Base or SoulGunForm.Laser_Freeze,
                    form is SoulGunForm.Laser_Constant,
                    form is SoulGunForm.Charge_Base or SoulGunForm.Charge_Overheat,
                    form is SoulGunForm.Orbit_Base,
                    form is SoulGunForm.Orbit_Stationary,
                    form is SoulGunForm.Orbit_Blast,
                    form is SoulGunForm.Sniper_Hunter,
                    form is SoulGunForm.Shotgun_Death,
                    form is SoulGunForm.Sniper_Base or SoulGunForm.Sniper_Hunter or SoulGunForm.Sniper_Reload);
            }
        }

        public static void InitForm(SoulGunForm form, float damagemult, float fireratemult, float rangemult, float projspeedmult, float spread, bool isReloadSniper, bool isShotgun, bool isCluster, bool isRocket, bool isTwin,
            bool isBurst, bool isBeam, bool isConstantBeam, bool isCharge, bool isOrbit, bool isOrbitStationary, bool isOrbitBlast, bool isHunterSniper, bool usesDeathProjectiles, bool usesSmallProjectiles)
        {
            string name = form.ToString();
            string shortdesc = "";
            string longdesc = "";
            var gun = EasyGunInit("guns/soulgun/soulgun_" + form.ToString().ToLower(), name, shortdesc, longdesc, $"soulgun_{form.ToString().ToLower()}_idle_001", 
                "gunsprites/ammonomicon/soulgun_soul_base_idle_001", "gunsprites/soulgun", 100, isReloadSniper ? 2f : 0f, new(0, 0), GetItemById<Gun>(223).muzzleFlashEffects, "Skullgun", 
                PickupObject.ItemQuality.EXCLUDED, GunClass.NONE, out var finish, null, null, null);
            var spriteId = "floating_eye_projectile_003";
            if (isRocket)
            {
                spriteId = "nikita_missile_001";
            }
            else if (usesDeathProjectiles)
            {
                spriteId = "blue_skull_projectile_001003";
            }
            var damage = 10f * damagemult;
            var projectile = EasyProjectileInit<Projectile>("projectiles/soulgun/projectile_" + form.ToString().ToLower(), "", damage, 11 * projspeedmult, 9 * rangemult, 10, true, false, false,
                        ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName(spriteId), tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            if (isRocket)
            {
                projectile.AddComponent<RocketLauncherProjectile>().Damage = damage;
            }
            if (isTwin)
            {
                var homing = projectile.GetOrAddComponent<HomingModifier>();
                homing.HomingRadius = 3;
                homing.AngularVelocity = 210;
            }
            if (usesSmallProjectiles)
            {
                projectile.AdditionalScaleMultiplier = 0.75f;
            }
            if (isHunterSniper)
            {
                var bounce = projectile.GetOrAddComponent<BounceProjModifier>();
                bounce.numberOfBounces += 1;
                bounce.damageMultiplierOnBounce *= 2f;
                projectile.AddComponent<BounceResetsDistance>();
            }
            float twinAngleFromAim = isTwin ? 15f : 0f;
            ProjectileModule module = new ProjectileModule
            {
                shootStyle = isReloadSniper ? ProjectileModule.ShootStyle.SemiAutomatic : isBurst ? ProjectileModule.ShootStyle.Burst : ProjectileModule.ShootStyle.Automatic,
                ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                projectiles = new List<Projectile>
                {
                    projectile
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = spread / 2,
                cooldownTime = 0.55f / fireratemult,
                burstCooldownTime = 0.1f,
                burstShotCount = 4,
                numberOfShotsInClip = isReloadSniper ? 3 : isBurst ? 4 : 1,
                angleFromAim = twinAngleFromAim
            };
            gun.RawSourceVolley.projectiles.Add(module);
            if (isTwin)
            {
                ProjectileModule module2 = new ProjectileModule
                {
                    shootStyle = isReloadSniper ? ProjectileModule.ShootStyle.SemiAutomatic : isBurst ? ProjectileModule.ShootStyle.Burst : ProjectileModule.ShootStyle.Automatic,
                    ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                    projectiles = new List<Projectile>
                    {
                        projectile
                    },
                    orderedGroupCounts = new List<int>(),
                    numberOfFinalProjectiles = 0,
                    angleVariance = spread / 2,
                    cooldownTime = 0.55f / fireratemult,
                    burstCooldownTime = 0.1f,
                    burstShotCount = 4,
                    numberOfShotsInClip = (isReloadSniper ? 3 : 1),
                    angleFromAim = -twinAngleFromAim
                };
                gun.RawSourceVolley.projectiles.Add(module2);
            }
            if (isShotgun)
            {
                float angleOffset = 30f;
                ProjectileModule module2 = new ProjectileModule
                {
                    shootStyle = isReloadSniper ? ProjectileModule.ShootStyle.SemiAutomatic : ProjectileModule.ShootStyle.Automatic,
                    ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                    projectiles = new List<Projectile>
                    {
                        projectile
                    },
                    orderedGroupCounts = new List<int>(),
                    numberOfFinalProjectiles = 0,
                    angleVariance = spread / 2,
                    cooldownTime = 0.55f / fireratemult,
                    numberOfShotsInClip = (isReloadSniper ? 3 : 1),
                    angleFromAim = angleOffset
                };
                gun.RawSourceVolley.projectiles.Add(module2);
                ProjectileModule module3 = new ProjectileModule
                {
                    shootStyle = isReloadSniper ? ProjectileModule.ShootStyle.SemiAutomatic : ProjectileModule.ShootStyle.Automatic,
                    ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                    projectiles = new List<Projectile>
                    {
                        projectile
                    },
                    orderedGroupCounts = new List<int>(),
                    numberOfFinalProjectiles = 0,
                    angleVariance = spread / 2,
                    cooldownTime = 0.55f / fireratemult,
                    numberOfShotsInClip = (isReloadSniper ? 3 : 1),
                    angleFromAim = -angleOffset
                };
                gun.RawSourceVolley.projectiles.Add(module3);
            }
            if (isCluster)
            {
                for(int i = 0; i < 4; i++)
                {
                    ProjectileModule module2 = new ProjectileModule
                    {
                        shootStyle = isReloadSniper ? ProjectileModule.ShootStyle.SemiAutomatic : ProjectileModule.ShootStyle.Automatic,
                        ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN,
                        projectiles = new List<Projectile>
                        {
                            projectile
                        },
                        orderedGroupCounts = new List<int>(),
                        numberOfFinalProjectiles = 0,
                        angleVariance = spread / 2,
                        cooldownTime = 0.55f / fireratemult,
                        numberOfShotsInClip = (isReloadSniper ? 3 : 1)
                    };
                    gun.RawSourceVolley.projectiles.Add(module2);
                }
            }
            gun.gunHandedness = GunHandedness.NoHanded;
            gun.carryPixelOffset = new(16, 0);
            gun.IgnoresAngleQuantization = true;
            finish();
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(CurrentForm is SoulGunForm.Shotgun_Cluster)
            {
                var val = Random.Range(0.5f, 2f);
                projectile.baseData.speed *= val;
                projectile.baseData.range *= val;
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            if(CurrentForm is SoulGunForm.Sniper_Reload)
            {
                return;
            }
            List<ProjectileModule> modules = new List<ProjectileModule>();
            if (gun.Volley != null)
            {
                foreach (ProjectileModule mod in gun.Volley.projectiles)
                {
                    modules.Add(mod);
                }
            }
            else
            {
                modules.Add(gun.singleModule);
            }
            if (CurrentForm is SoulGunForm.Machine_Burst)
            {
                var hasAtLeastOneFinishedModule = false;
                foreach(var mod in modules)
                {
                    if(mod.shootStyle != ProjectileModule.ShootStyle.Burst)
                    {
                        continue;
                    }
                    if(mod.burstShotCount <= gun.RuntimeModuleData[mod].numberShotsFiredThisBurst)
                    {
                        hasAtLeastOneFinishedModule = true;
                    }
                }
                if (!hasAtLeastOneFinishedModule)
                {
                    return;
                }
            }
            foreach (ProjectileModule mod in modules)
            {
                gun.RuntimeModuleData[mod].numberShotsFired = 0;
                gun.RuntimeModuleData[mod].needsReload = false;
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (manual && !gun.IsReloading)
            {
                CurrentForm++;
                if (CurrentForm > SoulGunForm.Rocket_Mine)
                {
                    CurrentForm = SoulGunForm.Soul_Base;
                }
                gun.TransformToTargetGun(CodeShortcuts.GetItemById<Gun>(gun.PickupObjectId + (int)CurrentForm));
            }
        }

        public SoulGunForm CurrentForm;

        public static List<float[]> GunStats = new()
        {
            // damage, firerate, range, speed, spread
            new float[] { 100f, 100f, 100f, 100f, 10f }, //soul base
            new float[] { 90f, 100f, 100f, 100f, 10f }, //soul buster
            new float[] { 80f, 100f, 100f, 100f, 10f }, //soul hyperlight
            new float[] { 70f, 100f, 100f, 100f, 10f }, //soul celestial
            new float[] { 100f, 80f, 50f, 120f, 0f }, //shotgun base
            new float[] { 100f, 80f, 50f, 120f, 45f }, //shotgun cluster
            new float[] { 50f, 80f, 100f, 120f, 0f }, //shotgun death
            new float[] { 60f, 200f, 100f, 100f, 20f }, //machine base
            new float[] { 70f, 60f, 100f, 100f, 15f }, //machine burst
            new float[] { 60f, 150f, 100f, 100f, 20f }, //machine clip
            new float[] { 160f, 60f, 150f, 215f, 0f }, //sniper base
            new float[] { 130f, 60f, 150f, 195f, 0f }, //sniper hunter
            new float[] { 150f, 350f, 150f, 215f, 0f }, //sniper reload
            new float[] { 80f, 80f, 90f, 100f, 10f }, //orbit base
            new float[] { 90f, 70f, 70f, 100f, 10f }, //orbit stationary
            new float[] { 100f, 80f, 100f, 100f, 10f }, //orbit blast
            new float[] { 90f, 100f, 100f, 100f, 10f }, //laser base
            new float[] { 40f, 100f, 100f, 100f, 10f }, //laser constant
            new float[] { 80f, 100f, 100f, 100f, 10f }, //laser freeze
            new float[] { 100f, 60f, 115f, 100f, 20f }, //charge base
            new float[] { 100f, 100f, 100f, 100f, 10f }, //charge minigun
            new float[] { 100f, 100f, 100f, 100f, 10f }, //charge overheat
            new float[] { 100f, 40f, 100f, 150f, 10f }, //rocket base
            new float[] { 60f, 40f, 120f, 70f, 0f }, //rocket twin
            new float[] { 60f, 60f, 100f, 150f, 10f }, //rocket mine
        };

        public enum SoulGunForm
        {
            Soul_Base,
            Soul_Buster,
            Soul_Hyperlight,
            Soul_Celestial,
            Shotgun_Base,
            Shotgun_Cluster,
            Shotgun_Death,
            Machine_Base,
            Machine_Burst,
            Machine_Clip,
            Sniper_Base,
            Sniper_Hunter,
            Sniper_Reload,
            Orbit_Base,
            Orbit_Stationary,
            Orbit_Blast,
            Laser_Base,
            Laser_Constant,
            Laser_Freeze,
            Charge_Base,
            Charge_Minigun,
            Charge_Overheat,
            Rocket_Base,
            Rocket_Twin,
            Rocket_Mine
        }
    }
}
