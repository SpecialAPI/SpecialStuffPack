using InControl;
using MonoMod.RuntimeDetour;
using SpecialStuffPack.ChallengeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Controls
{
    public class SpecialInput : MonoBehaviour
    {
        public static void Setup()
        {
            new Hook(typeof(BraveInput).GetMethod("CheckForActionInitialization", BindingFlags.Public | BindingFlags.Instance), typeof(SpecialInput).GetMethod("CheckForSpecialActionInitialization", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(BraveInput).GetMethod("ReassignAllControllers", BindingFlags.Public | BindingFlags.Static), typeof(SpecialInput).GetMethod("ReassignAllSpecialControllers", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(BraveInput).GetMethod("ReassignPlayerPort", BindingFlags.Public | BindingFlags.Static), typeof(SpecialInput).GetMethod("ReassignSpecialPlayerPort", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(BraveInput).GetMethod("ResetBindingsToDefaults", BindingFlags.Public | BindingFlags.Static), typeof(SpecialInput).GetMethod("ResetSpecialBindingsToDefaults", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(BraveInput).GetMethod("SaveBindingInfoToOptions", BindingFlags.Public | BindingFlags.Static), typeof(SpecialInput).GetMethod("AlsoSaveSpecialBindingInfoToOptions", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(BraveInput).GetMethod("ForceLoadBindingInfoFromOptions", BindingFlags.Public | BindingFlags.Static), typeof(SpecialInput).GetMethod("AlsoForceLoadSpecialBindingInfoFromOptions", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("InitializeKeyboardBindingsPanel", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialInput).GetMethod("AddAdditionalBindingPanels", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("ShowOptionsMenu", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialInput).GetMethod("CloneSpecialOptions", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("CloseAndApplyChanges", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialInput).GetMethod("NullCloneOptions", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("CloseAndRevertChanges", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialInput).GetMethod("ApplyCloneOptions", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("CloseAndMaybeApplyChangesWithPrompt", BindingFlags.Public | BindingFlags.Instance), typeof(SpecialInput).GetMethod("MaybeApplyChanges", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("CloseAndRevertChangesWithPrompt", BindingFlags.NonPublic | BindingFlags.Instance), typeof(SpecialInput).GetMethod("MaybeRevertChanges", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(FullOptionsMenuController).GetMethod("ClearBindingFromAllControls", BindingFlags.Public | BindingFlags.Instance), typeof(SpecialInput).GetMethod("ClearBindingFromAllSpecialControls", BindingFlags.Public | BindingFlags.Static));
            new Hook(typeof(KeyboardBindingMenuOption).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Instance), typeof(SpecialInput).GetMethod("InitializeSpecial", BindingFlags.Public | BindingFlags.Static));
            ChallengeBuilder.SetDFString("#OPTIONS_HOLD", "Hold Second Player");
        }

        public static void ClearBindingFromAllSpecialControls(Action<FullOptionsMenuController, int, BindingSource> orig, FullOptionsMenuController self, int targetPlayerIndex, BindingSource bindingSource)
        {
            orig(self, targetPlayerIndex, bindingSource);
            if (BraveInput.GetInstanceForPlayer(targetPlayerIndex) != null && BraveInput.GetInstanceForPlayer(targetPlayerIndex).SpecialInput() != null && BraveInput.GetInstanceForPlayer(targetPlayerIndex).SpecialInput().ActiveActions != null)
            {
                SpecialGungeonActions activeActions = BraveInput.GetInstanceForPlayer(targetPlayerIndex).SpecialInput().ActiveActions;
                List<KeyboardBindingMenuOption> keyboardBindingLines = (List<KeyboardBindingMenuOption>)typeof(FullOptionsMenuController).GetField("m_keyboardBindingLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
                for (int i = 0; i < keyboardBindingLines.Count; i++)
                {
                    bool flag = false;
                    if (keyboardBindingLines[i] is SpecialKeyboardBindingMenuOption)
                    {
                        SpecialGungeonActions.SpecialGungeonActionType actionType = (keyboardBindingLines[i] as SpecialKeyboardBindingMenuOption).SpecialActionType;
                        PlayerAction actionFromType = activeActions.GetActionFromType(actionType);
                        for (int j = 0; j < actionFromType.Bindings.Count; j++)
                        {
                            BindingSource bindingSource2 = actionFromType.Bindings[j];
                            if (bindingSource2 == bindingSource)
                            {
                                actionFromType.RemoveBinding(bindingSource2);
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            actionFromType.ForceUpdateVisibleBindings();
                            keyboardBindingLines[i].Initialize();
                        }
                    }
                }
            }
        }

        public static void MaybeRevertChanges(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            bool didChangeCloneOptions = false;
            object whatItWasBefore = null;
            if (self != null)
            {
                if (self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions != null)
                {
                    SaveManager.TargetSaveSlot = null;
                    SaveBindingInfoToOptions();
                    if (!SpecialOptions.CompareSettings(self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions, SpecialOptions.Instance))
                    {
                        didChangeCloneOptions = true;
                        whatItWasBefore = typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
                        typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(self, null);
                    }
                }
            }
            orig(self);
            if (didChangeCloneOptions)
            {
                typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(self, whatItWasBefore);
            }
        }

        public static void MaybeApplyChanges(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            bool didChangeCloneOptions = false;
            object whatItWasBefore = null;
            if (self != null)
            {
                if (self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions != null)
                {
                    SaveManager.TargetSaveSlot = null;
                    SaveBindingInfoToOptions();
                    if(!SpecialOptions.CompareSettings(self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions, SpecialOptions.Instance))
                    {
                        didChangeCloneOptions = true;
                        whatItWasBefore = typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
                        typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(self, null);
                        typeof(FullOptionsMenuController).GetField("m_cachedFocusedControl", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(self, dfGUIManager.ActiveControl);
                        ((dfPanel)typeof(FullOptionsMenuController).GetField("m_panel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self)).IsVisible = false;
                        GameUIRoot.Instance.DoAreYouSure("#AYS_SAVEOPTIONS", false, null);
                        self.StartCoroutine(WaitForAreYouSure(() => typeof(FullOptionsMenuController).GetMethod("CloseAndApplyChanges", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[0]), 
                            () => typeof(FullOptionsMenuController).GetMethod("CloseAndRevertChanges", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[0])));
                    }
                }
            }
            orig(self);
            if (didChangeCloneOptions)
            {
                typeof(FullOptionsMenuController).GetField("cloneOptions", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(self, whatItWasBefore);
            }
        }

        private static IEnumerator WaitForAreYouSure(Action OnYes, Action OnNo)
        {
            while (!GameUIRoot.Instance.HasSelectedAreYouSureOption())
            {
                yield return null;
            }
            if (GameUIRoot.Instance.GetAreYouSureOption())
            {
                OnYes?.Invoke();
            }
            else
            {
                OnNo?.Invoke();
            }
            yield break;
        }

        public static void ApplyCloneOptions(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            if(self != null)
            {
                if (self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions != null)
                {
                    SpecialOptions.Instance.ApplySettings(self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions);
                }
                self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions = null;
            }
            orig(self);
        }

        public static void NullCloneOptions(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            if(self != null)
            {
                self.GetOrAddComponent<SpecialOptionsCloneHolder>().specialCloneOptions = null;
            }
            SaveBindingInfoToOptions();
            SpecialOptions.Save();
            orig(self);
        }

        public static void CloneSpecialOptions(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            if(self != null && SpecialOptions.Instance != null)
            {
                SpecialOptionsCloneHolder holder = self.GetOrAddComponent<SpecialOptionsCloneHolder>();
                holder.specialCloneOptions = SpecialOptions.CloneOptions(SpecialOptions.Instance);
            }
            orig(self);
        }

        public static void AlsoForceLoadSpecialBindingInfoFromOptions(Action orig)
        {
            orig();
            ForceLoadBindingInfoFromOptions();
        }

        public static void AlsoSaveSpecialBindingInfoToOptions(Action orig)
        {
            orig();
            SaveBindingInfoToOptions();
        }

        public static void InitializeSpecial(Action<KeyboardBindingMenuOption> orig, KeyboardBindingMenuOption self)
        {
            if (self is SpecialKeyboardBindingMenuOption)
            {
                (self as SpecialKeyboardBindingMenuOption).SpecialInitialize();
            }
            else
            {
                orig(self);
            }
        }

        public static void AddAdditionalBindingPanels(Action<FullOptionsMenuController> orig, FullOptionsMenuController self)
        {
            orig(self);
            KeyboardBindingMenuOption componentInChildren = self.TabKeyboardBindings.GetComponentInChildren<KeyboardBindingMenuOption>();
            dfPanel component = componentInChildren.GetComponent<dfPanel>();
            List<KeyboardBindingMenuOption> keyboardBindingLines = (List<KeyboardBindingMenuOption>)typeof(FullOptionsMenuController).GetField("m_keyboardBindingLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
            KeyboardBindingMenuOption previousMenuOption = keyboardBindingLines[keyboardBindingLines.Count - 1];
            previousMenuOption = AddKeyboardBindingLine(component.Parent, component.gameObject, SpecialGungeonActions.SpecialGungeonActionType.HoldSecondPlayer, "#OPTIONS_HOLD", previousMenuOption, keyboardBindingLines, false);
        }

        private static KeyboardBindingMenuOption AddKeyboardBindingLine(dfControl parentPanel, GameObject prefabObject, SpecialGungeonActions.SpecialGungeonActionType actionType, string CommandStringKey, KeyboardBindingMenuOption previousMenuOption, 
            List<KeyboardBindingMenuOption> addTo, bool nonbindable = false)
        {
            dfControl dfControl = parentPanel.AddPrefab(prefabObject);
            dfControl.transform.localScale = prefabObject.transform.localScale;
            KeyboardBindingMenuOption oldComponent = dfControl.GetComponent<KeyboardBindingMenuOption>();
            SpecialKeyboardBindingMenuOption component = dfControl.AddComponent<SpecialKeyboardBindingMenuOption>();
            foreach (PropertyInfo prop in typeof(KeyboardBindingMenuOption).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        prop.SetValue(component, prop.GetValue(oldComponent, new object[0]), new object[0]);
                    }
                    catch { }
                }
            }
            foreach(FieldInfo field in typeof(KeyboardBindingMenuOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                try
                {
                    field.SetValue(component, field.GetValue(oldComponent));
                }
                catch { }
            }
            DestroyImmediate(oldComponent);
            component.SpecialActionType = actionType;
            component.CommandLabel.Text = CommandStringKey;
            component.NonBindable = nonbindable;
            component.KeyButton.GetComponent<UIKeyControls>().up = previousMenuOption.KeyButton;
            component.AltKeyButton.GetComponent<UIKeyControls>().up = previousMenuOption.AltKeyButton;
            previousMenuOption.KeyButton.GetComponent<UIKeyControls>().down = component.KeyButton;
            previousMenuOption.AltKeyButton.GetComponent<UIKeyControls>().down = component.AltKeyButton;
            component.KeyButton.Click += new MouseEventHandler(component.KeyClicked);
            component.AltKeyButton.Click += new MouseEventHandler(component.AltKeyClicked);
            component.Initialize();
            addTo.Add(component);
            return component;
        }

        public static void ResetSpecialBindingsToDefaults(Action orig)
        {
            orig();
            SpecialOptions.Instance.playerOneSpecialBindingData = string.Empty;
            SpecialOptions.Instance.playerTwoSpecialBindingData = string.Empty;
            Dictionary<int, BraveInput> instances = (Dictionary<int, BraveInput>)typeof(BraveInput).GetField("m_instances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            for (int i = 0; i < instances.Count; i++)
            {
                if(instances[i].SpecialInput() != null)
                {
                    if (instances[i].SpecialInput().m_activeSpecialGungeonActions != null)
                    {
                        instances[i].SpecialInput().m_activeSpecialGungeonActions.Destroy();
                    }
                    instances[i].SpecialInput().m_activeSpecialGungeonActions = null;
                    instances[i].CheckForActionInitialization();
                }
            }
            SaveBindingInfoToOptions();
        }

        private static void TryLoadBindings(int playerNum, SpecialGungeonActions actions)
        {
            string text;
            if (playerNum == 0)
            {
                text = SpecialOptions.Instance.playerOneSpecialBindingData;
            }
            else
            {
                if (playerNum != 1)
                {
                    return;
                }
                text = SpecialOptions.Instance.playerTwoSpecialBindingData;
            }
            if (!string.IsNullOrEmpty(text))
            {
                actions.Load(text, true);
            }
        }

        public static void SaveBindingInfoToOptions()
        {
            if (SpecialOptions.Instance == null || GameManager.Instance.PrimaryPlayer == null)
            {
                return;
            }
            Dictionary<int, BraveInput> instances = (Dictionary<int, BraveInput>)typeof(BraveInput).GetField("m_instances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            for (int i = 0; i < instances.Count; i++)
            {
                if(instances[i].SpecialInput() != null)
                {
                    if (instances[i].SpecialInput().PlayerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                    {
                        SpecialOptions.Instance.playerOneSpecialBindingData = instances[i].SpecialInput().ActiveActions.Save();
                    }
                    else
                    {
                        SpecialOptions.Instance.playerTwoSpecialBindingData = instances[i].SpecialInput().ActiveActions.Save();
                    }
                }
            }
        }

        public static void ReassignSpecialPlayerPort(Action<int, int> orig, int playerID, int portNum)
        {
            Dictionary<int, BraveInput> instances = (Dictionary<int, BraveInput>)typeof(BraveInput).GetField("m_instances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            for (int i = 0; i < instances.Count; i++)
            {
                if(instances[i].SpecialInput() != null)
                {
                    if (instances[i].SpecialInput().m_activeSpecialGungeonActions != null)
                    {
                        instances[i].SpecialInput().m_activeSpecialGungeonActions.Destroy();
                    }
                    instances[i].SpecialInput().m_activeSpecialGungeonActions = null;
                }
            }
            orig(playerID, portNum);
        }

        public static void ReassignAllSpecialControllers(Action<InputDevice> orig, InputDevice overrideLastActiveDevice)
        {
            Dictionary<int, BraveInput> instances = (Dictionary<int, BraveInput>)typeof(BraveInput).GetField("m_instances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            for (int j = 0; j < instances.Count; j++)
            {
                if (instances[j].SpecialInput() != null)
                {
                    if (instances[j].SpecialInput().m_activeSpecialGungeonActions != null)
                    {
                        instances[j].SpecialInput().m_activeSpecialGungeonActions.Destroy();
                    }
                    instances[j].SpecialInput().m_activeSpecialGungeonActions = null;
                }
            }
            orig(overrideLastActiveDevice);
            for (int k = 0; k < instances.Count; k++)
            {
                if(instances[k].SpecialInput() != null)
                {
                    if (instances[k].SpecialInput().m_activeSpecialGungeonActions == null)
                    {
                        instances[k].SpecialInput().m_activeSpecialGungeonActions = new SpecialGungeonActions();
                        instances[k].SpecialInput().AssignActionsDevice();
                        instances[k].SpecialInput().m_activeSpecialGungeonActions.InitializeDefaults();
                        if ((GameManager.Instance.PrimaryPlayer == null && instances[k].SpecialInput().PlayerId == 0) || instances[k].SpecialInput().PlayerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                        {
                            TryLoadBindings(0, instances[k].SpecialInput().ActiveActions);
                        }
                        else
                        {
                            TryLoadBindings(1, instances[k].SpecialInput().ActiveActions);
                        }
                    }
                    instances[k].SpecialInput().AssignActionsDevice();
                }
            }
            for (int l = 0; l < instances.Count; l++)
            {
                if (GameManager.Instance.AllPlayers.Length > 1)
                {
                    if(instances[l].SpecialInput() != null)
                    {
                        if (instances[l].SpecialInput().m_activeSpecialGungeonActions.Device == null)
                        {
                            instances[l].SpecialInput().m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.DeviceBindingSource);
                        }
                        else if (instances[l].SpecialInput().PlayerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                        {
                            if (BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput() != null && 
                                BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput().m_activeSpecialGungeonActions.Device == null)
                            {
                                instances[l].SpecialInput().m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                                instances[l].SpecialInput().m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
                            }
                        }
                        else
                        {
                            instances[l].SpecialInput().m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                            instances[l].SpecialInput().m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
                        }
                    }
                }
            }
        }

        public static void ForceLoadBindingInfoFromOptions()
        {
            if (SpecialOptions.Instance == null)
            {
                return;
            }
            Dictionary<int, BraveInput> instances = (Dictionary<int, BraveInput>)typeof(BraveInput).GetField("m_instances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            for (int i = 0; i < instances.Count; i++)
            {
                if(instances[i].SpecialInput() != null)
                {
                    if (GameManager.PreventGameManagerExistence || GameManager.Instance.PrimaryPlayer == null)
                    {
                        if (instances[i].SpecialInput().PlayerId == 0)
                        {
                            TryLoadBindings(0, instances[i].SpecialInput().ActiveActions);
                        }
                    }
                    else if (instances[i].SpecialInput().PlayerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                    {
                        TryLoadBindings(0, instances[i].SpecialInput().ActiveActions);
                    }
                    else
                    {
                        TryLoadBindings(1, instances[i].SpecialInput().ActiveActions);
                    }
                }
            }
        }

        public static void CheckForSpecialActionInitialization(Action<BraveInput> orig, BraveInput self)
        {
            orig(self);
            if(self != null && self.SpecialInput() != null)
            {
                SpecialInput input = self.SpecialInput();
                if (input.m_activeSpecialGungeonActions == null)
                {
                    input.m_activeSpecialGungeonActions = new SpecialGungeonActions();
                    input.AssignActionsDevice();
                    input.m_activeSpecialGungeonActions.InitializeDefaults();
                    int playerId = input.PlayerId;
                    if (GameManager.PreventGameManagerExistence || (GameManager.Instance.PrimaryPlayer == null && input.PlayerId == 0) || input.PlayerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                    {
                        TryLoadBindings(0, input.ActiveActions);
                    }
                    else
                    {
                        TryLoadBindings(1, input.ActiveActions);
                    }
                    if (!GameManager.PreventGameManagerExistence && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                    {
                        if (playerId == 0 && BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput() != null &&
                            BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput().m_activeSpecialGungeonActions == null)
                        {
                            BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).CheckForActionInitialization();
                        }
                        if (input.m_activeSpecialGungeonActions.Device == null)
                        {
                            input.m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.DeviceBindingSource);
                        }
                        else if (playerId == GameManager.Instance.PrimaryPlayer.PlayerIDX)
                        {
                            if (BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput() != null &&
                                BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).SpecialInput().m_activeSpecialGungeonActions.Device == null)
                            {
                                input.m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                                input.m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
                            }
                        }
                        else
                        {
                            input.m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                            input.m_activeSpecialGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
                        }
                    }
                }
                input.AssignActionsDevice();
            }
        }

        public void OnDestroy()
        {
            if (m_activeSpecialGungeonActions != null)
            {
                m_activeSpecialGungeonActions.Destroy();
                m_activeSpecialGungeonActions = null;
            }
        }

        private void AssignActionsDevice()
        {
            if (GameManager.PreventGameManagerExistence || GameManager.Instance.AllPlayers.Length < 2)
            {
                m_activeSpecialGungeonActions.Device = InputManager.ActiveDevice;
            }
            else
            {
                m_activeSpecialGungeonActions.Device = InputManager.GetActiveDeviceForPlayer(PlayerId);
                if (PlayerId != 0 && m_activeSpecialGungeonActions.Device == InputManager.GetActiveDeviceForPlayer(0))
                {
                    m_activeSpecialGungeonActions.ForceDisable = true;
                }
            }
        }

        public int PlayerId
        {
            get
            {
                if(this == null || braveInput == null)
                {
                    return 0;
                }
                return (int)typeof(BraveInput).GetField("m_playerID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(braveInput);
            }
        }

        public BraveInput braveInput
        {
            get
            {
                if(this == null)
                {
                    return null;
                }
                return GetComponent<BraveInput>();
            }
        }

        public SpecialGungeonActions ActiveActions
        {
            get
            {
                return m_activeSpecialGungeonActions;
            }
        }

        private SpecialGungeonActions m_activeSpecialGungeonActions;
    }
}
