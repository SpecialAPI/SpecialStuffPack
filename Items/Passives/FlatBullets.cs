using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class FlatBullets : PassiveItem
    {
        public static void Init()
        {
            string name = "Flat Bullets";
            string shortdesc = "Flat Damage";
            string longdesc = "Increases damage.";
            var item = EasyInitItem<FlatBullets>("items/flatbullets", "sprites/flat_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.B);
            item.FlatDamage = 2f;
            item.FlatDamageSynergy = 3f;
            item.AddToTrorkShop();
        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += PostProcessProjectile;
            player.PostProcessBeam += PostProcessBeam;
            base.Pickup(player);
        }

        public void PostProcessBeam(BeamController obj)
        {
            obj.projectile.baseData.damage += (Owner.PlayerHasActiveSynergy("Flatter Flat Bullets") ? FlatDamageSynergy : FlatDamage) / 3f;
        }

        public void PostProcessProjectile(Projectile obj, float effectChanceScalar)
        {
            obj.baseData.damage += Owner.PlayerHasActiveSynergy("Flatter Flat Bullets") ? FlatDamageSynergy : FlatDamage;
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.PostProcessProjectile -= PostProcessProjectile;
            disablingPlayer.PostProcessBeam -= PostProcessBeam;
            base.DisableEffect(disablingPlayer);
        }

        public float FlatDamage;
        public float FlatDamageSynergy;
    }
}
