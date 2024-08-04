using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class FossilFuel : PassiveItem
    {
        public static void Init()
        {
            string name = "Fossil Fuel";
            string shortdesc = "Carbon Footprint Up";
            string longdesc = "Projectiles leave oil, reloading ignites nearby oil.\n\nKilling two birds with one stone: killing both your enemies AND the environment!";
            var item = EasyItemInit<FossilFuel>("fossilfuel", "fossil_fuel_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.goop = GoopDatabase.DefaultOilGoop;
            item.fireVFX = PhoenixObject.muzzleFlashEffects;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += AddGooper;
            player.OnReloadedGun += DoFlames;
        }

        public void AddGooper(Projectile proj, float f)
        {
            if(proj.GetComponent<GoopModifier>() == null || !proj.GetComponent<GoopModifier>().SpawnGoopInFlight)
            {
                var oilGooper = proj.AddComponent<GoopModifier>();
                oilGooper.goopDefinition = goop;
                oilGooper.InFlightSpawnRadius = 1f;
                oilGooper.SpawnGoopInFlight = true;
            }
        }

        public void DoFlames(PlayerController player, Gun gun)
        {
            fireVFX.SpawnAtPosition(player.sprite.WorldBottomCenter, 90f, null, null, null, null, false, null, null, false);
            DeadlyDeadlyGoopManager.IgniteGoopsCircle(player.sprite.WorldBottomCenter, 3f);
        }

        public override void DisableEffect(PlayerController player)
        {
            if(player == null)
            {
                return;
            }
            base.DisableEffect(player);
            player.PostProcessProjectile -= AddGooper;
            player.OnReloadedGun -= DoFlames;
        }

        public GoopDefinition goop;
        public VFXPool fireVFX;
    }
}
