using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecialStuffPack.Components;
using SpecialStuffPack.ItemAPI;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class AmethystItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Amethyst";
            string shortdesc = "A Reward?";
            string longdesc = "Greatly improves damage of charmed foes.\n\nAbsorbed by the Blobulord to gain power.";
            AmethystItem item = ItemBuilder.EasyInit<AmethystItem>("items/amethyst", "sprites/amethyst_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            AmethystId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            GemDropper dropper = EnemyDatabase.GetOrLoadByGuid("d1c9781fdac54d9e8498ed89210a0238").AddComponent<GemDropper>();
            SpecialAssets.assets.Add(dropper.gameObject);
            dropper.UseDeathPosition = true;
            dropper.GemId = AmethystId;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(AmethystItem));
            ProjectileData.FixedFallbackDamageToEnemies *= 2;
        }

        public override void OnDestroy()
        {
            if (m_owner != null)
            {
                DecrementFlag(m_owner, typeof(AmethystItem));
                ProjectileData.FixedFallbackDamageToEnemies /= 2;
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(AmethystItem));
            ProjectileData.FixedFallbackDamageToEnemies /= 2;
            DebrisObject result = base.Drop(player);
            result.GetComponent<AmethystItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<AmethystItem>().Shatter;
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

        public static int AmethystId;
    }
}
