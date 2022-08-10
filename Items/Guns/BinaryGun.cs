using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using SpecialStuffPack.Components;

namespace SpecialStuffPack.Items
{
    public class BinaryGun
    {
        public static void Init()
        {
            string name = "Binarifle";
            string shortdesc = "01000111 01010101 01001110";
            string longdesc = "Shoots ones and zeroes. Despite it's name, isn't actually a rifle.\n\nA gun made out of ones and zeroes. Nobody quite knows what they mean, nobody knows how this gun appeared in the Gungoen and nobody knows how it shoots." +
                " Truly a mysterious gun.";
            Gun gun = EasyGunInit("guns/binary_gun", name, shortdesc, longdesc, "binary_gun_idle_001", "gunsprites/ammonomicon/binary_gun_idle_001.png", "gunsprites/binarygun", 101, 1f, new(26, 8), Empty, "Magnum",
                PickupObject.ItemQuality.B, GunClass.PISTOL, out var finish, 199, null, null);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).ApplyOffsetsToAnimation(new List<IntVector2> { new IntVector2(0, 0), new IntVector2(-2, 2), new IntVector2(-1, 1), new IntVector2(1, -1) });
            GameObject markVFX = AssetBundleManager.Load<GameObject>("vfx/binarygunhitindicator");
            int zeroId = SpriteBuilder.AddSpriteToCollection("vfxsprites/zero_indicator_001", ETGMod.Databases.Items.ProjectileCollection, "tk2d/CutoutVertexColorTilted");
            int oneId = SpriteBuilder.AddSpriteToCollection("vfxsprites/one_indicator_001", ETGMod.Databases.Items.ProjectileCollection, "tk2d/CutoutVertexColorTilted");
            tk2dSprite.AddComponent(markVFX, ETGMod.Databases.Items.ProjectileCollection, zeroId);
            SpecialAssets.assets.Add(markVFX);
            BinaryGunProjectile zeroProjectile = EasyProjectileInit<BinaryGunProjectile>("projectiles/binarygunprojectile_zero", "projectilesprites/zero_projectile_001", 10, 13, 60, 10, true, false, false, null, 
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            zeroProjectile.IsOne = false;
            zeroProjectile.MarkVFX = markVFX;
            zeroProjectile.VFXOffset = new Vector2(0, 2.5f);
            zeroProjectile.ZeroId = zeroId;
            zeroProjectile.OneId = oneId;
            zeroProjectile.InstakillSynergy = "why do you keep crashing";
            BinaryGunProjectile oneProjectile = EasyProjectileInit<BinaryGunProjectile>("projectiles/binarygunprojectile_one", "projectilesprites/one_projectile_001", 10, 13, 60, 10, true, false, false, null, 
                tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, null, null);
            oneProjectile.IsOne = true;
            oneProjectile.MarkVFX = markVFX;
            oneProjectile.VFXOffset = new Vector2(0, 2.5f);
            oneProjectile.ZeroId = zeroId;
            oneProjectile.OneId = oneId;
            oneProjectile.helixAmplitude = -1f;
            oneProjectile.InstakillSynergy = "why do you keep crashing";
            ProjectileModule module = new ProjectileModule
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "white",
                projectiles = new List<Projectile>
                {
                    zeroProjectile,
                    oneProjectile
                },
                orderedGroupCounts = new List<int>(),
                numberOfFinalProjectiles = 0,
                angleVariance = 0f,
                cooldownTime = 0.2f,
                numberOfShotsInClip = 6,
                sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered
            };
            gun.RawSourceVolley.projectiles.Add(module);
            finish();
        }
    }
}
