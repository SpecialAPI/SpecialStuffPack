using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class HandheldCatapult : GunBehaviour
    {
        public static void Init()
        {
            string name = "Handheld Catapult";
            string shortdesc = "Miniature";
            string longdesc = "Lobs rocks.\n\nA miniature replica of a catapult. The only practical thing about it is portability.";
            Gun gun = EasyGunInit("guns/catapult", name, shortdesc, longdesc, "catapult_idle_001", "gunsprites/ammonomicon/catapult_idle_001", "gunsprites/handheldcatapult", 100, 1f, new(0, 11), null, "Baseball",
                PickupObject.ItemQuality.C, GunClass.CHARGE, out var finish, null, null, null);
            LobbedProjectile proj = EasyProjectileInit<LobbedProjectile>("projectiles/catapult_projectile", string.Empty, 50f, 23f, 999999f, 0f, true, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("rock_projectile_001"),
                tk2dBaseSprite.Anchor.LowerLeft, 0, 0, null, null, null, null);
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).ApplyOffsetsToAnimation(new List<IntVector2> { IntVector2.Zero, new IntVector2(4, 0), new IntVector2(4, 0), new IntVector2(3, 0) });
            proj.angularVelocity = 360f;
            proj.hitEffects = SlingObject.GetProjectile().hitEffects;
            proj.shouldRotate = true;
            tk2dSprite.AddComponent(proj.transform.Find("Shadow").gameObject, ((GameObject)ResourceCache.Acquire("DefaultShadowSprite")).GetComponent<tk2dSprite>().Collection, 
                ((GameObject)ResourceCache.Acquire("DefaultShadowSprite")).GetComponent<tk2dSprite>().spriteId);
            ProjectileModule module = new AdvancedProjectileModule
            {
                shootStyle = ProjectileModule.ShootStyle.Charged,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "rock",
                projectiles = new List<Projectile> { },
                chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
                {
                    new ProjectileModule.ChargeProjectile
                    {
                        ChargeTime = 1f,
                        Projectile = proj
                    }
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = 0.11f,
                numberOfShotsInClip = 1,
                isChargedBurst = true
            };
            gun.RawSourceVolley.projectiles.Add(module);
            gun.LockedHorizontalOnCharge = true;
            gun.AddComponent<LobberGun>();
            gun.AddComponent<HandheldCatapult>();
            finish();
        }
    }
}
