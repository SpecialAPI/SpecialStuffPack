using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialStuffPack.SaveAPI;

namespace SpecialStuffPack.Items
{
    public class ProtectiveArmorItem : PassiveItem
    {
        public static void Init()
        {
            string name = "Marine's Helmet";
            string shortdesc = "Protected Armor";
            string longdesc = "Grants armor on pickup. Grants a chance to negate damage while having armor.\n\nMarine's helmet, which serves him as armor.";
            ProtectiveArmorItem item = ItemBuilder.EasyInit<ProtectiveArmorItem>("items/marinehelmet", "sprites/helmet_idle_001", name, shortdesc, longdesc, ItemQuality.B, SpecialStuffModule.globalPrefix, 525, null);
            item.DamageNegateChance = 0.25f;
            item.ArmorlessDamageNegateChance = 0.05f;
            item.ArmorToGive = 1;
            item.AddToBlacksmithShop();
            item.AddToOldRedShop();
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.ITEMSPECIFIC_MARINES_HELMET, true);
        }

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                player.healthHaver.Armor += ArmorToGive;
            }
            base.Pickup(player);
            player.healthHaver.ModifyDamage += ModifyIncomingDamage;
        }

        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if ((source.Armor > 0 && UnityEngine.Random.value < DamageNegateChance) || (source.Armor <= 0 && UnityEngine.Random.value < ArmorlessDamageNegateChance))
            {
                args.ModifiedDamage = 0f;
                GameObject blankVFXPrefab = (GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab");
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
                GameObject gameObject = new GameObject("silencer");
                SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                float additionalTimeAtMaxRadius = 0.25f;
                silencerInstance.TriggerSilencer(m_owner.CenterPosition, 20f, 3f, blankVFXPrefab, 0f, 3f, 3f, 3f, 30f, 3f, additionalTimeAtMaxRadius, m_owner, false, false);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.ModifyDamage -= ModifyIncomingDamage;
            return base.Drop(player);
        }

        public float DamageNegateChance;
        public float ArmorlessDamageNegateChance;
        public int ArmorToGive;
    }
}
