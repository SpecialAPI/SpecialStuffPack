using MonoMod.RuntimeDetour;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Reflection;

namespace SpecialStuffPack.Items
{
    public class EmeraldItem : PassiveItem
    {
        public static void Init()
        {
            string name = "The Emerald";
            string shortdesc = "A Reward?";
            string longdesc = "Grants poison immunity.\n\nStolen by the Resourceful Rat from an indestructable chest.";
            EmeraldItem item = EasyItemInit<EmeraldItem>("items/emerald", "sprites/emerald_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            EmeraldId = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            new Hook(typeof(PunchoutAIActor).GetMethod("Hit", BindingFlags.Instance | BindingFlags.Public), typeof(EmeraldItem).GetMethod("HandleEmeraldDrop", BindingFlags.Public | BindingFlags.Static));
        }

        public static void HandleEmeraldDrop(Action<PunchoutAIActor, bool, float, int, bool> orig, PunchoutAIActor self, bool isLeft, float damage, int starsUsed, bool skipProcessing)
        {
            bool droppedFirstKeyOld = (bool)typeof(PunchoutAIActor).GetField("m_droppedFirstKey", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
            orig(self, isLeft, damage, starsUsed, skipProcessing);
            bool droppedFirstKey = (bool)typeof(PunchoutAIActor).GetField("m_droppedFirstKey", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
            if(!droppedFirstKeyOld && droppedFirstKey)
            {
                self.DropReward(isLeft, EmeraldId);
                foreach(PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if (p.Ext().HasBeenKeyRobbed)
                    {
                        self.DropReward(isLeft, GoldKey.GoldKeyId);
                        p.Ext().HasBeenKeyRobbed = false;
                    }
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            m_poisonImmunity = new DamageTypeModifier() { damageMultiplier = 0.5f, damageType = CoreDamageTypes.Poison };
            player.healthHaver.damageTypeModifiers.Add(m_poisonImmunity);
            IncrementFlag(player, typeof(EmeraldItem));
        }

        public override void DisableEffect(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(m_poisonImmunity);
            m_poisonImmunity = null;
            DecrementFlag(player, typeof(EmeraldItem));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            result.GetComponent<EmeraldItem>().m_pickedUp = true;
            result.OnGrounded += result.GetComponent<EmeraldItem>().Shatter;
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

        private DamageTypeModifier m_poisonImmunity;
        public static int EmeraldId;
    }
}
