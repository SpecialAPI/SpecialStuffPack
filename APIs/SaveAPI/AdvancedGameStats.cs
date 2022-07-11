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
            m_flags = new HashSet<CustomCharacterSpecificGungeonFlags>();
            stats = new Dictionary<CustomTrackedStats, float>(new CustomTrackedStatsComparer());
            maxima = new Dictionary<CustomTrackedMaximums, float>(new CustomTrackedMaximumsComparer());
        }

        public float GetStatValue(CustomTrackedStats statToCheck)
        {
            if (!stats.ContainsKey(statToCheck))
            {
                return 0f;
            }
            return stats[statToCheck];
        }

        public float GetMaximumValue(CustomTrackedMaximums maxToCheck)
        {
            if (!maxima.ContainsKey(maxToCheck))
            {
                return 0f;
            }
            return maxima[maxToCheck];
        }

        public bool GetFlag(CustomCharacterSpecificGungeonFlags flag)
        {
            if (flag == CustomCharacterSpecificGungeonFlags.NONE)
            {
                Debug.LogError("Something is attempting to get a NONE character-specific save flag!");
                return false;
            }
            return m_flags.Contains(flag);
        }

        public void SetStat(CustomTrackedStats stat, float val)
        {
            if (stats.ContainsKey(stat))
            {
                stats[stat] = val;
            }
            else
            {
                stats.Add(stat, val);
            }
        }

        public void SetMax(CustomTrackedMaximums max, float val)
        {
            if (maxima.ContainsKey(max))
            {
                maxima[max] = Mathf.Max(maxima[max], val);
            }
            else
            {
                maxima.Add(max, val);
            }
        }

        public void SetFlag(CustomCharacterSpecificGungeonFlags flag, bool value)
        {
            if (flag == CustomCharacterSpecificGungeonFlags.NONE)
            {
                Debug.LogError("Something is attempting to set a NONE character-specific save flag!");
                return;
            }
            if (value)
            {
                m_flags.Add(flag);
            }
            else
            {
                m_flags.Remove(flag);
            }
        }

        public void IncrementStat(CustomTrackedStats stat, float val)
        {
            if (stats.ContainsKey(stat))
            {
                stats[stat] = stats[stat] + val;
            }
            else
            {
                stats.Add(stat, val);
            }
        }

        public void AddStats(AdvancedGameStats otherStats)
        {
            foreach (KeyValuePair<CustomTrackedStats, float> keyValuePair in otherStats.stats)
            {
                IncrementStat(keyValuePair.Key, keyValuePair.Value);
            }
            foreach (KeyValuePair<CustomTrackedMaximums, float> keyValuePair2 in otherStats.maxima)
            {
                SetMax(keyValuePair2.Key, keyValuePair2.Value);
            }
            foreach (CustomCharacterSpecificGungeonFlags item in otherStats.m_flags)
            {
                m_flags.Add(item);
            }
        }

        public void ClearAllState()
        {
            List<CustomTrackedStats> list = new List<CustomTrackedStats>();
            foreach (KeyValuePair<CustomTrackedStats, float> keyValuePair in stats)
            {
                list.Add(keyValuePair.Key);
            }
            foreach (CustomTrackedStats key in list)
            {
                stats[key] = 0f;
            }
            List<CustomTrackedMaximums> list2 = new List<CustomTrackedMaximums>();
            foreach (KeyValuePair<CustomTrackedMaximums, float> keyValuePair2 in maxima)
            {
                list2.Add(keyValuePair2.Key);
            }
            foreach (CustomTrackedMaximums key2 in list2)
            {
                maxima[key2] = 0f;
            }
        }

        [fsProperty]
        private Dictionary<CustomTrackedStats, float> stats;
        [fsProperty]
        private Dictionary<CustomTrackedMaximums, float> maxima;
        [fsProperty]
        public HashSet<CustomCharacterSpecificGungeonFlags> m_flags;
    }
}
