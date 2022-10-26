using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Magnificus
    {
        public static void Init()
        {
            string name = "Blowfish";
            string shortdesc = "His name is Magnificus";
            string londesc = "Shoots cards. The final shot is a gemstone that floats in the air, empowering cards that come near it.\n\nWhile this may seem like a regular blowfish, this is actually an almighty card-playing, " +
                "student-torturing future-predicting picture-painting blowfish with a magical eye. So, basically the exact same thing.";
            var gun = EasyGunInit("magnificus", name, shortdesc, londesc, "magnificus_idle_001", "magnificus_idle_001", "gunsprites/magnificus", 200, 1f, new(23, 8), BundleOfWandsObject.muzzleFlashEffects, "Kthulu",
                PickupObject.ItemQuality.B, GunClass.SILLY, out var finish);
            
        }
    }
}
