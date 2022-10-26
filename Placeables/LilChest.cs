using SpecialStuffPack.PlayMakerActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables //not really a placeable but whatever
{
	[HarmonyPatch]
	public class LilChest : Chest, IPlayerInteractable
	{
		[HarmonyPatch(typeof(Chest), nameof(BecomeCoopChest))]
		[HarmonyPatch(typeof(Chest), nameof(UnbecomeCoopChest))]
		[HarmonyPatch(typeof(Chest), nameof(MaybeBecomeMimic))]
		[HarmonyPatch(typeof(Chest), nameof(DoMimicTransformation))]
		[HarmonyPatch(typeof(Chest), nameof(RoomEntered))]
		[HarmonyPrefix]
		public static bool DisableStuff(Chest __instance)
		{
			if (__instance is LilChest)
			{
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(ForceUnlock))]
		[HarmonyPrefix]
		public static void PlayUnlockAnimationForce(Chest __instance)
		{
			if (__instance.IsLocked && __instance is LilChest lil && !string.IsNullOrEmpty(lil.unlockAnim))
			{
				lil.spriteAnimator.Play(lil.unlockAnim);
				AkSoundEngine.PostEvent("Play_OBJ_chest_unlock_01", lil.gameObject);
			}
		}

		[HarmonyPatch(typeof(Chest), nameof(Unlock))]
		[HarmonyPrefix]
		public static void PlayUnlockAnimation(Chest __instance)
		{
			if (__instance is LilChest lil && !string.IsNullOrEmpty(lil.unlockAnim))
			{
				lil.spriteAnimator.Play(lil.unlockAnim);
				AkSoundEngine.PostEvent("Play_OBJ_chest_unlock_01", lil.gameObject);
			}
		}

		[HarmonyPatch(typeof(Chest), nameof(BreakLock))]
		[HarmonyPrefix]
		public static void PlayBreakAnimation(Chest __instance)
		{
			if (__instance is LilChest lil && !string.IsNullOrEmpty(lil.breakAnim) && !lil.pickedUp)
			{
				AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", lil.gameObject);
				lil.spriteAnimator.Play(lil.breakAnim);
				var squishyBounce = lil.GetComponent<BetterSquishyBounceWiggler>();
				if (squishyBounce != null)
				{
					squishyBounce.StopBounce();
				}
				var material = SpriteOutlineManager.GetOutlineMaterial(lil.sprite);
				if (material != null)
				{
					var color = material.GetColor("_OverrideColor");
					SpriteOutlineManager.RemoveOutlineFromSprite(lil.sprite);
					SpriteOutlineManager.AddOutlineToSprite(lil.sprite, color, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
				}
			}
		}

		[HarmonyPatch(typeof(Chest), nameof(Interact))]
		[HarmonyPrefix]
		public static bool Override1(Chest __instance, PlayerController player)
		{
			if (__instance is LilChest lil)
			{
				lil.Interact(player);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(Initialize))]
		[HarmonyPrefix]
		public static bool Override2(Chest __instance)
		{
			if (__instance is LilChest lil)
			{
				lil.Initialize();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(HandleSpawnBehavior))]
		[HarmonyPrefix]
		public static bool Override3(Chest __instance)
		{
			if (__instance is LilChest lil)
			{
				lil.HandleSpawnBehavior();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(SpawnBehavior_CR))]
		[HarmonyPrefix]
		public static bool Override4(Chest __instance, ref IEnumerator __result)
		{
			if (__instance is LilChest lil)
			{
				__result = lil.SpawnBehavior_CR();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(Open))]
		[HarmonyPrefix]
		public static bool Override5(Chest __instance, PlayerController player)
		{
			if (__instance is LilChest lil)
			{
				lil.Open(player);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(PresentItem))]
		[HarmonyPrefix]
		public static bool Override6(Chest __instance, ref IEnumerator __result)
		{
			if (__instance is LilChest lil)
			{
				__result = lil.PresentItem();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(OnEnteredRange))]
		[HarmonyPrefix]
		public static bool Override7(Chest __instance, PlayerController interactor)
		{
			if (__instance is LilChest lil)
			{
				lil.OnEnteredRange(interactor);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(OnExitRange))]
		[HarmonyPrefix]
		public static bool Override8(Chest __instance, PlayerController interactor)
		{
			if (__instance is LilChest lil)
			{
				lil.OnExitRange(interactor);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(GetAnimationState))]
		[HarmonyPrefix]
		public static bool Override8(Chest __instance, PlayerController interactor, ref bool shouldBeFlipped)
		{
			if (__instance is LilChest lil)
			{
				lil.GetAnimationState(interactor, out shouldBeFlipped);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(BecomeRainbowChest))]
		[HarmonyPrefix]
		public static bool Override9(Chest __instance)
		{
			if (__instance is LilChest lil)
			{
				lil.BecomeRainbowChest();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Chest), nameof(HandleSynergyGambleChest))]
		[HarmonyPrefix]
		public static bool Override10(Chest __instance, ref IEnumerator __result, PlayerController player)
		{
			if (__instance is LilChest lil)
			{
				__result = lil.HandleSynergyGambleChest(player);
				return false;
			}
			return true;
		}

		public new void Awake()
		{
			base.Awake();
			PreventFuse = true;
            if (immobile)
            {
				if(debris != null)
				{
					debris.enabled = false;
				}
				var bounce = GetComponent<BetterSquishyBounceWiggler>();
				if(bounce != null)
                {
					bounce.WiggleHold = true;
                }
            }
		}

		public static void SetupChests()
		{
			var reward = GameManager.Instance.CurrentRewardManager;
			LilChestD = SetupSmallChest("Brown", "brown", reward.D_Chest.lootTable, reward.D_Chest.breakertronLootTable);
			LilChestC = SetupSmallChest("Blue", "blue", reward.C_Chest.lootTable, reward.C_Chest.breakertronLootTable);
			LilChestB = SetupSmallChest("Green", "green", reward.B_Chest.lootTable, reward.B_Chest.breakertronLootTable);
			LilChestA = SetupSmallChest("Red", "red", reward.A_Chest.lootTable, reward.A_Chest.breakertronLootTable);
			LilChestS = SetupSmallChest("Black", "black", reward.S_Chest.lootTable, reward.S_Chest.breakertronLootTable);
			(LilGlitch = SetupSmallChest("Glitch", "green", reward.B_Chest.lootTable, reward.B_Chest.breakertronLootTable)).forceGlitchChestSerialized = true;
			(LilRainbow = SetupSmallChest("Rainbow", "red", reward.Rainbow_Chest.lootTable, reward.Rainbow_Chest.breakertronLootTable)).IsRainbowChest = true;

			var lilsynergy = SetupSmallChest("Synergy", "synergy", reward.Synergy_Chest.lootTable, reward.Synergy_Chest.breakertronLootTable, 12);
			lilsynergy.spriteAnimator.Library.AddClip(lilsynergy.openAnimName = "small_chest_synergy_gamble", lilsynergy.sprite.Collection, 12);
			lilsynergy.spriteAnimator.Library.AddClip("small_chest_synergy_fail_open", lilsynergy.sprite.Collection, 12);
			lilsynergy.stealable = true;
			lilsynergy.immobile = true;
			LilSynergy = lilsynergy;

			SetupSmallChestPlacer("LilChestPlacer Random_Normal");
			SetupSmallChestPlacer("LilChestPlacer Random_Unlocked", overrideLockChance: 0f);
			SetupSmallChestPlacer("LilChestPlacer Random_BrokenLock", brokenLockChance: 100f);
			SetupSmallChestPlacer("LilChestPlacer Random_HiddenRainbow", hiddenRainbowChance: 100f);

			string[] regularChestColors = new string[]
			{
				"Brown",
				"Blue",
				"Green",
				"Red",
				"Black"
			};

			foreach(var color in regularChestColors)
            {
				SetupSmallChestPlacer($"LilChestPlacer {color}_Normal");
				SetupSmallChestPlacer($"LilChestPlacer {color}_Unlocked", overrideLockChance: 0f);
				SetupSmallChestPlacer($"LilChestPlacer {color}_BrokenLock", brokenLockChance: 100f);
				SetupSmallChestPlacer($"LilChestPlacer {color}_HiddenRainbow", hiddenRainbowChance: 100f);
			}

			SetupSmallChestPlacer("LilChestPlacer Rainbow_Normal", overrideChest: LilRainbow);
			SetupSmallChestPlacer("LilChestPlacer Rainbow_BrokenLock", overrideChest: LilRainbow, brokenLockChance: 100f); // :)

			SetupSmallChestPlacer("LilChestPlacer Synergy_Normal");
			SetupSmallChestPlacer("LilChestPlacer Synergy_Unlocked", overrideLockChance: 0f);
			SetupSmallChestPlacer("LilChestPlacer Synergy_BrokenLock", brokenLockChance: 100f);
		}

		public static void SetupSmallChestPlacer(string name, PickupObject.ItemQuality? overrideItemQuality = null, float? overrideLockChance = null, bool unlockIfWooden = false, float brokenLockChance = 0f, 
			float glitchChance = 0f, float hiddenRainbowChance = 0f, LilChest overrideChest = null)
        {
			var placer = AssetBundleManager.Load<GameObject>(name).AddComponent<LilChestPlacer>();
			placer.OverrideItemQuality = overrideItemQuality.HasValue;
			placer.ItemQuality = overrideItemQuality.GetValueOrDefault();

			placer.OverrideLockChance = overrideLockChance.HasValue || unlockIfWooden;
			placer.LockChance = overrideLockChance.GetValueOrDefault();
			placer.ForceUnlockedIfWooden = unlockIfWooden;

			placer.brokenLockChance = brokenLockChance;
			placer.glitchChance = glitchChance;
			placer.secretRainbowChance = hiddenRainbowChance;

			placer.UseOverrideChest = overrideChest != null;
			placer.OverrideChestPrefab = overrideChest;
		}

		public static LilChest LilChestD;
		public static LilChest LilChestC;
		public static LilChest LilChestB;
		public static LilChest LilChestA;
		public static LilChest LilChestS;
		public static LilChest LilGlitch;
		public static LilChest LilRainbow;
		public static LilChest LilSynergy;

		public static Chest GetLilChestForQuality(PickupObject.ItemQuality targetQuality)
		{
            return targetQuality switch
            {
                PickupObject.ItemQuality.D => LilChestD,
                PickupObject.ItemQuality.C => LilChestC,
                PickupObject.ItemQuality.B => LilChestB,
                PickupObject.ItemQuality.A => LilChestA,
                PickupObject.ItemQuality.S => LilChestS,
                _ => null,
            };
        }

		public static LilChest SetupSmallChest(string objectName, string spriteName, LootData lootTable, LootData breakertronLootTable, int openAnimFps = 15)
		{
			var greenChest = AssetBundleManager.Load<GameObject>($"SmallChest{objectName}", null, null);
			var chest = greenChest.AddComponent<LilChest>();
			chest.maxVelocityForOpen = 0.25f;
			chest.lootTable = lootTable;
			chest.breakertronLootTable = breakertronLootTable;
			chest.IsLocked = true;
			var coll = EasyCollectionSetup("PlaceableCollection");
			tk2dSprite.AddComponent(greenChest, coll, AddSpriteToCollection($"small_chest_{spriteName}_idle_locked_001", coll));
			greenChest.AddComponent<BetterSquishyBounceWiggler>().WiggleHold = false;
			var debris = greenChest.AddComponent<DebrisObject>();
			debris.canRotate = false;
			debris.AssignFinalWorldDepth(0f);
			var nonActiveRigidbody = greenChest.AddComponent<SpeculativeRigidbody>();
			nonActiveRigidbody.enabled = false;
			nonActiveRigidbody.PixelColliders = new()
			{
				new()
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					ManualWidth = 17,
					ManualHeight = 13,
					IsTrigger = true,
					CollisionLayer = CollisionLayer.LowObstacle,
					Enabled = false
				}
			};
			var animator = greenChest.AddComponent<tk2dSpriteAnimator>();
			var animation = EasyAnimationSetup("PlaceableAnimation");
			animator.Library = animation;
			animation.AddClip(chest.idleAnim = $"small_chest_{spriteName}_idle_locked", coll);
			animation.AddClip(chest.breakAnim = $"small_chest_{spriteName}_idle_broken", coll);
			animation.AddClip(chest.unlockAnim = $"small_chest_{spriteName}_idle", coll);
			animation.AddClip(chest.openAnimName = $"small_chest_{spriteName}_open", coll, openAnimFps);
			var nonActiveBreakable = greenChest.AddComponent<MajorBreakable>();
			nonActiveBreakable.enabled = false;
			nonActiveBreakable.InvulnerableToEnemyBullets = true;
			ETGModConsole.ModdedChests.Add($"{objectName.ToLower()}_small", chest);
			return chest;
		}

		public new IEnumerator HandleSynergyGambleChest(PlayerController player)
		{
			DetermineContents(player, 0);
			spriteAnimator.Play(openAnimName);
			AkSoundEngine.PostEvent("stop_obj_fuse_loop_01", gameObject);
			if (SubAnimators.Length > 0 && SubAnimators[0])
			{
				SubAnimators[0].gameObject.SetActive(true);
				SubAnimators[0].Play("synergy_chest_open_gamble_vfx");
			}
			while (spriteAnimator.IsPlaying(openAnimName))
			{
				yield return null;
			}
			bool succeeded = false;
			for (int i = 0; i < contents.Count; i++)
			{
				PickupObject prefab = contents[i];
				bool flag = false;
				if (RewardManager.AnyPlayerHasItemInSynergyContainingOtherItem(prefab, ref flag))
				{
					succeeded = true;
					break;
				}
			}
			if (succeeded)
			{
				spriteAnimator.Play("small_chest_synergy_open");
				if (SubAnimators.Length > 0 && SubAnimators[0])
				{
					SubAnimators[0].PlayAndDisableObject("synergy_chest_open_synergy_vfx", null);
				}
				yield return new WaitForSeconds(0.42f);
			}
			else
			{
				spriteAnimator.Play("small_chest_synergy_fail_open");
				if (SubAnimators.Length > 0 && SubAnimators[0])
				{
					SubAnimators[0].PlayAndDisableObject("synergy_chest_open_fail_vfx", null);
				}
				yield return new WaitForSeconds(0.42f);
			}
			if (!m_isMimic)
			{
				StartCoroutine(PresentItem());
			}
			yield break;
		}

		public new void BecomeRainbowChest()
		{
			IsRainbowChest = true;
			lootTable.S_Chance = 0.2f;
			lootTable.A_Chance = 0.7f;
			lootTable.B_Chance = 0.4f;
			lootTable.C_Chance = 0f;
			lootTable.D_Chance = 0f;
			lootTable.Common_Chance = 0f;
			lootTable.canDropMultipleItems = true;
			lootTable.multipleItemDropChances = new();
			lootTable.multipleItemDropChances.elements = new WeightedInt[1];
			lootTable.overrideItemLootTables = new List<GenericLootTable>();
			lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
			lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
			if (GameStatsManager.Instance.IsRainbowRun)
			{
				lootTable.C_Chance = 0.2f;
				lootTable.D_Chance = 0.2f;
				lootTable.overrideItemQualities = new List<PickupObject.ItemQuality>();
				float value = UnityEngine.Random.value;
				if (value < 0.5f)
				{
					lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.S);
					lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.A);
				}
				else
				{
					lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.A);
					lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.S);
				}
			}
			WeightedInt weightedInt = new();
			weightedInt.value = 8;
			weightedInt.weight = 1f;
			weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
			lootTable.multipleItemDropChances.elements[0] = weightedInt;
			lootTable.onlyOneGunCanDrop = false;
			if (ChestIdentifier == SpecialChestIdentifier.SECRET_RAINBOW)
			{
				idleAnim = "small_chest_brown_idle_locked";
				unlockAnim = "small_chest_brown_idle";
				breakAnim = "small_chest_brown_idle_broken";
				openAnimName = "small_chest_brown_open";
			}
			else
			{
				idleAnim = "small_chest_red_idle_locked";
				unlockAnim = "small_chest_red_idle";
				breakAnim = "small_chest_red_idle_broken";
				openAnimName = "small_chest_red_open";
			}
			sprite.usesOverrideMaterial = true;
			if (ChestIdentifier != SpecialChestIdentifier.SECRET_RAINBOW)
			{
				IsLocked = false;
				sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
				spriteAnimator.Play(unlockAnim);
			}
			else
			{
				spriteAnimator.Play(idleAnim);
			}
		}

		public new void Open(PlayerController player)
		{
			if (player != null)
			{
				GetRidOfBowler();
				if (GameManager.Instance.InTutorial || AlwaysBroadcastsOpenEvent)
				{
					GameManager.BroadcastRoomTalkDoerFsmEvent("playerOpenedChest");
				}
				if (m_isGlitchChest)
				{
					if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
					{
						PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
						if (otherPlayer && otherPlayer.IsGhost)
						{
							StartCoroutine(GiveCoopPartnerBack(false));
						}
					}
					GameManager.Instance.InjectedFlowPath = "Core Game Flows/Secret_DoubleBeholster_Flow";
					Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
					GameManager.Instance.DelayedLoadNextLevel(0.5f);
					return;
				}
				if (m_isMimic && !m_IsCoopMode)
				{
					DetermineContents(player, 0);
					DoMimicTransformation(contents);
					return;
				}
				if (ChestIdentifier == SpecialChestIdentifier.SECRET_RAINBOW)
				{
					RevealSecretRainbow();
				}
				pickedUp = true;
				var squishyBounce = GetComponent<BetterSquishyBounceWiggler>();
				if (squishyBounce != null)
				{
					squishyBounce.StopBounce();
				}
				IsOpen = true;
				HandleGeneratedMagnificence();
				RoomHandler.unassignedInteractableObjects.Remove(this);
				if (lootTable.CompletesSynergy)
				{
					StartCoroutine(HandleSynergyGambleChest(player));
				}
				else
				{
					DetermineContents(player, 0);
					if (name.Contains("SmallChestRed") && contents != null && contents.Count == 1 && contents[0] && contents[0].ItemSpansBaseQualityTiers)
					{
						contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.Key));
					}
					spriteAnimator.Play((!string.IsNullOrEmpty(overrideOpenAnimName)) ? overrideOpenAnimName : openAnimName);
					AkSoundEngine.PostEvent("play_obj_chest_open_01", gameObject);
					AkSoundEngine.PostEvent("stop_obj_fuse_loop_01", gameObject);
					if (!m_isMimic)
					{
						if (SubAnimators != null && SubAnimators.Length > 0)
						{
							for (int i = 0; i < SubAnimators.Length; i++)
							{
								SubAnimators[i].Play();
							}
						}
						StartCoroutine(PresentItem());
					}
				}
			}
		}

		public new string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			if (debris.m_velocity.XY().magnitude <= maxVelocityForOpen &&
				(IsSealed || IsLockBroken || (IsLocked && ChestIdentifier != SpecialChestIdentifier.RAT && interactor.carriedConsumables.KeyBullets <= 0 && !interactor.carriedConsumables.InfiniteKeys)))
			{
				var angle = (sprite.WorldBottomCenter - interactor.sprite.WorldBottomCenter).normalized.ToAngle();
				if (angle < 0)
				{
					angle = 360 + angle;
				}
				shouldBeFlipped = angle >= 90 && angle < 270;
				if (angle < 45 || angle >= 315)
				{
					return "tablekick_right";
				}
				else if (angle >= 45 && angle < 135)
				{
					return "tablekick_up";
				}
				else if (angle >= 135 && angle < 225)
				{
					return "tablekick_right";
				}
				else
				{
					return "tablekick_down";
				}
			}
			return string.Empty;
		}

		public new IEnumerator PresentItem()
		{
			bool shouldActuallyPresent = !GameStatsManager.Instance.IsRainbowRun || IsRainbowChest || m_forceDropOkayForRainbowRun;
			if (shouldActuallyPresent)
			{
				foreach (var content in contents)
				{
					var position = sprite.WorldTopCenter;
					if (spawnTransform != null)
					{
						position = spawnTransform.position;
					}
					if (content.sprite != null)
					{
						position -= content.sprite.GetBounds().extents.XY();
					}
					LootEngine.SpawnItem(content.gameObject, position, Vector2.down, 3f, true, false, false);
				}
			}
			else
			{
				Vector2 a;
				if (spawnTransform != null)
				{
					a = spawnTransform.position;
				}
				else
				{
					Bounds bounds = sprite.GetBounds();
					a = transform.position + bounds.extents;
				}
				LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteChest, a + new Vector2(-0.5f, -2.25f), m_room, true);
			}
			yield break;
		}

		public float HeightOffGround = -0.2f;

		public new void Update()
		{
			base.Update();
			if (sprite.HeightOffGround != HeightOffGround)
			{
				sprite.HeightOffGround = HeightOffGround;
				sprite.UpdateZDepth();
			}
            if (stealable && ShouldBeTakenByRat(sprite.WorldCenter))
			{
				GameManager.Instance.Dungeon.StartCoroutine(HandleRatTheft());
			}
		}

		public IEnumerator HandleRatTheft()
		{
			PickupObject.ItemIsBeingTakenByRat = true;
			yield return new WaitForSeconds(2f);
			if (!this || IsOpen || pickedUp)
			{
				PickupObject.ItemIsBeingTakenByRat = false;
				yield break;
			}
			Vector2 spawnPos = sprite.WorldCenter + Vector2.right / -2f;
			GameObject ratInstance = Instantiate(PrefabDatabase.Instance.ResourcefulRatThief, spawnPos.ToVector3ZUp(0f), Quaternion.identity);
			ThievingRatGrabbyLilChest grabby = null;
			PlayMakerFSM fsm = ratInstance.GetComponent<PlayMakerFSM>();
			for (int i = 0; i < fsm.FsmStates.Length; i++)
			{
				for (int j = 0; j < fsm.FsmStates[i].Actions.Length; j++)
				{
					if (fsm.FsmStates[i].Actions[j] is ThievingRatGrabby)
					{
						ThievingRatGrabby oldGrabby = fsm.FsmStates[i].Actions[j] as ThievingRatGrabby;
						fsm.FsmStates[i].Actions[j] = grabby ??= new ThievingRatGrabbyLilChest
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
							notePrefab = oldGrabby.notePrefab
						};
						fsm.FsmStates[i].Actions[j].Init(fsm.FsmStates[i]);
						fsm.FsmStates[i].Actions[j].Awake();
					}
				}
			}
			for (int i = 0; i < fsm.FsmStates.Length; i++)
			{
				fsm.FsmStates[i].ActionData.SaveActions(fsm.FsmStates[i], fsm.FsmStates[i].Actions);
			}
			if (grabby != null)
			{
				grabby.TargetObject = this;
			}
			while (ratInstance)
			{
				yield return null;
			}
			PickupObject.ItemIsBeingTakenByRat = false;
			yield break;
		}

		public bool ShouldBeTakenByRat(Vector2 point)
		{
			if(IsOpen || pickedUp)
            {
				return false;
            }
			if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating)
			{
				return false;
			}
			if (!gameObject.activeSelf)
			{
				return false;
			}
			if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
			{
				return false;
			}
			if (transform.position == Vector3.zero)
			{
				return false;
			}
			if (PickupObject.ItemIsBeingTakenByRat)
			{
				return false;
			}
			if (GameManager.Instance.AllPlayers != null)
			{
				for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
				{
					if (Vector2.Distance(point, GameManager.Instance.AllPlayers[i].CenterPosition) < 10f)
					{
						return false;
					}
				}
			}
			if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
			{
				return false;
			}
			if ((GameManager.Instance.PrimaryPlayer == null || GameManager.Instance.PrimaryPlayer.healthHaver.IsDead) && (GameManager.Instance.SecondaryPlayer == null || GameManager.Instance.SecondaryPlayer.healthHaver.IsDead))
			{
				return false;
			}
			RoomHandler currentRoom = GameManager.Instance.GetPlayerClosestToPoint(point).CurrentRoom;
			RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(transform.position.IntXY(VectorConversions.Round));
			return currentRoom != absoluteRoomFromPosition;
		}

		public new void Initialize()
		{
			debris.Trigger(Vector2.zero, 0f, 1f);
			bool becomeRainbow = UnityEngine.Random.value < 0.000333f;
			if (ChestIdentifier == SpecialChestIdentifier.RAT || (lootTable != null && lootTable.CompletesSynergy))
			{
				becomeRainbow = false;
			}
			else if ((!becomeRainbow && spawnAnimName.StartsWith("wood_") && GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.WOODEN_CHESTS_BROKEN) >= 5f && UnityEngine.Random.value < 0.000333f) || forceHiddenRainbow)
			{
				becomeRainbow = true;
				ChestIdentifier = SpecialChestIdentifier.SECRET_RAINBOW;
			}
			if (IsMirrorChest)
			{
				sprite.renderer.enabled = false;
				if (LockAnimator)
				{
					LockAnimator.Sprite.renderer.enabled = false;
				}
				if (ShadowSprite)
				{
					ShadowSprite.renderer.enabled = false;
				}
			}
			else if (IsRainbowChest || becomeRainbow)
			{
				BecomeRainbowChest();
			}
			else if (forceGlitchChestSerialized || ForceGlitchChest || (
				(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId ==
				GlobalDungeonData.ValidTilesets.CASTLEGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) &&
				GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS) > 10f && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_BEHOLSTER) &&
				!GameManager.Instance.InTutorial && UnityEngine.Random.value < 0.001f))
			{
				BecomeGlitchChest();
			}
		}

		[HarmonyPatch(typeof(Chest), nameof(Spawn), typeof(Chest), typeof(Vector3), typeof(RoomHandler), typeof(bool))]
		[HarmonyPrefix]
		public static bool ReplaceSpawn(ref Chest __result, Chest chestPrefab, Vector3 basePosition, RoomHandler room)
		{
			if (chestPrefab == null)
			{
				return true;
			}
			if (chestPrefab is LilChest)
			{
				GameObject go = Instantiate(chestPrefab.gameObject, basePosition, Quaternion.identity);
				LilChest chest = go.GetComponent<LilChest>();
				//if (ForceNoMimic)
				//{
				//	component.MimicGuid = null;
				//} they cant become mimics anyways
				chest.m_room = room; //lil chests arent assigned to a singular room but its there for possible bug prevention
				chest.HandleSpawnBehavior();
				__result = chest;
				return false;
			}
			return true;
		}

		public new void HandleSpawnBehavior()
		{
			Initialize();
			RoomHandler.unassignedInteractableObjects.Add(this);
			LootEngine.DoDefaultItemPoof(sprite.WorldCenter, false, false);
		}

		public new IEnumerator SpawnBehavior_CR()
		{
			HandleSpawnBehavior();
			yield break;
		}

		public new void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			sprite.UpdateZDepth();
		}

		public new void OnExitRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
			SpriteOutlineManager.AddOutlineToSprite(sprite, BaseOutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			sprite.UpdateZDepth();
		}

		public new void Interact(PlayerController player)
		{
			if (debris.m_velocity.XY().magnitude > maxVelocityForOpen)
			{
				return;
			}
			if (IsSealed || IsLockBroken)
			{
				if (debris != null && !immobile)
				{
					debris.ApplyVelocity((sprite.WorldBottomCenter - player.sprite.WorldBottomCenter).normalized * kickForce);
				}
				AkSoundEngine.PostEvent("Play_OBJ_table_flip_01", gameObject);
				return;
			}
			if (!IsLocked)
			{
				if (!pickedUp)
				{
					Open(player);
				}
				return;
			}
			if (ChestIdentifier == SpecialChestIdentifier.RAT)
			{
				if (player.carriedConsumables.ResourcefulRatKeys > 0)
				{
					player.carriedConsumables.ResourcefulRatKeys--;
					Unlock();
					if (!pickedUp)
					{
						if (forceContentIds != null && forceContentIds.Count == 1)
						{
							for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
							{
								PlayerController playerController = GameManager.Instance.AllPlayers[i];
								if (playerController && playerController.HasPickupID(forceContentIds[0]))
								{
									forceContentIds.Clear();
									if (UnityEngine.Random.value < 0.5f)
									{
										ChestType = GeneralChestType.WEAPON;
										lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
									}
									else
									{
										ChestType = GeneralChestType.ITEM;
										lootTable.lootTable = GameManager.Instance.RewardManager.ItemsLootTable;
									}
								}
							}
						}
						Open(player);
					}
				}
				return;
			}
			if (player.carriedConsumables.KeyBullets <= 0 && !player.carriedConsumables.InfiniteKeys)
			{
				if (debris != null && !immobile)
				{
					debris.ApplyVelocity((sprite.WorldBottomCenter - player.sprite.WorldBottomCenter).normalized * kickForce);
				}
				AkSoundEngine.PostEvent("Play_OBJ_table_flip_01", gameObject);
				return;
			}
			if (!player.carriedConsumables.InfiniteKeys)
			{
				player.carriedConsumables.KeyBullets--;
			}
			GameStatsManager.Instance.RegisterStatChange(TrackedStats.CHESTS_UNLOCKED_WITH_KEY_BULLETS, 1f);
			Unlock();
			if (!pickedUp)
			{
				Open(player);
			}
		}

		public bool forceGlitchChestSerialized;
		public bool immobile;
		public float kickForce = 6f;
		public float maxVelocityForOpen;
		public string idleAnim;
		public string unlockAnim;
		public string breakAnim;
		public bool stealable;
		public bool forceHiddenRainbow;
	}
}
