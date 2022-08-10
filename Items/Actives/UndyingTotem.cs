using SpecialStuffPack.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Items
{
    public class UndyingTotem : PlayerItem
    {
        public static void Init()
        {
            string name = "Totem of Gundying";
            string shortdesc = "Cheat Death";
            string longdesc = "Revives the owner, but only once.\n\nFound in a mansion in the woods.";
            UndyingTotem item = EasyInitItem<UndyingTotem>("items/totem", "sprites/totem_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
            item.InvulnerabilityDuration = 3f;
            item.HeartsToSpawn = 5f;
            item.ArmorToGive = 3;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.OnPreDeath += CheatDeath;
        }

        public void CheatDeath(Vector2 damageDirection)
        {
            if(PickedUp && LastOwner != null && LastOwner.CurrentItem == this)
            {
                if (LastOwner.ForceZeroHealthState)
                {
                    LastOwner.healthHaver.Armor += 1;
                }
                else
                {
                    LastOwner.healthHaver.ApplyHealing(0.5f);
                }
                LastOwner.ClearDeadFlags();
                AkSoundEngine.PostEvent("Play_OBJ_powerstar_use_01", LastOwner.gameObject);
                Material[] array = LastOwner.SetOverrideShader(ShaderCache.Acquire("Brave/Internal/RainbowChestShader"));
                for (int i = 0; i < array.Length; i++)
                {
                    if (!(array[i] == null))
                    {
                        array[i].SetFloat("_AllColorsToggle", 1f);
                    }
                }
                float heartsToSpawn = HeartsToSpawn;
                List<GameObject> hearts = new List<GameObject>();
                while(heartsToSpawn > 0)
                {
                    if(heartsToSpawn > 0.5f && BraveUtility.RandomBool())
                    {
                        hearts.Add(PickupObjectDatabase.GetById(GlobalItemIds.FullHeart).gameObject);
                        heartsToSpawn -= 1f;
                    }
                    else
                    {
                        hearts.Add(PickupObjectDatabase.GetById(GlobalItemIds.SmallHeart).gameObject);
                        heartsToSpawn -= 0.5f;
                    }
                }
                foreach(GameObject heart in hearts)
                {
                    LootEngine.SpawnItem(heart, LastOwner.CenterPosition, UnityEngine.Random.insideUnitCircle, UnityEngine.Random.Range(5.8f, 6.2f), true, false, false);
                }
                LastOwner.StartCoroutine(HandleDuration(LastOwner, InvulnerabilityDuration));
                LastOwner.healthHaver.Armor += ArmorToGive;
                string[] confetti = new string[]
                {
                    "Global VFX/Confetti_Blue_001",
                    "Global VFX/Confetti_Yellow_001",
                    "Global VFX/Confetti_Green_001"
                };
                for (int i = 0; i < 8; i++)
                {
                    GameObject original = (GameObject)BraveResources.Load(confetti[UnityEngine.Random.Range(0, 3)], ".prefab");
                    WaftingDebrisObject component = Instantiate(original).GetComponent<WaftingDebrisObject>();
                    component.sprite.PlaceAtPositionByAnchor(LastOwner.CenterPosition.ToVector3ZUp(0f) + new Vector3(0.5f, 0.5f, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
                    Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                    insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                    component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
                }
                LastOwner.RemoveActiveItem(PickupObjectId);
            }
        }

        private static IEnumerator HandleDuration(PlayerController user, float dura)
        {
            float ela = 0f;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                user.healthHaver.IsVulnerable = false;
                yield return null;
            }
            user.ClearOverrideShader();
            user.healthHaver.IsVulnerable = true;
            AkSoundEngine.PostEvent("Stop_SND_OBJ", user.gameObject);
            yield break;
        }

        public override void OnDestroy()
        {
            if(LastOwner != null)
            {
                LastOwner.healthHaver.OnPreDeath -= CheatDeath;
            }
            base.OnDestroy();
        }

        public override void OnPreDrop(PlayerController user)
        {
            user.healthHaver.OnPreDeath -= CheatDeath;
            base.OnPreDrop(user);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return false;
        }

        public float HeartsToSpawn;
        public float InvulnerabilityDuration;
        public int ArmorToGive;
    }
}
