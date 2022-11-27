using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialStuffPack.CursorAPI
{
    /// <summary>
    /// The core class of CursorAPI, this class has methods to create custom cursors.
    /// </summary>
    public static class CursorMaker
    {
        /// <summary>
        /// Initializes <see cref="CursorMaker"/>, <see cref="ResourceGetter"/> and <see cref="CursorTools"/>.
        /// </summary>
        public static void Init()
        {
            if (m_initialized)
            {
                return;
            }
            addedCursorLabelTexts = new List<string>();
            addedCursorTextures = new List<Texture2D>();
            UIRootPrefab = LoadHelper.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>();
            dfManagerStartHook = new Hook(
                typeof(dfGUIManager).GetMethod("Start"),
                typeof(CursorMaker).GetMethod("DFManagerStartHook")
            );
            if(GameUIRoot.Instance != null)
            {
                GameUIRoot.Instance.gameObject.GetOrAddComponent<UIRootProcessedFlag>();
            }
            m_initialized = true;
        }

        /// <summary>
        /// Unloads <see cref="CursorMaker"/>, <see cref="ResourceGetter"/> and <see cref="CursorTools"/>.
        /// </summary>
        public static void Unload()
        {
            if (!m_initialized)
            {
                return;
            }
            if(addedCursorLabelTexts != null)
            {
                BraveOptionsMenuItem menu = UIRootPrefab.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel").GetComponent<BraveOptionsMenuItem>();
                foreach (string str in addedCursorLabelTexts)
                {
                    CursorTools.Remove(ref menu.labelOptions, str);
                }
                if (GameUIRoot.Instance != null)
                {
                    BraveOptionsMenuItem instanceMenu = GameUIRoot.Instance.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel").GetComponent<BraveOptionsMenuItem>();
                    foreach (string str in addedCursorLabelTexts)
                    {
                        CursorTools.Remove(ref instanceMenu.labelOptions, str);
                    }
                }
                addedCursorLabelTexts.Clear();
                addedCursorLabelTexts = null;
            }
            if(addedCursorTextures != null)
            {
                foreach(Texture2D texture in addedCursorTextures)
                {
                    CursorTools.Remove(ref UIRootPrefab.GetComponentInChildren<GameCursorController>().cursors, texture);
                }
                if (GameUIRoot.Instance != null)
                {
                    foreach (Texture2D texture in addedCursorTextures)
                    {
                        CursorTools.Remove(ref GameUIRoot.Instance.GetComponentInChildren<GameCursorController>().cursors, texture);
                    }
                }
                addedCursorTextures.Clear();
                addedCursorTextures = null;
            }
            if(GameUIRoot.Instance != null && GameUIRoot.Instance.GetComponent<UIRootProcessedFlag>() != null)
            {
                UnityEngine.Object.Destroy(GameUIRoot.Instance.GetComponent<UIRootProcessedFlag>());
            }
            dfManagerStartHook?.Dispose();
            UIRootPrefab = null;
            m_initialized = false;
        }

        public static void DFManagerStartHook(Action<dfGUIManager> orig, dfGUIManager manager)
        {
            orig(manager);
            GameUIRoot root = manager.GetComponent<GameUIRoot>();
            if (root != null && GameUIRoot.Instance != null && root == GameUIRoot.Instance && UIRootPrefab != null && root.name == UIRootPrefab.name && root.GetComponent<UIRootProcessedFlag>() == null)
            {
                BraveOptionsMenuItem menu = root.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel").GetComponent<BraveOptionsMenuItem>();
                BraveOptionsMenuItem prefabMenu = UIRootPrefab.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel").GetComponent<BraveOptionsMenuItem>();
                menu.labelOptions = prefabMenu.labelOptions.ToArray();
                root.GetComponentInChildren<GameCursorController>().cursors = UIRootPrefab.GetComponentInChildren<GameCursorController>().cursors.ToArray();
                root.gameObject.AddComponent<UIRootProcessedFlag>();
            }
        }

        /// <summary>
        /// Builds a cursor using <paramref name="cursorTex"/> as it's texture.
        /// </summary>
        /// <param name="cursorTex">The cursor's texture.</param>
        public static int BuildCursor(Texture2D cursorTex)
        {
            if(addedCursorTextures == null || addedCursorLabelTexts == null)
            {
                Init();
            }
            var count = UIRootPrefab.GetComponentInChildren<GameCursorController>().cursors.Length;
            CursorTools.Add(ref UIRootPrefab.GetComponentInChildren<GameCursorController>().cursors, cursorTex);
            CursorTools.Add(ref UIRootPrefab.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel").GetComponent<BraveOptionsMenuItem>().labelOptions, 
                "[sprite \"spapi_" + cursorTex.name + "\"]");
            if (GameUIRoot.Instance != null)
            {
                CursorTools.Add(ref GameUIRoot.Instance.GetComponentInChildren<GameCursorController>().cursors, cursorTex);
                CursorTools.Add(ref GameUIRoot.Instance.transform.Find("OptionsMenuPanelDave").Find("CentralOptionsPanel").Find("GameplayOptionsScrollablePanel").Find("CursorSelectorPanel")
                    .GetComponent<BraveOptionsMenuItem>().labelOptions, "[sprite \"spapi_" + cursorTex.name + "\"]");
                GameUIRoot.Instance.gameObject.GetOrAddComponent<UIRootProcessedFlag>();
            }
            addedCursorTextures.Add(cursorTex);
            addedCursorLabelTexts.Add("[sprite \"spapi_" + cursorTex.name + "\"]");
            var cursorTexForAtlas = Object.Instantiate(cursorTex);
            cursorTexForAtlas.TrimTexture();
            UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(cursorTexForAtlas, $"spapi_{cursorTex.name}");
            return count;
        }

        /// <summary>
        /// The prefab of <see cref="GameUIRoot"/>.
        /// </summary>
        public static GameUIRoot UIRootPrefab;
        private static Hook dfManagerStartHook;
        private static bool m_initialized;
        private static List<Texture2D> addedCursorTextures;
        private static List<string> addedCursorLabelTexts;
    }
}
