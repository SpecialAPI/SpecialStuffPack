using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ExcludeGunInBlessedMode : GunBehaviour
    {
        public override void Update()
        {
            base.Update();
            if (PlayerOwner != null && PlayerOwner.CharacterUsesRandomGuns)
            {
                PlayerOwner.ChangeToRandomGun();
            }
        }
    }
}
