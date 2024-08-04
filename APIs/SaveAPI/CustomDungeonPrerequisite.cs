using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.SaveAPI
{
    /// <summary>
    /// Overridable <see cref="DungeonPrerequisite"/>
    /// </summary>
    public class CustomDungeonPrerequisite : DungeonPrerequisite
    {
        /// <summary>
        /// Overridable condition checker method
        /// </summary>
        /// <returns><see langword="true"/> if all conditions are fulfilled</returns>
        public virtual new bool CheckConditionsFulfilled()
        {
            if (advancedPrerequisiteType == AdvancedPrerequisiteType.CUSTOM_CHARACTER_SPECIFIC_FLAG)
            {
                return GameStatsManager.Instance.GetCharacterSpecificFlag(characterToCheck, customCharacterSpecificFlagToCheck) == requireCustomCharacterSpecificFlag;
            }
            else
            {
                return CheckConditionsFulfilledOrig();
            }
        }

        /// <summary>
        /// Base condition checker method
        /// </summary>
        /// <returns><see langword="true"/> if all conditions are fulfilled</returns>
        public bool CheckConditionsFulfilledOrig()
        {
            EncounterDatabaseEntry encounterDatabaseEntry = null;
            if (!string.IsNullOrEmpty(encounteredObjectGuid))
            {
                encounterDatabaseEntry = EncounterDatabase.GetEntry(encounteredObjectGuid);
            }
            switch (prerequisiteType)
            {
                case PrerequisiteType.ENCOUNTER:
                    if (encounterDatabaseEntry == null && encounteredRoom == null)
                    {
                        return true;
                    }
                    if (encounterDatabaseEntry != null)
                    {
                        int num = GameStatsManager.Instance.QueryEncounterable(encounterDatabaseEntry);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return num < requiredNumberOfEncounters;
                            case PrerequisiteOperation.EQUAL_TO:
                                return num == requiredNumberOfEncounters;
                            case PrerequisiteOperation.GREATER_THAN:
                                return num > requiredNumberOfEncounters;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    else if (encounteredRoom != null)
                    {
                        int num2 = GameStatsManager.Instance.QueryRoomEncountered(encounteredRoom.GUID);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return num2 < requiredNumberOfEncounters;
                            case PrerequisiteOperation.EQUAL_TO:
                                return num2 == requiredNumberOfEncounters;
                            case PrerequisiteOperation.GREATER_THAN:
                                return num2 > requiredNumberOfEncounters;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    return false;
                case PrerequisiteType.COMPARISON:
                    {
                        float playerStatValue = GameStatsManager.Instance.GetPlayerStatValue(statToCheck);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return playerStatValue < comparisonValue;
                            case PrerequisiteOperation.EQUAL_TO:
                                return playerStatValue == comparisonValue;
                            case PrerequisiteOperation.GREATER_THAN:
                                return playerStatValue > comparisonValue;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                        break;
                    }
                case PrerequisiteType.CHARACTER:
                    {
                        PlayableCharacters playableCharacters = (PlayableCharacters)(-1);
                        if (!BraveRandom.IgnoreGenerationDifferentiator)
                        {
                            if (GameManager.Instance.PrimaryPlayer != null)
                            {
                                playableCharacters = GameManager.Instance.PrimaryPlayer.characterIdentity;
                            }
                            else if (GameManager.PlayerPrefabForNewGame != null)
                            {
                                playableCharacters = GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>().characterIdentity;
                            }
                            else if (GameManager.Instance.BestGenerationDungeonPrefab != null)
                            {
                                playableCharacters = GameManager.Instance.BestGenerationDungeonPrefab.defaultPlayerPrefab.GetComponent<PlayerController>().characterIdentity;
                            }
                        }
                        return requireCharacter == (playableCharacters == requiredCharacter);
                    }
                case PrerequisiteType.TILESET:
                    if (GameManager.Instance.BestGenerationDungeonPrefab != null)
                    {
                        return requireTileset == (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == requiredTileset);
                    }
                    return requireTileset == (GameManager.Instance.Dungeon.tileIndices.tilesetId == requiredTileset);
                case PrerequisiteType.FLAG:
                    return GameStatsManager.Instance.GetFlag(saveFlagToCheck) == requireFlag;
                case PrerequisiteType.DEMO_MODE:
                    return !requireDemoMode;
                case PrerequisiteType.MAXIMUM_COMPARISON:
                    {
                        float playerMaximum = GameStatsManager.Instance.GetPlayerMaximum(maxToCheck);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return playerMaximum < comparisonValue;
                            case PrerequisiteOperation.EQUAL_TO:
                                return playerMaximum == comparisonValue;
                            case PrerequisiteOperation.GREATER_THAN:
                                return playerMaximum > comparisonValue;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                        break;
                    }
                case PrerequisiteType.ENCOUNTER_OR_FLAG:
                    if (GameStatsManager.Instance.GetFlag(saveFlagToCheck) == requireFlag)
                    {
                        return true;
                    }
                    if (encounterDatabaseEntry != null)
                    {
                        int num3 = GameStatsManager.Instance.QueryEncounterable(encounterDatabaseEntry);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return num3 < requiredNumberOfEncounters;
                            case PrerequisiteOperation.EQUAL_TO:
                                return num3 == requiredNumberOfEncounters;
                            case PrerequisiteOperation.GREATER_THAN:
                                return num3 > requiredNumberOfEncounters;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    else if (encounteredRoom != null)
                    {
                        int num4 = GameStatsManager.Instance.QueryRoomEncountered(encounteredRoom.GUID);
                        switch (prerequisiteOperation)
                        {
                            case PrerequisiteOperation.LESS_THAN:
                                return num4 < requiredNumberOfEncounters;
                            case PrerequisiteOperation.EQUAL_TO:
                                return num4 == requiredNumberOfEncounters;
                            case PrerequisiteOperation.GREATER_THAN:
                                return num4 > requiredNumberOfEncounters;
                            default:
                                Debug.LogError("Switching on invalid stat comparison operation!");
                                break;
                        }
                    }
                    return false;
                case PrerequisiteType.NUMBER_PASTS_COMPLETED:
                    return (float)GameStatsManager.Instance.GetNumberPastsBeaten() >= comparisonValue;
                default:
                    Debug.LogError("Switching on invalid prerequisite type!!!");
                    break;
            }
            return false;
        }

        public AdvancedPrerequisiteType advancedPrerequisiteType;
        public CharacterSpecificGungeonFlags customCharacterSpecificFlagToCheck;
        public bool requireCustomCharacterSpecificFlag;
        public PlayableCharacters characterToCheck;
        public Type requiredPassiveFlag;
        public enum AdvancedPrerequisiteType
        {
            NONE,
            CUSTOM_CHARACTER_SPECIFIC_FLAG
        }
    }
}
