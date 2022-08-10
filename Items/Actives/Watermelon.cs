using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class Watermelon : PlayerItem
    {
        public static void Init()
        {
            string name = "Watermelon";
            string shortdesc = "Heavy Hitter";
            string longdesc = "Throws watermelons at all enemies in the room.";
            Watermelon item = ItemBuilder.EasyInit<Watermelon>("items/watermelon", "sprites/watermelon_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
            LobbedProjectile proj = GunBuilder.EasyProjectileInit<LobbedProjectile>("projectiles/melonprojectile", "projectilesprites/melon_projectile_001", 67f, 1f, 999999f, 0f, true, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, 
                null, null, null, null);
            proj.initialSpeed = 23f;
            proj.speedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, -10f));
            proj.flySpeedMultiplier = 1f;
            proj.destinationOffset = new Vector2(0f, 0.6875f);
            proj.angularVelocity = 360f;
            proj.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            proj.shouldRotate = true;
            ExplosiveModifier modifier = proj.AddComponent<ExplosiveModifier>();
            modifier.doExplosion = true;
            modifier.explosionData = new ExplosionData
            {
                useDefaultExplosion = false,
                doDamage = true,
                forceUseThisRadius = false,
                damageRadius = 1f,
                damageToPlayer = 0f,
                damage = 33f,
                breakSecretWalls = false,
                secretWallsRadius = 0f,
                forcePreventSecretWallDamage = false,
                doDestroyProjectiles = false,
                doForce = false,
                pushRadius = 2f,
                force = 0f,
                debrisForce = 7.5f,
                preventPlayerForce = true,
                explosionDelay = 0.1f,
                usesComprehensiveDelay = false,
                comprehensiveDelay = 0f,
                effect = null,
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
            tk2dSprite.AddComponent(proj.transform.Find("Shadow").gameObject, ((GameObject)ResourceCache.Acquire("DefaultShadowSprite")).GetComponent<tk2dSprite>().Collection,
                ((GameObject)ResourceCache.Acquire("DefaultShadowSprite")).GetComponent<tk2dSprite>().spriteId);
            item.ProjectileToLob = proj;
            item.SetCooldownType(ItemBuilder.CooldownType.Damage, 250f);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            if(user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All) != null && user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All).Count > 0)
            {
                foreach(AIActor actor in user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All))
                {
                    if(actor != null)
                    {
                        Vector2 actorPosition = actor.CenterPosition;
                        if(actor.sprite != null)
                        {
                            actorPosition = actor.sprite.WorldBottomCenter;
                        }
                        else if(actor.specRigidbody != null)
                        {
                            actorPosition = actor.specRigidbody.UnitBottomCenter;
                        }
                        LobbedProjectile proj = (LobbedProjectile)VolleyUtility.ShootSingleProjectile(ProjectileToLob, user.CenterPosition, (actorPosition - user.CenterPosition).ToAngle(), false, user);
                        proj.SetDestination(actorPosition);
                    }
                }
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All) != null && 
                user.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All).Count > 0;
        }

        public LobbedProjectile ProjectileToLob;
    }
}
