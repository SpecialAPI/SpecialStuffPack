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
            var name = "Round-s";
            var shortdesc = "Completely Circular";
            var longdesc = "Bullets spin around the shooter.";
            var item = EasyItemInit<RoundBullets>("OrbitBullets", "orbit_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
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
                proj.baseData.range *= 2.5f;
                proj.AddComponent<OrbitProjectile>().expandTime = 0.25f * proj.baseData.speed / 20f;
                proj.specRigidbody.CollideWithTileMap = false;
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            player.PostProcessProjectile -= Spin;
            base.DisableEffect(player);
        }
    }
}
