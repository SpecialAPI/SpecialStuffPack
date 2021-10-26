using Dungeonator;
using SpecialStuffPack.ItemAPI;
using SpecialStuffPack.PlayMakerActions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using System.Text;
using SpecialStuffPack.SaveAPI;
using HutongGames.PlayMaker;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class RatWhistle : PlayerItem
    {
        public static void Init()
        {
            string name = "Resourceful Whistle";
            string shortdesc = "An unlikely truce?";
            string longdesc = "Summons the resourceful rat to steal guns and place bombs near enemies. Prevents the rat from stealing items from the owner.";
            RatWhistle item = ItemBuilder.EasyInit<RatWhistle>("items/ratwhistle", "sprites/resrat_whistle_idle_001", name, shortdesc, longdesc, ItemQuality.A, SpecialStuffModule.globalPrefix, null, null);
            item.SetCooldownType(ItemBuilder.CooldownType.Damage, 275f);
            item.IgnoredByRat = true;
            item.BombPrefab = CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(108).objectToSpawn;
            item.IceBombPrefab = CodeShortcuts.GetItemById<SpawnObjectPlayerItem>(109).objectToSpawn;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            RatBeatenAtPunchout = true;
        }

        protected override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_ENM_smiley_whistle_01", user.gameObject);
            GameManager.Instance.Dungeon.StartCoroutine(HandleRatSummon(user.CurrentRoom));
        }

        private IEnumerator HandleRatSummon(RoomHandler room)
        {
            List<AIActor> enemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            for(int a = 0; a < enemies.Count; a++)
            {
                if(a < enemies.Count)
                {
                    AIActor aiactor = enemies[a];
                    if (aiactor != null && aiactor.gameObject != null)
                    {
                        Coroutine cr = GameManager.Instance.Dungeon.StartCoroutine(HandleRatSteal(aiactor));
                        if (!LastOwner.PlayerHasActiveSynergy("All At Once"))
                        {
                            yield return cr;
                        }
                    }
                }
            }
            yield break;
        }

        private IEnumerator HandleRatSteal(AIActor aiactor)
        {
            if (aiactor.behaviorSpeculator != null)
            {
                aiactor.behaviorSpeculator.Stun(0.1f);
            }
            GameObject ratInstance = Instantiate(PrefabDatabase.Instance.ResourcefulRatThief, aiactor.sprite.WorldCenter + Vector2.right / -2f, Quaternion.identity);
            ratInstance.SetActive(false);
            FriendlyRatGrabby grabby = null;
            PlayMakerFSM fsm = ratInstance.GetComponent<PlayMakerFSM>();
            for (int i = 0; i < fsm.FsmStates.Length; i++)
            {
                for (int j = 0; j < fsm.FsmStates[i].Actions.Length; j++)
                {
                    if (fsm.FsmStates[i].Actions[j] is ThievingRatGrabby)
                    {
                        ThievingRatGrabby oldGrabby = fsm.FsmStates[i].Actions[j] as ThievingRatGrabby;
                        fsm.FsmStates[i].Actions[j] = new FriendlyRatGrabby
                        {
                            Name = oldGrabby.Name,
                            Enabled = oldGrabby.Enabled,
                            IsOpen = oldGrabby.IsOpen,
                            Active = oldGrabby.Active,
                            Finished = oldGrabby.Finished,
                            IsAutoNamed = oldGrabby.IsAutoNamed,
                            Owner = oldGrabby.Owner,
                            State = oldGrabby.State,
                            Fsm = oldGrabby.Fsm,
                            bombObject = LastOwner.PlayerHasActiveSynergy("Chill") ? IceBombPrefab : BombPrefab
                        };
                    }
                    else if (fsm.FsmStates[i].Actions[j] is BeginConversation || fsm.FsmStates[i].Actions[j] is EndConversation || fsm.FsmStates[i].Actions[j] is DialogueBox)
                    {
                        FsmStateAction[] actions = fsm.FsmStates[i].Actions;
                        SaveTools.Remove(ref actions, fsm.FsmStates[i].Actions[j]);
                        fsm.FsmStates[i].Actions = actions;
                        j--;
                    }
                }
            }
            for (int i = 0; i < fsm.FsmStates.Length; i++)
            {
                fsm.FsmStates[i].ActionData.SaveActions(fsm.FsmStates[i], fsm.FsmStates[i].Actions);
            }
            for (int i = 0; i < fsm.FsmStates.Length; i++)
            {
                for (int j = 0; j < fsm.FsmStates[i].Actions.Length; j++)
                {
                    if (fsm.FsmStates[i].Actions[j] is FriendlyRatGrabby)
                    {
                        grabby = fsm.FsmStates[i].Actions[j] as FriendlyRatGrabby;
                    }
                }
            }
            ratInstance.SetActive(true);
            GameObject ratInstance2 = Instantiate(ratInstance, aiactor.sprite.WorldCenter + Vector2.right / -2f + Vector2.down, Quaternion.identity);
            Destroy(ratInstance2.GetComponent<UltraFortunesFavor>());
            TalkDoerLite liteEdition = ratInstance2.GetComponent<TalkDoerLite>();
            liteEdition.playerApproachRadius = -1;
            liteEdition.conversationBreakRadius = float.MaxValue;
            ratInstance2.GetComponent<SpeculativeRigidbody>().OnPreRigidbodyCollision += NoCollisions; //because destroying it causes problems
            PlayMakerFSM fsm2 = ratInstance2.GetComponent<PlayMakerFSM>();
            Destroy(ratInstance);
            for (int i = 0; i < fsm2.FsmStates.Length; i++)
            {
                for (int j = 0; j < fsm2.FsmStates[i].Actions.Length; j++)
                {
                    if (fsm2.FsmStates[i].Actions[j] is FriendlyRatGrabby)
                    {
                        grabby = fsm2.FsmStates[i].Actions[j] as FriendlyRatGrabby;
                    }
                }
            }
            if (aiactor.CurrentGun != null && !aiactor.healthHaver.IsBoss)
            {
                if (grabby != null)
                {
                    grabby.TargetObject = aiactor.CurrentGun;
                    if (aiactor.behaviorSpeculator != null)
                    {
                        aiactor.behaviorSpeculator.AttackBehaviors = new List<AttackBehaviorBase>();
                        aiactor.behaviorSpeculator.MovementBehaviors = new List<MovementBehaviorBase>();
                        aiactor.behaviorSpeculator.OtherBehaviors = new List<BehaviorBase>();
                        aiactor.behaviorSpeculator.TargetBehaviors = new List<TargetBehaviorBase>();
                        aiactor.behaviorSpeculator.OverrideBehaviors = new List<OverrideBehaviorBase>();
                    }
                }
            }
            while (ratInstance2 != null)
            {
                if (aiactor != null && aiactor.gameObject != null && aiactor.behaviorSpeculator != null)
                {
                    aiactor.behaviorSpeculator.Stun(0.1f);
                }
                yield return null;
            }
        }

        public void NoCollisions(SpeculativeRigidbody rigid, PixelCollider pixel, SpeculativeRigidbody body, PixelCollider collider)
        {
            PhysicsEngine.SkipCollision = true;
        }

        public GameObject BombPrefab;
        public GameObject IceBombPrefab;
    }
}
