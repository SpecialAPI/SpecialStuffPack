﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using UnityEngine;
using System.Reflection;

namespace SpecialStuffPack.SaveAPI
{
    public static class CustomHuntQuests
    {
        public static void DoSetup()
        {
            if (m_loaded)
            {
                return;
            }
            HuntData = (MonsterHuntData)BraveResources.Load("Monster Hunt Data", ".asset");
            huntProgressLoadedHook = new Hook(
                typeof(MonsterHuntProgress).GetMethod("OnLoaded"),
                typeof(CustomHuntQuests).GetMethod("HuntProgressLoadedHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntProgressCompleteHook = new Hook(
                typeof(MonsterHuntProgress).GetMethod("Complete"),
                typeof(CustomHuntQuests).GetMethod("HuntProgressCompleteHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntProgressQuestCompleteHook = new Hook(
                typeof(MonsterHuntProgress).GetMethod("IsQuestComplete"),
                typeof(CustomHuntQuests).GetMethod("HuntProgressQuestCompleteHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntProgressNextQuestHook = new Hook(
                typeof(MonsterHuntProgress).GetMethod("TriggerNextQuest"),
                typeof(CustomHuntQuests).GetMethod("HuntProgressNextQuestHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntProgressProcessKillHook = new Hook(
                typeof(MonsterHuntProgress).GetMethod("ProcessKill"),
                typeof(CustomHuntQuests).GetMethod("HuntProgressProcessKillHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntQuestCompleteHook = new Hook(
                typeof(MonsterHuntQuest).GetMethod("IsQuestComplete"),
                typeof(CustomHuntQuests).GetMethod("HuntQuestCompleteHook", BindingFlags.Public | BindingFlags.Static)
            );
            huntQuestUnlockRewardsHook = new Hook(
                typeof(MonsterHuntQuest).GetMethod("UnlockRewards"),
                typeof(CustomHuntQuests).GetMethod("HuntQuestUnlockRewardsHook", BindingFlags.Public | BindingFlags.Static)
            );
            m_loaded = true;
        }

        public static void Unload()
        {
            if (!m_loaded)
            {
                return;
            }
            if(addedOrderedQuests != null)
            {
                foreach(MonsterHuntQuest q in addedOrderedQuests)
                {
                    if (HuntData.OrderedQuests.Contains(q))
                    {
                        HuntData.OrderedQuests.Remove(q);
                    }
                }
                addedOrderedQuests.Clear();
                addedOrderedQuests = null;
            }
            if (addedProceduralQuests != null)
            {
                foreach (MonsterHuntQuest q in addedProceduralQuests)
                {
                    if (HuntData.ProceduralQuests.Contains(q))
                    {
                        HuntData.ProceduralQuests.Remove(q);
                    }
                }
                addedProceduralQuests.Clear();
                addedProceduralQuests = null;
            }
            if (GameStatsManager.HasInstance && GameStatsManager.Instance.huntProgress != null)
            {
                GameStatsManager.Instance.huntProgress.OnLoaded();
            }
            else
            {
                int? cachedHuntIndex = null;
                if (AdvancedGameStatsManager.HasInstance)
                {
                    cachedHuntIndex = AdvancedGameStatsManager.Instance.cachedHuntIndex;
                    AdvancedGameStatsManager.Save();
                }
                GameStatsManager.Load();
                if (cachedHuntIndex != null && AdvancedGameStatsManager.HasInstance)
                {
                    AdvancedGameStatsManager.Instance.cachedHuntIndex = cachedHuntIndex.Value;
                }
            }
            HuntData = null;
            huntProgressLoadedHook?.Dispose();
            huntProgressCompleteHook?.Dispose();
            huntProgressNextQuestHook?.Dispose();
            huntProgressProcessKillHook?.Dispose();
            huntQuestCompleteHook?.Dispose();
            huntQuestUnlockRewardsHook?.Dispose();
            huntProgressQuestCompleteHook?.Dispose();
            m_loaded = false;
        }

        public static void HuntProgressProcessKillHook(Action<MonsterHuntProgress, AIActor> orig, MonsterHuntProgress self, AIActor target)
        {
            if (self.ActiveQuest != null)
            {
                if (self.CurrentActiveMonsterHuntProgress >= self.ActiveQuest.NumberKillsRequired)
                {
                    return;
                }
                if (self.ActiveQuest is CustomHuntQuest)
                {
                    if (!(self.ActiveQuest as CustomHuntQuest).IsEnemyValid(target, self))
                    {
                        return;
                    }
                }
            }
            orig(self, target);
        }

        public static MonsterHuntQuest FindNextQuestNoProcedural()
        {
            for (int i = 0; i < HuntData.OrderedQuests.Count; i++)
            {
                if (!GameStatsManager.Instance.GetFlag(HuntData.OrderedQuests[i].QuestFlag))
                {
                    return HuntData.OrderedQuests[i];
                }
            }
            return null;
        }

        public static int HuntProgressNextQuestHook(Func<MonsterHuntProgress, int> orig, MonsterHuntProgress self)
        {
            MonsterHuntQuest advancedResult = null;
            int advancedResultIndex = 0;
            for (int i = 0; i < HuntData.OrderedQuests.Count; i++)
            {
                if (HuntData.OrderedQuests[i] != null)
                {
                    if (!HuntData.OrderedQuests[i].IsQuestComplete())
                    {
                        advancedResult = HuntData.OrderedQuests[i];
                        advancedResultIndex = i;
                        break;
                    }
                }
            }
            List<MonsterHuntQuest> origQuests = HuntData.OrderedQuests;
            List<MonsterHuntQuest> tempQuests = new List<MonsterHuntQuest>();
            for (int i = 0; i < origQuests.Count; i++)
            {
                if (origQuests[i] != null)
                {
                    if (origQuests[i].QuestFlag != GungeonFlags.NONE)
                    {
                        tempQuests.Add(origQuests[i]);
                    }
                }
            }
            HuntData.OrderedQuests = tempQuests;
            int result = orig(self);
            MonsterHuntQuest normalResult = FindNextQuestNoProcedural();
            HuntData.OrderedQuests = origQuests;
            if (self.ActiveQuest != null && normalResult != null && HuntData.OrderedQuests.IndexOf(self.ActiveQuest) != self.CurrentActiveMonsterHuntID)
            {
                self.CurrentActiveMonsterHuntID = HuntData.OrderedQuests.IndexOf(self.ActiveQuest);
            }
            if (advancedResult != null && normalResult == null)
            {
                self.ActiveQuest = advancedResult;
                self.CurrentActiveMonsterHuntID = advancedResultIndex;
                self.CurrentActiveMonsterHuntProgress = 0;
            }
            else if (advancedResult != null && normalResult != null)
            {
                if (advancedResultIndex < self.CurrentActiveMonsterHuntID)
                {
                    self.ActiveQuest = advancedResult;
                    self.CurrentActiveMonsterHuntID = advancedResultIndex;
                    self.CurrentActiveMonsterHuntProgress = 0;
                }
            }
            return result;
        }

        public static void HuntProgressCompleteHook(Action<MonsterHuntProgress> orig, MonsterHuntProgress self)
        {
            GungeonFlags cachedFlag = self.ActiveQuest.QuestFlag;
            bool cachedFlagValue = GameStatsManager.Instance.GetFlag(GungeonFlags.INTERNALDEBUG_HAS_SEEN_DEMO_TEXT);
            if (self.ActiveQuest is CustomHuntQuest)
            {
                (self.ActiveQuest as CustomHuntQuest).Complete();
                if(self.ActiveQuest.QuestFlag == GungeonFlags.NONE)
                {
                    self.ActiveQuest.QuestFlag = GungeonFlags.INTERNALDEBUG_HAS_SEEN_DEMO_TEXT;
                }
            }
            orig(self);
            GameStatsManager.Instance.SetFlag(GungeonFlags.INTERNALDEBUG_HAS_SEEN_DEMO_TEXT, cachedFlagValue);
            self.ActiveQuest.QuestFlag = cachedFlag;
        }

        public static bool HuntQuestCompleteHook(Func<MonsterHuntQuest, bool> orig, MonsterHuntQuest self)
        {
            if (self is CustomHuntQuest)
            {
                return (self as CustomHuntQuest).IsQuestComplete();
            }
            return orig(self);
        }

        public static bool HuntProgressQuestCompleteHook(Func<MonsterHuntProgress, bool> orig, MonsterHuntProgress self)
        {
            if(self.ActiveQuest is CustomHuntQuest)
            {
                return (self.ActiveQuest as CustomHuntQuest).IsQuestComplete();
            }
            return orig(self);
        }

        public static void HuntQuestUnlockRewardsHook(Action<MonsterHuntQuest> orig, MonsterHuntQuest self)
        {
            if (self is CustomHuntQuest)
            {
                (self as CustomHuntQuest).UnlockRewards();
            }
            else
            {
                orig(self);
            }
        }

        public static void HuntProgressLoadedHook(Action<MonsterHuntProgress> orig, MonsterHuntProgress self)
        {
            if (GameManager.HasInstance)
            {
                if (GameManager.Instance.platformInterface == null)
                {
                    if (PlatformInterfaceSteam.IsSteamBuild())
                    {
                        GameManager.Instance.platformInterface = new PlatformInterfaceSteam();
                    }
                    else if (PlatformInterfaceGalaxy.IsGalaxyBuild())
                    {
                        GameManager.Instance.platformInterface = new PlatformInterfaceGalaxy();
                    }
                    else
                    {
                        GameManager.Instance.platformInterface = new PlatformInterfaceGenericPC();
                    }
                }
                GameManager.Instance.platformInterface.Start();
            }
            var huntQuests = new List<GungeonFlags>
            {
                GungeonFlags.FRIFLE_MONSTERHUNT_01_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_02_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_03_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_04_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_05_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_06_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_07_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_08_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_09_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_10_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_11_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_12_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_13_COMPLETE,
                GungeonFlags.FRIFLE_MONSTERHUNT_14_COMPLETE,
                GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE
            };
            MonsterHuntQuest lastUncompletedQuest = null;
            bool cachedFlagValue = false;
            foreach (MonsterHuntQuest quest in HuntData.OrderedQuests)
            {
                if (quest != null && !quest.IsReallyCompleted())
                {
                    lastUncompletedQuest = quest;
                }
            }
            if (lastUncompletedQuest != null)
            {
                cachedFlagValue = GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE);
                GameStatsManager.Instance.SetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE, false);
            }
            if (SaveAPIManager.IsFirstLoad)
            {
                if (!AdvancedGameStatsManager.HasInstance)
                {
                    AdvancedGameStatsManager.Load();
                }
                AdvancedGameStatsManager.Instance.cachedHuntIndex = self.CurrentActiveMonsterHuntID;
            }
            else
            {
                if (!AdvancedGameStatsManager.HasInstance)
                {
                    AdvancedGameStatsManager.Load();
                }
                if (AdvancedGameStatsManager.HasInstance && self.CurrentActiveMonsterHuntID == -1 && AdvancedGameStatsManager.Instance.cachedHuntIndex != -1)
                {
                    if (GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_REWARD_GREY_MAUSER))
                    {
                        if (AdvancedGameStatsManager.Instance.cachedHuntIndex >= 0 && AdvancedGameStatsManager.Instance.cachedHuntIndex < HuntData.ProceduralQuests.Count)
                        {
                            self.CurrentActiveMonsterHuntID = AdvancedGameStatsManager.Instance.cachedHuntIndex;
                            AdvancedGameStatsManager.Instance.cachedHuntIndex = -1;
                        }
                    }
                    else if (AdvancedGameStatsManager.Instance.cachedHuntIndex >= 0 || AdvancedGameStatsManager.Instance.cachedHuntIndex < HuntData.OrderedQuests.Count)
                    {
                        self.CurrentActiveMonsterHuntID = AdvancedGameStatsManager.Instance.cachedHuntIndex;
                        AdvancedGameStatsManager.Instance.cachedHuntIndex = -1;
                    }
                }
            }
            orig(self);
            if (lastUncompletedQuest == null && !GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE))
            {
                cachedFlagValue = true;
                if (huntQuests != null)
                {
                    int num = 0;
                    for (int i = 0; i < huntQuests.Count; i++)
                    {
                        num++;
                    }
                    if (GameManager.Instance == null)
                    {
                        if (GameManager.Instance.platformInterface == null)
                        {
                            GameManager.Instance.platformInterface.SetStat(PlatformStat.FRIFLE_CORE_COMPLETED, num);
                        }
                    }
                }
            }
            if (cachedFlagValue)
            {
                GameStatsManager.Instance.m_flags.Add(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE);
            }
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to he procedural quest list
        /// </summary>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemyGuids">List of enemy guids that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <returns>The built quest</returns>
        /// <returns></returns>
        public static MonsterHuntQuest AddProceduralQuest(List<string> questIntroConversation, string targetEnemyName, List<string> targetEnemyGuids, int numberKillsRequired, JammedEnemyState requiredState = JammedEnemyState.NoCheck, 
            Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, List<string> customRewardFlags = null, List<GungeonFlags> rewardFlags = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_PROCEDURAL_" + Guid.NewGuid().ToString().ToUpper() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Enemies.Set(questTargetEnemyString, targetEnemyName);
            return AddProceduralQuest(new CustomHuntQuest() 
            {
                QuestFlag = GungeonFlags.NONE,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = new(targetEnemyGuids),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = null
            });
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to he procedural quest list
        /// </summary>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemies">List of enemies that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <returns>The built quest</returns>
        /// <returns></returns>
        public static MonsterHuntQuest AddProceduralQuest(List<string> questIntroConversation, string targetEnemyName, List<AIActor> targetEnemies, int numberKillsRequired, JammedEnemyState requiredState = JammedEnemyState.NoCheck,
            Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, List<string> customRewardFlags = null, List<GungeonFlags> rewardFlags = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_PROCEDURAL_" + Guid.NewGuid().ToString().ToUpper() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Core.Set(questTargetEnemyString, targetEnemyName);
            return AddProceduralQuest(new CustomHuntQuest()
            {
                QuestFlag = GungeonFlags.NONE,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = targetEnemies.ConvertAll(x => x.EnemyGuid),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new List<GungeonFlags>(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = null
            });
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to the quest list
        /// </summary>
        /// <param name="questFlag">The flag that will be used to check if the quest is complete and will be set to true when the quest is completed</param>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemyGuids">List of enemy guids that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <param name="index">Index to add the quest at</param>
        /// <returns>The built quest</returns>
        public static MonsterHuntQuest AddQuest(string questFlag, List<string> questIntroConversation, string targetEnemyName, List<string> targetEnemyGuids, int numberKillsRequired, List<GungeonFlags> rewardFlags = null,
            List<string> customRewardFlags = null, JammedEnemyState requiredState = JammedEnemyState.NoCheck, Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, int? index = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_" + questFlag.ToString() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Core.Set(questTargetEnemyString, targetEnemyName);
            return AddQuest(new CustomHuntQuest()
            {
                QuestFlag = GungeonFlags.NONE,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = new(targetEnemyGuids),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = questFlag
            }, index);
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to the quest list
        /// </summary>
        /// <param name="questFlag">The flag that will be used to check if the quest is complete and will be set to true when the quest is completed</param>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemyGuids">List of enemy guids that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <param name="index">Index to add the quest at</param>
        /// <returns>The built quest</returns>
        public static MonsterHuntQuest AddQuest(GungeonFlags questFlag, List<string> questIntroConversation, string targetEnemyName, List<string> targetEnemyGuids, int numberKillsRequired, List<GungeonFlags> rewardFlags = null, 
            List<string> customRewardFlags = null, JammedEnemyState requiredState = JammedEnemyState.NoCheck, Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, int? index = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_" + questFlag.ToString() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Core.Set(questTargetEnemyString, targetEnemyName);
            return AddQuest(new CustomHuntQuest()
            {
                QuestFlag = questFlag,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = new(targetEnemyGuids),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new List<GungeonFlags>(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = null
            }, index);
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to the quest list
        /// </summary>
        /// <param name="questFlag">The flag that will be used to check if the quest is complete and will be set to true when the quest is completed</param>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemies">List of enemies that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <param name="index">Index to add the quest at</param>
        /// <returns>The built quest</returns>
        public static MonsterHuntQuest AddQuest(string questFlag, List<string> questIntroConversation, string targetEnemyName, List<AIActor> targetEnemies, int numberKillsRequired, List<GungeonFlags> rewardFlags = null,
            List<string> customRewardFlags = null, JammedEnemyState requiredState = JammedEnemyState.NoCheck, Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, int? index = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_" + questFlag.ToString() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Core.Set(questTargetEnemyString, targetEnemyName);
            return AddQuest(new CustomHuntQuest()
            {
                QuestFlag = GungeonFlags.NONE,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = targetEnemies.ConvertAll(x => x.EnemyGuid),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new List<GungeonFlags>(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = questFlag
            }, index);
        }

        /// <summary>
        /// Builds a new <see cref="MonsterHuntQuest"/> and adds it to the quest list
        /// </summary>
        /// <param name="questFlag">The flag that will be used to check if the quest is complete and will be set to true when the quest is completed</param>
        /// <param name="questIntroConversation">The list of text Frifle will say when giving the player the quest</param>
        /// <param name="targetEnemyName">The name that will be used when Frifle or Grey Mauser say how much enemies are remaining</param>
        /// <param name="targetEnemies">List of enemies that can progress the quest</param>
        /// <param name="numberKillsRequired">Amount of kills that are required for the quest to be completed</param>
        /// <param name="rewardFlags">Flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="customRewardFlags">Custom flags that will be set when the player talks to Frifle or the Grey Mauser after completing the quest</param>
        /// <param name="requiredState">Enemy state that will be required for the quest to count the kill</param>
        /// <param name="validTargetCheck">Custom check function that will be used to check if a kill is valid</param>
        /// <param name="index">Index to add the quest at</param>
        /// <returns>The built quest</returns>
        public static MonsterHuntQuest AddQuest(GungeonFlags questFlag, List<string> questIntroConversation, string targetEnemyName, List<AIActor> targetEnemies, int numberKillsRequired, List<GungeonFlags> rewardFlags = null,
            List<string> customRewardFlags = null, JammedEnemyState requiredState = JammedEnemyState.NoCheck, Func<AIActor, MonsterHuntProgress, bool> validTargetCheck = null, int? index = null)
        {
            string questStringPrefix = "#CUSTOMQUEST_" + questFlag.ToString() + "_" + Guid.NewGuid().ToString().ToUpper();
            string questIntroString = questStringPrefix + "_INTRO";
            string questTargetEnemyString = questStringPrefix + "_TARGET";
            ETGMod.Databases.Strings.Core.SetComplex(questIntroString, questIntroConversation.ToArray());
            ETGMod.Databases.Strings.Enemies.Set(questTargetEnemyString, targetEnemyName);
            return AddQuest(new CustomHuntQuest()
            {
                QuestFlag = questFlag,
                QuestIntroString = questIntroString,
                TargetStringKey = questTargetEnemyString,
                ValidTargetMonsterGuids = targetEnemies.ConvertAll(x => x.EnemyGuid),
                FlagsToSetUponReward = rewardFlags != null ? new(rewardFlags) : new List<GungeonFlags>(),
                CustomFlagsToSetUponReward = customRewardFlags != null ? new(customRewardFlags) : new(),
                NumberKillsRequired = numberKillsRequired,
                RequiredEnemyState = requiredState,
                ValidTargetCheck = validTargetCheck,
                CustomQuestFlag = null
            }, index);
        }

        public static MonsterHuntQuest AddQuest(MonsterHuntQuest quest, int? index = null)
        {
            if(HuntData == null)
            {
                DoSetup();
            }
            if (index == null)
            {
                HuntData.OrderedQuests.Add(quest);
            }
            else
            {
                if (index.Value < 0)
                {
                    HuntData.OrderedQuests.Add(quest);
                }
                else
                {
                    HuntData.OrderedQuests.InsertOrAdd(index.Value, quest);
                }
            }
            if (GameStatsManager.HasInstance && GameStatsManager.Instance.huntProgress != null)
            {
                GameStatsManager.Instance.huntProgress.OnLoaded();
            }
            else
            {
                int? cachedHuntIndex = null;
                if (AdvancedGameStatsManager.HasInstance)
                {
                    cachedHuntIndex = AdvancedGameStatsManager.Instance.cachedHuntIndex;
                    AdvancedGameStatsManager.Save();
                }
                GameStatsManager.Load();
                if(cachedHuntIndex != null && AdvancedGameStatsManager.HasInstance)
                {
                    AdvancedGameStatsManager.Instance.cachedHuntIndex = cachedHuntIndex.Value;
                }
            }
            if (addedOrderedQuests == null)
            {
                addedOrderedQuests = new List<MonsterHuntQuest>();
            }
            addedOrderedQuests.Add(quest);
            return quest;
        }

        public static MonsterHuntQuest AddProceduralQuest(MonsterHuntQuest quest)
        {
            if (HuntData == null)
            {
                DoSetup();
            }
            HuntData.ProceduralQuests.Add(quest);
            if (GameStatsManager.HasInstance && GameStatsManager.Instance.huntProgress != null)
            {
                GameStatsManager.Instance.huntProgress.OnLoaded();
            }
            else
            {
                int? cachedHuntIndex = null;
                if (AdvancedGameStatsManager.HasInstance)
                {
                    cachedHuntIndex = AdvancedGameStatsManager.Instance.cachedHuntIndex;
                    AdvancedGameStatsManager.Save();
                }
                GameStatsManager.Load();
                if (cachedHuntIndex != null && AdvancedGameStatsManager.HasInstance)
                {
                    AdvancedGameStatsManager.Instance.cachedHuntIndex = cachedHuntIndex.Value;
                }
            }
            if(addedProceduralQuests == null)
            {
                addedProceduralQuests = new List<MonsterHuntQuest>();
            }
            addedProceduralQuests.Add(quest);
            return quest;
        }

        private static bool m_loaded;
        private static Hook huntProgressLoadedHook;
        private static Hook huntProgressCompleteHook;
        private static Hook huntProgressQuestCompleteHook;
        private static Hook huntProgressNextQuestHook;
        private static Hook huntProgressProcessKillHook;
        private static Hook huntQuestCompleteHook;
        private static Hook huntQuestUnlockRewardsHook;
        public static MonsterHuntData HuntData;
        public static List<MonsterHuntQuest> addedOrderedQuests;
        public static List<MonsterHuntQuest> addedProceduralQuests;
    }
}
