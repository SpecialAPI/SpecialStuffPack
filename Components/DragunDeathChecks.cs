using SpecialStuffPack.SaveAPI;
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
            SaveAPIManager.SetCharacterSpecificFlag("BeatDragun", true);
            if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
            {
                SaveAPIManager.SetCharacterSpecificFlag("BeatBossrush", true);
            }
            else
            {
                if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT && GameManager.Instance.LastShortcutFloorLoaded == 4)
                {
                    SaveAPIManager.SetFlag("WoodenToken", true);
                }
            }
        }
    }
}
