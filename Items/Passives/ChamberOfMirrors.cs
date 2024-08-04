using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class ChamberOfMirrors : PassiveItem
    {
        public static void Init()
        {
            var name = "Chamber of Mirrors";
            var shortdesc = "I cast infinite dice";
            var longdesc = "When reloading an empty clip in combat, a single bullet is added to that gun's clip. This effect resets when the owner switches guns or clears a room.\n\nOriginally a device used to create an infinite amount of dice, the chamber of mirrors has been modified to instead increase clip sizes.";
            var item = EasyItemInit<ChamberOfMirrors>("chamberofmirrors", "chamber_of_mirrors_idle_001", name, shortdesc, longdesc, ItemQuality.S, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.stats.AdditionalVolleyModifiers += AddExtraShots;
            player.OnReloadedGun += ExtraShot;
            player.GunChanged += RemoveExtraShotsGunChanged;
            player.OnRoomClearEvent += RemoveExtraShotsRoomClear;
        }

        public void ExtraShot(PlayerController p, Gun g)
        {
            if(g.ClipShotsRemaining <= 0 && p.IsInCombat)
            {
                extraShots++;
                p.RecalculateStats();
            }
        }

        public void RemoveExtraShotsGunChanged(Gun g1, Gun g2, bool b)
        {
            extraShots = 0;
            if(Owner != null)
            {
                Owner.RecalculateStats();
            }
        }

        public void RemoveExtraShotsRoomClear(PlayerController p)
        {
            extraShots = 0;
            p.RecalculateStats();
        }

        public void AddExtraShots(ProjectileVolleyData volley)
        {
            if(volley != null && volley.projectiles != null)
            {
                volley.projectiles.Where(x => x.numberOfShotsInClip > 0).Do(x => x.numberOfShotsInClip += extraShots);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player != null)
            {
                player.stats.AdditionalVolleyModifiers -= AddExtraShots;
                player.OnReloadedGun -= ExtraShot;
                player.GunChanged -= RemoveExtraShotsGunChanged;
                player.OnRoomClearEvent -= RemoveExtraShotsRoomClear;
            }
        }

        private int extraShots;
    }
}
