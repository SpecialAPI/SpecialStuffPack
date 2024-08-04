using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class AsteroidBelt : PassiveItem
    {
        public static void Init()
        {
            string name = "Asteroid Belt";
            string shortdesc = "Shoot Asteroids!";
            string longdesc = "Makes all the user's guns shoot asteroids.\n\nA belt made out of space metal, with a piece of an asteroid in the middle.";
            AsteroidBelt item = EasyItemInit<AsteroidBelt>("items/asteroidbelt", "sprites/asteroid_belt_idle_001", name, shortdesc, longdesc, ItemQuality.S, 662, null);
            Projectile proj = EasyProjectileInit<Projectile>("projectiles/asteroidprojectile", "projectilesprites/asteroid_projectile_001", 1.25f, 23, 10000, 40, true, false, false, null,
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            AsteroidProjectile asteroid = proj.AddComponent<AsteroidProjectile>();
            asteroid.Fire = GetItemById<BulletStatusEffectItem>(295).FireModifierEffect;
            asteroid.IgniteRadius = 3f;
            asteroid.InheritDamage = true;
            ExplosiveModifier modifier = proj.AddComponent<ExplosiveModifier>();
            modifier.explosionData = new ExplosionData
            {
                useDefaultExplosion = false,
                doDamage = true,
                forceUseThisRadius = false,
                damageRadius = 1f,
                damageToPlayer = 0f,
                damage = 5f,
                breakSecretWalls = false,
                secretWallsRadius = 0f,
                forcePreventSecretWallDamage = false,
                doDestroyProjectiles = false,
                doForce = true,
                pushRadius = 2f,
                force = 15f,
                debrisForce = 7.5f,
                preventPlayerForce = true,
                explosionDelay = 0.1f,
                usesComprehensiveDelay = false,
                comprehensiveDelay = 0f,
                effect = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Explosion_Newtiny"),
                doScreenShake = true,
                ss = new ScreenShakeSettings { },
                doStickyFriction = true,
                doExplosionRing = true,
                isFreezeExplosion = false,
                freezeRadius = 5f,
                freezeEffect = null,
                IsChandelierExplosion = false,
                ignoreList = new List<SpeculativeRigidbody>(),
                playDefaultSFX = true,
                overrideRangeIndicatorEffect = null,
                rotateEffectToNormal = false
            };
            modifier.IgnoreQueues = true;
            item.ReplacementProjectile = proj;
            item.PlanetaryTravelEffect = ElimentalerObject.GetProjectile().cheeseEffect;
            item.AddToBlacksmithShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnPreFireProjectileModifier += ReplaceProjectile;
            player.PostProcessProjectile += AsteroidSynergies;
        }

        public Projectile ReplaceProjectile(Gun g, Projectile p)
        {
            return ReplacementProjectile;
        }

        public void AsteroidSynergies(Projectile proj, float f)
        {
            if (Owner.PlayerHasActiveSynergy("SPIN"))
            {
                BounceProjModifier b = proj.GetOrAddComponent<BounceProjModifier>();
                b.numberOfBounces = Mathf.Max(b.numberOfBounces, 1);
                b.onlyBounceOffTiles = true;
                b.OnBounceContext += HandleStartOrbit;
            }
            if(Owner.PlayerHasActiveSynergy("Planetary Travel") && 0.25f.ScaledRandom(f))
            {
                proj.statusEffectsToApply.Add(PlanetaryTravelEffect);
            }
            if (Owner.PlayerHasActiveSynergy("Orbital Orbit") && 0.25f.ScaledRandom(f))
            {
                var childproj = OwnedShootProjectile(ReplacementProjectile, proj.transform.position, proj.Direction.ToAngle(), proj.Owner);
                childproj.OverrideMotionModule = new OrbitProjectileMotionModule()
                {
                    OrbitGroup = 0,
                    alternateOrbitTarget = proj.specRigidbody,
                    MaxRadius = 1f,
                    MinRadius = 1f,
                    lifespan = 15f,
                    usesAlternateOrbitTarget = true
                };
                childproj.baseData.damage *= 0.25f;
                childproj.AdditionalScaleMultiplier *= 0.5f;
            }
        }

        public void HandleStartOrbit(BounceProjModifier bouncer, SpeculativeRigidbody srb)
        {
            int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(-1);
            if (orbitersInGroup < 20)
            {
                bouncer.projectile.specRigidbody.CollideWithTileMap = false;
                bouncer.projectile.ResetDistance();
                bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 500f);
                if (bouncer.projectile.baseData.speed > 50f)
                {
                    bouncer.projectile.baseData.speed = 20f;
                    bouncer.projectile.UpdateSpeed();
                }
                var orbitProjectileMotionModule = new OrbitProjectileMotionModule()
                {
                    lifespan = 15f
                };
                if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
                {
                    orbitProjectileMotionModule.StackHelix = true;
                    orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
                }
                bouncer.projectile.OverrideMotionModule = orbitProjectileMotionModule;
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player == null)
            {
                return;
            }
            player.PostProcessProjectile -= AsteroidSynergies;
            player.OnPreFireProjectileModifier -= ReplaceProjectile;
        }

        public Projectile ReplacementProjectile;
        public GameActorCheeseEffect PlanetaryTravelEffect;
    }
}
