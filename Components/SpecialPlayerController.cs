using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class SpecialPlayerController : MonoBehaviour
    {
        public static bool AnyoneHasBeenKeyRobbed()
        {
            return AnyoneHasBeenKeyRobbed(out _);
        }

        public static bool AnyoneHasBeenKeyRobbed(out List<PlayerController> robbedPlayers)
        {
            robbedPlayers = new List<PlayerController>();
            if(GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
            {
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if(p != null && p.SpecialPlayer().HasBeenKeyRobbed)
                    {
                        robbedPlayers.Add(p);
                    }
                }
                return robbedPlayers.Count > 0;
            }
            return false;
        }

        public bool HasBeenKeyRobbed;
    }
}
