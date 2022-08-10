using MonoMod.RuntimeDetour;
using SpecialStuffPack.ItemAPI;
using System.Reflection;
using System;
using Dungeonator;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class MirrorOfTruth : PassiveItem
    {
        public static void Init()
        {
            string name = "Mirror of Truth";
            string shortdesc = "Look at you.";
            string longdesc = "Replaces all chests with mirrors.\n\nA piece of a mirror from an another world. Seems like it came from a cursed place...";
            MirrorOfTruth item = ItemBuilder.EasyInit<MirrorOfTruth>("items/truthmirror", "sprites/mirror_of_truth_idle_001", name, shortdesc, longdesc, ItemQuality.C, 289, null);
            item.ShadowSynergyClonePrefab = CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(820).objectToSpawn;
            item.AddToCursulaShop();
            new Hook(typeof(RewardManager).GetMethod("GenerationSpawnRewardChestAt", BindingFlags.Public | BindingFlags.Instance), typeof(MirrorOfTruth).GetMethod("ReplaceChestWithMirror", BindingFlags.Public | BindingFlags.Static));
        }

        public static Chest ReplaceChestWithMirror(Func<RewardManager, IntVector2, RoomHandler, ItemQuality?, float, Chest> orig, RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, ItemQuality? targetQuality, float 
            overrideMimicChance)
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            bool isFromMirror = false;
            foreach(StackFrame frame in frames)
            {
                if(frame != null && frame.GetMethod() != null && frame.GetMethod().DeclaringType == typeof(MirrorController))
                {
                    isFromMirror = true;
                    break;
                }
            }
            if (IsFlagSetAtAll(typeof(MirrorOfTruth)) && !IsFlagSetAtAll(typeof(BadLuckClover)) && !isFromMirror)
            {
                GameObject g = LoadHelper.LoadAssetFromAnywhere<GameObject>("Shrine_Mirror").GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(targetRoom, positionInRoom + new IntVector2(0, 2), true);
                targetRoom.RegisterInteractable(g.GetInterfaceInChildren<IPlayerInteractable>());
                g.GetComponentInChildren<MirrorController>().CURSE_EXPOSED /= SynergyBuilder.AnyoneHasActiveSynergy("Blessed Mirror") ? 7 : 3.5f;
                return null;
            }
            else
            {
                return orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            IncrementFlag(player, typeof(MirrorOfTruth));
            player.OnEnteredCombat += HandleShadowSynergy;
        }

        public void HandleShadowSynergy()
        {
            if(m_owner != null && m_owner.CurrentRoom != null && m_owner.PlayerHasActiveSynergy("Shadow Mirror"))
            {
                GameObject go = Instantiate(ShadowSynergyClonePrefab, m_owner.transform.position, Quaternion.identity);
                KageBunshinController c = go.GetComponent<KageBunshinController>();
                if(c != null)
                {
                    c.InitializeOwner(m_owner);
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= HandleShadowSynergy;
            DecrementFlag(player, typeof(MirrorOfTruth));
            return base.Drop(player);
        }

        public GameObject ShadowSynergyClonePrefab;
    }
}
