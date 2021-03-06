using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class ConsoleController : PlayerItem
    {
        public static void Init()
        {
            string name = "Console Controller";
            string shortdesc = "It Is I";
            string longdesc = "Can be thrown.\n\nHas a soul of someone named \"SpecialAPI\" inside of it.";
            ConsoleController item = ItemBuilder.EasyInit<ConsoleController>("items/consolecontroller", "sprites/console_controller_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, null, null);
            item.fire = CodeShortcuts.GetItemById<BulletStatusEffectItem>(295).FireModifierEffect;
            item.poison = CodeShortcuts.GetItemById<BulletStatusEffectItem>(204).HealthModifierEffect;
            item.freeze = CodeShortcuts.GetItemById<BulletStatusEffectItem>(278).FreezeModifierEffect;
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            bool synergyActive = user.PlayerHasActiveSynergy("I am YesEngine");
            Projectile proj = this.ThrowActive(user, false);
            if(proj != null && synergyActive)
            {
                PierceProjModifier pierce = proj.GetOrAddComponent<PierceProjModifier>();
                pierce.penetratesBreakables = true;
                pierce.penetration += 10;
                BounceProjModifier bounce = proj.GetOrAddComponent<BounceProjModifier>();
                bounce.numberOfBounces += 5;
                HomingModifier home = proj.GetOrAddComponent<HomingModifier>();
                home.HomingRadius += 10;
                home.AngularVelocity += 420;
                proj.OnBecameDebrisGrounded += HandleReturnLikeBoomerang;
                proj.baseData.damage *= 2f;
                proj.baseData.speed *= 1.5f;
                proj.AppliesFire = true;
                proj.FireApplyChance = 1f;
                proj.fireEffect = fire;
                proj.AppliesPoison = true;
                proj.PoisonApplyChance = 1f;
                proj.healthEffect = poison;
                proj.AppliesFreeze = true;
                proj.freezeEffect = freeze;
                proj.FreezeApplyChance = 1f;
                proj.AppliesStun = true;
                proj.StunApplyChance = 1f;
                proj.AppliedStunDuration = 5f;
            }
        }

        private void HandleReturnLikeBoomerang(DebrisObject obj)
        {
            obj.OnGrounded -= HandleReturnLikeBoomerang;
            PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
            if (pickupMover.specRigidbody)
            {
                pickupMover.specRigidbody.CollideWithTileMap = false;
            }
            pickupMover.minRadius = 1f;
            pickupMover.moveIfRoomUnclear = true;
            pickupMover.stopPathingOnContact = true;
        }

        public GameActorFireEffect fire;
        public GameActorFreezeEffect freeze;
        public GameActorHealthEffect poison;
    }
}
