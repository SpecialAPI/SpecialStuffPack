using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    [HarmonyPatch]
    public static class SPCultist
    {
        [HarmonyPatch(typeof(Foyer), nameof(Foyer.SetUpCharacterCallbacks))]
        [HarmonyPostfix]
        public static void AddSPCultist(Foyer __instance)
        {
            var flag = Object.FindObjectsOfType<FoyerCharacterSelectFlag>().Where(x => !x.IsCoopCharacter).FirstOrDefault();
            if(flag != null)
            {
                var coopFlag = flag.transform.parent.GetComponentsInChildren<FoyerCharacterSelectFlag>(false).Where(x => x.IsCoopCharacter).FirstOrDefault();
                if(coopFlag != null)
                {
                    var go = Object.Instantiate(coopFlag.gameObject, coopFlag.transform.position, Quaternion.identity);
                    var playmaker = go.GetComponent<PlayMakerFSM>();
                    playmaker.fsm = JsonUtility.FromJson<Fsm>(JsonUtility.ToJson(flag.playmakerFsm.fsm));
                    playmaker.fsm.Init(playmaker);
                    playmaker.fsm.states.Do(x => x.actions.Do(x =>
                    {
                        if(x is ChangeToNewCharacter change)
                        {
                            change.PlayerPrefabPath = "PlayerCoopCultist";
                        }
                    }));
                    var newFlag = go.GetComponent<FoyerCharacterSelectFlag>();
                    newFlag.IsCoopCharacter = false;
                    newFlag.ToggleSelf(true);
                    go.AddComponent<DisableWhenSecondDeviceConnected>().flag = newFlag;
                    go.transform.position.GetAbsoluteRoom().RegisterInteractable(go.GetComponentInChildren<IPlayerInteractable>());
                    __instance.OnPlayerCharacterChanged += newFlag.OnSelectedCharacterCallback;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Start))]
        [HarmonyPrefix]
        public static void MakeCultistSP(PlayerController __instance)
        {
            if(__instance.characterIdentity == PlayableCharacters.CoopCultist && GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
            {
                __instance.m_overridePlayerSwitchState = "CoopCultist";
                __instance.characterIdentity = PlayableCharactersE.SPCultist;
            }
        }

        [HarmonyPatch(typeof(GameStatsManager), nameof(GameStatsManager.SetFlag))]
        [HarmonyPostfix]
        public static void UnlockNumber2(GameStatsManager __instance, GungeonFlags flag, bool value)
        {
            if (value && flag == GungeonFlags.BOSSKILLED_DRAGUN && GameManager.HasInstance && GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharactersE.SPCultist)
            {
                __instance.SetFlag(GungeonFlags.COOP_PAST_REACHED, true);
            }
        }

        [HarmonyPatch(typeof(GameStatsManager), nameof(GameStatsManager.GetCharacterSpecificFlag), typeof(PlayableCharacters), typeof(CharacterSpecificGungeonFlags))]
        [HarmonyPostfix]
        public static void MakeCultistPastCleared(ref bool __result, PlayableCharacters character, CharacterSpecificGungeonFlags flag)
        {
            if(character == PlayableCharactersE.SPCultist && (flag == CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME || flag == CharacterSpecificGungeonFlags.KILLED_PAST))
            {
                __result = true;
            }
            else if(character == PlayableCharacters.CoopCultist)
            {
                __result = true;
            }
        }

        [HarmonyPatch(typeof(CharacterCostumeSwapper), nameof(CharacterCostumeSwapper.Start))]
        [HarmonyPrefix]
        public static bool MakeCultistsCostumeAlwaysAppear(CharacterCostumeSwapper __instance)
        {
            if(__instance.TargetCharacter == PlayableCharacters.CoopCultist)
            {
                __instance.m_active = true;
                __instance.AlternateCostumeSprite.renderer.enabled = true;
                __instance.CostumeSprite.renderer.enabled = false;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterCostumeSwapper), nameof(CharacterCostumeSwapper.Interact))]
        [HarmonyPrefix]
        public static bool AddAltCostume(CharacterCostumeSwapper __instance, PlayerController interactor)
        {
            if (interactor.characterIdentity != __instance.TargetCharacter && interactor.characterIdentity == PlayableCharactersE.SPCultist && __instance.TargetCharacter == PlayableCharacters.CoopCultist)
            {
                if (interactor.IsUsingAlternateCostume)
                {
                    interactor.SwapToAlternateCostume(null);
                }
                else
                {
                    if (__instance.TargetLibrary)
                    {
                        interactor.AlternateCostumeLibrary = __instance.TargetLibrary;
                    }
                    interactor.SwapToAlternateCostume(null);
                }
                SpriteOutlineManager.RemoveOutlineFromSprite(__instance.AlternateCostumeSprite, false);
                SpriteOutlineManager.RemoveOutlineFromSprite(__instance.CostumeSprite, false);
                __instance.AlternateCostumeSprite.renderer.enabled = !__instance.AlternateCostumeSprite.renderer.enabled;
                __instance.CostumeSprite.renderer.enabled = !__instance.CostumeSprite.renderer.enabled;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterCostumeSwapper), nameof(CharacterCostumeSwapper.OnEnteredRange))]
        [HarmonyPrefix]
        public static bool FixOutlines1(CharacterCostumeSwapper __instance, PlayerController interactor)
        {
            if (interactor.characterIdentity != __instance.TargetCharacter && interactor.characterIdentity == PlayableCharactersE.SPCultist && __instance.TargetCharacter == PlayableCharacters.CoopCultist)
            {
                if (__instance.AlternateCostumeSprite.renderer.enabled)
                {
                    SpriteOutlineManager.AddOutlineToSprite(__instance.AlternateCostumeSprite, Color.white);
                }
                else
                {
                    SpriteOutlineManager.AddOutlineToSprite(__instance.CostumeSprite, Color.white);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterCostumeSwapper), nameof(CharacterCostumeSwapper.OnExitRange))]
        [HarmonyPrefix]
        public static bool FixOutlines2(CharacterCostumeSwapper __instance, PlayerController interactor)
        {
            if (interactor.characterIdentity != __instance.TargetCharacter && interactor.characterIdentity == PlayableCharactersE.SPCultist && __instance.TargetCharacter == PlayableCharacters.CoopCultist)
            {
                if (__instance.AlternateCostumeSprite.renderer.enabled)
                {
                    SpriteOutlineManager.RemoveOutlineFromSprite(__instance.AlternateCostumeSprite, false);
                }
                else
                {
                    SpriteOutlineManager.RemoveOutlineFromSprite(__instance.CostumeSprite, false);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(MetalGearRatDeathController), nameof(MetalGearRatDeathController.OnDeathCR))]
        [HarmonyPostfix]
        public static IEnumerator NoPunchout(IEnumerator rat, MetalGearRatDeathController __instance)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER && GameManager.Instance.PrimaryPlayer != null && 
                GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharactersE.SPCultist)
            {
                SuperReaperController.PreventShooting = true;
                foreach (PlayerController playerController in GameManager.Instance.AllPlayers)
                {
                    playerController.SetInputOverride("metal gear death");
                }
                yield return new WaitForSeconds(2f);
                Pixelator.Instance.FadeToColor(0.75f, Color.white, false, 0f);
                yield return new WaitForSeconds(3f);
                MetalGearRatIntroDoer introDoer = __instance.GetComponent<MetalGearRatIntroDoer>();
                introDoer.ModifyCamera(false);
                yield return new WaitForSeconds(0.75f);
                __instance.aiActor.StealthDeath = true;
                __instance.healthHaver.persistsOnDeath = true;
                __instance.healthHaver.DeathAnimationComplete(null, null);
                if (__instance.aiActor)
                {
                    UnityEngine.Object.Destroy(__instance.aiActor);
                }
                if (__instance.healthHaver)
                {
                    UnityEngine.Object.Destroy(__instance.healthHaver);
                }
                if (__instance.behaviorSpeculator)
                {
                    UnityEngine.Object.Destroy(__instance.behaviorSpeculator);
                }
                if (__instance.aiAnimator.ChildAnimator)
                {
                    UnityEngine.Object.Destroy(__instance.aiAnimator.ChildAnimator.gameObject);
                }
                if (__instance.aiAnimator)
                {
                    UnityEngine.Object.Destroy(__instance.aiAnimator);
                }
                if (__instance.specRigidbody)
                {
                    UnityEngine.Object.Destroy(__instance.specRigidbody);
                }
                __instance.RegenerateCache();
                RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
                MetalGearRatRoomController room = UnityEngine.Object.FindObjectOfType<MetalGearRatRoomController>();
                room.TransformToDestroyedRoom();
                GameManager.Instance.PrimaryPlayer.WarpToPoint(room.transform.position + new Vector3(17.25f, 14.5f), false, false);
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
                {
                    GameManager.Instance.SecondaryPlayer.WarpToPoint(room.transform.position + new Vector3(27.5f, 14.5f), false, false);
                }
                __instance.aiActor.ToggleRenderers(false);
                yield return null;
                foreach (PlayerController playerController2 in GameManager.Instance.AllPlayers)
                {
                    playerController2.ClearInputOverride("metal gear death");
                }
                Pixelator.Instance.FadeToColor(1f, Color.white, true, 0f);
                GameObject gameObject = PickupObjectDatabase.GetById(GlobalItemIds.RatKey).gameObject;
                Vector3 position = room.transform.position;
                RoomHandler bossRoom = null;
                foreach (RoomHandler roomHandler2 in GameManager.Instance.Dungeon.data.rooms)
                {
                    if (roomHandler2.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
                    {
                        bossRoom = roomHandler2;
                        break;
                    }
                }
                var mgRatController = __instance.GetComponent<MetalGearRatController>();
                var keys = Mathf.Clamp(
                    1 +
                    mgRatController.m_isTailgunDestroyed.ToInt() +
                    mgRatController.m_isRadomeDestroyed.ToInt() +
                    (!currentRoom.PlayerHasTakenDamageInThisRoom).ToInt() +
                    (!(bossRoom?.PlayerHasTakenDamageInThisRoom).GetValueOrDefault()).ToInt() +
                    (mgRatController.m_isTailgunDestroyed && mgRatController.m_isRadomeDestroyed && currentRoom.PlayerHasTakenDamageInThisRoom && (bossRoom?.PlayerHasTakenDamageInThisRoom).GetValueOrDefault()).ToInt()
                    , 1, 6);
                if (keys >= 1)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(14.25f, 17f), Vector2.zero, 0f, true, false, false);
                }
                if (keys >= 2)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(13.25f, 14.5f), Vector2.zero, 0f, true, false, false);
                }
                if (keys >= 3)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(14.25f, 12f), Vector2.zero, 0f, true, false, false);
                }
                if (keys >= 4)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(30.25f, 17f), Vector2.zero, 0f, true, false, false);
                }
                if (keys >= 5)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(31.25f, 14.5f), Vector2.zero, 0f, true, false, false);
                }
                if (keys >= 6)
                {
                    LootEngine.SpawnItem(gameObject, position + new Vector3(30.25f, 12f), Vector2.zero, 0f, true, false, false);
                }
                var notePrefab = __instance.PunchoutMinigamePrefab.GetComponent<PunchoutController>().PlayerLostNotePrefab;
                if (notePrefab != null)
                {
                    IntVector2 intVector = currentRoom.GetCenteredVisibleClearSpot(3, 3, out var flag, true);
                    intVector = intVector - currentRoom.area.basePosition + IntVector2.One;
                    if (flag)
                    {
                        GameObject note = DungeonPlaceableUtility.InstantiateDungeonPlaceable(notePrefab.gameObject, currentRoom, intVector, false, AIActor.AwakenAnimationType.Default, false);
                        if (note)
                        {
                            IPlayerInteractable[] interfacesInChildren = note.GetInterfacesInChildren<IPlayerInteractable>();
                            for (int i = 0; i < interfacesInChildren.Length; i++)
                            {
                                currentRoom.RegisterInteractable(interfacesInChildren[i]);
                            }
                            var doer = note.GetComponentInChildren<NoteDoer>();
                            if(doer != null)
                            {
                                doer.useAdditionalStrings = false;
                                doer.alreadyLocalized = false;
                                doer.stringKey = "#SPAPI_RAT_NOTE_SP_CULTIST";
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
                UnityEngine.Object.Destroy(__instance.gameObject);
            }
            else
            {
                yield return rat;
            }
            yield break;
        }

        [HarmonyPatch(typeof(FoyerCharacterSelectFlag), nameof(FoyerCharacterSelectFlag.OnSelectedCharacterCallback))]
        [HarmonyPrefix]
        public static bool TrackException(FoyerCharacterSelectFlag __instance, PlayerController newCharacter)
        {
            Debug.Log(string.Concat(new object[]
           {
            newCharacter.name,
            "|",
            newCharacter.characterIdentity,
            " <===="
           }));
            if (newCharacter.gameObject.name.Contains(__instance.CharacterPrefabPath))
            {
                __instance.gameObject.SetActive(false);
                __instance.GetComponent<SpeculativeRigidbody>().enabled = false;
                if (__instance.IsEevee)
                {
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, -5f);
                }
                if (__instance.IsGunslinger)
                {
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, -7f);
                }
            }
            else if (!__instance.gameObject.activeSelf)
            {
                __instance.gameObject.SetActive(true);
                SpriteOutlineManager.RemoveOutlineFromSprite(__instance.sprite, true);
                __instance.specRigidbody.enabled = true;
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(__instance.specRigidbody, null, false);
                if (!__instance.m_isAlternateCostume)
                {
                    CharacterSelectIdleDoer component = __instance.GetComponent<CharacterSelectIdleDoer>();
                    if(component != null)
                    {
                        component.enabled = true;
                    }
                }
            }
            return false;
        }

        private class DisableWhenSecondDeviceConnected : MonoBehaviour
        {
            public void Update()
            {
                if(flag == null)
                {
                    return;
                }
                if (flag.m_active && InputManager.Devices.Count > 0)
                {
                    flag.ToggleSelf(false);
                }
                else if (!flag.m_active && InputManager.Devices.Count == 0)
                {
                    flag.ToggleSelf(true);
                }
            }

            public FoyerCharacterSelectFlag flag;
        }
    }
}
