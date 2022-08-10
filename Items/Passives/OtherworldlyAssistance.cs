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
    public class OtherworldlyAssistance : UniqueOutlineItem
    {
        public static void Init()
        {
            string name = "Otherworldly Assistance";
            string shortdesc = "Help From... Somewhere";
            string longdesc = "Opens a door to another plane of existence, causing spirits to ocassinally appear. Confused, they will target random enemies.\n\nA shiny golden key. Looks very familiar.";
            OtherworldlyAssistance item = EasyInitItem<OtherworldlyAssistance>("items/keytobeyond", "sprites/key_to_beyond_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.GhostSummonInterval = 7f;
            item.GhostSummonIntervalVariance = 1f;
            tk2dSpriteDefinition def = item.sprite.GetCurrentSpriteDef();
            Material material = new Material(ShaderCache.Acquire("Brave/LitBlendUber"));
            material.SetTexture("_MainTex", def.material.mainTexture);
            def.material = material;
            def.materialInst = material;
            SpecialEnemies.InitFriendlyLockGhostPrefab();
            item.GhostPrefabToSummon = SpecialEnemies.FriendlyLockGhostPrefab;
            AdditionalBraveLight light = item.transform.Find("Light Source").AddComponent<AdditionalBraveLight>();
            light.transform.position = item.sprite.WorldCenter;
            light.LightColor = Color.red;
            light.LightIntensity = 4.25f;
            light.LightRadius = 5.4375f;
            item.whiteReplacement = Color.white;
        }

        protected override bool OverrideAddBlackOutline()
        {
            if(GetComponentInChildren<AdditionalBraveLight>() != null)
            {
                GetComponentInChildren<AdditionalBraveLight>().LightColor = Color.red;
                return true;
            }
            return false;
        }

        protected override bool OverrideAddWhiteOutline()
        {
            return false;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (GetComponentInChildren<AdditionalBraveLight>() != null)
            {
                GetComponentInChildren<AdditionalBraveLight>().LightIntensity = 0f;
                GetComponentInChildren<AdditionalBraveLight>().LightRadius = 0f;
            }
            AssignSummonTime();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            GetComponentInChildren<AdditionalBraveLight>().LightIntensity = 4.25f;
            GetComponentInChildren<AdditionalBraveLight>().LightRadius = 5.4375f;
            return base.Drop(player);
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
                if (Owner.IsInCombat && Owner.CurrentRoom != null && GetEnemiesWithRigidbodies().Count > 0)
                {
                    m_ghostSummonTimer += BraveTime.DeltaTime;
                    if(m_ghostSummonTimer >= m_ghostSummonTime)
                    {
                        AIActor enemy = BraveUtility.RandomElement(GetEnemiesWithRigidbodies());
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
                else
                {
                    m_ghostSummonTimer = Mathf.Max(m_ghostSummonTimer - BraveTime.DeltaTime, 0f);
                }
            }
        }

        private List<AIActor> GetEnemiesWithRigidbodies()
        {
            List<AIActor> result = new List<AIActor>();
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
