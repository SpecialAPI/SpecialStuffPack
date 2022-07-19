using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class RoundBullets : PassiveItem
    {
        public static void Init()
        {
            try
            {
                var name = "Round-s";
                var shortdesc = "Completely Circular";
                var longdesc = "Bullets spin around the shooter.";
                var item = ItemBuilder.EasyInit<RoundBullets>("OrbitBullets", "orbit_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, null, null);
                item.AddPassiveStatModifier(PlayerStats.StatType.RangeMultiplier, 2.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            }
            catch(Exception ex)
            {
                ETGModConsole.Log("Something bad happened while loading SpecialAPI's Stuff Reloaded: " + ex);
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += Spin;
        }

        public void Spin(Projectile proj, float f)
        {
            if (proj.GetComponent<OrbitProjectile>() == null)
            {
                proj.AddComponent<OrbitProjectile>().expandTime = 0.25f * proj.baseData.speed / 20f;
                proj.specRigidbody.CollideWithTileMap = false;
            }
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.PostProcessProjectile -= Spin;
            base.DisableEffect(disablingPlayer);
        }
    }
}
