using SpecialStuffPack.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Magnificus
    {
        public static void Init()
        {
            string name = "Blowfish";
            string shortdesc = "His name is Magnificus";
            string londesc = "Shoots cards. The final shot is a gemstone that floats in the air, empowering cards that come near it.\n\nWhile this may seem like a regular blowfish, this is actually an almighty card-playing, " +
                "student-torturing future-predicting picture-painting blowfish with a magical eye. So, basically the exact same thing.";
            var gun = EasyGunInit("magnificus", name, shortdesc, londesc, "magnificus_idle_001", "magnificus_idle_001", "gunsprites/magnificus", 200, 1f, new(23, 8), BundleOfWandsObject.muzzleFlashEffects, "Kthulu",
                PickupObject.ItemQuality.B, GunClass.SILLY, out var finish);
            var card = EasyProjectileInit<Projectile>("MagnificusCardProjectile", "magnificus_projectile_001", 9, 23, 1000, 10f, true, false, false, null);
            card.angularVelocity = 220f;
            card.angularVelocityVariance = 180f;
            var aura = 2.5f;
            var auraSynergy = 4f;
            var damp = 3f;
            var destroyTime = 5f;
            var destroyTimeSynergy = 6f;
            var finalGreen = EasyProjectileInit<Projectile>("MagnificusMoxProjectile_Green", "magnificus_mox_green_001", 2.5f, 23, 1000, 10f, false, false, false, null);
            finalGreen.baseData.damping = damp;
            finalGreen.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 99999;
            var greenMox = finalGreen.AddComponent<MoxProjectile>();
            greenMox.auraRadius = aura;
            greenMox.type = MoxProjectile.MoxType.Green;
            greenMox.auraColor = new Color32(187, 220, 118, 255);
            greenMox.destroyTime = destroyTime;
            greenMox.synergy = "The Goo Mage";
            greenMox.slowness = new()
            {
                SpeedMultiplier = 0.2f,
                CooldownMultiplier = 1.5f,
                OverheadVFX = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Speed_Status"),
                effectIdentifier = "effect",
                AffectsEnemies = true,
                AffectsPlayers = false,
                OnlyAffectPlayerWhenGrounded = false,
                AppliesDeathTint = true,
                AppliesTint = true,
                TintColor = new Color32(187, 220, 118, 255),
                AppliesOutlineTint = false,
                DeathTintColor = new Color32(187, 220, 118, 255),
                duration = 0.1f,
                maxStackedDuration = -1f,
                OutlineTintColor = Color.black,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = GameActorEffect.EffectStackingMode.Refresh,
            };
            greenMox.synergyRadius = auraSynergy;
            greenMox.destroyTimeSynergy = destroyTimeSynergy;
            var finalOrange = EasyProjectileInit<Projectile>("MagnificusMoxProjectile_Orange", "magnificus_mox_orange_001", 2.5f, 23, 1000, 10f, false, false, false, null);
            finalOrange.baseData.damping = damp;
            finalOrange.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 99999;
            var orangeMox = finalOrange.AddComponent<MoxProjectile>();
            orangeMox.auraRadius = aura;
            orangeMox.type = MoxProjectile.MoxType.Orange;
            orangeMox.auraColor = new Color32(246, 129, 37, 255);
            orangeMox.destroyTime = destroyTime;
            orangeMox.synergy = "The Pike Mage";
            orangeMox.burn = new()
            {
                flameBuffer = new(0.0625f, 0.3f),
                flameFpsVariation = 0.5f,
                flameMoveChance = 1f,
                flameNumPerSquareUnit = 10,
                FlameVfx = new()
                {
                    LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Fire_Status_Body_001"),
                    LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Fire_Status_Body_002")
                },
                IsGreenFire = false,
                DamagePerSecondToEnemies = 4f,
                ignitesGoops = true,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesDeathTint = true,
                AppliesOutlineTint = false,
                AppliesTint = true,
                DeathTintColor = new(0.3882f, 0.3882f, 0.3882f),
                duration = 0.1f,
                effectIdentifier = "fire",
                maxStackedDuration = -1f,
                OutlineTintColor = Color.black,
                OverheadVFX = null,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = GameActorEffect.EffectStackingMode.Refresh,
                TintColor = Color.red
            };
            orangeMox.synergyRadius = auraSynergy;
            orangeMox.destroyTimeSynergy = destroyTimeSynergy;
            var finalBlue = EasyProjectileInit<Projectile>("MagnificusMoxProjectile_Blue", "magnificus_mox_blue_001", 2.5f, 23, 1000, 10f, false, false, false, null);
            finalBlue.baseData.damping = damp;
            finalBlue.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 99999;
            var blueMox = finalBlue.AddComponent<MoxProjectile>();
            blueMox.auraRadius = aura;
            blueMox.type = MoxProjectile.MoxType.Blue;
            blueMox.auraColor = new Color32(63, 162, 115, 255);
            blueMox.destroyTime = destroyTime;
            blueMox.synergy = "The Lonely Mage";
            blueMox.synergyRadius = auraSynergy;
            blueMox.destroyTimeSynergy = destroyTimeSynergy;
            /*var finalMagnum = EasyProjectileInit<Projectile>("MagnificusMoxProjectile_Magnum", "magnificus_mox_magnum_001", 2.5f, 23, 1000, 10f, false, false, false, null);
            finalMagnum.baseData.damping = damp;
            finalMagnum.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 99999;
            var magnumMox = finalBlue.AddComponent<MoxProjectile>();
            magnumMox.auraRadius = aura;
            magnumMox.type = MoxProjectile.MoxType.Magnum;
            magnumMox.auraColor = new Color32(238, 54, 93, 255);
            magnumMox.destroyTime = destroyTime;
            magnumMox.synergy = "The Lonely Mage";
            magnumMox.otherSynergy = "The Pike Mage";
            magnumMox.otherOtherSynergy = "The Goo Mage";
            magnumMox.synergyRadius = auraSynergy;
            magnumMox.destroyTimeSynergy = destroyTimeSynergy;
            magnumMox.slowness = new()
            {
                SpeedMultiplier = 0.2f,
                CooldownMultiplier = 1.5f,
                OverheadVFX = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Speed_Status"),
                effectIdentifier = "effect",
                AffectsEnemies = true,
                AffectsPlayers = false,
                OnlyAffectPlayerWhenGrounded = false,
                AppliesDeathTint = true,
                AppliesTint = true,
                TintColor = new Color32(187, 220, 118, 255),
                AppliesOutlineTint = false,
                DeathTintColor = new Color32(187, 220, 118, 255),
                duration = 0.1f,
                maxStackedDuration = -1f,
                OutlineTintColor = Color.black,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = GameActorEffect.EffectStackingMode.Refresh,
            };
            magnumMox.burn = new()
            {
                flameBuffer = new(0.0625f, 0.3f),
                flameFpsVariation = 0.5f,
                flameMoveChance = 1f,
                flameNumPerSquareUnit = 10,
                FlameVfx = new()
                {
                    LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Fire_Status_Body_001"),
                    LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Fire_Status_Body_002")
                },
                IsGreenFire = false,
                DamagePerSecondToEnemies = 4f,
                ignitesGoops = true,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesDeathTint = true,
                AppliesOutlineTint = false,
                AppliesTint = true,
                DeathTintColor = new(0.3882f, 0.3882f, 0.3882f),
                duration = 0.1f,
                effectIdentifier = "fire",
                maxStackedDuration = -1f,
                OutlineTintColor = Color.black,
                OverheadVFX = null,
                PlaysVFXOnActor = false,
                resistanceType = EffectResistanceType.None,
                stackMode = GameActorEffect.EffectStackingMode.Refresh,
                TintColor = Color.red
            };*/
            gun.Volley.projectiles.Add(new()
            {
                numberOfShotsInClip = 6,
                numberOfFinalProjectiles = 1,
                projectiles = new()
                {
                    card
                },
                finalVolley = new()
                {
                    name = "Magnificus Final",
                    projectiles = new()
                    {
                        new()
                        {
                            projectiles = new()
                            {
                                finalGreen,
                                finalOrange,
                                finalBlue
                            },
                            numberOfShotsInClip = 1,
                            cooldownTime = 0.15f,
                            angleVariance = 5f
                        }
                    }
                },
                usesOptionalFinalProjectile = true,
                cooldownTime = 0.15f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = CustomAmmoUtility.AddCustomAmmoType("magnificus", "MagnificusAmmoType", "MagnificusAmmoTypeEmpty", "magnificus_projectile", "magnificus_projectile_empty"),
                finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM,
                finalCustomAmmoType = CustomAmmoUtility.AddCustomAmmoType("magnificus_final", "MagnificusFinalAmmoType", "MagnificusFinalAmmoTypeEmpty", "magnificus_final", "magnificus_final_empty"),
                angleVariance = 8f
            });
            var replace = gun.AddComponent<VolleyReplacementSynergyProcessor>();
            replace.RequiredSynergy = ETGModCompatibility.ExtendEnum<CustomSynergyType>(SpecialStuffModule.GUID, "MagnumMox");
            replace.SynergyVolley = new()
            {
                projectiles = new()
                {
                    new()
                    {
                        numberOfShotsInClip = 6,
                        numberOfFinalProjectiles = 1,
                        projectiles = new()
                        {
                            card
                        },
                        finalVolley = new()
                        {
                            name = "Magnificus Final",
                            projectiles = new()
                            {
                                new()
                                {
                                    projectiles = new()
                                    {
                                        finalGreen
                                    },
                                    numberOfShotsInClip = -1,
                                    cooldownTime = 0.15f,
                                    angleVariance = 5f,
                                    angleFromAim = 30f
                                },
                                new()
                                {
                                    projectiles = new()
                                    {
                                        finalOrange
                                    },
                                    numberOfShotsInClip = -1,
                                    cooldownTime = 0.15f,
                                    angleVariance = 5f,
                                    angleFromAim = 0f,
                                    ammoCost = 0
                                },
                                new()
                                {
                                    projectiles = new()
                                    {
                                        finalBlue
                                    },
                                    numberOfShotsInClip = -1,
                                    cooldownTime = 0.15f,
                                    angleVariance = 5f,
                                    angleFromAim = -30f,
                                    ammoCost = 0
                                }
                            }
                        },
                        usesOptionalFinalProjectile = true,
                        cooldownTime = 0.15f,
                        ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                        customAmmoType = "magnificus",
                        finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM,
                        finalCustomAmmoType = "magnificus_final",
                        angleVariance = 8f,
                    }
                }
            };
            finish();
        }
    }
}
