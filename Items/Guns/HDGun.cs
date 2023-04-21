using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public static class HDGun
    {
        public static void Init()
        {
            static void ResizeDef(tk2dSpriteDefinition def)
            {
                def.position0 /= 2f;
                def.position1 /= 2f;
                def.position2 /= 2f;
                def.position3 /= 2f;
                def.boundsDataCenter /= 2f;
                def.boundsDataExtents /= 2f;
                def.untrimmedBoundsDataCenter /= 2f;
                def.untrimmedBoundsDataExtents /= 2f;
            }
            var name = "HD Gun";
            var shortdesc = "1080p";
            var longdesc = "Shots charm enemies.\n\nA magnum, but with an HD resolution, this gun has 4 times more pixels!";
            var gun = EasyGunInit("hd", name, shortdesc, longdesc, "hd_idle_001", "hd_idle_001", "gunsprites/hdgun", 280, 1f, new(17, 8), MagnumObject.muzzleFlashEffects, "Magnum", PickupObject.ItemQuality.S, GunClass.PISTOL, out var finish);
            var proj = EasyProjectileInit<Projectile>("hdgunprojectile", "hd_bullet_001", 16f, 23f, 100f, 30f, false, false, false, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, 6, 6);
            proj.AppliesCharm = true;
            proj.CharmApplyChance = 0.25f;
            proj.charmEffect = CharmingRoundsObject.CharmModifierEffect;
            gun.SetAnimationFPS(gun.shootAnimation, 13);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.shellsToLaunchOnReload = 6;
            gun.reloadShellLaunchFrame = 2;
            gun.shellCasing = EasyCasingSetup("hdcasing", "hd_shell_001");
            gun.shellCasing.layer = LayerMask.NameToLayer("Unpixelated");
            ResizeDef(gun.shellCasing.GetComponent<tk2dSprite>().CurrentSprite);
            gun.shellCasing.AddComponent<AlwaysUnpixelated>();
            var smoke = EasyMuzzleSetup("HDSmoke", "hd_smoke", 13, "tk2d/CutoutVertexColorTilted", tk2dBaseSprite.Anchor.LowerLeft, new()
            {
                new(0, -8),
                new(2, -10),
                new(2, -10)
            });
            smoke.layer = LayerMask.NameToLayer("Unpixelated");
            smoke.GetComponent<tk2dSpriteAnimator>().DefaultClip.frames.Do(x => ResizeDef(x.spriteCollection.spriteDefinitions[x.spriteId]));
            var flare = EasyMuzzleSetup("HDFlare", "hd_flare", 20, "Brave/LitTk2dCustomFalloffTiltedCutoutEmissive", tk2dBaseSprite.Anchor.LowerLeft, new()
            {
                new(0, -8),
                new(6, -10),
                new(12, -14),
                new(14, -14),
                new(20, -8)
            }, x =>
            {
                x.SetFloat("_EmissivePower", 34.77f);
                x.SetFloat("_EmissiveColorPower", 24.9f);
            });
            flare.layer = LayerMask.NameToLayer("Unpixelated");
            flare.GetComponent<tk2dSpriteAnimator>().DefaultClip.frames.Do(x => ResizeDef(x.spriteCollection.spriteDefinitions[x.spriteId]));
            gun.muzzleFlashEffects = new()
            {
                type = VFXPoolType.All,
                effects = new VFXComplex[]
                {
                    new()
                    {
                        effects = new VFXObject[]
                        {
                            new()
                            {
                                effect = smoke,
                                attached = true,
                                alignment = VFXAlignment.Fixed,
                                destructible = false,
                                orphaned = true,
                                persistsOnDeath = false,
                                usesZHeight = false,
                                zHeight = 0f
                            },
                            new()
                            {
                                effect = flare,
                                attached = true,
                                alignment = VFXAlignment.Fixed,
                                destructible = false,
                                orphaned = false,
                                persistsOnDeath = false,
                                usesZHeight = false,
                                zHeight = 0f
                            }
                        }
                    }
                }
            };
            ResizeDef(proj.GetComponentInChildren<tk2dSprite>().CurrentSprite);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                angleVariance = 7f,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = AddCustomAmmoTypeHD("hdgun", "HDAmmo", "HDAmmoBG", "hd_ammo", "hd_ammo_bg"),
                cooldownTime = 0.15f,
                numberOfShotsInClip = 7,
                projectiles = new()
                {
                    proj
                }
            });
            ResizeDef(SpriteBuilder.ammonomiconCollection.GetSpriteDefinition("hd_idle_001"));
            new List<string> { gun.idleAnimation, gun.shootAnimation, gun.reloadAnimation }.Select(x => gun.spriteAnimator.GetClipByName(x).frames).SelectMany(x => x).Do(x => ResizeDef(x.spriteCollection.spriteDefinitions[x.spriteId]));
            gun.IgnoresAngleQuantization = true;
            gun.AddComponent<AlwaysUnpixelated>();
            finish();
        }

        public static string AddCustomAmmoTypeHD(string name, string fgSpriteObject, string bgSpriteObject, string fgTexture, string bgTexture)
        {
            dfTiledSprite fgSprite = AssetBundleManager.Load<GameObject>(fgSpriteObject).SetupDfSpriteFromTextureHD<dfTiledSprite>(AssetBundleManager.Load<Texture2D>(fgTexture), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            dfTiledSprite bgSprite = AssetBundleManager.Load<GameObject>(bgSpriteObject).SetupDfSpriteFromTextureHD<dfTiledSprite>(AssetBundleManager.Load<Texture2D>(bgTexture), ShaderCache.Acquire("Daikon Forge/Default UI Shader"));
            fgSprite.zindex = 7;
            bgSprite.zindex = 5;
            GameUIAmmoType uiammotype = new()
            {
                ammoBarBG = bgSprite,
                ammoBarFG = fgSprite,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = name
            };
            CustomAmmoUtility.addedAmmoTypes.Add(uiammotype);
            if (GameUIRoot.HasInstance)
            {
                foreach (GameUIAmmoController uiammocontroller in GameUIRoot.Instance.ammoControllers)
                {
                    if (uiammocontroller.m_initialized)
                    {
                        uiammocontroller.ammoTypes = uiammocontroller.ammoTypes.AddToArray(uiammotype);
                    }
                }
            }
            return name;
        }

        public static T SetupDfSpriteFromTextureHD<T>(this GameObject obj, Texture2D texture, Shader shader) where T : dfSprite
        {
            T sprite = obj.GetOrAddComponent<T>();
            dfAtlas atlas = obj.GetOrAddComponent<dfAtlas>();
            atlas.Material = new(shader);
            atlas.Material.mainTexture = texture;
            atlas.Items.Clear();
            dfAtlas.ItemInfo info = new()
            {
                border = new RectOffset(),
                deleted = false,
                name = "main_sprite",
                region = new Rect(Vector2.zero, new Vector2(1, 1)),
                rotated = false,
                sizeInPixels = new Vector2(texture.width / 2f, texture.height / 2f),
                texture = null,
                textureGUID = "main_sprite"
            };
            atlas.AddItem(info);
            sprite.Atlas = atlas;
            sprite.SpriteName = "main_sprite";
            sprite.zindex = 0;
            return sprite;
        }
    }
}
