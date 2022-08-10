using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

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
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("SpecialStuffPack.specialeverything.objinfo"))
            {
                byte[] b = new byte[s.Length];
                s.Read(b, 0, b.Length);
                specialobjectinfo = ObjectInfo.FromFile(Encoding.UTF8.GetString(b).Split('\n').ToList());
            }
        }

        public static T Load<T>(string path, tk2dSpriteCollectionData spriteCollection = null, string shader = null) where T : Object
        {
            if(loaded.TryGetValue(path, out var val))
            {
                return (T)val;
            }
            if (path.Contains("/"))
            {
                if (!path.StartsWith("assets/"))
                {
                    path = "assets/" + path;
                }
                if (typeof(T) == typeof(GameObject) && !path.EndsWith(".prefab"))
                {
                    path += ".prefab";
                }
                else if (typeof(T) == typeof(Texture2D) && !path.EndsWith(".png"))
                {
                    path += ".png";
                }
                else if(typeof(T) == typeof(Material) && !path.EndsWith(".mat"))
                {
                    path += ".mat";
                }
                else if(typeof(T) == typeof(Shader) && !path.EndsWith(".shader"))
                {
                    path += ".shader";
                }
            }
            var result = specialeverything.LoadAsset<T>(path);
            if (!loaded.ContainsKey(path))
            {
                loaded.Add(path, result);
            }
            GameObject go = null;
            if(result is GameObject gameobj)
            {
                go = gameobj;
            }
            else if(result is Component comp)
            {
                go = comp.gameObject;
            }
            if(go != null)
            {
                go.ProcessPrefab(spriteCollection);
                if (specialobjectinfo.objectNames.ContainsKey(result.name))
                {
                    foreach(var comp in specialobjectinfo.objectNames[result.name])
                    {
                        var type = 
                                typeof(AssetBundleManager).Assembly.GetTypes().Where(x2 => x2.Name == comp.Key && x2.IsSubclassOf(typeof(Component)) && !x2.IsAbstract).FirstOrDefault() ?? 
                                typeof(PlayerController).Assembly.GetTypes().Where(x2 => x2.Name == comp.Key && x2.IsSubclassOf(typeof(Component)) && !x2.IsAbstract).FirstOrDefault();
                        if(type != null && type.IsSubclassOf(typeof(Component)) && !type.IsAbstract)
                        {
                            var co = go.AddComponent(type);
                            if(co != null)
                            {
                                var cotype = co.GetType();
                                foreach(var field in comp.Value)
                                {
                                    try
                                    {
                                        var f = cotype.GetField(field.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                        if (f != null)
                                        {
                                            f.SetValue(co, ObjectInfo.DeformatValue(field.Value));
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static GameObject ProcessPrefab(this GameObject self, tk2dSpriteCollectionData collection = null, string shader = null)
        {
            foreach(var sprite in self.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sprite != null)
                {
                    var s = sprite.sprite;
                    var go = sprite.gameObject;
                    Object.DestroyImmediate(sprite);
                    collection ??= self.SetupCollection();
                    var id = SpriteBuilder.AddSpriteToCollection(s.texture, collection, shader ?? PlayerController.DefaultShaderName);
                    collection.spriteDefinitions[id].AddOffset(Vector2.Scale(s.pivot, new(s.texture.width, s.texture.height)) / 16f);
                    tk2dSprite.AddComponent(go, collection, id);
                }
            }
            return self;
        }

        public static AssetBundle specialeverything;
        public static ObjectInfo specialobjectinfo;
        public static Dictionary<string, Object> loaded = new();

        public class ObjectInfo
        {
            //              object name        component name     field name         field type  field value
            public Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<string, string>>>> objectNames = new Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<string, string>>>>();

            public List<string> FormatFile()
            {
                var l = new List<string>();
                foreach (var kvp in objectNames)
                {
                    l.Add(kvp.Key + ":" + string.Join("|", kvp.Value.ToList().ConvertAll(x => x.Key + ":" + string.Join(",", x.Value.ToList().ConvertAll(x2 => x2.Key + ":" + x2.Value.Key + "%" + x2.Value.Value).ToArray())).ToArray()));
                }
                return l;
            }

            public static ObjectInfo FromFile(List<string> file)
            {
                var obj = new ObjectInfo();
                foreach (var line in file)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    var name = line.Substring(0, line.IndexOf(":"));
                    var everythingElse = line.Substring(line.IndexOf(":") + 1).Trim();
                    obj.objectNames.Add(name, everythingElse.Split('|').Select(x =>
                    {
                        var componentName = x.Substring(0, x.IndexOf(":"));
                        var componentFields = x.Substring(x.IndexOf(":") + 1).Trim();
                        return new KeyValuePair<string, Dictionary<string, KeyValuePair<string, string>>>(componentName, componentFields.Split(',').Select(x2 =>
                        {
                            var fieldName = x2.Substring(0, x2.IndexOf(":"));
                            var fieldValue = x2.Substring(x2.IndexOf(":") + 1).Trim();
                            return new KeyValuePair<string, KeyValuePair<string, string>>(fieldName, new KeyValuePair<string, string>(fieldValue.Substring(0, fieldValue.IndexOf("%")), fieldValue.Substring(fieldValue.IndexOf("%") + 1)));
                        }).ToDictionary());
                    }).ToDictionary());
                }
                return obj;
            }

            public static object DeformatValue(KeyValuePair<string, string> value)
            {
                switch (value.Key)
                {
                    case "System.Int32": return int.Parse(value.Value);
                    case "System.Int64": return long.Parse(value.Value);
                    case "System.Boolean": return bool.Parse(value.Value);
                    case "System.Single": return float.Parse(value.Value);
                    case "System.String": return value.Value.ToString().Replace("🐸", ",");
                    case "System.Int16": return short.Parse(value.Value);
                    case "System.Double": return double.Parse(value.Value);
                    case "System.UInt16": return ushort.Parse(value.Value);
                    case "System.UInt64": return ulong.Parse(value.Value);
                    case "System.UInt32": return uint.Parse(value.Value);
                }
                return JsonUtility.FromJson(value.Value, AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetTypes()).SelectMany(x => x).Where(x =>
                {
                    if (value.Key.Contains("."))
                    {
                        return x.Namespace == value.Key.Substring(0, value.Key.LastIndexOf(".")) && x.Name == value.Key.Substring(value.Key.LastIndexOf(".") + 1);
                    }
                    else
                    {
                        return x.Name == value.Key;
                    }
                }).FirstOrDefault());
            }

            public static string FormatValue(object value)
            {
                if (value is int || value is long || value is bool || value is float || value is string || value is short || value is double || value is ushort || value is ulong || value is uint)
                {
                    return value.ToString().Replace(",", "🐸");
                }
                else
                {
                    return JsonUtility.ToJson(value);
                }
            }
        }
    }
}
