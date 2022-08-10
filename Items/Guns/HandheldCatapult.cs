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
            Gun gun = GunBuilder.EasyGunInit("guns/catapult", name, shortdesc, longdesc, "catapult_idle_001", "gunsprites/ammonomicon/catapult_idle_001", "gunsprites/handheldcatapult", 100, 1f, new(0, 11), null, "Baseball",
                PickupObject.ItemQuality.C, GunClass.CHARGE, out var finish, null, null, null);
            LobbedProjectile proj = GunBuilder.EasyProjectileInit<LobbedProjectile>("projectiles/catapult_projectile", string.Empty, 50f, 1f, 999999f, 0f, true, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("rock_projectile_001"),
                tk2dBaseSprite.Anchor.LowerLeft, 0, 0, null, null, null, null);
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).ApplyOffsetsToAnimation(new List<IntVector2> { IntVector2.Zero, new IntVector2(4, 0), new IntVector2(4, 0), new IntVector2(3, 0) });
            proj.initialSpeed = 23f;
            proj.speedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, -10f));
            proj.flySpeedMultiplier = 1f;
            proj.destinationOffset = new Vector2(0f, 0.6875f);
            proj.angularVelocity = 360f;
            proj.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
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
            AddDualWieldSynergyProcessor(gun, CodeShortcuts.GetItemById<Gun>(382), "Stone x2");
        }

        public override void OnPlayerPickup(PlayerController playerOwner)
        {
            base.OnPlayerPickup(playerOwner);
            if (!BraveInput.GetInstanceForPlayer(playerOwner.PlayerIDX).IsKeyboardAndMouse(true) && !EverPickedUp)
            {
                var consoleController = PickupObjectDatabase.GetById(ItemBuilder.ItemIds["consolecontroller"]);
                GameUIRoot.Instance.notificationController.DoCustomNotification("CONTROLLER USER DETECTED", "giving compensation", consoleController.sprite.Collection, consoleController.sprite.spriteId, 
                    UINotificationController.NotificationColor.SILVER, false, false);
                GameUIRoot.Instance.notificationController.DoCustomNotification("(this item doesnt work on controller)", "", consoleController.sprite.Collection, consoleController.sprite.spriteId,
                    UINotificationController.NotificationColor.SILVER, true, true);
                LootEngine.SpawnItem(consoleController.gameObject, playerOwner.CenterPosition, Vector2.down, 0f, false, true, false);
            }
        }
    }
}
