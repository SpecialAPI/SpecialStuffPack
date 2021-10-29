using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class SubscribeButton : PassiveItem
    {
        public static void Init()
        {
            string name = "Subscribe Button";
            string shortdesc = "Don't forget to LIKE & SUBSCRIBE!";
            string longdesc = "Power increases with the number of subscribers you have.\n\nBrought to a gungeon from another dimension by a famous GunTuber";
            SubscribeButton item = ItemBuilder.EasyInit<SubscribeButton>("items/subscribebutton", "sprites/subscribe_button_idle_001.png", name, shortdesc, longdesc, ItemQuality.S, SpecialStuffModule.globalPrefix, 524, null);
            item.DamageIncreasePerSubscriber = 0.05f;
            item.DamageIncreasePerSubscriberSynergy = 0.07f;
            ETGMod.Databases.Strings.Core.Set("#SUBUTTON_NUM_SUBSCRIBERS", "%SUBS subscribers");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += SubscriberPower;
            dfLabel label = GameUIRoot.Instance.p_playerCoinLabel.Parent.AddControl<dfLabel>();
            label.RelativePosition = new Vector3(88, 0);
            label.IsLocalized = false;
            label.Text = StringTableManager.GetString("#SUBUTTON_NUM_SUBSCRIBERS").Replace("%SUBS", GetNumSubscribers(player).ToString());
            label.Size = new Vector2(152152, 152152);
            label.TextScale = 3;
            m_label = label;
        }

        public void SubscriberPower(Projectile proj, float f)
        {
            proj.baseData.damage *= 1f + (GetNumSubscribers(m_owner) * (m_owner.PlayerHasActiveSynergy("Also click that bell") ? DamageIncreasePerSubscriberSynergy : DamageIncreasePerSubscriber) * 
                (m_owner.PlayerHasActiveSynergy("Also you can leave a comment") ? 3 : 1));
        }

        protected override void Update()
        {
            base.Update();
            if(m_pickedUp && m_owner != null && m_label != null)
            {
                m_label.RelativePosition = new Vector3(88 + (GameUIRoot.Instance.p_playerCoinLabel.Text.Length - 1) * 13, 0);
                m_label.Text = StringTableManager.GetString("#SUBUTTON_NUM_SUBSCRIBERS").Replace("%SUBS", GetNumSubscribers(m_owner).ToString());
            }
        }

        public int GetNumSubscribers(PlayerController player)
        {
            int subscribers = 0;
            //playercontroller actions
            subscribers += (player.GetEventDelegate("OnPitfall")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("PostProcessProjectile")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("PostProcessBeam")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("PostProcessBeamTick")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("PostProcessBeamChanceTick")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("PostProcessThrownGun")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnDealtDamage")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnDealtDamageContext")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnKilledEnemy")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnKilledEnemyContext")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnUsedPlayerItem")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnTriedToInitiateAttack")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnUsedBlank")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("GunChanged")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnDidUnstealthyAction")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnPreDodgeRoll")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnRollStarted")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnIsRolling")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnRolledIntoEnemy")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnDodgedProjectile")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnDodgedBeam")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnReceivedDamage")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnItemPurchased")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnItemStolen")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.GetEventDelegate("OnRoomClearEvent")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnIgnited?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnEnteredCombat?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnChestBroken?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnHitByProjectile?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnReloadPressed?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnReloadedGun?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnTableFlipped?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnTableFlipCompleted?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnNewFloorLoaded?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnAnyEnemyReceivedDamage?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.LostArmor?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnRealPlayerDeath?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnBlinkShadowCreated?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.OnPreFireProjectileModifier?.GetInvocationList().Length).GetValueOrDefault();
            //healthhaver actions
            subscribers += (player.healthHaver.GetEventDelegate("OnPreDeath")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.healthHaver.GetEventDelegate("OnDeath")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.healthHaver.GetEventDelegate("OnDamaged")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.healthHaver.GetEventDelegate("OnHealthChanged")?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.healthHaver.ModifyDamage?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.healthHaver.ModifyHealing?.GetInvocationList().Length).GetValueOrDefault();
            //rigidbody actions
            subscribers += (player.specRigidbody.OnPreMovement?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnPreRigidbodyCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnPreTileCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnRigidbodyCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnBeamCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnTileCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnEnterTrigger?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnTriggerCollision?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnExitTrigger?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnPathTargetReached?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.MovementRestrictor?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.OnHitByBeam?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.ReflectProjectilesNormalGenerator?.GetInvocationList().Length).GetValueOrDefault();
            subscribers += (player.specRigidbody.ReflectBeamsNormalGenerator?.GetInvocationList().Length).GetValueOrDefault();
            //playerstats actions
            subscribers += (player.stats.GetEventDelegate("AdditionalVolleyModifiers")?.GetInvocationList().Length).GetValueOrDefault();
            //final modifications
            subscribers -= 8;
            subscribers = Mathf.Max(subscribers, 0);
            return subscribers;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= SubscriberPower;
            GameUIRoot.Instance.p_playerCoinLabel.Parent.RemoveControl(m_label);
            Destroy(m_label.gameObject);
            return base.Drop(player);
        }

        public float DamageIncreasePerSubscriber;
        public float DamageIncreasePerSubscriberSynergy;
        private dfLabel m_label;
    }
}
