using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class MirroredBullet : PassiveItem
    {
        public static void Init()
        {
            string name = "Mirrored Bullet";
            string shortdesc = "Double bullets for a cost";
            string longdesc = "All bullets are doubled, but doubles the user's spread.";
            MirroredBullet item = EasyInitItem<MirroredBullet>("items/mirrorbullet", "sprites/mirrored_bullet_idle_001", name, shortdesc, longdesc, ItemQuality.S, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddToCursulaShop();
        }

        public override void Pickup(PlayerController player)
        {
            player.stats.AdditionalVolleyModifiers += ModifyVolley;
            base.Pickup(player);
        }

        public void ModifyVolley(ProjectileVolleyData volley)
        {
            volley.projectiles.ForEach(x => x.mirror = true);
            if(Owner.PlayerHasActiveSynergy("Infinite Mirror"))
            {
                int count = volley.projectiles.Count;
                for (int i = 0; i < count; i++)
                {
                    ProjectileModule projectileModule = volley.projectiles[i];
                    int sourceIndex = i;
                    if (projectileModule.CloneSourceIndex >= 0)
                    {
                        sourceIndex = projectileModule.CloneSourceIndex;
                    }
                    ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                    projectileModule2.ignoredForReloadPurposes = true;
                    projectileModule2.angleFromAim = projectileModule.angleFromAim + 180;
                    projectileModule2.ammoCost = 0;
                    projectileModule.mirror = true;
                    volley.projectiles.Add(projectileModule2);
                }
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.stats.AdditionalVolleyModifiers -= ModifyVolley;
        }
    }
}
