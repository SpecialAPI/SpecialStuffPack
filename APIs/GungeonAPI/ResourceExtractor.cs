using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.GungeonAPI
{
    public static class ResourceExtractor
    {
        public static byte[] ExtractEmbeddedResource(string path)
        {
            path = path.Replace("/", ".").Replace("\\", ".");
            using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            {
                if(stream == null)
                {
                    return new byte[0];
                }
                byte[] b = new byte[stream.Length];
                stream.Read(b, 0, b.Length);
                return b;
            }
        }

        public static Texture2D GetTextureFromResource(string path)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.LoadImage(ExtractEmbeddedResource(path));
            tex.filterMode = FilterMode.Point;
            string name = path.Substring(0, path.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            tex.name = name;
            return tex;
        }

        public static string BytesToString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }
    }
}
