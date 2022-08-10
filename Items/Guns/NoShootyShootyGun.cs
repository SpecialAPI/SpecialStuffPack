using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class NoShootyShootyGun : GunBehaviour
    {
        public override void Update()
        {
            base.Update();
            if(gun == null || gun.RuntimeModuleData == null)
            {
                return;
            }
            if(PlayerOwner == null || !PlayerOwner.PlayerHasActiveSynergy(enableShootyShootySynergy))
            {
                foreach(var msd in gun.RuntimeModuleData)
                {
                    if (msd.Key.IsDefaultOrClone(gun))
                    {
                        msd.Value.onCooldown = true;
                    }
                }
                didNoShootyShooty = true;
            }
            else if(didNoShootyShooty)
            {
                foreach (var msd in gun.RuntimeModuleData)
                {
                    msd.Value.onCooldown = false;
                }
                didNoShootyShooty = false;
            }
        }

        public string enableShootyShootySynergy;
        public bool didNoShootyShooty;
    }
}
