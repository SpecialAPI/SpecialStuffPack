using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class AlwaysUnpixelated : MonoBehaviour
    {
        public void Update()
        {
            if (gameObject.layer != LayerMask.NameToLayer("Unpixelated"))
            {
                gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            }
        }
    }
}
