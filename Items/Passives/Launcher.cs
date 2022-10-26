using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Launcher : PassiveItem
    {
        public static void Init()
        {
            string name = "Launcher";
            string shortdesc = "Launch your bullets!";
            string longdesc = "Bullets are launched into the air when fired, but increases accuracy and damage.";
            var launcher = EasyItemInit<Launcher>("items/launcher", "sprites/jumppad_idle_001", name, shortdesc, longdesc, ItemQuality.C, null, null);
            launcher.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 0.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            launcher.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
            launcher.AddToTrorkShop();
        }

        public override void Pickup(PlayerController player)
        {
            if (!BraveInput.GetInstanceForPlayer(player.PlayerIDX).IsKeyboardAndMouse(true) && !m_pickedUpThisRun)
            {
                var consoleController = PickupObjectDatabase.GetById(ItemIds["consolecontroller"]);
                GameUIRoot.Instance.notificationController.DoCustomNotification("CONTROLLER USER DETECTED", "giving compensation", consoleController.sprite.Collection, consoleController.sprite.spriteId,
                    UINotificationController.NotificationColor.SILVER, false, false);
                GameUIRoot.Instance.notificationController.DoCustomNotification("(this item doesnt work on controller)", "", consoleController.sprite.Collection, consoleController.sprite.spriteId,
                    UINotificationController.NotificationColor.SILVER, true, true);
                LootEngine.SpawnItem(consoleController.gameObject, player.CenterPosition, Vector2.down, 0f, false, true, false);
            }
            base.Pickup(player);
            player.PostProcessProjectile += PostProcessProjectile;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if (proj.GetType() == typeof(Projectile))
            {
                proj.baseData.range = 999999f;
                proj.baseData.damping = 0f;
                proj.baseData.force = 0f;
                if (proj.baseData.UsesCustomAccelerationCurve)
                {
                    proj.baseData.speed *= proj.baseData.AccelerationCurve.Evaluate(1f);
                    proj.baseData.AccelerationCurve = null;
                    proj.baseData.UsesCustomAccelerationCurve = false;
                }
                LobbedProjectileMotion motion = proj.GetOrAddComponent<LobbedProjectileMotion>();
                motion.initialSpeed = 23f;
                motion.speedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, -10f));
                motion.flySpeedMultiplier = 1f;
                motion.destinationOffset = new Vector2(0f, 0.6875f);
                motion.SetDestination(Owner.CenterPosition + Owner.GetRelativeAim());
            }
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.PostProcessProjectile -= PostProcessProjectile;
            base.DisableEffect(disablingPlayer);
        }
    }
}
