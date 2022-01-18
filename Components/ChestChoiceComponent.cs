using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class ChestChoiceComponent : MonoBehaviour
    {
        public void Setup()
        {
            ETGMod.Chest.OnPostOpen += BreakOtherChoiceChests;
        }

        public static void BreakOtherChoiceChests(Chest choice, PlayerController play)
        {
            if(choice != null && choice.GetComponent<ChestChoiceComponent>() != null && choice.GetComponent<ChestChoiceComponent>().OtherChoiceChests != null)
            {
                foreach(Chest c in choice.GetComponent<ChestChoiceComponent>().OtherChoiceChests)
                {
                    if(c != null)
                    {
                        c.BreakChestWithoutDrops();
                    }
                }
            }
        }

        public List<Chest> OtherChoiceChests;
    }
}
