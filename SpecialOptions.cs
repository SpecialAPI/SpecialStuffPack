using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using FullInspector;
using FullSerializer;
using SpecialStuffPack.Controls;
using MonoMod.RuntimeDetour;

namespace SpecialStuffPack
{
    public class SpecialOptions
    {
		public static void Setup()
        {
			new Hook(typeof(GameOptions).GetMethod("Save", BindingFlags.Public | BindingFlags.Static), typeof(SpecialOptions).GetMethod("SaveSpecialOptions", BindingFlags.Static | BindingFlags.Public));
		}

		public static bool SaveSpecialOptions(Func<bool> orig)
        {
			bool result = orig();
			if(Instance != null)
            {
				Save();
            }
			return result;
        }

		public static bool CompareSettings(SpecialOptions clone, SpecialOptions source)
		{
			if (clone == null || source == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
				clone,
				"|",
				source,
				" OPTIONS ARE NULL!"
				}));
				return false;
			}
			bool flag = true;
			foreach (FieldInfo fieldInfo in typeof(SpecialOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (fieldInfo != null)
				{
					bool flag2 = false;
					if (fieldInfo.GetCustomAttributes(typeof(fsPropertyAttribute), false).Length > 0)
					{
						flag2 = true;
					}
					if (flag2)
					{
						object value = fieldInfo.GetValue(clone);
						object value2 = fieldInfo.GetValue(source);
						if (value != null && value2 != null)
						{
							bool flag3 = value.Equals(value2);
							flag = (flag && flag3);
						}
					}
				}
			}
			return flag;
		}

		public static SpecialOptions CloneOptions(SpecialOptions source)
        {
			SpecialOptions gameOptions = new SpecialOptions();
            foreach (FieldInfo fieldInfo in typeof(SpecialOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
            {
                bool flag = false;
                if (fieldInfo.GetCustomAttributes(typeof(fsPropertyAttribute), false).Length > 0)
                {
                    flag = true;
                }
                if (flag)
                {
                    fieldInfo.SetValue(gameOptions, fieldInfo.GetValue(source));
                }
            }
            return gameOptions;
        }

		public void ApplySettings(SpecialOptions clone)
		{
			foreach (FieldInfo fieldInfo in typeof(SpecialOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				bool flag = fieldInfo.GetCustomAttributes(typeof(fsPropertyAttribute), false).Length > 0;
				if (flag && fieldInfo.GetValue(this) != fieldInfo.GetValue(clone))
				{
					fieldInfo.SetValue(this, fieldInfo.GetValue(clone));
				}
			}
			playerOneSpecialBindingData = clone.playerOneSpecialBindingData;
			playerTwoSpecialBindingData = clone.playerTwoSpecialBindingData;
			if (this == Instance)
			{
				SpecialInput.ForceLoadBindingInfoFromOptions();
			}
		}

		public static bool Save()
		{
			return SaveManager.Save(Instance, SpecialOptionsSave, 0, 0u, null);
		}

		public static void Load()
		{
			SaveManager.Init();
			bool flag = SaveManager.Load(SpecialOptionsSave, out SpecialOptions gameOptions, true, 0u, null, null);
			if (!flag)
			{
				int num = 0;
				while (num < 3 && !flag)
				{
					if (num != (int)SaveManager.CurrentSaveSlot)
					{
						gameOptions = null;
						SaveManager.SaveType optionsSave = SpecialOptionsSave;
						ref SpecialOptions obj = ref gameOptions;
						bool allowDecrypted = true;
						SaveManager.SaveSlot? overrideSaveSlot = new SaveManager.SaveSlot?((SaveManager.SaveSlot)num);
						flag = SaveManager.Load(optionsSave, out obj, allowDecrypted, 0u, null, overrideSaveSlot);
						flag &= (gameOptions != null);
					}
					num++;
				}
			}
			if (!flag || gameOptions == null)
			{
				m_instance = new SpecialOptions();
				GameOptions.RequiresLanguageReinitialization = true;
			}
			else
			{
				m_instance = gameOptions;
			}
		}

		public static SpecialOptions Instance
        {
            get
            {
				if(m_instance == null)
				{
					Load();
				}
				return m_instance;
            }
        }

		private static SpecialOptions m_instance;
		[fsProperty]
		public string playerOneSpecialBindingData;
		[fsProperty]
		public string playerTwoSpecialBindingData;
		public static SaveManager.SaveType SpecialOptionsSave = new SaveManager.SaveType
		{
			filePattern = "Slot{0}.spapistuffOptions",
			legacyFilePattern = "spapistuffOptionsSlot{0}.txt"
		};
	}
}
