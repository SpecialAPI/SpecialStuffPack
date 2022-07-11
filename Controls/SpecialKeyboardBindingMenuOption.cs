using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InControl;
using System.Collections;
using System.Text;
using System.CodeDom;

namespace SpecialStuffPack.Controls
{
	public class SpecialKeyboardBindingMenuOption : KeyboardBindingMenuOption
	{
		public void SpecialInitialize()
		{
			if (m_parentOptionsMenu == null)
			{
				m_parentOptionsMenu = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
			}
			if (IsControllerMode)
			{
				InitializeController();
			}
			else
			{
				InitializeKeyboard();
			}
			if (NonBindable)
			{
				CommandLabel.IsInteractive = false;
				CommandLabel.Color = new Color(0.25f, 0.25f, 0.25f);
			}
			else
			{
				CommandLabel.IsInteractive = true;
				CommandLabel.Color = new Color(0.596f, 0.596f, 0.596f, 1f);
			}
		}

		private new void InitializeController()
		{
			SpecialGungeonActions activeActions = GetBestInputInstance().SpecialInput().ActiveActions;
			PlayerAction actionFromType = activeActions.GetActionFromType(SpecialActionType);
			bool flag = false;
			string text = "-";
			bool flag2 = false;
			string text2 = "-";
			for (int i = 0; i < actionFromType.Bindings.Count; i++)
			{
				BindingSource bindingSource = actionFromType.Bindings[i];
				if (bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource)
				{
					DeviceBindingSource deviceBindingSource = bindingSource as DeviceBindingSource;
					GameOptions.ControllerSymbology currentSymbology = BraveInput.GetCurrentSymbology(FullOptionsMenuController.CurrentBindingPlayerTargetIndex);
					if (!flag)
					{
						flag = true;
						text = UIControllerButtonHelper.GetUnifiedControllerButtonTag(deviceBindingSource.Control, currentSymbology);
					}
					else if (!flag2)
					{
						text2 = UIControllerButtonHelper.GetUnifiedControllerButtonTag(deviceBindingSource.Control, currentSymbology);
						break;
					}
				}
				if (bindingSource.BindingSourceType == BindingSourceType.UnknownDeviceBindingSource)
				{
					UnknownDeviceBindingSource unknownDeviceBindingSource = bindingSource as UnknownDeviceBindingSource;
					if (!flag)
					{
						flag = true;
						text = unknownDeviceBindingSource.Control.Control.ToString();
					}
					else if (!flag2)
					{
						flag2 = true;
						text2 = unknownDeviceBindingSource.Control.Control.ToString();
					}
				}
			}
			KeyButton.Text = (string.IsNullOrEmpty(OverrideKeyString) ? text.Trim() : OverrideKeyString);
			AltKeyButton.Text = (string.IsNullOrEmpty(OverrideAltKeyString) ? text2.Trim() : OverrideAltKeyString);
			AltKeyButton.transform.position = AltKeyButton.transform.position.WithX(AltAlignLabel.GetCenter().x);
			if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ITALIAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.GERMAN)
			{
				KeyButton.Padding = new RectOffset(60, 0, 0, 0);
			}
			else if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH)
			{
				KeyButton.Padding = new RectOffset(180, 0, 0, 0);
			}
			else
			{
				KeyButton.Padding = new RectOffset(0, 0, 0, 0);
			}
			if (CenterColumnLabel)
			{
				CenterColumnLabel.Padding = KeyButton.Padding;
			}
			GetComponent<dfPanel>().PerformLayout();
			CommandLabel.RelativePosition = CommandLabel.RelativePosition.WithX(0f);
		}

		private new void InitializeKeyboard()
		{
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
			{
				BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(0);
				BraveInput instanceForPlayer2 = BraveInput.GetInstanceForPlayer(1);
				if (instanceForPlayer && instanceForPlayer2)
				{
					SpecialInput specialInstanceForPlayer = instanceForPlayer.SpecialInput();
					SpecialInput specialInstanceForPlayer2 = instanceForPlayer2.SpecialInput();
					if(specialInstanceForPlayer && specialInstanceForPlayer2)
					{
						SpecialGungeonActions activeActions = specialInstanceForPlayer.ActiveActions;
						SpecialGungeonActions activeActions2 = specialInstanceForPlayer2.ActiveActions;
						if (activeActions != null && activeActions2 != null)
						{
							PlayerAction actionFromType = activeActions.GetActionFromType(SpecialActionType);
							PlayerAction actionFromType2 = activeActions2.GetActionFromType(SpecialActionType);
							actionFromType2.ClearBindingsOfType(BindingSourceType.KeyBindingSource);
							for (int i = 0; i < actionFromType.Bindings.Count; i++)
							{
								BindingSource bindingSource = actionFromType.Bindings[i];
								if (bindingSource.BindingSourceType == BindingSourceType.KeyBindingSource && bindingSource is KeyBindingSource)
								{
									BindingSource binding = new KeyBindingSource((bindingSource as KeyBindingSource).Control);
									actionFromType2.AddBinding(binding);
								}
							}
						}
					}
				}
			}
			SpecialInput bestInputInstance = GetBestInputInstance().SpecialInput();
			SpecialGungeonActions activeActions3 = bestInputInstance.ActiveActions;
			PlayerAction actionFromType3 = activeActions3.GetActionFromType(SpecialActionType);
			bool flag = false;
			string text = "-";
			bool flag2 = false;
			string text2 = "-";
			for (int j = 0; j < actionFromType3.Bindings.Count; j++)
			{
				BindingSource bindingSource2 = actionFromType3.Bindings[j];
				if (bindingSource2.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource2.BindingSourceType == BindingSourceType.MouseBindingSource)
				{
					if (!flag)
					{
						flag = true;
						text = bindingSource2.Name;
					}
					else if (!flag2)
					{
						text2 = bindingSource2.Name;
						break;
					}
				}
			}
			KeyButton.Text = text.Trim();
			AltKeyButton.Text = text2.Trim();
			AltKeyButton.transform.position = AltKeyButton.transform.position.WithX(AltAlignLabel.GetCenter().x);
			if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ITALIAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.GERMAN)
			{
				KeyButton.Padding = new RectOffset(60, 0, 0, 0);
			}
			else if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH)
			{
				KeyButton.Padding = new RectOffset(180, 0, 0, 0);
			}
			else
			{
				KeyButton.Padding = new RectOffset(0, 0, 0, 0);
			}
			if (CenterColumnLabel)
			{
				CenterColumnLabel.Padding = KeyButton.Padding;
			}
			GetComponent<dfPanel>().PerformLayout();
			CommandLabel.RelativePosition = CommandLabel.RelativePosition.WithX(0f);
		}

		public new void KeyClicked(dfControl source, dfControlEventArgs args)
		{
			if (NonBindable)
			{
				return;
			}
			EnterAssignmentMode(false);
			GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.DoModalKeyBindingDialog(CommandLabel.Text);
			StartCoroutine(WaitForAssignmentModeToEnd());
		}

		public new void AltKeyClicked(dfControl source, dfControlEventArgs args)
		{
			if (NonBindable)
			{
				return;
			}
			EnterAssignmentMode(true);
			GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.DoModalKeyBindingDialog(CommandLabel.Text);
			StartCoroutine(WaitForAssignmentModeToEnd());
		}

		private new void Update()
		{
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				if (KeyButton.HasFocus)
				{
					SpecialGungeonActions activeActions = GetBestInputInstance().SpecialInput().ActiveActions;
					PlayerAction actionFromType = activeActions.GetActionFromType(SpecialActionType);
					if (IsControllerMode)
					{
						actionFromType.ClearSpecificBindingByType(0, new BindingSourceType[]
						{
						BindingSourceType.DeviceBindingSource
						});
						InitializeController();
					}
					else
					{
						actionFromType.ClearSpecificBindingByType(0, new BindingSourceType[]
						{
						BindingSourceType.KeyBindingSource,
						BindingSourceType.MouseBindingSource
						});
						InitializeKeyboard();
					}
				}
				else if (AltKeyButton.HasFocus)
				{
					SpecialGungeonActions activeActions2 = GetBestInputInstance().SpecialInput().ActiveActions;
					PlayerAction actionFromType2 = activeActions2.GetActionFromType(SpecialActionType);
					if (IsControllerMode)
					{
						actionFromType2.ClearSpecificBindingByType(1, new BindingSourceType[]
						{
						BindingSourceType.DeviceBindingSource
						});
						InitializeController();
					}
					else
					{
						actionFromType2.ClearSpecificBindingByType(1, new BindingSourceType[]
						{
						BindingSourceType.KeyBindingSource,
						BindingSourceType.MouseBindingSource
						});
						InitializeKeyboard();
					}
				}
			}
		}

		private new IEnumerator WaitForAssignmentModeToEnd()
		{
			SpecialGungeonActions activeActions = GetBestInputInstance().SpecialInput().ActiveActions;
			PlayerAction targetAction = activeActions.GetActionFromType(SpecialActionType);
			while (targetAction.IsListeningForBinding)
			{
				yield return null;
			}
			Initialize();
			yield break;
		}

		public new void EnterAssignmentMode(bool isAlternateKey)
		{
			SpecialGungeonActions activeActions = GetBestInputInstance().SpecialInput().ActiveActions;
			PlayerAction actionFromType = activeActions.GetActionFromType(SpecialActionType);
			BindingListenOptions bindingOptions = new BindingListenOptions();
			if (IsControllerMode)
			{
				bindingOptions.IncludeControllers = true;
				bindingOptions.IncludeNonStandardControls = true;
				bindingOptions.IncludeKeys = true;
				bindingOptions.IncludeMouseButtons = false;
				bindingOptions.IncludeMouseScrollWheel = false;
				bindingOptions.IncludeModifiersAsFirstClassKeys = false;
				bindingOptions.IncludeUnknownControllers = GameManager.Options.allowUnknownControllers;
			}
			else
			{
				bindingOptions.IncludeControllers = false;
				bindingOptions.IncludeNonStandardControls = false;
				bindingOptions.IncludeKeys = true;
				bindingOptions.IncludeMouseButtons = true;
				bindingOptions.IncludeMouseScrollWheel = true;
				bindingOptions.IncludeModifiersAsFirstClassKeys = true;
			}
			bindingOptions.MaxAllowedBindingsPerType = 2u;
			bindingOptions.OnBindingFound = delegate (PlayerAction action, BindingSource binding)
			{
				if (binding == new KeyBindingSource(new Key[]
				{
				Key.Escape
				}))
				{
					action.StopListeningForBinding();
					GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ClearModalKeyBindingDialog(null, null);
					return false;
				}
				if (binding == new KeyBindingSource(new Key[]
				{
				Key.Delete
				}))
				{
					action.StopListeningForBinding();
					GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ClearModalKeyBindingDialog(null, null);
					return false;
                }
                if (IsControllerMode && binding is KeyBindingSource)
                {
                    return false;
				}
				action.StopListeningForBinding();
				m_parentOptionsMenu.ClearBindingFromAllControls(FullOptionsMenuController.CurrentBindingPlayerTargetIndex, binding);
				action.SetBindingOfTypeByNumber(binding, binding.BindingSourceType, (!isAlternateKey) ? 0 : 1, bindingOptions.OnBindingAdded);
				GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ToggleKeyBindingDialogState(binding);
				Initialize();
				return false;
			};
			bindingOptions.OnBindingAdded = delegate (PlayerAction action, BindingSource binding)
			{
				if (FullOptionsMenuController.CurrentBindingPlayerTargetIndex == 1)
				{
					GameManager.Options.CurrentControlPresetP2 = GameOptions.ControlPreset.CUSTOM;
				}
				else
				{
					GameManager.Options.CurrentControlPreset = GameOptions.ControlPreset.CUSTOM;
				}
				BraveOptionsMenuItem[] componentsInChildren = CenterColumnLabel.Parent.Parent.Parent.GetComponentsInChildren<BraveOptionsMenuItem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].InitializeFromOptions();
					componentsInChildren[i].ForceRefreshDisplayLabel();
				}
				Initialize();
			};
			actionFromType.ListenOptions = bindingOptions;
			if (!actionFromType.IsListeningForBinding)
			{
				actionFromType.ListenForBinding();
			}
		}

		public SpecialGungeonActions.SpecialGungeonActionType SpecialActionType;
		private new FullOptionsMenuController m_parentOptionsMenu;
	}

}
