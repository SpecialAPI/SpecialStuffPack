using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class OutlineAdder : BraveBehaviour
    {
        public void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(sprite, outlineColor);
        }

        public Color outlineColor;
    }
}
