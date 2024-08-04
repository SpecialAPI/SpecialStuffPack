using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class Launcher : PassiveItem
    {
        public static void Init()
        {
            string name = "Launcher";
            string shortdesc = "Launch your bullets!";
            string longdesc = "Bullets are launched into the air when fired, but increases accuracy and damage.";
            var launcher = EasyItemInit<Launcher>("items/launcher", "sprites/jumppad_idle_001", name, shortdesc, longdesc, ItemQuality.C, null, null);
            launcher.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 0.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            launcher.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
            launcher.AddToTrorkShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += PostProcessProjectile;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if(proj is LobbedProjectile || proj.GetComponent<LobbedProjectileMotion>() != null)
            {
                return;
            }
            LobbedProjectileMotion motion = proj.GetOrAddComponent<LobbedProjectileMotion>();
            proj.baseData.force *= 0.3f;
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            player.PostProcessProjectile -= PostProcessProjectile;
            base.DisableEffect(player);
        }
    }
}
