using SpecialStuffPack.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class CactiClub
    {
        public static void Init()
        {
            string name = "Cacti Launcher";
            string shortdesc = "Prickly and Explosive";
            string longdesc = "Shoots cactus rockets that shoot spikes when exploding\n\nA version of the famous Cacti Club modified to suit the Gungeon more.";
            var gun = EasyGunInit("guns/club", name, shortdesc, longdesc, "club_idle_001", "gunsprites/ammonomicon/club_idle_001", "gunsprites/cacticlub", 30, 3f, new(23, 7), Empty,
                "RPG", PickupObject.ItemQuality.B, GunClass.EXPLOSIVE, out var finish);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            var proj = EasyProjectileInit<Projectile>("projectiles/clubprojectile", "cacti_rocket_projectile_001", 25, 40f, 90f, 10f, true, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, 10, 6, 2, 2);
            proj.AddComponent<CactiClubProjectile>().projectileToShoot = GetItemById<Gun>(124).DefaultModule.projectiles[0];
            proj.baseData.UsesCustomAccelerationCurve = true;
            proj.baseData.CustomAccelerationCurveDuration = 1f;
            proj.baseData.AccelerationCurve = new(
                new Keyframe()
                {
                    time = 0.0002f,
                    value = 0.1996f,
                    inTangent = 0.649f,
                    outTangent = 0.649f
                },
                new Keyframe()
                {
                    time = 0.4031f,
                    value = 0.3721f,
                    inTangent = 0.6139f,
                    outTangent = 0.6139f
                },
                new Keyframe()
                {
                    time = 0.4949f,
                    value = 0.6545f,
                    inTangent = 2.8676f,
                    outTangent = 2.8676f
                },
                new Keyframe()
                {
                    time = 1f,
                    value = 1.013f,
                    inTangent = 0.2711f,
                    outTangent = 0.2711f
                });
            var explosion = proj.AddComponent<ExplosiveModifier>();
            explosion.doExplosion = true;
            explosion.explosionData = RpgObject.GetProjectile().GetComponent<ExplosiveModifier>().explosionData;
            Vector2 sparkOffset = new(-0.5f, 0f);
            var smoke = proj.AddComponent<ProjectileSparks>();
            smoke.startingLifetime = 0.3f;
            smoke.type = GlobalSparksDoer.SparksType.EMBERS_SWIRLING;
            smoke.size = 0.5f;
            smoke.sparkCooldown = 0.01f;
            smoke.angleVariance = 10f;
            smoke.color = Color.white;
            smoke.offset = sparkOffset;
            /*var fire = proj.AddComponent<ProjectileSparks>();
            fire.startingLifetime = 0.2f;
            fire.type = GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE;
            fire.size = 1f;
            fire.sparkCooldown = 0.15f;
            fire.angleVariance = 10f;
            fire.color = Color.white;
            fire.offset = sparkOffset;*/
            gun.RawSourceVolley.projectiles.Add(new()
            {
                numberOfShotsInClip = 1,
                cooldownTime = 1f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = CustomAmmoUtility.AddCustomAmmoType("cacticlub", "CactiAmmoType", "CactiAmmoTypeEmpty", "cacti_ammo_type", "cacti_ammo_type_empty"),
                projectiles = new()
                {
                    proj
                }
            });
            finish();
        }
    }
}
