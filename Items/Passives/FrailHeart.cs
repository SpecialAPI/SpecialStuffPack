using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class FrailHeart : PassiveItem
    {
        public static void Init()
        {
            string name = "Frail Heart";
            string shortdesc = "Health up..?";
            string longdesc = "Seems to increase health by quite a bit. Doesn't seem to be in the best state.";
            FrailHeart item = ItemBuilder.EasyInit<FrailHeart>("items/frailheart", "sprites/frail_heart_idle_001", name, shortdesc, longdesc, ItemQuality.C, SpecialStuffModule.globalPrefix, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 3, StatModifier.ModifyMethod.ADDITIVE);
            item.BloodGoop = LoadHelper.LoadAssetFromAnywhere<GoopDefinition>("blobulongoop");
            item.BloodExplosionVFX = CodeShortcuts.GetItemById<TeleporterPrototypeItem>(449).TelefragVFXPrefab;
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                player.healthHaver.FullHeal();
            }
            base.Pickup(player);
            player.healthHaver.OnDamaged += MaybeReduceHealth;
        }

        public void MaybeReduceHealth(float result, float max, CoreDamageTypes damageType, DamageCategory category, Vector2 damageDirection)
        {
            if(Mathf.Ceil(result) < max && Owner != null)
            {
                if(Owner.PlayerHasActiveSynergy("Armored Support"))
                {
                    Owner.healthHaver.Armor += max - Mathf.Ceil(result);
                }
                Owner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, Mathf.Ceil(result) - max));
                Owner.stats.RecalculateStats(Owner, false, false);
                if(BloodGoop != null)
                {
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(BloodGoop).TimedAddGoopCircle(Owner.sprite.WorldBottomCenter, 3f, 0.5f, false);
                }
                if(BloodExplosionVFX != null)
                {
                    Instantiate(BloodExplosionVFX, Owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                }
                AkSoundEngine.PostEvent("Play_BOSS_blobulord_burst_01", Owner.gameObject);
            }
        }

        public override void OnDestroy()
        {
            if(Owner != null)
            {
                Owner.healthHaver.OnDamaged -= MaybeReduceHealth;
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= MaybeReduceHealth;
            return base.Drop(player);
        }

        public GoopDefinition BloodGoop;
        public GameObject BloodExplosionVFX;
    }
}
