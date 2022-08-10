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
            var item = ItemBuilder.EasyInit<Plushie>("items/plushie", "sprites/plushie_idle_001", name, shortdesc, longdesc, ItemQuality.S);
            item.AddToFlyntShop();
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.LOCK_UNLOCKED, true);
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

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= ShootKeyBullet;
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(Owner != null)
            {
                Owner.PostProcessProjectile -= ShootKeyBullet;
            }
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
                    key.OnHitEnemy += (x, enemy, b) => LootEngine.SpawnItem(PickupObjectDatabase.GetById(GlobalItemIds.Key).gameObject, enemy.UnitCenter, Random.insideUnitCircle, 1f, false, false, false);
                    /*if (Owner.carriedConsumables.InfiniteKeys)
                    {
                        key.baseData.damage *= 1.25f;
                        key.ignoreDamageCaps = true;
                        pierce.preventPenetrationOfActors = false;
                        pierce.penetration = 50;
                        pierce.penetratesBreakables = true;
                    }*/
                }
                Owner.carriedConsumables.KeyBullets--;
            }
        }
    }
}
