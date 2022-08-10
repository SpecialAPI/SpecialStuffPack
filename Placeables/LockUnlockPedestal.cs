using Dungeonator;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialStuffPack.Enemies;

namespace SpecialStuffPack.Placeables
{
    public class LockUnlockPedestal : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public float GetDistanceToPoint(Vector2 point)
        {
            Bounds bounds = sprite.GetBounds();
            bounds.SetMinMax(bounds.min + transform.position, bounds.max + transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

		public void ConfigureOnPlacement(RoomHandler room)
		{
			this.room = room;
			room.Entered += DarkenRoom;
			room.Exited += UndarkenRoom;
			room.BecameInvisible += ObscureRoom;
		}

		public void DarkenRoom(PlayerController play)
        {
			room.BecomeTerrifyingDarkRoomNoEnemies(0f, 0.15f, 0.5f, string.Empty);
        }

		public void UndarkenRoom()
        {
			room.EndTerrifyingDarkRoom(0.1f, 0.15f, 0.5f, string.Empty);
		}

		public void ObscureRoom()
		{
			room.visibility = RoomHandler.VisibilityStatus.REOBSCURED;
			Pixelator.Instance.ProcessOcclusionChange(IntVector2.Zero, 0f, room, false);
		}

		public IEnumerator Start()
		{
			sprite = GetComponentInChildren<tk2dBaseSprite>();
			contents = PickupObjectDatabase.GetById(itemId);
			itemSprite = tk2dSprite.AddComponent(new GameObject("Display Sprite"), contents.sprite.Collection, contents.sprite.spriteId);
			itemSprite.transform.parent = spawnTransform;
			SpriteOutlineManager.AddOutlineToSprite(itemSprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
			sprite.AttachRenderer(itemSprite);
			itemSprite.HeightOffGround = 0.25f;
			itemSprite.depthUsesTrimmedBounds = true;
			itemSprite.PlaceAtPositionByAnchor(spawnTransform.position, tk2dBaseSprite.Anchor.LowerCenter);
			itemSprite.transform.position = itemSprite.transform.position.Quantize(0.0625f);
			itemSprite.usesOverrideMaterial = true;
            itemSprite.renderer.material = new Material(itemSprite.renderer.material)
            {
                shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTilted")
            };
            sprite.UpdateZDepth();
            while (Dungeon.IsGenerating)
            {
				yield return null;
            }
			if (bookPagePrefab != null && room.connectedRooms.Count > 0)
			{
				RoomHandler room2 = BraveUtility.RandomElement(room.connectedRooms);
                IntVector2 v = room2.GetRandomAvailableCell(new IntVector2(1, 1), CellTypes.FLOOR, false, null).GetValueOrDefault();
				if(GameManager.Instance?.Dungeon?.data != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(v) && GameManager.Instance.Dungeon.data[v].type == CellType.WALL)
                {
					v.y += 1;
                }
				if (GameManager.Instance?.Dungeon?.data != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(v + IntVector2.Down) && GameManager.Instance.Dungeon.data[v + IntVector2.Down].type == CellType.WALL)
				{
					v.y += 1;
				}
                GameObject bookPage = bookPagePrefab.GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(room2, v - room2.area.basePosition, false);
				foreach(IPlayerInteractable interact in bookPage.GetComponentsInChildren<IPlayerInteractable>())
                {
					room2.RegisterInteractable(interact);
                }
			}
			GameManager.Instance.Dungeon.StartCoroutine(ObscureSecretRoomOnReveal(room));
			yield break;
		}

		public static IEnumerator ObscureSecretRoomOnReveal(RoomHandler roomToCheck)
        {
			if(roomToCheck != null && roomToCheck.WasEverSecretRoom)
			{
				while (true)
				{
					if(roomToCheck == null)
                    {
						yield break;
                    }
                    if (!roomToCheck.IsSecretRoom)
                    {
						break;
                    }
					yield return null;
				}
				roomToCheck.visibility = RoomHandler.VisibilityStatus.REOBSCURED;
				Pixelator.Instance.ProcessOcclusionChange(IntVector2.Zero, 0f, roomToCheck, false);
				Transform godRay = roomToCheck.hierarchyParent.Find("GodRay_Placeable(Clone)");
				if(godRay != null)
                {
					Destroy(godRay.gameObject);
                }
			}
			yield break;
        }

        public void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			if (itemSprite != null)
			{
				SpriteOutlineManager.RemoveOutlineFromSprite(itemSprite, true);
				SpriteOutlineManager.AddOutlineToSprite(itemSprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			sprite.UpdateZDepth();
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

		public void Interact(PlayerController player)
        {
            if (!pickedUp)
            {
				room.DeregisterInteractable(this);
				if (itemSprite != null)
				{
					SpriteOutlineManager.RemoveOutlineFromSprite(itemSprite, false);
				}
				LootEngine.GivePrefabToPlayer(contents.gameObject, player);
				if (contents is Gun)
				{
					AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", base.gameObject);
				}
				else
				{
					AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", base.gameObject);
				}
				GameObject original = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Pickup");
				GameObject gameObject = Instantiate(original);
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.PlaceAtPositionByAnchor(itemSprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
				component.HeightOffGround = 6f;
				component.UpdateZDepth();
				Destroy(itemSprite);
				pickedUp = true;
				GameManager.Instance.Dungeon.StartCoroutine(HandleLockGhostCutscene(player));
			}
        }

		public IEnumerator HandleLockGhostCutscene(PlayerController player)
        {
			AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", gameObject);
			Minimap.Instance.TemporarilyPreventMinimap = true;
			Minimap.Instance.PreventAllTeleports = true;
			StatModifier speed = StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.MULTIPLICATIVE, 0.5f);
			foreach(PlayerController play in GameManager.Instance.AllPlayers)
            {
				play.ownerlessStatModifiers.Add(speed);
				play.CurrentInputState = PlayerInputState.OnlyMovement;
				play.stats.RecalculateStats(play, false, false);
            }
			while(Vector2.Distance(player.CenterPosition, sprite.WorldCenter) < 10f)
            {
				AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.DungeonMusicController.gameObject);
				yield return null;
			}
			foreach (PlayerController play in GameManager.Instance.AllPlayers)
			{
				play.CurrentInputState = PlayerInputState.AllInput;
				play.SetInputOverride("lock ghost cutscene");
			}
			Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
			Pixelator.Instance.DoFinalNonFadedLayer = true;
			GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
			GameUIRoot.Instance.HideCoreUI(string.Empty);
			CameraController mainCameraController = GameManager.Instance.MainCameraController;
			mainCameraController.SetManualControl(true, true);
			Vector3 position = sprite.WorldCenter + Vector2.up * 4;
			mainCameraController.OverridePosition = position;
			while (Vector2.Distance(mainCameraController.transform.position, mainCameraController.OverridePosition) > 0.05f)
            {
				yield return null;
            }
			GameObject ghost = Instantiate(SpecialEnemies.LockGhostPrefab, position, Quaternion.identity);
			LockGhostController ghostController = ghost.GetComponent<LockGhostController>();
			ghostController.renderer.enabled = false;
			yield return null;
			while (ghostController.isSpawning)
            {
				yield return null;
			}
			foreach (PlayerController play in GameManager.Instance.AllPlayers)
			{
				play.ownerlessStatModifiers.Remove(speed);
				play.ClearInputOverride("lock ghost cutscene");
				play.stats.RecalculateStats(play, false, false);
			}
			Pixelator.Instance.LerpToLetterbox(0.5f, 0.25f);
			Pixelator.Instance.DoFinalNonFadedLayer = false;
			GameUIRoot.Instance.ToggleLowerPanels(true, false, string.Empty);
			GameUIRoot.Instance.ShowCoreUI(string.Empty);
			GameManager.Instance.MainCameraController.SetManualControl(false, true);
			Minimap.Instance.TemporarilyPreventMinimap = false;
			Minimap.Instance.PreventAllTeleports = false;
			yield break;
        }

		public void OnExitRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			if (itemSprite != null)
			{
				SpriteOutlineManager.RemoveOutlineFromSprite(itemSprite, true);
				SpriteOutlineManager.AddOutlineToSprite(itemSprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			sprite.UpdateZDepth();
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		public int itemId;
		public GameObject bookPagePrefab;
		public Transform spawnTransform;
		private tk2dBaseSprite itemSprite;
		private PickupObject contents;
		private bool pickedUp;
		private RoomHandler room;
	}
}
