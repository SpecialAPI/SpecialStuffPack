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
            damageDone *= Owner.stats.Ext().GetStatValue("ActiveChargeMultiplier");
        }

        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.GetMultiplierForItem))]
        [HarmonyPostfix]
        public static void HandleCollectorActiveBonuses(ref float __result, PickupObject prefab, PlayerController player, bool completesSynergy)
        {
            if (prefab is PlayerItem)
            {
                if (player.Ext().tarotCards.Contains(TarotCards.TarotCardType.TheCollector))
                {
                    __result *= Mathf.Pow(player.PlayerHasActiveSynergy("A Better Fate") ? 3f : 2f, player.Ext().TarotCardCount(TarotCards.TarotCardType.TheCollector));
                }
            }
        }

        [HarmonyPatch(typeof(PlayerItem), nameof(PlayerItem.ApplyCooldown))]
        [HarmonyPostfix]
        public static void DivineCurse(PlayerItem __instance, PlayerController user)
        {
            __instance.remainingDamageCooldown *= user.stats.Ext().GetStatValue("CooldownMultiplier");
            __instance.remainingTimeCooldown *= user.stats.Ext().GetStatValue("CooldownMultiplier");
            if(__instance.remainingRoomCooldown > 0)
            {
                __instance.remainingRoomCooldown = Mathf.Max(1, Mathf.RoundToInt(__instance.remainingRoomCooldown * user.stats.Ext().GetStatValue("CooldownMultiplier")));
            }
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.DoPostProcessProjectile))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ModifyScale (IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Div)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, stats);
                    yield return new CodeInstruction(OpCodes.Call, PlayerStatsExt.statsext);
                    yield return new CodeInstruction(OpCodes.Ldstr, "BulletChanceMultiplier");
                    yield return new CodeInstruction(OpCodes.Call, svalue);
                    yield return new CodeInstruction(OpCodes.Mul);
                }
            }
            yield break;
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.ShootSingleProjectile))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SSP1(IEnumerable<CodeInstruction> instructions)
        {
            var playerstatsloadcount = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(stats))
                {
                    playerstatsloadcount++;
                    if(playerstatsloadcount == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 10);
                        yield return new CodeInstruction(OpCodes.Ldloc_0);
                        yield return new CodeInstruction(OpCodes.Call, moddmggun);
                    }
                }
                yield return instruction;
                if (instruction.opcode == OpCodes.Stloc_S && instruction.operand is LocalBuilder loca && loca.LocalType == typeof(Projectile) && loca.LocalIndex == 10)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 10);
                    yield return new CodeInstruction(OpCodes.Call, pext);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Stfld, sourcegun);
                }
            }
            yield break;
        }

        [HarmonyPatch(typeof(SummonTigerModifier), nameof(SummonTigerModifier.ShootSingleProjectile))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SSP2(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(stats))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, getcompproj);
                    yield return new CodeInstruction(OpCodes.Call, pext);
                    yield return new CodeInstruction(OpCodes.Ldfld, sourcegun);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, projowner);
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(PlayerController));
                    yield return new CodeInstruction(OpCodes.Call, moddmggun);
                }
                yield return instruction;
            }
            yield break;
        }

        [HarmonyPatch(typeof(FireVolleyOnRollItem), nameof(FireVolleyOnRollItem.ShootSingleProjectile))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SSP3(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(stats))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 6);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, moddmgrollitem);
                }
                yield return instruction;
            }
            yield break;
        }

        [HarmonyPatch(typeof(ShopItemController), nameof(ShopItemController.ModifiedPrice), MethodType.Getter)]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PriceMods(IEnumerable<CodeInstruction> instructions)
        {
            var rtiloadcount = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(rti))
                {
                    rtiloadcount++;
                    if (rtiloadcount == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, pricemult);
                        yield return new CodeInstruction(OpCodes.Mul);
                    }
                }
                yield return instruction;
            }
            yield break;
        }

        public static FieldInfo stats = AccessTools.Field(typeof(PlayerController), nameof(PlayerController.stats));
        public static FieldInfo sourcegun = AccessTools.Field(typeof(ProjectileExt), nameof(ProjectileExt.DefiniteSourceGun));
        public static MethodInfo svalue = AccessTools.Method(typeof(PlayerStatsExt), nameof(PlayerStatsExt.GetStatValue));
        public static MethodInfo pext = AccessTools.Method(typeof(CodeShortcuts), nameof(CodeShortcuts.Ext), new Type[] { typeof(Projectile) });
        public static MethodInfo getcompproj = AccessTools.Method(typeof(Component), nameof(Component.GetComponent), new Type[] { }, new Type[] { typeof(Projectile) });
        public static MethodInfo moddmggun = AccessTools.Method(typeof(PlayerControllerExt), nameof(PlayerControllerExt.ModProjectileDamageGun));
        public static MethodInfo moddmgrollitem = AccessTools.Method(typeof(PlayerControllerExt), nameof(PlayerControllerExt.ModProjectileDamageRollItem));
        public static MethodInfo rti = AccessTools.Method(typeof(Mathf), nameof(Mathf.RoundToInt));
        public static MethodInfo pricemult = AccessTools.Method(typeof(PlayerControllerExt), nameof(GetPriceMultForItemAllPlayers));
        public static MethodInfo projowner = AccessTools.PropertyGetter(typeof(Projectile), nameof(Projectile.Owner));

        public static void ModProjectileDamage(float m, Projectile proj, PlayerController play)
        {
            proj.baseData.damage += play.stats.Ext().GetStatValue("UnscaledFlatDamage");
            proj.baseData.damage += play.stats.Ext().GetStatValue("FlatDamage") * m;
        }

        public static void ModProjectileDamageGun(Gun g, Projectile proj, PlayerController player)
        {
            float m = 1f;
            if (g != null && g.DefaultModule != null)
            {
                float num = 0f;
                if (g.Volley != null)
                {
                    List<ProjectileModule> projectiles = g.Volley.projectiles;
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        num += projectiles[i].GetEstimatedShotsPerSecond(g.reloadTime);
                    }
                }
                else if (g.DefaultModule != null)
                {
                    num += g.DefaultModule.GetEstimatedShotsPerSecond(g.reloadTime);
                }
                if (num > 0f)
                {
                    m = 3.5f / num;
                }
            }
            ModProjectileDamage(m, proj, player);
        }

        public static void ModProjectileDamageRollItem(FireVolleyOnRollItem i, Projectile proj, PlayerController player)
        {
            ModProjectileDamage(3.5f * Mathf.Max(i.FireCooldown, player.rollStats.GetModifiedTime(player)), proj, player);
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

        [HarmonyPatch(typeof(RoomHandler), nameof(RoomHandler.OnEntered))]
        [HarmonyPrefix]
        public static void RememberVisibility(RoomHandler __instance, ref RoomHandler.VisibilityStatus __state)
        {
            __state = __instance.visibility;
        }

        [HarmonyPatch(typeof(RoomHandler), nameof(RoomHandler.OnEntered))]
        [HarmonyPostfix]
        public static void ProcessEventEnter(RoomHandler __instance, RoomHandler.VisibilityStatus __state, PlayerController p)
        {
            p.Ext().OnEnteredRoom?.Invoke(p, __instance, __state);
        }

        [HarmonyPatch(typeof(RoomHandler), nameof(RoomHandler.OnExited))]
        [HarmonyPostfix]
        public static void ProcessEventExit(RoomHandler __instance, PlayerController p)
        {
            p.Ext().OnExitedRoom?.Invoke(p, __instance);
        }

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

        public float GetPriceMultForItem(ShopItemController controller)
        {
            if(ModifyPriceMult != null)
            {
                var m = 1f;
                foreach(var invoke in ModifyPriceMult.GetInvocationList())
                {
                    m *= (float)invoke.DynamicInvoke(player, controller);
                }
                return m;
            }
            return 1f;
        }

        public static float GetPriceMultForItemAllPlayers(ShopItemController controller)
        {
            if(GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
            {
                var m = 1f;
                foreach(var p in GameManager.Instance.AllPlayers)
                {
                    if(p != null)
                    {
                        m *= p.Ext().GetPriceMultForItem(controller);
                    }
                }
                return m;
            }
            return 1f;
        }

        public void DamageOnActive(PlayerController player, PlayerItem item)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.Ambrosia) && (item.damageCooldown > 0f || item.roomCooldown > 0) && player.CurrentRoom != null && player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
            {
                var unreferenced = player.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
                foreach (var enemy in unreferenced)
                {
                    if (enemy == null || enemy.healthHaver == null)
                    {
                        continue;
                    }
                    var synergyActive = player.PlayerHasActiveSynergy("A Better Fate");
                    enemy.healthHaver.ApplyDamage(TarotCardCount(TarotCards.TarotCardType.Ambrosia) * (synergyActive ? 0.2f : 0.15f) * item.damageCooldown + TarotCardCount(TarotCards.TarotCardType.Ambrosia) * (synergyActive ? 7.5f : 5f) * item.roomCooldown, Vector2.zero, "An Active Item", CoreDamageTypes.None, 
                        DamageCategory.Unstoppable, true, null, true);
                }
            }
        }

        public void Update()
        {
            if (player.stats.Ext().GetStatValue("ActiveChargePerSecond") > 0f && player.IsInCombat && player.activeItems != null)
            {
                foreach (var item in player.activeItems)
                {
                    if (item != null)
                    {
                        item.DidDamage(player, player.stats.Ext().GetStatValue("ActiveChargePerSecond") * BraveTime.DeltaTime);
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
                        item.DidDamage(player, player.stats.Ext().GetStatValue("ActiveChargeOnRoomEnter"));
                    }
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.GodlyMoment))
            {
                player.TriggerStuffedStarInvulnerability(Mathf.Sqrt(TarotCardCount(TarotCards.TarotCardType.GodlyMoment)) * (player.PlayerHasActiveSynergy("A Better Fate") ? 3f : 2f));
            }
        }

        public void EnemyReceivedDamage(float dmg, bool fatal, HealthHaver hh)
        {
            if(tarotCards.Contains(TarotCards.TarotCardType.BurningDead) && fatal && hh.specRigidbody != null)
            {
                Exploder.Explode(hh.specRigidbody.UnitCenter, bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
                var count = TarotCardCount(TarotCards.TarotCardType.BurningDead);
                if(player.PlayerHasActiveSynergy("A Better Fate"))
                {
                    count *= 2;
                }
                if(count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        Exploder.Explode(hh.specRigidbody.UnitCenter + Random.insideUnitCircle * Random.Range(0.5f, 1.5f), bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
                    }
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.NaturesBoon) && fatal && hh.specRigidbody != null)
            {
                for (int i = 0; i < (player.PlayerHasActiveSynergy("A Better Fate") ? 1 : 2) * TarotCardCount(TarotCards.TarotCardType.NaturesBoon); i++)
                {
                    OwnedShootProjectile(MahogunyObject.Volley.projectiles[1].projectiles[0], hh.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), player).specRigidbody.RegisterTemporaryCollisionException(hh.specRigidbody,
                        1f, null);
                }
                if(player.PlayerHasActiveSynergy("A Better Fate"))
                {
                    OwnedShootProjectile(MahogunyObject.Volley.projectiles[0].projectiles[0], hh.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), player).specRigidbody.RegisterTemporaryCollisionException(hh.specRigidbody,
                        1f, null);
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.NeptunesCurse) && fatal && hh.specRigidbody != null)
            {
                for (int i = 0; i < TarotCardCount(TarotCards.TarotCardType.NeptunesCurse); i++)
                {
                    OwnedShootProjectile(player.PlayerHasActiveSynergy("A Better Fate") ? LikeShootingFishSynergyObject.DefaultModule.finalProjectile : BarrelObject.GetProjectile(), hh.specRigidbody.UnitCenter, Random.insideUnitCircle.ToAngle(), player).specRigidbody.RegisterTemporaryCollisionException(hh.specRigidbody,
                    1f, null);
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.SoulSnatcher))
            {
                soulSnatcherDamageLeft -= dmg * TarotCardCount(TarotCards.TarotCardType.SoulSnatcher) * (player.PlayerHasActiveSynergy("A Better Fate") ? 1.5f : 1f);
                if(soulSnatcherDamageLeft <= 0)
                {
                    soulSnatcherDamageLeft = 1500;
                    LootEngine.GivePrefabToPlayer(HalfHeartObject.gameObject, player);
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.GiftFromBelow))
            {
                giftFromBelowDamageLeft -= dmg * TarotCardCount(TarotCards.TarotCardType.GiftFromBelow) * (player.PlayerHasActiveSynergy("A Better Fate") ? 1.5f : 1f);
                if (giftFromBelowDamageLeft <= 0)
                {
                    giftFromBelowDamageLeft = 2000;
                    LootEngine.GivePrefabToPlayer(ArmorObject.gameObject, player);
                }
            }
        }

        public void RollStarted(PlayerController play, Vector2 dir)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.Bomb))
            {
                Exploder.Explode(play.CenterPosition, bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
                var count = TarotCardCount(TarotCards.TarotCardType.Bomb);
                if(play.PlayerHasActiveSynergy("A Better Fate"))
                {
                    count *= 2;
                }
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        Exploder.Explode(play.CenterPosition + Random.insideUnitCircle * Random.Range(0.5f, 1.5f), bombExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
                    }
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.IchorLingered))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(play.PlayerHasActiveSynergy("A Better Fate") ? GoopDatabase.LambIchorWhite : GoopDatabase.LambIchor).TimedAddGoopCircle(play.specRigidbody.UnitBottomCenter, 3.5f * TarotCardCount(TarotCards.TarotCardType.IchorLingered), 0.5f, false);
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
                args.ModifiedHealing *= (player.PlayerHasActiveSynergy("A Better Fate") ? 1.5f : 1f) + TarotCardCount(TarotCards.TarotCardType.FortunesBlessing);
            }
        }

        public void ModifyDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.Intangible) && !player.IsInCombat)
            {
                var intangibleCount = TarotCardCount(TarotCards.TarotCardType.Intangible);
                if(!player.PlayerHasActiveSynergy("A Better Fate"))
                {
                    intangibleCount -= 1;
                }
                if (intangibleCount > 0 && DiminishingReturnsChance(intangibleCount, 1f, 1f).RandomChance())
                {
                    args.ModifiedDamage = 0f;
                }
                return;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.ShieldOfFaith) && 
                DiminishingReturnsChance(TarotCardCount(TarotCards.TarotCardType.ShieldOfFaith), 
                    player.PlayerHasActiveSynergy("A Better Fate") ? 0.125f : 0.25f,
                    player.PlayerHasActiveSynergy("A Better Fate") ? 0.986f : 0.926f)
                .RandomChance())
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
                    enemy.healthHaver.ApplyDamage((player.PlayerHasActiveSynergy("A Better Fate") ? 150f : 100f) * TarotCardCount(TarotCards.TarotCardType.DeathsDoor), Vector2.zero, "died because of death", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }
            if (player.activeItems != null)
            {
                foreach (var item in player.activeItems)
                {
                    if (item != null)
                    {
                        item.DidDamage(player, player.stats.Ext().GetStatValue("ActiveChargeOnDamage"));
                    }
                }
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.IchorEarned))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(player.PlayerHasActiveSynergy("A Better Fate") ? GoopDatabase.LambIchorWhite : GoopDatabase.LambIchor).TimedAddGoopCircle(player.specRigidbody.UnitBottomCenter, 6f * TarotCardCount(TarotCards.TarotCardType.IchorEarned), 1f, false);
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.KinOfTurua) && player.CurrentRoom != null && player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
            {
                var nearestEnemy = player.CurrentRoom.GetNearestEnemy(player.CenterPosition, out _, true, true);
                if (nearestEnemy != null)
                {
                    BeamController.FreeFireBeam((player.PlayerHasActiveSynergy("A Better Fate") ? KalibersGripSynergyObject : AbyssalTentacleObject).GetProjectile(), player, (nearestEnemy.CenterPosition - player.CenterPosition).ToAngle(), 3f * TarotCardCount(TarotCards.TarotCardType.KinOfTurua), true);
                }
            }
            if(tarotCards.Contains(TarotCards.TarotCardType.Retribution) && player.CurrentRoom != null && player.IsInCombat)
            {
                var amount = 10 * TarotCardCount(TarotCards.TarotCardType.Retribution);
                for(int i = 0; i < amount; i++)
                {
                    var randomPos = player.CurrentRoom.GetRandomAvailableCell(new(1, 1), CellTypes.FLOOR, false, null);
                    if (randomPos.HasValue)
                    {
                        Instantiate((player.PlayerHasActiveSynergy("A Better Fate") ? ProximityMineObject : BombObject).objectToSpawn, randomPos.Value.ToVector2(), Quaternion.identity);
                        LootEngine.DoDefaultPurplePoof(randomPos.Value.ToVector2(), false);
                    }
                }
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
                    enemy.healthHaver.ApplyDamage((player.PlayerHasActiveSynergy("A Better Fate") ? 50f : 75f) * TarotCardCount(TarotCards.TarotCardType.DiseasedHeart), Vector2.zero, "died because of death", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }
        }

        public void HandleRevives(Vector2 d)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.TheDeal))
            {
                tarotCards.Remove(TarotCards.TarotCardType.TheDeal);
                player.healthHaver.ApplyHealing(player.PlayerHasActiveSynergy("A Better Fate") ? 2f : 1f);
                if (player.ForceZeroHealthState)
                {
                    player.healthHaver.Armor += player.PlayerHasActiveSynergy("A Better Fate") ? 4f : 2f;
                }
                player.ClearDeadFlags();
            }
        }

        public void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherColliders)
        {
            if(tarotCards.Contains(TarotCards.TarotCardType.BlazingTrail) && player.IsDodgeRolling && otherRigidbody != null && otherRigidbody.aiActor != null && otherRigidbody.healthHaver != null)
            {
                otherRigidbody.healthHaver.ApplyDamage((player.PlayerHasActiveSynergy("A Better Fate") ? 150f : 100f) * BraveTime.DeltaTime * TarotCardCount(TarotCards.TarotCardType.BlazingTrail), Vector2.zero, "incinerated", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
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
            if(tarotCards.Contains(TarotCards.TarotCardType.WraithsWill) && !player.IsDodgeRolling && otherRigidbody != null && otherRigidbody.aiActor != null && otherRigidbody.healthHaver)
            {
                otherRigidbody.healthHaver.ApplyDamage((player.PlayerHasActiveSynergy("A Better Fate") ? 20f : 15f) * TarotCardCount(TarotCards.TarotCardType.WraithsWill), Vector2.zero, "the wraiths", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
            }
        }

        public void FloorLoadEffects(PlayerController play)
        {
            if (tarotCards.Contains(TarotCards.TarotCardType.Telescope))
            {
                Minimap.Instance.RevealAllRooms(TarotCardCount(TarotCards.TarotCardType.Telescope) > 1 || player.PlayerHasActiveSynergy("A Better Fate"));
            }
        }

        public void HandleDefaultEffects(Projectile proj, float scale)
        {
            if (player.stats.Ext().GetStatValue("LambPoisonChance").ScaledRandom(scale))
            {
                proj.statusEffectsToApply.Add(defaultPoison);
                proj.AdjustPlayerProjectileTint(Color.green, 0, 0f);
            }
            if (player.stats.Ext().GetStatValue("LambCritChance").RandomChance())
            {
                proj.AdjustPlayerProjectileTint(Color.red, 0, 0f);
                proj.baseData.damage *= 2f;
            }
            if (tarotCards.Contains(TarotCards.TarotCardType.HandsOfRage) && Time.time - lastHandsOfRageTime >= (player.PlayerHasActiveSynergy("A Better Fate") ? 10f : 7.5f) / TarotCardCount(TarotCards.TarotCardType.HandsOfRage))
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

        public int TarotCardCount(TarotCards.TarotCardType t)
        {
            return tarotCards.FindAll(x => x == t).Count;
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
        public PlayerController player;
        public bool HasBeenKeyRobbed;
        public float lastHandsOfRageTime;
        public float soulSnatcherDamageLeft = 1500f;
        public float giftFromBelowDamageLeft = 2000f;
        public int cccAbId;
        public bool isCCCRun;
        public StatModifier floorDamageModifier;
        public List<TarotCards.TarotCardType> tarotCards = new();
        public Action<PlayerController, RoomHandler, RoomHandler.VisibilityStatus> OnEnteredRoom;
        public Action<PlayerController, RoomHandler> OnExitedRoom;
        public Func<PlayerController, ShopItemController, float> ModifyPriceMult;
    }
}
