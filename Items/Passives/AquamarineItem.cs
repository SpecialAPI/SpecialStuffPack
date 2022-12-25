using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecialStuffPack.ItemAPI;
using UnityEngine;
using SpecialStuffPack.Components;

namespace SpecialStuffPack.Items
{
    public class AquamarineItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Aquamarine";
            string shortdesc = "A Reward?";
            string longdesc = "Grants electric immunity.\n\nUsed by Agunim in one of his attempts to bring back his master.";
            AquamarineItem item = EasyItemInit<AquamarineItem>("items/aquamarine", "sprites/aquamarine_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            AquamarineId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            GemDropper dropper = EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").AddComponent<GemDropper>();
            SpecialAssets.assets.Add(dropper.gameObject);
            dropper.GemId = AquamarineId;
            dropper.IsRNGBossRoom = true;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            m_electricImmunity = new DamageTypeModifier() { damageMultiplier = 0f, damageType = CoreDamageTypes.Electric };
            player.healthHaver.damageTypeModifiers.Add(m_electricImmunity);
            IncrementFlag(player, typeof(AquamarineItem));
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            base.DisableEffect(player);
            player.healthHaver.damageTypeModifiers.Remove(m_electricImmunity);
            m_electricImmunity = null;
            DecrementFlag(player, typeof(AquamarineItem));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            result.GetComponent<AquamarineItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<AquamarineItem>().Shatter;
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

        private DamageTypeModifier m_electricImmunity;
        public static int AquamarineId;
    }
}
