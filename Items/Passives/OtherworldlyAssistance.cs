using SpecialStuffPack.Components;
using SpecialStuffPack.Enemies;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class OtherworldlyAssistance : PassiveItem
    {
        public static void Init()
        {
            string name = "Otherworldly Assistance";
            string shortdesc = "Help From... Somewhere";
            string longdesc = "Opens a door to another plane of existence, causing spirits to ocassinally appear. Confused, they will target random enemies.\n\nA shiny golden key. Looks very familiar.";
            OtherworldlyAssistance item = EasyItemInit<OtherworldlyAssistance>("items/keytobeyond", "sprites/key_to_beyond_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.GhostSummonInterval = 7f;
            item.GhostSummonIntervalVariance = 1f;
            SpecialEnemies.InitFriendlyLockGhostPrefab();
            item.GhostPrefabToSummon = SpecialEnemies.FriendlyLockGhostPrefab;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            AssignSummonTime();
        }

        public override void Update()
        {
            base.Update();
            if(PickedUp && Owner != null)
            {
                if(m_ghostSummonTime <= 0f)
                {
                    AssignSummonTime();
                }
                var rbEnemies = GetEnemiesWithRigidbodies();
                if (Owner.IsInCombat && Owner.CurrentRoom != null && rbEnemies.Count > 0)
                {
                    m_ghostSummonTimer += BraveTime.DeltaTime;
                    if(m_ghostSummonTimer >= m_ghostSummonTime)
                    {
                        var ghosts = Math.Max(Random.Range(0, 3), 1);
                        if(rbEnemies.Count > 1)
                        {
                            ghosts = Random.Range(2, 4);
                        }
                        if(Owner.PlayerHasActiveSynergy("Coming Soon (tm)"))
                        {
                            ghosts++;
                        }
                        for(int i = 0; i < ghosts; i++)
                        {
                            AIActor enemy = BraveUtility.RandomElement(rbEnemies);
                            float toLeft = Vector2.Distance(enemy.specRigidbody.UnitCenter, enemy.specRigidbody.UnitCenterLeft);
                            float toRight = Vector2.Distance(enemy.specRigidbody.UnitCenter, enemy.specRigidbody.UnitCenterRight);
                            float toTop = Vector2.Distance(enemy.specRigidbody.UnitCenter, enemy.specRigidbody.UnitTopCenter);
                            float toBottom = Vector2.Distance(enemy.specRigidbody.UnitCenter, enemy.specRigidbody.UnitBottomCenter);
                            float fromCenter = Mathf.Max(toLeft, toRight, toTop, toBottom);
                            FriendlyLockGhostController c = Instantiate(GhostPrefabToSummon, enemy.specRigidbody.UnitCenter + BraveMathCollege.DegreesToVector(UnityEngine.Random.Range(0f, 360f), fromCenter * 3f),
                                Quaternion.identity).GetComponent<FriendlyLockGhostController>();
                            c.target = enemy;
                            c.owner = Owner;
                            m_ghostSummonTimer = 0f;
                            AssignSummonTime();
                        }
                    }
                }
                else
                {
                    m_ghostSummonTimer = Mathf.Max(m_ghostSummonTimer - BraveTime.DeltaTime, 0f);
                }
            }
        }

        private List<AIActor> GetEnemiesWithRigidbodies()
        {
            List<AIActor> result = new();
            if(Owner != null && Owner.CurrentRoom != null && Owner.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All).Count > 0)
            {
                foreach(AIActor enemy in Owner.CurrentRoom.GetActiveEnemiesUnreferenced(Dungeonator.RoomHandler.ActiveEnemyType.All))
                {
                    if(enemy != null && enemy.specRigidbody != null && enemy.healthHaver != null)
                    {
                        result.Add(enemy);
                    }
                }
            }
            result.RemoveAll(x => x.GetComponentInChildren<DraGunController>() != null); //bye bye dragun
            return result;
        }

        private void AssignSummonTime()
        {
            m_ghostSummonTime = GhostSummonInterval + UnityEngine.Random.Range(-GhostSummonIntervalVariance, GhostSummonIntervalVariance);
        }

        public GameObject GhostPrefabToSummon;
        public float GhostSummonInterval;
        public float GhostSummonIntervalVariance;
        private float m_ghostSummonTime;
        private float m_ghostSummonTimer;
    }
}
