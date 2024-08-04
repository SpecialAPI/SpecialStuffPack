using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class RandomOwnedProjectiles : GunBehaviour
    {
        public override void PostProcessVolley(ProjectileVolleyData volley)
        {
            if(PlayerOwner == null)
            {
                return;
            }
            var possibleStuff = PlayerOwner.inventory.AllGuns
                .Where(x => x != gun)
                .Select(x =>
                    x.GetProjectileModules()
                    .Where(x => (x.projectiles != null && x.projectiles.Count > 0) || (x.chargeProjectiles != null && x.chargeProjectiles.Count > 0))
                    .Select(x => x.shootStyle == ProjectileModule.ShootStyle.Charged && x.chargeProjectiles != null && x.chargeProjectiles.Count > 0 ? x.chargeProjectiles.Select(x => x.Projectile) : x.projectiles))
                .SelectMany(x => x.SelectMany(x2 => x2))
                .ToList();
            if (possibleStuff.Count > 0)
            {
                for (int i = 0; i < volley.projectiles.Count; i++)
                {
                    var mod = volley.projectiles[i];
                    if (i < baseModules || mod.CloneSourceIndex < baseModules)
                    {
                        mod.projectiles = possibleStuff;
                    }
                }
            }
        }

        public int baseModules;
    }
}
