using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class BounceResetsDistance : BraveBehaviour
    {
        public void Start()
        {
            this.GetOrAddComponent<BounceProjModifier>().OnBounce += projectile.ResetDistance;
        }
    }
}
