using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class DisguiseHat : PlayerItem
    {
        public static void Init()
        {
            string name = "Disguise Hat";
            string shortdesc = "Temporary Disguise";
            string longdesc = "Grants stealth, can't be used to steal from shops.\n\nThis hat allows for easy and quick disguise, but the shopkeepers won't be tricked that easily";
            var item = EasyItemInit<DisguiseHat>("disguisehat", "disguise_hat_idle_001", name, shortdesc, longdesc, ItemQuality.SPECIAL, null, null);
            item.SetCooldownType(CooldownType.PerRoom, 1f);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            user.StealthPlayer("spapi_disguise", false, false, stealthyPoof, "Play_ENM_wizardred_appear_01", true);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return !user.IsStealthed;
        }
    }
}
