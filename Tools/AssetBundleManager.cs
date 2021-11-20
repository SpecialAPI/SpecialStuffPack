using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public static class AssetBundleManager
    {
        public static void LoadBundle()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("SpecialStuffPack.specialeverything"))
            {
                specialeverything = AssetBundle.LoadFromStream(s);
            }
        }

        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            if (!path.StartsWith("assets/"))
            {
                path = "assets/" + path;
            }
            if(typeof(T) == typeof(GameObject) && !path.EndsWith(".prefab"))
            {
                path += ".prefab";
            }
            if(typeof(T) == typeof(Texture2D) && !path.EndsWith(".png"))
            {
                path += ".png";
            }
            return specialeverything.LoadAsset<T>(path);
        }

        public static AssetBundle specialeverything;
    }
}
