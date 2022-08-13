using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class OpalItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Opal";
            string shortdesc = "A Reward?";
            string longdesc = "Pits no longer damage the owner.\n\nOnce used by the Old King as a second eye.";
            OpalItem item = EasyInitItem<OpalItem>("items/opal", "sprites/opal_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            OpalId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            GemDropper dropper = EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").AddComponent<GemDropper>();
            SpecialAssets.assets.Add(dropper.gameObject);
            dropper.GemId = OpalId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(OpalItem));
            player.ImmuneToPits.AddOverride("opal");
        }

        public override void DisableEffect(PlayerController player)
        {
            DecrementFlag(player, typeof(OpalItem));
            m_owner.ImmuneToPits.RemoveOverride("opal");
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            result.GetComponent<OpalItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<OpalItem>().Shatter;
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

        public static int OpalId;
    }
}
