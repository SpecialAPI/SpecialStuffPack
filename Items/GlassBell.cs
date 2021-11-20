using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class GlassBell : PlayerItem
    {
        public static void Init()
        {
            string name = "Glass Bell";
            string shortdesc = "Ring It Carefully";
            string longdesc = "Can be used to summon glass guon stones. Will break when the owner is wounded, but will mend itself when going to the next floor.";
            GlassBell item = ItemBuilder.EasyInit<GlassBell>("items/glassbell", "sprites/glass_bell_idle_001", name, shortdesc, longdesc, ItemQuality.B, SpecialStuffModule.globalPrefix, null, null);
            item.SetCooldownType(ItemBuilder.CooldownType.PerRoom, 1f);
            item.MaxGuons = 6;
            item.MaxGuonsSynergy = 7;
            item.NormalSpriteId = item.sprite.spriteId;
            item.BrokenSpriteId = ItemBuilder.AddSpriteToCollection("sprites/glass_bell_idle_002", SpriteBuilder.itemCollection);
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(GlobalItemIds.GlassGuonStone).gameObject, user);
            if (GetNumberOfGuons(user) < MaxGuonsSynergy && user.PlayerHasActiveSynergy("Ring It Twice"))
            {
                LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(GlobalItemIds.GlassGuonStone).gameObject, user);
            }
            AkSoundEngine.PostEvent("Play_BOSS_mineflayer_jingle_01", user.gameObject);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += Unbreak;
            player.OnReceivedDamage += Break;
        }

        public void Break(PlayerController player)
        {
            if (!m_isBroken)
            {
                m_isBroken = true;
                sprite.SetSprite(BrokenSpriteId);
                AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", player.gameObject);
            }
        }

        public void Unbreak(PlayerController player)
        {
            if (m_isBroken)
            {
                m_isBroken = false;
                sprite.SetSprite(NormalSpriteId);
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            user.OnNewFloorLoaded -= Unbreak;
            user.OnReceivedDamage -= Break;
            base.OnPreDrop(user);
        }

        protected override void OnDestroy()
        {
            if (LastOwner != null)
            {
                LastOwner.OnNewFloorLoaded -= Unbreak;
                LastOwner.OnReceivedDamage -= Break;
            }
            base.OnDestroy();
        }

        public int GetNumberOfGuons(PlayerController user)
        {
            int val = 0;
            if(user != null && user.passiveItems != null)
            {
                foreach(PassiveItem item in user.passiveItems)
                {
                    if(item != null && item.PickupObjectId == GlobalItemIds.GlassGuonStone)
                    {
                        val++;
                    }
                }
            }
            return val;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && !m_isBroken && GetNumberOfGuons(user) < (user.PlayerHasActiveSynergy("Ring It Twice") ? MaxGuonsSynergy : MaxGuons);
        }

        public int MaxGuons;
        public int MaxGuonsSynergy;
        public int NormalSpriteId;
        public int BrokenSpriteId;
        [SerializeField]
        public bool m_isBroken;
    }
}
