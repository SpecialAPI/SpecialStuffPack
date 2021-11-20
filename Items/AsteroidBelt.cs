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
            AsteroidBelt item = ItemBuilder.EasyInit<AsteroidBelt>("items/asteroidbelt", "sprites/asteroid_belt_idle_001", name, shortdesc, longdesc, ItemQuality.S, SpecialStuffModule.globalPrefix, null, null);
            Projectile proj = GunBuilder.EasyProjectileInit<Projectile>("projectiles/asteroidprojectile", "projectilesprites/asteroid_projectile_001", 1.25f, 23, 10000, 40, true, false, false, null,
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            AsteroidProjectile asteroid = proj.AddComponent<AsteroidProjectile>();
            asteroid.Fire = CodeShortcuts.GetItemById<BulletStatusEffectItem>(295).FireModifierEffect;
            asteroid.IgniteRadius = 3f;
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
            item.ReplacementProjectile = proj;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnPreFireProjectileModifier += ReplaceProjectile;
        }

        public Projectile ReplaceProjectile(Gun g, Projectile p)
        {
            return ReplacementProjectile;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreFireProjectileModifier -= ReplaceProjectile;
            return base.Drop(player);
        }

        public Projectile ReplacementProjectile;
    }
}
