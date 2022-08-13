using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class HotCoal : BulletStatusEffectItem
    {
        public static void Init()
        {
            string name = "Hot Coal";
            string shortdesc = "Be Careful With It!";
            string longdesc = "Sometimes ignites the owner's shots, but also ignites the owner when getting hit.\n\nA piece of coal mined in the Black Powder Mines. Very hot to the touch!";
            HotCoal item = EasyInitItem<HotCoal>("items/hotcoal", "sprites/hot_coal_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.AppliesFire = true;
            item.FireModifierEffect = GetItemById<BulletStatusEffectItem>(295).FireModifierEffect;
            item.chanceOfActivating = 0.75f;
            item.fireGoop = LoadHelper.LoadAssetFromAnywhere<GoopDefinition>("NapalmGoopQuickIgnite");
            item.TintBullets = true;
            item.TintBeams = true;
            item.TintPriority = 1;
            item.TintColor = GetItemById<BulletStatusEffectItem>(295).TintColor;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += IgnitePlayer;
            player.PostProcessProjectile += AddGooper;
        }

        public void AddGooper(Projectile p, float f)
        {
            if(!Owner.PlayerHasActiveSynergy("Hotter Kiln"))
            {
                return;
            }
            var gooper = p.AddComponent<GoopModifier>();
            gooper.goopDefinition = fireGoop;
            gooper.SpawnGoopInFlight = true;
            gooper.InFlightSpawnRadius = 1f;
        }

        public void IgnitePlayer(PlayerController playerToIgnite)
        {
            playerToIgnite.IsOnFire = true;
        }

        public override void Update()
        {
            base.Update();
            if(PickedUp && Owner != null)
            {
                var hasSynergy = Owner.PlayerHasActiveSynergy("Hotter Kiln");
                if (hasSynergy)
                {
                    if(modifier == null)
                    {
                        Owner.healthHaver.damageTypeModifiers.Add(modifier = new() { damageMultiplier = 0f, damageType = CoreDamageTypes.Fire });
                    }
                }
                else if(Owner.IsOnFire && fireGoop != null)
                {
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fireGoop).AddGoopCircle(Owner.sprite.WorldBottomCenter, 1f, -1, false, -1);
                }
                if(!hasSynergy && modifier != null)
                {
                    Owner.healthHaver.damageTypeModifiers.Remove(modifier);
                    modifier = null;
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.OnReceivedDamage -= IgnitePlayer;
            player.PostProcessProjectile -= AddGooper;
            if(modifier != null)
            {
                player.healthHaver.damageTypeModifiers.Remove(modifier);
                modifier = null;
            }
        }

        public GoopDefinition fireGoop;
        public DamageTypeModifier modifier;
    }
}
