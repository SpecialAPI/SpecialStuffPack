using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class FishingRodGun : GunBehaviour
    {
        public static void Init()
        {
            HookProjectile proj = GunBuilder.EasyProjectileInit<HookProjectile>("projectiles/hookprojectile", string.Empty, 10f, 1f, 999999f, 0f, true, false, false, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("rock_projectile_001"),
                tk2dBaseSprite.Anchor.LowerLeft, 0, 0, null, null, null, null);
            proj.initialSpeed = 23f;
            proj.GetOrAddComponent<BounceProjModifier>().numberOfBounces += 9999;
            proj.speedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, -10f));
            proj.flySpeedMultiplier = 1f;
            proj.destinationOffset = new Vector2(0f, 0.6875f);
            proj.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            proj.shouldRotate = true;
            proj.dragSpeed = 5f;
            proj.rewardKeepTime = 1f;
            proj.rewards = new()
            {
                elements = new()
                {
                    new(new("143be8c9bbb84e3fb3ab98bcd4cf5e5b"), 0.1f), //fish bullet
                    new(new("06f5623a351c4f28bc8c6cda56004b80"), 0.1f), //other fish bullet
                    new(new("72d2f44431da43b8a3bae7d8a114a46d"), 0.1f), //shark
                    new(new("b70cbd875fea498aa7fd14b970248920"), 0.1f), //big shark
                    new(new(68), 0.5f), //single casing
                    new(new(), 0.25f), //random room reward
                    new(new(true), 0.025f), //random item
                    new(new(7), 0.007f), //barrel
                    new(new(404), 0.007f), //siren
                    new(new(359), 0.007f), //air tank
                }
            };
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(projectile is HookProjectile hook)
            {
                if(currentHooks != null)
                {
                    currentHooks.RemoveAll(x => x == null);
                    if(currentHooks.Count > 0)
                    {
                        Destroy(hook.gameObject);
                        return;
                    }
                }
                hook.source = gun;
                (hooksShotForNow ??= new()).Add(hook);
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            /*if(currentHooks != null)
            {
                currentHooks.RemoveAll(x => x == null);
                if(currentHooks.Count > 0)
                {
                    foreach(var hook in currentHooks)
                    {
                        hook?.HookingTime();
                    }
                }
            }*/
        }

        public override void Update()
        {
            base.Update();
            if(gun != null)
            {
                var isDischarging = false;
                foreach (var kvp in gun.RuntimeModuleData)
                {
                    if (kvp.Key.IsDefaultOrClone(gun))
                    {
                        isDischarging |= kvp.Value.chargeFired;
                    }
                }
                if (isDischarging && hooksShotForNow != null)
                {
                    hooksShotForNow.RemoveAll(x => x == null);
                    if (hooksShotForNow.Count > 0)
                    {
                        (currentHooks ??= new()).AddRange(hooksShotForNow);
                        currentHooks.RemoveAll(x => x == null);
                        hooksShotForNow.Clear();
                        return;
                    }
                }
                foreach (var kvp in gun.RuntimeModuleData)
                {
                    var data = kvp.Value;
                    var mod = kvp.Key;
                    if (!mod.IsDefaultOrClone(gun))
                    {
                        continue;
                    }
                    data.needsReload = false;
                    data.numberShotsFired = 0;
                    if (gun.IsCharging)
                    {
                        if (currentHooks != null)
                        {
                            currentHooks.RemoveAll(x => x == null);
                            if (currentHooks.Count > 0)
                            {
                                data.chargeTime = 0f;
                                foreach(var hook in currentHooks)
                                {
                                    hook.Drag();
                                }
                                continue;
                            }
                        }
                        //data.chargeTime = Mathf.Max(data.chargeTime, 1f);
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (currentHooks != null)
            {
                currentHooks.RemoveAll(x => x == null);
                if (currentHooks.Count > 0)
                {
                    foreach (var hook in currentHooks)
                    {
                        hook.HookingTime();
                    }
                }
            }
        }

        private List<HookProjectile> currentHooks;
        private List<HookProjectile> hooksShotForNow;
    }
}
