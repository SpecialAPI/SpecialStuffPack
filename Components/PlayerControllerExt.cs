using SpecialStuffPack.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    [HarmonyPatch]
    public class PlayerControllerExt : MonoBehaviour
    {
        [HarmonyPatch(typeof(PlayerItem), nameof(PlayerItem.DidDamage))]
        [HarmonyPrefix]
        public static void FervoursHarvest(PlayerController Owner, ref float damageDone)
        {
            damageDone *= Owner.Ext().activeChargeMultiplier;
        }

        [HarmonyPatch(typeof(PlayerItem), nameof(PlayerItem.ApplyCooldown))]
        [HarmonyPostfix]
        public static void DivineCurse(PlayerItem __instance, PlayerController user)
        {
            __instance.remainingDamageCooldown *= user.Ext().cooldownMultiplier;
            __instance.remainingTimeCooldown *= user.Ext().cooldownMultiplier;
            if(__instance.remainingRoomCooldown > 0)
            {
                __instance.remainingRoomCooldown = Mathf.Max(1, Mathf.RoundToInt(__instance.remainingRoomCooldown * user.Ext().cooldownMultiplier));
            }
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.DoPostProcessProjectile))]
        [HarmonyPrefix]
        public static void SetJank(PlayerController __instance)
        {
            JankJankJankJankJank = __instance.Ext().bulletChanceEffectScaleMultiplier;
        }

        [HarmonyPatch(typeof(ProjectileModule), nameof(ProjectileModule.GetEstimatedShotsPerSecond))]
        [HarmonyPostfix]
        public static void ApplyEffectMultiplier(ref float __result)
        {
            __result /= JankJankJankJankJank;
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.DoPostProcessProjectile))]
        [HarmonyPostfix]
        public static void ResetJank()
        {
            JankJankJankJankJank = 1f;
        }

        [HarmonyPatch(typeof(SimpleFlagDisabler), nameof(SimpleFlagDisabler.Update))]
        [HarmonyPostfix]
        public static void EnableOnCCCRun(SimpleFlagDisabler __instance)
        {
            if (__instance.EnableOnGunGameMode && !GameManager.Instance.IsSelectingCharacter && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer.HasExt() && GameManager.Instance.PrimaryPlayer.Ext().isCCCRun)
            {
                SpeculativeRigidbody component = __instance.GetComponent<SpeculativeRigidbody>();
                if (!component.enabled)
                {
                    component.enabled = true;
                    component.Reinitialize();
                    __instance.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        [HarmonyPatch(typeof(Chest), nameof(Chest.Open))]
        [HarmonyPrefix]
        public static bool BreakChestAndYieldAbstraction(Chest __instance, PlayerController player)
        {
            if (player.Ext().isCCCRun)
            {
                if (__instance.majorBreakable != null)
                {
                    __instance.pickedUp = true;
                    __instance.majorBreakable.Break(Vector2.zero);
                    __instance.m_room.DeregisterInteractable(__instance);
                    if (__instance.m_registeredIconRoom != null)
                    {
                        Minimap.Instance.DeregisterRoomIcon(__instance.m_registeredIconRoom, __instance.minimapIconInstance);
                    }
                    __instance.contents = new() { player.Ext().GetCCCAbstraction() };
                    __instance.StartCoroutine(__instance.PresentItem());
                    Instantiate(VFXDatabase.MiniBlank, __instance.sprite.WorldCenter, Quaternion.identity);
                    Exploder.DoDistortionWave(__instance.sprite.WorldCenter, 2f, 0.2f, 4f, 0.2f);
                    var ss = new GameObject("sound");
                    ss.transform.position = __instance.transform.position;
                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", ss);
                }
                else
                {
                    Exploder.DoDefaultExplosion(__instance.sprite.WorldCenter, Vector2.zero, null, true, CoreDamageTypes.None, false);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Chest), nameof(Chest.OnBroken))]
        [HarmonyPrefix]
        public static bool GiveNothing(Chest __instance)
        {
            if (GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer.Ext().isCCCRun)
            {
                __instance.pickedUp = true;
                __instance.m_room.DeregisterInteractable(__instance);
                if (__instance.m_registeredIconRoom != null)
                {
                    Minimap.Instance.DeregisterRoomIcon(__instance.m_registeredIconRoom, __instance.minimapIconInstance);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(RewardPedestal), nameof(RewardPedestal.DetermineContents))]
        [HarmonyPrefix]
        public static bool DoNotGiveBossRewardInCCCMode(RewardPedestal __instance, PlayerController player)
        {
            if(__instance.contents == null && __instance.IsBossRewardPedestal && player != null && player.Ext().isCCCRun)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(RewardPedestal), nameof(RewardPedestal.RegisterChestOnMinimap))]
        [HarmonyPrefix]
        public static bool DoNotGiveBossRewardInCCCMode(RewardPedestal __instance)
        {
            if (__instance.contents == null && __instance.IsBossRewardPedestal && GameManager.HasInstance && GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer.Ext().isCCCRun)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SpawnPickup), nameof(SpawnPickup.OnEnter))]
        [HarmonyPrefix]
        public static bool GiveAbstractionInstead(SpawnPickup __instance)
        {
            TalkDoerLite component = __instance.Owner.GetComponent<TalkDoerLite>();
            PlayerController playerController = (!component.TalkingPlayer) ? GameManager.Instance.PrimaryPlayer : component.TalkingPlayer;
            if (playerController != null && playerController.Ext().isCCCRun)
            {
                GameObject item = playerController.Ext().GetCCCAbstraction().gameObject;
                if (__instance.spawnLocation == SpawnPickup.SpawnLocation.GiveToPlayer)
                {
                    LootEngine.TryGivePrefabToPlayer(item, playerController, false);
                }
                else if (__instance.spawnLocation == SpawnPickup.SpawnLocation.GiveToBothPlayers)
                {
                    LootEngine.TryGivePrefabToPlayer(item, GameManager.Instance.PrimaryPlayer, false);
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                    {
                        LootEngine.TryGivePrefabToPlayer(item, GameManager.Instance.SecondaryPlayer, false);
                    }
                }
                else
                {
                    Vector2 vector;
                    if (__instance.spawnLocation == SpawnPickup.SpawnLocation.AtPlayer || __instance.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromPlayer)
                    {
                        vector = playerController.specRigidbody.UnitCenter;
                    }
                    else if (__instance.spawnLocation == SpawnPickup.SpawnLocation.AtTalkDoer || __instance.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromTalkDoer)
                    {
                        vector = ((!(component.specRigidbody != null)) ? component.sprite.WorldCenter : component.specRigidbody.UnitCenter);
                    }
                    else if (__instance.spawnLocation == SpawnPickup.SpawnLocation.RoomSpawnPoint)
                    {
                        vector = playerController.CurrentRoom.GetBestRewardLocation(IntVector2.One, RoomHandler.RewardLocationStyle.Original, false).ToVector2();
                    }
                    else
                    {
                        Debug.LogError("Tried to give an item to the player but no valid spawn location was selected.");
                        vector = GameManager.Instance.PrimaryPlayer.CenterPosition;
                    }
                    if (__instance.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromPlayer || __instance.spawnLocation == SpawnPickup.SpawnLocation.OffsetFromTalkDoer)
                    {
                        vector += __instance.spawnOffset;
                    }
                    LootEngine.SpawnItem(item, vector, Vector2.zero, 0f, true, false, false);
                    LootEngine.DoDefaultItemPoof(vector, false, false);
                }
                __instance.Finish();
                return false;
            }
            return true;
        }

        public static float JankJankJankJankJank = 1f;

        public void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        public void Start()
        {
            if(player != null)
            {
                player.PostProcessProjectile += HandleDefaultEffects;
                player.OnNewFloorLoaded += FloorLoadEffects;
                floorDamageModifier = player.AddOwnerlessModifier(PlayerStats.StatType.Damage, 0f, StatModifier.ModifyMethod.ADDITIVE);
                player.specRigidbody.OnPreRigidbodyCollision += OnPreCollision;
                player.healthHaver.OnPreDeath += HandleRevives;
                player.healthHaver.OnDamaged += OnDamaged;
                player.healthHaver.ModifyHealing += ModifyHealing;
                player.healthHaver.ModifyDamage += ModifyDamage;
                player.OnRollStarted += RollStarted;
                player.OnAnyEnemyReceivedDamage += EnemyReceivedDamage;
                player.LostArmor += LostArmor;
                player.OnEnteredCombat += EnteredCombat;
                player.OnUsedPlayerItem += DamageOnActive;
            }
        }

        public void DamageOnActive(PlayerController player, PlayerItem item)
        {
            if (((roomDamageOnActiveDamageCharge > 0f && item.damageCooldown > 0f) || (roomDamageOnActiveRoomCharge > 0f && item.roomCooldown > 0)) && player.CurrentRoom != null && player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
            {
                var unreferenced = player.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                foreach (var enemy in unreferenced)
                {
                    if (enemy == null || enemy.healthHaver == null)
                    {
                        continue;
                    }
                    enemy.healthHaver.ApplyDamage(roomDamageOnActiveDamageCharge * item.damageCooldown + roomDamageOnActiveRoomCharge * item.roomCooldown, Vector2.zero, "An Active Item", CoreDamageTypes.None, 
                        DamageCategory.Unstoppable, true, null, true);
                }
            }
        }

        public void Update()
        {
            if (activeChargePerSecond > 0f && player.IsInCombat && player.activeItems != null)
            {
                foreach (var item in player.activeItems)
                {
                    if (item != null)
                    {
                        item.DidDamage(player, activeChargePerSecond * BraveTime.DeltaTime);
                    }
                }
            }
            if (isCCCRun && wasLastInFoyer)
            {
                if (!GameManager.Instance.IsFoyer)
                {
                    wasLastInFoyer = false;
                    player.CharacterUsesRandomGuns = false;
                    GameStatsManager.Instance.rainbowRunToggled = false;
                    player.RemoveAllPassiveItems();
                    player.RemoveAllActiveItems();
                    for (int i = 0; i < player.inventory.m_guns.Count; i++)
                    {
                        Gun gun = player.inventory.m_guns[i];
                        if(gun.PickupObjectId != RustySidearmId && gun.PickupObjectId != MachinePistolId)
                        {
                            player.inventory.RemoveGunFromInventory(gun);
                            Destroy(gun.gameObject);
                            i--;
                        }
                    }
                    player.inventory.GunLocked.ClearOverrides();
                }
            }
        }

        public void EnteredCombat()
        {
            if(player.activeItems != null)
            {
                foreach(var item in player.activeItems)
                {
                    if(item != null)
                    {
                        item.DidDamage(player, activeChargeOnRoomEnter);
                    }
                }
            }
        }

        public void EnemyReceivedDamage(float dmg, bool fatal, HealthHaver hh)
        {
            if(tarotCards.Contains(TarotCards.TarotCardType.BurningDead) && fatal && hh.specRigidbody != null)
            {
                Exploder.Explode(hh.specRigidbody.UnitCenter, bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.NaturesBoon) && fatal && hh.specRigidbody != null)
            {
                OwnedShootProjectile(BarrelObject.GetProjectile(), hh.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), player).specRigidbody.RegisterTemporaryCollisionException(hh.specRigidbody, 
                    1f, null);
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.NeptunesCurse) && fatal && hh.specRigidbody != null)
            {
                for(int i = 0; i < 2; i++)
                {
                    OwnedShootProjectile(MahogunyObject.Volley.projectiles[1].projectiles[0], hh.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), player).specRigidbody.RegisterTemporaryCollisionException(hh.specRigidbody,
                        1f, null);
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.SoulSnatcher))
            {
                soulSnatcherDamageLeft -= dmg;
                if(soulSnatcherDamageLeft <= 0)
                {
                    soulSnatcherDamageLeft = 1500;
                    player.healthHaver.ApplyHealing(0.5f);
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.GiftFromBelow))
            {
                giftFromBelowDamageLeft -= dmg;
                if (giftFromBelowDamageLeft <= 0)
                {
                    giftFromBelowDamageLeft = 2000;
                    player.healthHaver.Armor += 1;
                }
            }
        }

        public void RollStarted(PlayerController play, Vector2 dir)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.Bomb))
            {
                Exploder.Explode(play.CenterPosition, bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.IchorLingered))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.LambIchor).TimedAddGoopCircle(play.specRigidbody.UnitBottomCenter, 3.5f, 0.5f, false);
            }
        }

        public void ModifyHealing(HealthHaver hh, HealthHaver.ModifyHealingEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.FortunesBlessing))
            {
                args.ModifiedHealing *= 2f;
            }
        }

        public void ModifyDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.ShieldOfFaith) && 0.1f.RandomChance())
            {
                args.ModifiedDamage = 0f;
            }
        }

        public void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if(tarotCards.Contains(TarotCards.TarotCardType.DeathsDoor) && player.CurrentRoom != null && (resultValue == 0.5f || (player.ForceZeroHealthState && player.healthHaver.Armor == 1f)) && 
                player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
            {
                var unreferenced = player.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                foreach(var enemy in unreferenced)
                {
                    if(enemy == null || enemy.healthHaver == null)
                    {
                        continue;
                    }
                    enemy.healthHaver.ApplyDamage(100f, Vector2.zero, "died because of death", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }
            if (player.activeItems != null)
            {
                foreach (var item in player.activeItems)
                {
                    if (item != null)
                    {
                        item.DidDamage(player, activeChargeOnDamage);
                    }
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.IchorEarned))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.LambIchor).TimedAddGoopCircle(player.specRigidbody.UnitBottomCenter, 6f, 1f, false);
            }
        }

        public void LostArmor()
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.DiseasedHeart))
            {
                var unreferenced = player.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                foreach (var enemy in unreferenced)
                {
                    if (enemy == null || enemy.healthHaver == null)
                    {
                        continue;
                    }
                    enemy.healthHaver.ApplyDamage(50f, Vector2.zero, "died because of death", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }
        }

        public void HandleRevives(Vector2 d)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.TheDeal))
            {
                tarotCards.Remove(TarotCards.TarotCardType.TheDeal);
                player.healthHaver.ApplyHealing(1f);
                if (player.ForceZeroHealthState)
                {
                    player.healthHaver.Armor += 2f;
                }
                player.ClearDeadFlags();
            }
        }

        public void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherColliders)
        {
            if(tarotCards.Contains(TarotCards.TarotCardType.BlazingTrail) && player.IsDodgeRolling && otherRigidbody != null && otherRigidbody.aiActor != null && otherRigidbody.healthHaver != null)
            {
                otherRigidbody.healthHaver.ApplyDamage(100f * BraveTime.DeltaTime, Vector2.zero, "incinerated", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                PhysicsEngine.SkipCollision = true;
                var sprite = otherRigidbody.sprite;
                Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0f);
                Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0f);
                float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                float num2 = 25f * num;
                int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                int num4 = num3;
                Vector3 minPosition = vector;
                Vector3 maxPosition = vector2;
                Vector3 direction = Vector3.up / 2f;
                float angleVariance = 120f;
                float magnitudeVariance = 0.2f;
                float? startLifetime = UnityEngine.Random.Range(0.8f, 1.25f);
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            }
        }

        public void RecalculateFloorBonus()
        {
            if (GameManager.Instance.nextLevelIndex % 2 == 0)
            {
                floorDamageModifier.amount = oddFloorDamageBonus;
            }
            else
            {
                floorDamageModifier.amount = evenFloorDamageBonus;
            }
            player.RecalculateStats();
        }

        public void FloorLoadEffects(PlayerController play)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.Telescope))
            {
                Minimap.Instance.RevealAllRooms(false);
            }
            RecalculateFloorBonus();
        }

        public void HandleDefaultEffects(Projectile proj, float scale)
        {
            if (poisonProjectileChance.ScaledRandom(scale))
            {
                proj.statusEffectsToApply.Add(defaultPoison);
                proj.AdjustPlayerProjectileTint(Color.green, 0, 0f);
            }
            if (lambCritChance.RandomChance())
            {
                proj.AdjustPlayerProjectileTint(Color.red, 0, 0f);
                proj.baseData.damage *= 2f;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.HandsOfRage) && Time.time - lastHandsOfRageTime >= 10f)
            {
                var rage = OwnedShootProjectile(ThePredatorObject.GetProjectile(), player.CurrentGun != null ? player.CurrentGun.barrelOffset.position.XY() : player.CenterPosition, player.GetAimDirection(), player);
                rage.AdjustPlayerProjectileTint(Color.red, 0, 0f);
                rage.baseData.damage *= 2f;
                DestroyImmediate(rage.GetComponent<LockOnHomingModifier>());
                var homing = rage.GetOrAddComponent<HomingModifier>();
                homing.HomingRadius = 10f;
                homing.AngularVelocity = 420f;
                lastHandsOfRageTime = Time.time;
            }
        }

        public void OnDestroy()
        {
            if(player != null)
            {
                player.PostProcessProjectile -= HandleDefaultEffects;
                player.OnNewFloorLoaded -= FloorLoadEffects;
            }
        }

        public static bool AnyoneHasBeenKeyRobbed()
        {
            return AnyoneHasBeenKeyRobbed(out _);
        }

        public static bool AnyoneHasBeenKeyRobbed(out List<PlayerController> robbedPlayers)
        {
            robbedPlayers = new List<PlayerController>();
            if(GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
            {
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if(p != null && p.Ext().HasBeenKeyRobbed)
                    {
                        robbedPlayers.Add(p);
                    }
                }
                return robbedPlayers.Count > 0;
            }
            return false;
        }

        public PassiveItem GetCCCAbstraction()
        {
            var thingy = GetPassiveById(CCCAbstractions.IdOfTheFirstThing + cccAbId);
            if(cccAbId < 6)
            {
                cccAbId++;
            }
            return thingy;
        }

        public void CCCSequenceNextFloor()
        {
            player.OnNewFloorLoaded += StartCCCSequence;
        }

        public void StartCCCSequence(PlayerController play)
        {
            player.OnNewFloorLoaded -= StartCCCSequence;
            player.StartCoroutine(CCCSequence());
        }

        public IEnumerator CCCSequence()
        {
            while (!player.AcceptingAnyInput || Dungeon.IsGenerating || player.CurrentRoom == null)
            {
                yield return null;
            }
            var room = player.CurrentRoom;
            room.SealRoom();
            var unsealEvents = room.area.runtimePrototypeData.roomEvents.Where(x => x.condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && x.action == RoomEventTriggerAction.UNSEAL_ROOM).ToList();
            foreach (var roomEvent in unsealEvents)
            {
                room.area.runtimePrototypeData.roomEvents.Remove(roomEvent);
            }
            yield return new WaitForSeconds(1f);
            var unseal = LootEngine.SpawnItem(Item["chamberchamberchamber"].gameObject, room.GetCenterCell().ToVector2(), Vector2.down, 0f, true, false, false).AddComponent<Unsealer>();
            unseal.roomToUnseal = room;
            unseal.addTheseBack = unsealEvents;
            Instantiate(VFXDatabase.MiniBlank, room.GetCenterCell().ToVector2(), Quaternion.identity);
            Exploder.DoDistortionWave(room.GetCenterCell().ToVector2(), 2f, 0.2f, 4f, 0.2f);
            var ss = new GameObject("sound");
            ss.transform.position = room.GetCenterCell().ToVector2();
            AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", ss);
            Destroy(ss, 3f);
            yield break;
        }

        public class Unsealer : MonoBehaviour
        {
            public void Awake()
            {
                GetComponent<PassiveItem>().OnPickedUp += UnsealRoomAndMore;
            }

            public void UnsealRoomAndMore(PlayerController play)
            {
                if (roomToUnseal != null)
                {
                    play.CurrentRoom.UnsealRoom();
                    roomToUnseal.area?.runtimePrototypeData?.roomEvents?.AddRange(addTheseBack);
                }
                GetComponent<PassiveItem>().OnPickedUp -= UnsealRoomAndMore;
                Destroy(this);
            }

            public RoomHandler roomToUnseal;
            public IEnumerable<RoomEventDefinition> addTheseBack;
        }

        public bool wasLastInFoyer;
        public float poisonProjectileChance;
        public float lambCritChance;
        public PlayerController player;
        public bool HasBeenKeyRobbed;
        public float evenFloorDamageBonus;
        public float oddFloorDamageBonus;
        public float lastHandsOfRageTime;
        public float activeChargeMultiplier = 1f;
        public float activeChargeOnRoomEnter;
        public float activeChargeOnDamage;
        public float activeChargePerSecond;
        public float cooldownMultiplier = 1f;
        public float roomDamageOnActiveDamageCharge;
        public float roomDamageOnActiveRoomCharge;
        public float bulletChanceEffectScaleMultiplier = 1f;
        public float soulSnatcherDamageLeft = 1500f;
        public float giftFromBelowDamageLeft = 2000f;
        public int cccAbId;
        public bool isCCCRun;
        public StatModifier floorDamageModifier;
        public List<TarotCards.TarotCardType> tarotCards = new();
    }
}
