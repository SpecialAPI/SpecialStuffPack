using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Tools
{
    [HarmonyPatch]
    public static class CustomAmmoUtility
    {
        public static string AddCustomAmmoType(string name, string fgSpriteObject, string bgSpriteObject, string fgTexture, string bgTexture)
        {
            dfTiledSprite fgSprite = AssetBundleManager.Load<GameObject>(fgSpriteObject).SetupDfSpriteFromTexture<dfTiledSprite>(AssetBundleManager.Load<Texture2D>(fgTexture), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            dfTiledSprite bgSprite = AssetBundleManager.Load<GameObject>(bgSpriteObject).SetupDfSpriteFromTexture<dfTiledSprite>(AssetBundleManager.Load<Texture2D>(bgTexture), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            fgSprite.zindex = 7;
            bgSprite.zindex = 5;
            GameUIAmmoType uiammotype = new()
            {
                ammoBarBG = bgSprite,
                ammoBarFG = fgSprite,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = name
            };
            addedAmmoTypes.Add(uiammotype);
            if (GameUIRoot.HasInstance)
            {
                foreach (GameUIAmmoController uiammocontroller in GameUIRoot.Instance.ammoControllers)
                {
                    if (uiammocontroller.m_initialized)
                    {
                        uiammocontroller.ammoTypes = uiammocontroller.ammoTypes.AddToArray(uiammotype);
                    }
                }
            }
            return name;
        }

        [HarmonyPatch(typeof(GameUIAmmoController), nameof(GameUIAmmoController.Initialize))]
        [HarmonyPostfix]
        public static void AddMissingAmmotypes(GameUIAmmoController __instance)
        {
            __instance.ammoTypes = __instance.ammoTypes.AddRangeToArray(addedAmmoTypes);
        }

        public static List<GameUIAmmoType> addedAmmoTypes = new();
    }
}
