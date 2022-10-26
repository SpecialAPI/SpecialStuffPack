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
            if (loaded.TryGetValue($"{path}||{typeof(T)}", out var val))
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
            if (!loaded.ContainsKey($"{path}||{typeof(T)}"))
            {
                loaded.Add($"{path}||{typeof(T)}", result);
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
                                typeof(AssetBundleManager).Assembly.GetTypes().Where(x2 => x2.Name == comp.Key && x2.IsSubclassOf(typeof(Component)) && !x2.IsAbstract).FirstOrDefault();
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
                                            f.SetValue(co, ObjectInfo.DeformatValue(field.Value, go));
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
                var obj = new ObjectInfo { objectNames = new Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<string, string>>>>() };
                for (int i = 1; i < file.Count; i++)
                {
                    var line = file[i];
                    if (line.Trim() == "}" || string.IsNullOrEmpty(line.Trim()))
                    {
                        break;
                    }
                    var name = line.Trim().Substring(line.Trim().IndexOf("\"") + 1);
                    name = name.Substring(0, name.LastIndexOf("\""));
                    Dictionary<string, Dictionary<string, KeyValuePair<string, string>>> components = new Dictionary<string, Dictionary<string, KeyValuePair<string, string>>>();
                    i += 2;
                    for (; i < file.Count; i++)
                    {
                        line = file[i];
                        if (line.Trim() == "},")
                        {
                            break;
                        }
                        var compname = line.Trim().Substring(line.Trim().IndexOf("\"") + 1);
                        compname = compname.Substring(0, compname.LastIndexOf("\""));
                        Dictionary<string, KeyValuePair<string, string>> fields = new Dictionary<string, KeyValuePair<string, string>>();
                        i += 2;
                        for (; i < file.Count; i++)
                        {
                            line = file[i];
                            if (line.Trim() == "},")
                            {
                                break;
                            }
                            var fieldname = line.Trim().Substring(line.Trim().IndexOf("\"") + 1);
                            fieldname = fieldname.Substring(0, fieldname.IndexOf(":"));
                            fieldname = fieldname.Substring(0, fieldname.LastIndexOf("\""));
                            var typeAndValue = line.Trim().Substring(line.Trim().IndexOf(":") + 1).Trim();
                            typeAndValue = typeAndValue.Substring(0, typeAndValue.LastIndexOf(","));
                            var type = typeAndValue.Substring(0, typeAndValue.IndexOf("%"));
                            var value = typeAndValue.Substring(typeAndValue.IndexOf("%") + 1);
                            KeyValuePair<string, string> d = new KeyValuePair<string, string>(type, value);
                            fields.Add(fieldname, d);
                        }
                        components.Add(compname, fields);
                    }
                    obj.objectNames.Add(name, components);
                }
                return obj;
            }

            public static Type GetTypeFromAssemblies(string name)
            {
                return AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetTypes()).SelectMany(x => x).Where(x => x.FullName == name).FirstOrDefault();
            }

            public static object DeformatValue(KeyValuePair<string, string> value, GameObject serializedObject)
            {
                if (value.Value.StartsWith("ENUM^"))
                {
                    try
                    {
                        return Enum.Parse(GetTypeFromAssemblies(value.Key), value.Value.Substring(value.Value.IndexOf("^") + 1));
                    }
                    catch
                    {
                        try
                        {
                            return ETGModCompatibility.ExtendEnum(SpecialStuffModule.GUID, value.Value.Substring(value.Value.IndexOf("^") + 1), GetTypeFromAssemblies(value.Key));
                        }
                        catch
                        {
                            ETGModConsole.Log($"bad and evil enum encountered: {value.Key}, {value.Value}");
                        }
                    }
                }
                switch (value.Key)
                {
                    case "System.Int32": return int.Parse(value.Value);
                    case "System.Int64": return long.Parse(value.Value);
                    case "System.Boolean": return bool.Parse(value.Value);
                    case "System.Single": return float.Parse(value.Value);
                    case "System.String": return value;
                    case "System.Int16": return short.Parse(value.Value);
                    case "System.Double": return double.Parse(value.Value);
                    case "System.UInt16": return ushort.Parse(value.Value);
                    case "System.UInt64": return ulong.Parse(value.Value);
                    case "System.UInt32": return uint.Parse(value.Value);
                    case "System.Byte": return byte.Parse(value.Value);
                }
                var type = GetTypeFromAssemblies(value.Key);
                if (type.IsSubclassOf(typeof(Object)))
                {
                    if (value.Value.StartsWith("?CHILD_"))
                    {
                        if (type.IsSubclassOf(typeof(Component)))
                        {
                            try
                            {
                                var childidx = value.Value.Substring(value.Value.IndexOf("_") + 1);
                                childidx = childidx.Substring(value.Value.IndexOf("*"));
                                return serializedObject.GetComponentsInChildren<Transform>()[int.Parse(childidx)].GetComponents(type)[int.Parse(value.Value.Substring(value.Value.IndexOf("*") + 1))];
                            }
                            catch
                            {
                                ETGModConsole.Log($"bad and evil child component encountered: {value.Key}, {value.Value}, {type.FullName}");
                            }
                        }
                        else if (type == typeof(GameObject))
                        {
                            try
                            {
                                return serializedObject.GetComponentsInChildren<Transform>()[int.Parse(value.Value.Substring(value.Value.IndexOf("_") + 1))].gameObject;
                            }
                            catch
                            {
                                ETGModConsole.Log($"bad and evil child object encountered: {value.Key}, {value.Value}");
                            }
                        }
                    }
                    return Load<Object>(value.Value, null, null);
                }
                return JsonUtility.FromJson(value.Value, type);
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
