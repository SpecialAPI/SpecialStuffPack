using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class FrogWand : GunBehaviour
    {
        public static void Init()
        {
            var name = "Frog Wizard's Wand";
            var shortdesc = "Magic Gems";
            var longdesc = "Fires magical beams. Enemies now carry magic gems that have special effects when hit with a beam.\n\nThis wand was once used by a frog wizard in order to get rid of a curse.";
            var gun = EasyGunInit("frogwand", name, shortdesc, longdesc, "frogwand_idle_001", "frogwand_idle_001", "gunsprites/frogwand", 200, 1f, new(20, 5), Empty, "Trident", PickupObject.ItemQuality.B, GunClass.BEAM, out var finish);
            gun.AddComponent<FrogWand>();
            var bullet = EasyProjectileInit<Projectile>("frogwandprojectile", "", 9f, 200f, 1000f, 10f, true, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, 17, 2);
            bullet.hitEffects = TridentObject.GetProjectile().hitEffects;
            bullet.AddComponent<FrogWandProjectile>().destroyBeam = DuelingLaserObject.GetProjectile();
            bullet.AddComponent<DontLoseDamageOnPierce>();
            var trailsprite = bullet.transform.Find("Trail 1").gameObject;
            var beamvfxcoll = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX Beam Collection").GetComponent<tk2dSpriteCollectionData>();
            var tiled = trailsprite.AddComponent<tk2dTiledSprite>();
            tiled.SetSprite(beamvfxcoll, 376);
            tiled.dimensions = new(0f, 14f);
            tiled.anchor = tk2dBaseSprite.Anchor.MiddleLeft;
            tiled.HeightOffGround = 0.05f;
            var trail = trailsprite.AddComponent<TrailController>();
            trail.animation = "venom_beam_start";
            trail.boneSpawnOffset = new();
            trail.cascadeTimer = 0.1f;
            trail.destroyOnEmpty = false;
            trail.FlipUvsY = false;
            trail.globalTimer = 0f;
            trail.rampHeight = false;
            trail.rampStartHeight = 2f;
            trail.rampTime = 1f;
            trail.softMaxLength = 0f;
            trail.startAnimation = "venom_beam_start";
            trail.usesAnimation = true;
            trail.usesCascadeTimer = false;
            trail.UsesDispersalParticles = false;
            trail.usesGlobalTimer = true;
            trail.usesSoftMaxLength = false;
            trail.usesStartAnimation = true;
            var anim = trailsprite.AddComponent<tk2dSpriteAnimator>();
            anim.Library = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX Beam Animation").GetComponent<tk2dSpriteAnimation>();
            anim.DefaultClipId = 72;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.Charged,
                chargeProjectiles = new()
                {
                    new()
                    {
                        ChargeTime = 0f,
                        Projectile = bullet
                    }
                },
                numberOfShotsInClip = -1,
                cooldownTime = 0.25f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "Star",
                angleVariance = 4f
            });
            var vfxcoll = EasyCollectionSetup("SpecialVFXCollection");
            tk2dSprite.AddComponent(AssetBundleManager.Load<GameObject>("magicgemteleport"), vfxcoll, AddSpriteToCollection("magicgem_teleport_001", vfxcoll));
            tk2dSprite.AddComponent(AssetBundleManager.Load<GameObject>("magicgemdestroy"), vfxcoll, AddSpriteToCollection("magicgem_destroy_001", vfxcoll));
            tk2dSprite.AddComponent(AssetBundleManager.Load<GameObject>("magicgemmirror"), vfxcoll, AddSpriteToCollection("magicgem_mirror_001", vfxcoll));
            ETGMod.AIActor.OnPostStart += MaybeGemifyEnemy;
            finish();
        }

        public override void OnPlayerPickup(PlayerController playerOwner)
        {
            base.OnPlayerPickup(playerOwner);
            lastOwner = playerOwner;
            lastOwner.IncrementFlag<FrogWand>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (lastOwner != null)
            {
                lastOwner.DecrementFlag<FrogWand>();
                lastOwner = null;
            }
        }

        public override void OnDropped()
        {
            base.OnDropped();
            if(lastOwner != null)
            {
                lastOwner.DecrementFlag<FrogWand>();
                lastOwner = null;
            }
        }

        public static void MaybeGemifyEnemy(AIActor enemy)
        {
            if (enemy != null && PassiveItem.IsFlagSetAtAll(typeof(FrogWand)))
            {
                enemy.AddComponent<GemifiedEnemy>();
            }
        }

        public PlayerController lastOwner;
    }
}
