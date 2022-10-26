using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialStuffPack.Items;
using System.Collections;
using SpecialStuffPack.SaveAPI;

namespace SpecialStuffPack.Placeables
{
    public class SomethingSpecialPlaceable : DungeonPlaceableBehaviour, IPlayerInteractable
    {
		public IEnumerator Start()
        {
            while (Dungeon.IsGenerating)
            {
				yield return null;
			}
			m_hasAmethyst = SaveAPIManager.GetFlag("SSAmethyst");
			m_hasOpal = SaveAPIManager.GetFlag("SSOpal");
			m_hasEmerald = SaveAPIManager.GetFlag("SSEmerald");
			m_hasAquamarine = SaveAPIManager.GetFlag("SSAquamarine");
			m_hasRuby = SaveAPIManager.GetFlag("SSRuby");
            if (!m_hasAmethyst)
            {
				m_hasOpal = false;
				m_hasEmerald = false;
				m_hasAquamarine = false;
				m_hasRuby = false;
            }
			else if (!m_hasOpal)
			{
				m_hasEmerald = false;
				m_hasAquamarine = false;
				m_hasRuby = false;
			}
            else if(!m_hasEmerald)
			{
				m_hasAquamarine = false;
				m_hasRuby = false;
			}
			else if (!m_hasAquamarine)
			{
				m_hasRuby = false;
			}
			SaveAPIManager.SetFlag("SSAmethyst", m_hasAmethyst);
			SaveAPIManager.SetFlag("SSOpal", m_hasOpal);
			SaveAPIManager.SetFlag("SSEmerald", m_hasEmerald);
			SaveAPIManager.SetFlag("SSAquamarine", m_hasAquamarine);
			SaveAPIManager.SetFlag("SSRuby", m_hasRuby);
			UpdateSprite();
			bool canBeUsed = false;
			foreach (PlayerController play in GameManager.Instance.AllPlayers)
            {
                if (CanBeUsedForCurrentState(play))
                {
					canBeUsed = true;
					break;
                }
            }
            if (!canBeUsed)
            {
				GetAbsoluteParentRoom().DeregisterInteractable(this);
				specRigidbody.enabled = false;
				gameObject.SetActive(false);
			}
		}

		public void Interact(PlayerController player)
        {
			if (TextBoxManager.HasTextBox(TalkPoint))
            {
				return;
            }
			StartCoroutine(HandleConversation(player));
        }

		public IEnumerator HandleConversation(PlayerController player)
        {
			TextBoxManager.ShowStoneTablet(TalkPoint.position, TalkPoint, -1f, StringTableManager.GetLongString(ShrineDescription), true, false);
			player.SetInputOverride("diamondShrineConversation");
			yield return StartCoroutine(ProcessConversationState(player));
            if (!m_hasDiamond)
			{
				TextBoxManager.ClearTextBox(TalkPoint);
				player.ClearInputOverride("diamondShrineConversation");
			}
			yield break;
        }

