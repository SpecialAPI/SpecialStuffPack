using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Plushie : PassiveItem
    {
        public static void Init()
        {
            string name = "Lock Plushie";
            string shortdesc = "He believes in you";
            string longdesc = "A weird looking plushie that almost looks alive.";
            var item = EasyItemInit<Plushie>("items/plushie", "sprites/plushie_idle_001", name, shortdesc, longdesc, ItemQuality.S);
            item.AddToFlyntShop();
            item.SetupUnlockOnCustomFlag("LockUnlocked", true);
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                for(int i = 0; i < 3; i++) { LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(GlobalItemIds.Key).gameObject, player); }
            }
            base.Pickup(player);
            player.PostProcessProjectile += ShootKeyBullet;
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= ShootKeyBullet;
        }

        public void ShootKeyBullet(Projectile proj, float f)
        {
            if (Owner.carriedConsumables.KeyBullets > 0)
            {
                GameObject go = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(95) as Gun).DefaultModule.projectiles[0].gameObject, Owner.CenterPosition, Quaternion.Euler(0f, 0f, Owner.GetRelativeAim().ToAngle()), true);
                Projectile key = go.GetComponent<Projectile>();
                if (key != null)
                {
                    key.baseData.damage = 55f;
                    key.AdditionalScaleMultiplier = 3f;
                    PierceProjModifier pierce = key.gameObject.GetOrAddComponent<PierceProjModifier>();
                    pierce.preventPenetrationOfActors = false;
                    pierce.penetration = 5;
                    pierce.penetratesBreakables = true;
                    key.Owner = Owner;
                    key.Shooter = Owner.specRigidbody;
                    key.OnHitEnemy += (x, enemy, b) =>
                    { 
                        if (enemy != null && enemy.aiActor != null && enemy.aiActor.IsNormalEnemy && !enemy.aiActor.IsHarmlessEnemy && !enemy.aiActor.IgnoreForRoomClear)
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(GlobalItemIds.Key).gameObject, enemy.UnitCenter, Random.insideUnitCircle, 1f, false, false, false);
                        }
                    };
                    /*if (Owner.carriedConsumables.InfiniteKeys)
                    {
                        key.baseData.damage *= 1.25f;
                        key.ignoreDamageCaps = true;
                        pierce.preventPenetrationOfActors = false;
                        pierce.penetration = 50;
                        pierce.penetratesBreakables = true;
                    }*/
                    if(Owner.PlayerHasActiveSynergy("Wrath of the Keys"))
                    {
                        var homing = key.GetOrAddComponent<HomingModifier>();
                        homing.HomingRadius = Mathf.Max(homing.HomingRadius, 10f);
                        homing.AngularVelocity = Mathf.Max(homing.AngularVelocity, 420f);
                    }
                }
                if ((!Owner.PlayerHasActiveSynergy("Keybag") || Random.value < 0.25f) && (!Owner.PlayerHasActiveSynergy("Ring of Lock Friendship") || Owner.IsInCombat))
                {
                    Owner.carriedConsumables.KeyBullets--;
                }
            }
        }
    }
}
