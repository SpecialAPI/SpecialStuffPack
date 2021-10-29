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
            if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
            {
                switch (GameManager.Instance.PrimaryPlayer.characterIdentity)
                {
                    case PlayableCharacters.Convict:
                        SaveAPIManager.SetFlag(CustomDungeonFlags.ITEMSPECIFIC_CONVICTS_SHACKLES, true);
                        break;
                    case PlayableCharacters.Soldier:
                        SaveAPIManager.SetFlag(CustomDungeonFlags.ITEMSPECIFIC_MARINES_HELMET, true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT && GameManager.Instance.LastShortcutFloorLoaded == 4)
                {
                    SaveAPIManager.SetFlag(CustomDungeonFlags.ITEMSPECIFIC_WOODEN_TOKEN, true);
                }
            }
        }
    }
}
