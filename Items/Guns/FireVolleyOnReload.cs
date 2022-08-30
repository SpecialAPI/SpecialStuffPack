using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class FireVolleyOnReload : GunBehaviour
    {
        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            if (gun.IsReloading && player != null && player.PlayerHasActiveSynergy(synergy))
            {
                AkSoundEngine.SetSwitch("WPN_Guns", switchGroup, gameObject);
                AkSoundEngine.PostEvent(sfx, gameObject);
                VolleyUtility.FireVolley(volley, gun.barrelOffset.transform.position.XY() + shootOffset.Rotate(gun.barrelOffset.transform.eulerAngles.z), BraveMathCollege.DegreesToVector(gun.CurrentAngle), player, true);
                AkSoundEngine.SetSwitch("WPN_Guns", gun.gunSwitchGroup, gameObject);
            }
        }

        public string sfx;
        public string switchGroup;
        public Vector2 shootOffset;
        public string synergy;
        public ProjectileVolleyData volley;
    }
}
