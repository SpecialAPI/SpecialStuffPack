using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeonator;
using System.Text;
using System.Collections;

namespace SpecialStuffPack.Placeables
{
    public class DiamondShrine : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
		public void Start()
        {
			sprite = GetComponentInChildren<tk2dBaseSprite>();
			sprite.UpdateZDepth();
		}

		public void ConfigureOnPlacement(RoomHandler room)
		{
			m_parentRoom = room;
			room.OptionalDoorTopDecorable = (ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject);
			if (!room.IsOnCriticalPath && room.connectedRooms.Count == 1)
			{
				room.ShouldAttemptProceduralLock = true;
				room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, UnityEngine.Random.Range(0.3f, 0.5f));
			}
			RegisterMinimapIcon();
		}

		public void RegisterMinimapIcon()
		{
			m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(m_parentRoom, MinimapIcon, false);
		}

		public void GetRidOfMinimapIcon()
		{
			if (m_instanceMinimapIcon != null)
			{
				Minimap.Instance.DeregisterRoomIcon(m_parentRoom, m_instanceMinimapIcon);
				m_instanceMinimapIcon = null;
			}
		}

		public void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			if(m_diamond != null)
            {
				SpriteOutlineManager.RemoveOutlineFromSprite(m_diamond);
            }
			SpriteOutlineManager.AddOutlineToSprite(m_diamond ?? sprite, Color.white);
			sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(m_diamond ?? sprite);
			if (m_diamond != null)
			{
				SpriteOutlineManager.AddOutlineToSprite(m_diamond, Color.black);
			}
			sprite.UpdateZDepth();
        }

		public float GetDistanceToPoint(Vector2 point)
		{
			Bounds bounds = sprite.GetBounds();
			bounds.SetMinMax(bounds.min + transform.position, bounds.max + transform.position);
			float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
			float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
			return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

		public void Interact(PlayerController player)
		{
			if (TextBoxManager.HasTextBox(TalkPoint))
			{
				return;
			}
            if (m_diamond != null)
			{
				LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(DiamondId).gameObject, player);
				AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", base.gameObject);
				GameObject original = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Pickup");
				GameObject gameObject = Instantiate(original);
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.PlaceAtPositionByAnchor(m_diamond.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
				component.HeightOffGround = 6f;
				component.UpdateZDepth();
				SpriteOutlineManager.RemoveOutlineFromSprite(m_diamond);
				Destroy(m_diamond.gameObject);
				GetRidOfMinimapIcon();
				m_parentRoom.DeregisterInteractable(this);
			}
            else
            {
				StartCoroutine(HandleShrineConversation(player));
			}
		}

		public IEnumerator HandleShrineConversation(PlayerController player)
        {
			TextBoxManager.ShowStoneTablet(TalkPoint.position, TalkPoint, -1f, StringTableManager.GetLongString(ShrineDescription), true, false);
			player.SetInputOverride("diamondShrineConversation");
			yield return null;
			bool enoughHealth = player.healthHaver.GetMaxHealth() > 1;
            if (enoughHealth)
			{
				GameUIRoot.Instance.DisplayPlayerConversationOptions(player, null, StringTableManager.GetString(AcceptString), StringTableManager.GetString(DeclineString));
			}
            else
			{
				GameUIRoot.Instance.DisplayPlayerConversationOptions(player, null, StringTableManager.GetString(DeclineString), string.Empty);
			}
			int selectedResponse;
			while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
			{
				yield return null;
			}
			TextBoxManager.ClearTextBox(TalkPoint);
			if(enoughHealth && selectedResponse == 0)
            {
				player.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, -1f));
				player.stats.RecalculateStats(player, false, false);
				yield return StartCoroutine(HandleDiamondConjure(player));
				player.ForceRefreshInteractable = true;
				GetComponentInChildren<ParticleSystem>().Stop();
			}
			player.ClearInputOverride("diamondShrineConversation");
			yield break;
        }

		public IEnumerator HandleDiamondConjure(PlayerController player)
        {
			GameObject obj = new GameObject("diamond sprite");
			obj.transform.parent = DiamondSpawnPoint;
			PickupObject po = PickupObjectDatabase.GetById(DiamondId);
			m_diamond = tk2dSprite.AddComponent(obj, po.sprite.Collection, po.sprite.spriteId);
			m_diamond.scale = Vector2.zero;
            m_diamond.PlaceAtLocalPositionByAnchor(Vector2.zero, tk2dBaseSprite.Anchor.LowerCenter);
			sprite.AttachRenderer(m_diamond);
			m_diamond.HeightOffGround = 1f;
			m_diamond.depthUsesTrimmedBounds = true;
			m_diamond.transform.position = m_diamond.transform.position.Quantize(0.0625f);
			m_diamond.usesOverrideMaterial = true;
			m_diamond.renderer.material = new Material(m_diamond.renderer.material)
			{
				shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTilted")
			};
			sprite.UpdateZDepth();
			SpriteOutlineManager.AddOutlineToSprite(m_diamond, Color.black);
			float ela = 0f;
			while(ela < DiamondConjureTime)
            {
				ela += BraveTime.DeltaTime;
				m_diamond.scale = Vector2.Lerp(Vector2.zero, Vector2.one, ela / DiamondConjureTime);
				m_diamond.PlaceAtLocalPositionByAnchor(Vector2.zero, tk2dBaseSprite.Anchor.LowerCenter);
				m_diamond.transform.position = m_diamond.transform.position.Quantize(0.0625f);
				GlobalSparksDoer.DoRadialParticleBurst(3, player.specRigidbody.HitboxPixelCollider.UnitBottomLeft, player.specRigidbody.HitboxPixelCollider.UnitTopRight, 90f, 4f, 0f, null, null, null, GlobalSparksDoer.SparksType.RED_MATTER);
				RedMatterParticleController redMatterController = GlobalSparksDoer.GetRedMatterController();
				if (redMatterController != null)
				{
					redMatterController.target.position = DiamondSpawnPoint.position;
					redMatterController.ProcessParticles();
				}
				sprite.UpdateZDepth();
				yield return null;
            }
			m_diamond.scale = Vector2.one;
			m_diamond.PlaceAtLocalPositionByAnchor(Vector2.zero, tk2dBaseSprite.Anchor.LowerCenter);
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
			sprite.UpdateZDepth();
			yield break;
        }

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		public GameObject MinimapIcon;
		public int DiamondId;
		public string ShrineDescription;
		public string AcceptString;
		public string DeclineString;
		public float DiamondConjureTime;
		public Transform DiamondSpawnPoint;
		public Transform TalkPoint;
		private RoomHandler m_parentRoom;
		private tk2dSprite m_diamond;
		private GameObject m_instanceMinimapIcon;
	}
}
