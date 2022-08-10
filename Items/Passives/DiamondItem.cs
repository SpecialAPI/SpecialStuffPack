using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class DiamondItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Diamond";
            string shortdesc = "A Reward?";
            string longdesc = "Grants a little bit of everything.\n\nImproves the owner's ability to hold massive power.\n\nA powerful gemstone formed from the combination of tiny cosmic particles swarming around the Shrine and the " +
                "blood of a gungeoneer determined to kill the past. It pulsates with " +
                "incredible power.";
            DiamondItem item = ItemBuilder.EasyInit<DiamondItem>("items/diamond", "sprites/diamond_idle_001", name, shortdesc, longdesc , ItemQuality.SPECIAL, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.05f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, -0.05f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 0.05f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, -0.05f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.RangeMultiplier, 0.05f, StatModifier.ModifyMethod.ADDITIVE);
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            DiamondId = item.PickupObjectId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(DiamondItem));
        }

        public override void OnDestroy()
        {
            if (m_owner != null)
            {
                DecrementFlag(m_owner, typeof(DiamondItem));
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(DiamondItem));
            DebrisObject result = base.Drop(player);
            result.GetComponent<DiamondItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<DiamondItem>().Shatter;
            return result;
        }

        public void Shatter(DebrisObject debris)
        {
            GameObject soundSource = new GameObject("sound source");
            soundSource.transform.position = sprite.WorldCenter;
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", soundSource);
            Destroy(soundSource, 3f);
            Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX_Ghost", ".prefab"), debris.sprite.WorldCenter, Quaternion.identity);
            Destroy(debris.gameObject);
        }

        public static int DiamondId;
    }
}
