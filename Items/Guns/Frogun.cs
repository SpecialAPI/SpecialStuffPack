using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class Frogun : GunBehaviour
    {
        public static void Init()
        {
            string name = "Frogun";
            string shortdesc = "What the...";
            string longdesc = "Shoots bubbles. Reloading an empty clip pops the bubbles, releasing bullets directed at nearby enemies.";
            var gun = GunBuilder.EasyGunInit("frogun", name, shortdesc, longdesc, "frogun_idle_001", "frogun_idle_001", "gunsprites/frogun/", 130, 1f, new(22, 9), Empty, "BubbleGun", PickupObject.ItemQuality.A,
                GunClass.SHOTGUN, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 3);
            for(int i = 0; i < 2; i++)
            {
                gun.RawSourceVolley.projectiles.Add(new()
                {
                    cooldownTime = 0.15f,
                    numberOfShotsInClip = 6,
                    projectiles = new()
                    {
                        GetProjectile(599)
                    },
                    angleVariance = 30f,
                    ammoCost = i > 0 ? 0 : 1
                });
            }
            gun.carryPixelOffset = new(4, 0);
            gun.RawSourceVolley.UsesShotgunStyleVelocityRandomizer = true;
            gun.RawSourceVolley.IncreaseFinalSpeedPercentMax = 1.3f;
            gun.RawSourceVolley.DecreaseFinalSpeedPercentMin = 0.7f;
            gun.AddComponent<Frogun>().PopBubbleProjectile = GetProjectile(86);
            finish();
            gun.AddToGooptonShop();
        }

        public override void OnAutoReload(PlayerController player, Gun gun)
        {
            base.OnAutoReload(player, gun);
            foreach(Projectile proj in StaticReferenceManager.AllProjectiles)
            {
                if(proj?.PossibleSourceGun == gun)
                {
                    var unitCenter = proj.specRigidbody.UnitCenter;
                    proj.DieInAir();
                    var closestEnemyPos = player?.CurrentRoom?.GetNearestEnemy(unitCenter, out _, true, true)?.CenterPosition;
                    var direction = (closestEnemyPos.HasValue ? (closestEnemyPos.GetValueOrDefault() - unitCenter) : Random.insideUnitCircle).ToAngle();
                    OwnedShootProjectile(PopBubbleProjectile, unitCenter, direction, player);
                }
            }
        }

        public Projectile PopBubbleProjectile;
    }
}
