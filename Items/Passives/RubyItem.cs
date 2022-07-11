using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class RubyItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Ruby";
            string shortdesc = "A Reward?";
            string longdesc = "Grants fire immunity.\n\nThe blood of the High Dragun, crystallized after his defeat.";
            RubyItem item = ItemBuilder.EasyInit<RubyItem>("items/ruby", "sprites/ruby_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, SpecialStuffModule.globalPrefix, null, null);
            RubyId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").AddComponent<GemDropper>().GemId = RubyId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            m_fireImmunity = new DamageTypeModifier() { damageMultiplier = 0f, damageType = CoreDamageTypes.Fire };
            player.healthHaver.damageTypeModifiers.Add(m_fireImmunity);
            IncrementFlag(player, typeof(RubyItem));
        }

        public override void OnDestroy()
        {
            if (m_owner != null)
            {
                m_owner.healthHaver.damageTypeModifiers.Remove(m_fireImmunity);
                m_fireImmunity = null;
                DecrementFlag(m_owner, typeof(RubyItem));
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(m_fireImmunity);
            m_fireImmunity = null;
            DecrementFlag(player, typeof(RubyItem));
            DebrisObject result = base.Drop(player);
            result.GetComponent<RubyItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<RubyItem>().Shatter;
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

        private DamageTypeModifier m_fireImmunity;
        public static int RubyId;
    }
}
