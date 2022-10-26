using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace SpecialStuffPack.SaveAPI
{
    /// <summary>
    /// Class for storing game stats like tracked stats, tracked maximums and character-specific flags
    /// </summary>
    [fsObject]
    public class AdvancedGameStats
    {
        public AdvancedGameStats()
        {
            flags = new HashSet<string>();
            trackedStats = new Dictionary<string, float>();
            maximums = new Dictionary<string, float>();
        }

        public float GetStatValue(string statToCheck)
        {
            if (!trackedStats.ContainsKey(statToCheck))
            {
                return 0f;
            }
            return trackedStats[statToCheck];
        }

        public float GetMaximumValue(string maxToCheck)
        {
            if (!maximums.ContainsKey(maxToCheck))
            {
                return 0f;
            }
            return maximums[maxToCheck];
        }

        public bool GetFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {
                Debug.LogError("Something is attempting to get a NONE character-specific save flag!");
                return false;
            }
            return flags.Contains(flag);
        }

        public void SetStat(string stat, float val)
        {
            if (trackedStats.ContainsKey(stat))
            {
                trackedStats[stat] = val;
            }
            else
            {
                trackedStats.Add(stat, val);
            }
        }

        public void SetMax(string max, float val)
        {
            if (maximums.ContainsKey(max))
            {
                maximums[max] = Mathf.Max(maximums[max], val);
            }
            else
            {
                maximums.Add(max, val);
            }
        }

        public void SetFlag(string flag, bool value)
        {
            if (string.IsNullOrEmpty(flag))
            {
                Debug.LogError("Something is attempting to set a NONE character-specific save flag!");
                return;
            }
            if (value)
            {
                flags.Add(flag);
            }
            else
            {
                flags.Remove(flag);
            }
        }

        public void IncrementStat(string stat, float val)
        {
            if (trackedStats.ContainsKey(stat))
            {
                trackedStats[stat] = trackedStats[stat] + val;
            }
            else
            {
                trackedStats.Add(stat, val);
            }
        }

        public void AddStats(AdvancedGameStats otherStats)
        {
            foreach (KeyValuePair<string, float> keyValuePair in otherStats.trackedStats)
            {
                IncrementStat(keyValuePair.Key, keyValuePair.Value);
            }
            foreach (KeyValuePair<string, float> keyValuePair2 in otherStats.maximums)
            {
                SetMax(keyValuePair2.Key, keyValuePair2.Value);
            }
            foreach (string item in otherStats.flags)
            {
                flags.Add(item);
            }
        }

        public void ClearAllState()
        {
            List<string> list = new();
            foreach (KeyValuePair<string, float> keyValuePair in trackedStats)
            {
                list.Add(keyValuePair.Key);
            }
            foreach (string key in list)
            {
                trackedStats[key] = 0f;
            }
            List<string> list2 = new();
            foreach (KeyValuePair<string, float> keyValuePair2 in maximums)
            {
                list2.Add(keyValuePair2.Key);
            }
            foreach (string key2 in list2)
            {
                maximums[key2] = 0f;
            }
        }

        [fsProperty]
        private Dictionary<string, float> trackedStats;
        [fsProperty]
        private Dictionary<string, float> maximums;
        [fsProperty]
        public HashSet<string> flags;
    }
}