		public IEnumerator ProcessConversationState(PlayerController player)
		{
			yield return null;
			bool canBeUsed = CanBeUsedForCurrentState(player);
			if (canBeUsed)
			{
				GameUIRoot.Instance.DisplayPlayerConversationOptions(player, null, StringTableManager.GetString(AcceptString).Replace("%GEM", StringTableManager.GetString(GetCurrentReplacementKey())), StringTableManager.GetString(DeclineString));
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
            if (selectedResponse == 0 && canBeUsed)
            {
                int state = GetConversationState();
                switch (state)
                {
                    case 0:
						player.RemovePassiveItem(AmethystItem.AmethystId);
                        m_hasAmethyst = true;
						SaveAPIManager.SetFlag("SSAmethyst", true);
                        break;
                    case 1:
                        player.RemovePassiveItem(OpalItem.OpalId);
                        m_hasOpal = true;
						SaveAPIManager.SetFlag("SSOpal", true);
						break;
                    case 2:
                        player.RemovePassiveItem(EmeraldItem.EmeraldId);
                        m_hasEmerald = true;
						SaveAPIManager.SetFlag("SSEmerald", true);
						break;
                    case 3:
                        player.RemovePassiveItem(AquamarineItem.AquamarineId);
                        m_hasAquamarine = true;
						SaveAPIManager.SetFlag("SSAquamarine", true);
						break;
                    case 4:
                        player.RemovePassiveItem(RubyItem.RubyId);
                        m_hasRuby = true;
						SaveAPIManager.SetFlag("SSRuby", true);
						break;
                    case 5:
						yield return null;
                        TextBoxManager.ClearTextBox(TalkPoint);
                        TextBoxManager.ShowStoneTablet(TalkPoint.position, TalkPoint, -1f, StringTableManager.GetLongString(AreYouSureString), true, false);
                        GameUIRoot.Instance.DisplayPlayerConversationOptions(player, null, StringTableManager.GetString(AcceptString).Replace("%GEM", StringTableManager.GetString(GetCurrentReplacementKey())), StringTableManager.GetString(DeclineString));
                        while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
                        {
                            yield return null;
                        }
                        if (selectedResponse == 0)
                        {
                            player.RemovePassiveItem(DiamondItem.DiamondId);
                            m_hasDiamond = true;
							UpdateSprite();
							TextBoxManager.ClearTextBox(TalkPoint);
							yield return HandleDiamondTransitionCutscene();
							yield break;
                        }
                        else
                        {
							yield break;
                        }
                }
				UpdateSprite();
				if (CanBeUsedForCurrentState(player))
				{
					yield return StartCoroutine(ProcessConversationState(player));
				}
			}
			yield break;
        }

		public IEnumerator HandleDiamondTransitionCutscene()
        {
			AkSoundEngine.PostEvent("Play_OBJ_time_bell_01", gameObject);
			AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
			CoolEffects.DoFadeFlash(sprite, 0.5f, 4f, false);
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white);
            yield return new WaitForSeconds(0.5f);
			AkSoundEngine.PostEvent("Play_MUS_Lich_Transition_01", GameManager.Instance.gameObject);
			yield return new WaitForSeconds(4f);
			Vector2 origPos = transform.position;
			Vector2 truePos = transform.position;
			float ela = 0f;
			float dura = 4f;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
				if (ela > 1f && ela < 3f)
				{
					truePos = Vector2.Lerp(origPos, origPos + new Vector2(0f, 1.5f), (ela - 1) / (dura - 2));
				}
				transform.position = truePos + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0.0625f, -0.0625f);
                yield return null;
            }
			transform.position = origPos + new Vector2(0f, 1.5f);
			float vel = 0f;
			float accel = -3f;
			while(transform.position.y > origPos.y)
            {
				vel += accel * 60 * BraveTime.DeltaTime;
				transform.position = new Vector3(transform.position.x, Mathf.Max(origPos.y, transform.position.y + vel * BraveTime.DeltaTime), transform.position.z);
				yield return null;
            }
			FindObjectOfType<EndTimesNebulaController>().BecomeActive();
			List<Renderer> rends = FindObjectsOfType<Renderer>().ToList();
			List<Renderer> affectedRends = new List<Renderer>();
			for(int i = 0; i < 35; i++)
			{
				foreach (Renderer rend in rends)
				{
					if(rend == null || rend.gameObject == null || rend.transform == null)
                    {
						continue;
                    }
                    if (affectedRends.Contains(rend))
                    {
						continue;
                    }
					if (Vector2.Distance(sprite.WorldBottomCenter, rend.transform.position/* + rend.bounds.min + rend.bounds.extents*/) > i + 0.25f)
					{
						continue;
                    }
					if (GetComponentsInChildren<Renderer>().Contains(rend))
					{
						continue;
					}
					bool isDustVFX = false;
					DustPoofVFX.ForEach(delegate (GameObject g) { if (g.GetComponentsInChildren<Renderer>().Contains(rend)) { isDustVFX = true; } });
                    if (isDustVFX)
                    {
						continue;
                    }
					foreach (PlayerController play in GameManager.Instance.AllPlayers)
					{
						if (play.GetComponentsInChildren<Renderer>().Contains(rend))
						{
							continue;
						}
					}
					DustPoofVFX.SpawnAtPosition(rend.transform.position, 0f, null, null, null, null, false, null, null, false);
                    rend.enabled = false;
					affectedRends.Add(rend);
                }
                AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", gameObject);
				AkSoundEngine.PostEvent("Play_OBJ_stone_crumble_01", gameObject);
				yield return new WaitForSeconds(0.25f);
			}
            yield break;
        }

		public string GetCurrentReplacementKey()
        {
			int state = GetConversationState();
            switch (state)
            {
				case 0:
					return AmethystReplacement;
				case 1:
					return OpalReplacement;
				case 2:
					return EmeraldReplacement;
				case 3:
					return AquamarineReplacement;
				case 4:
					return RubyReplacement;
				case 5:
					return DiamondReplacement;
			}
			return string.Empty;
        }

		public int GetConversationState()
        {
			int conversationState = 0;
			if (m_hasAmethyst)
			{
				conversationState += 1;
			}
			if (m_hasOpal)
			{
				conversationState += 1;
			}
			if (m_hasEmerald)
			{
				conversationState += 1;
			}
			if (m_hasAquamarine)
			{
				conversationState += 1;
			}
			if (m_hasRuby)
			{
				conversationState += 1;
			}
			if (m_hasDiamond)
			{
				conversationState += 1;
			}
			return conversationState;
		}

		public void UpdateSprite()
        {
			sprite.SetSprite(BaseSpriteId + GetConversationState());
        }

		public bool CanBeUsedForCurrentState(PlayerController player)
        {
			int state = GetConversationState();
			return (state == 0 && PassiveItem.IsFlagSetForCharacter(player, typeof(AmethystItem))) || (state == 1 && PassiveItem.IsFlagSetForCharacter(player, typeof(OpalItem))) || (state == 2 && PassiveItem.IsFlagSetForCharacter(player,
				typeof(EmeraldItem))) || (state == 3 && PassiveItem.IsFlagSetForCharacter(player, typeof(AquamarineItem))) || (state == 4 && PassiveItem.IsFlagSetForCharacter(player, typeof(RubyItem))) || (state == 5 &&
				PassiveItem.IsFlagSetForCharacter(player, typeof(DiamondItem)));
		}

		public bool CanBeUsed(PlayerController player)
        {
			return (PassiveItem.IsFlagSetForCharacter(player, typeof(EmeraldItem)) || m_hasEmerald) && (PassiveItem.IsFlagSetForCharacter(player, typeof(AquamarineItem)) || m_hasAquamarine) && (PassiveItem.IsFlagSetForCharacter(player, typeof(RubyItem))
				|| m_hasRuby) && (PassiveItem.IsFlagSetForCharacter(player, typeof(DiamondItem)) || m_hasDiamond);
		}
		
		public float GetDistanceToPoint(Vector2 point)
		{
			if (sprite == null)
			{
				return 100f;
			}
			Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, specRigidbody.UnitBottomLeft, specRigidbody.UnitDimensions);
			return Vector2.Distance(point, v) / 1.5f;
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white);
		}

		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		public Transform TalkPoint;
		public string ShrineDescription;
		public string DeclineString;
		public string AcceptString;
		public string AmethystReplacement;
		public string OpalReplacement;
		public string EmeraldReplacement;
		public string AquamarineReplacement;
		public string RubyReplacement;
		public string DiamondReplacement;
		public string AreYouSureString;
		public int BaseSpriteId;
		public VFXPool DustPoofVFX;
		private bool m_hasAmethyst;
		private bool m_hasOpal;
		private bool m_hasEmerald;
		private bool m_hasAquamarine;
		private bool m_hasRuby;
		private bool m_hasDiamond;
	}
}
