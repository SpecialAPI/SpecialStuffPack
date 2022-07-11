using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class CactiClub
    {
        public static void Init()
        {
            string name = "Cacti Club";
            string shortdesc = "Ranged Melee";
            string longdesc = "Shoots cactus spikes on death.\n\nClubs like these are mostly weilded by potato people.";
            float swingOffset = 2f;
            var gun = GunBuilder.EasyGunInit("guns/club", name, shortdesc, longdesc, "club_idle_001", "gunsprites/ammonomicon/club_idle_001", "gunsprites/cacticlub", 100, 0f, new(1.3125f, 0.3125f), new() { effects = 
                new VFXComplex[0], type = VFXPoolType.None }, "BoxingGlove", PickupObject.ItemQuality.B, GunClass.SILLY, SpecialStuffModule.globalPrefix, out var finish);
            gun.spriteAnimator.GetClipByName("club_fire").ApplyOffsetsToAnimation(new List<Vector2>()
            {
                new(swingOffset / 2f, 0f),
                new(swingOffset, 0f),
                new(swingOffset / 2f, 0f)
            });
            var proj = GunBuilder.EasyProjectileInit<Projectile>("projectiles/clubprojectile", "", 15, swingOffset * 30, swingOffset, 10f, false, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, 6, 6, 0, 0);
            proj.AddComponent<CactiClubProjectile>().projectileToShoot = CodeShortcuts.GetItemById<Gun>(124).DefaultModule.projectiles[0];
            gun.RawSourceVolley.projectiles.Add(new()
            {
                numberOfShotsInClip = 100,
                cooldownTime = 0.77f,
                projectiles = new()
                {
                    proj
                }
            });
            gun.InfiniteAmmo = true;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            finish();
        }
    }
}
