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
            HotCoal item = ItemBuilder.EasyInit<HotCoal>("items/hotcoal", "sprites/hot_coal_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.AppliesFire = true;
            item.FireModifierEffect = CodeShortcuts.GetItemById<BulletStatusEffectItem>(295).FireModifierEffect;
            item.chanceOfActivating = 0.75f;
            item.fireGoop = LoadHelper.LoadAssetFromAnywhere<GoopDefinition>("NapalmGoopQuickIgnite");
            item.TintBullets = true;
            item.TintBeams = true;
            item.TintPriority = 1;
            item.TintColor = CodeShortcuts.GetItemById<BulletStatusEffectItem>(295).TintColor;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += IgnitePlayer;
        }

        public void IgnitePlayer(PlayerController playerToIgnite)
        {
            playerToIgnite.IsOnFire = true;
        }

        public override void Update()
        {
            base.Update();
            if(PickedUp && Owner != null && Owner.IsOnFire && fireGoop != null)
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(fireGoop).AddGoopCircle(Owner.sprite.WorldBottomCenter, 1f, -1, false, -1);
            }
        }

        public override void OnDestroy()
        {
            if(Owner != null)
            {
                Owner.OnReceivedDamage -= IgnitePlayer;
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= IgnitePlayer;
            return base.Drop(player);
        }

        public GoopDefinition fireGoop;
    }
}
