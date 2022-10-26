using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.SaveAPI
{
    public class SpecialAIActor : MonoBehaviour
    {
        public bool SetsCustomFlagOnActivation;
        public string CustomFlagToSetOnActivation;
        public bool SetsCustomFlagOnDeath;
        public string CustomFlagToSetOnDeath;
        public bool SetsCustomCharacterSpecificFlagOnDeath;
        public string CustomCharacterSpecificFlagToSetOnDeath;
    }
}
