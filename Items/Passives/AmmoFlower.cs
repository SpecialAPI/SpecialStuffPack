using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class AmmoFlower : PassiveItem
    {
        public static void Init()
        {
            string name = "Ammo Flower";
            string shortdesc = "Grown in an Ammo Box";
            string longdesc = "Guns are reloaded when their clip is empty, semiautomatic guns become automatic and charged guns autofire. Also slightly increases reload speed.";
            EasyInitItem<AmmoFlower>("items/flower", "sprites/flower_idle_001.png", name, shortdesc, longdesc, ItemQuality.C, null, null)
                .AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, -0.05f, StatModifier.ModifyMethod.ADDITIVE).AddToGooptonShop();
        }

        public override void Pickup(PlayerController player)
        {
            player.stats.AdditionalVolleyModifiers += ModifyVolley;
            base.Pickup(player);
        }

        public void ModifyVolley(ProjectileVolleyData volley)
        {
            volley.projectiles.ForEach(x => x.shootStyle = x.shootStyle is ProjectileModule.ShootStyle.SemiAutomatic ? ProjectileModule.ShootStyle.Automatic : x.shootStyle);
        }

        public void LateUpdate()
        {
            if (PickedUp && Owner != null)
            {
                if (Owner.CurrentGun != null)
                {
                    var currentGun = Owner.CurrentGun;
                    var allProjectiles = currentGun.GetProjectileModules();
                    bool allModulesCharged = allProjectiles.Exists(x => x.shootStyle == ProjectileModule.ShootStyle.Charged) && 
                        !allProjectiles.Exists(x => x.shootStyle == ProjectileModule.ShootStyle.Charged && currentGun.RuntimeModuleData[x].chargeTime < x.LongestChargeTime);
                    bool allModulesEmpty = currentGun.ClipShotsRemaining <= 0;
                    if (currentGun.Volley != null)
                    {
                        allModulesEmpty = !currentGun.CheckHasLoadedModule(currentGun.Volley);
                    }
                    else
                    {
                        allModulesEmpty = !currentGun.CheckHasLoadedModule(currentGun.singleModule);
                    }
                    if (allModulesEmpty)
                    {
                        currentGun.AttemptedFireNeedReload();
                    }
                    else if (allModulesCharged)
                    {
                        currentGun.CeaseAttack(true, null);
                    }
                }
            }
        }

        public override void DisableEffect(PlayerController disablingPlayer)
        {
            disablingPlayer.stats.AdditionalVolleyModifiers -= ModifyVolley;
            base.DisableEffect(disablingPlayer);
        }
    }
}
