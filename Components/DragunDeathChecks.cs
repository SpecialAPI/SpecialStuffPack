using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class DragunDeathChecks : BraveBehaviour
    {
        public void Start()
        {
            healthHaver.OnDeath += DeathChecks;
        }

        public void DeathChecks(Vector2 v)
        {
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<CharacterSpecificGungeonFlags>(SpecialStuffModule.GUID, "BeatDragun"), true);
            if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
            {
                GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<CharacterSpecificGungeonFlags>(SpecialStuffModule.GUID, "BeatBossrush"), true);
            }
            else
            {
                if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT && GameManager.Instance.LastShortcutFloorLoaded == 4)
                {
                    GameStatsManager.Instance.SetFlag(ETGModCompatibility.ExtendEnum<GungeonFlags>(SpecialStuffModule.GUID, "WoodenToken"), true);
                }
            }
        }
    }
}
