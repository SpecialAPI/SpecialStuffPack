using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
	public class LoadHelper
	{
		public static UnityEngine.Object LoadAssetFromAnywhere(string path)
		{
			foreach (string name in BundlePrereqs)
			{
				try
				{
					var asset = ResourceManager.LoadAssetBundle(name).LoadAsset(path);
					if(asset == null)
                    {
						continue;
                    }
					return asset;
				}
				catch
				{
				}
			}
			var backupObject = AssetBundleManager.specialeverything.LoadAsset<UnityEngine.Object>(path);
			if (backupObject == null)
            {
				objectsBackup.TryGetValue(path, out backupObject);
			}
			return backupObject;
		}

		public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
		{
			foreach (string name in BundlePrereqs)
			{
				try
				{
					var asset = ResourceManager.LoadAssetBundle(name).LoadAsset<T>(path);
					if(asset == null)
                    {
						continue;
                    }
					return asset;
				}
				catch
				{
				}
			}
			T obj = AssetBundleManager.specialeverything.LoadAsset<T>(path);
			if (obj == null)
			{
				if(objectsBackup.TryGetValue(path, out var backupObject) && backupObject is T d)
                {
					return d;
                }
			}
			return obj;
		}

		public static List<T> Find<T>(string toFind) where T : UnityEngine.Object
		{
			List<T> objects = new List<T>();
			foreach (string name in BundlePrereqs)
			{
				try
				{
					foreach (string str in ResourceManager.LoadAssetBundle(name).GetAllAssetNames())
					{
						if (str.ToLower().Contains(toFind))
						{
							if (ResourceManager.LoadAssetBundle(name).LoadAsset(str).GetType() == typeof(T) && !objects.Contains(ResourceManager.LoadAssetBundle(name).LoadAsset<T>(str)))
							{
								objects.Add(ResourceManager.LoadAssetBundle(name).LoadAsset<T>(str));
							}
						}
					}
				}
				catch
				{
				}
			}
			return objects;
		}

		public static List<UnityEngine.Object> Find(string toFind)
		{
			List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
			foreach (string name in BundlePrereqs)
			{
				try
				{
					foreach (string str in ResourceManager.LoadAssetBundle(name).GetAllAssetNames())
					{
						if (str.ToLower().Contains(toFind))
						{
							if (!objects.Contains(ResourceManager.LoadAssetBundle(name).LoadAsset(str)))
							{
								objects.Add(ResourceManager.LoadAssetBundle(name).LoadAsset(str));
							}
						}
					}
				}
				catch
				{
				}
			}
			return objects;
		}

		static LoadHelper()
		{
			BundlePrereqs = new string[]
			{
				"brave_resources_001",
				"dungeon_scene_001",
				"encounters_base_001",
				"enemies_base_001",
				"flows_base_001",
				"foyer_001",
				"foyer_002",
				"foyer_003",
				"shared_auto_001",
				"shared_auto_002",
				"shared_base_001",
				"dungeons/base_bullethell",
				"dungeons/base_castle",
				"dungeons/base_catacombs",
				"dungeons/base_cathedral",
				"dungeons/base_forge",
				"dungeons/base_foyer",
				"dungeons/base_gungeon",
				"dungeons/base_mines",
				"dungeons/base_nakatomi",
				"dungeons/base_resourcefulrat",
				"dungeons/base_sewer",
				"dungeons/base_tutorial",
				"dungeons/finalscenario_bullet",
				"dungeons/finalscenario_convict",
				"dungeons/finalscenario_coop",
				"dungeons/finalscenario_guide",
				"dungeons/finalscenario_pilot",
				"dungeons/finalscenario_robot",
				"dungeons/finalscenario_soldier"
			};
		}

		private static string[] BundlePrereqs;
		public static Dictionary<string, Object> objectsBackup = new();
	}
}
