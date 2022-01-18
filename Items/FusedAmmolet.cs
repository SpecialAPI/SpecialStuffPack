using MonoMod.RuntimeDetour;
using SpecialStuffPack.ItemAPI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialStuffPack.Components;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class FusedAmmolet : PassiveItem
    {
        public static void Init()
        {
            string name = "Fused Ammolet";
            string shortdesc = "Explode Bullets";
            string longdesc = "Turns blanks into bombs, which explode and erase all bullets in the room. The bombs will not explode instantly.\n\nAre blanks a good idea? Yes. Are bombs a good idea? Yes. Are bomb blanks a good idea? Probably not.";
            FusedAmmolet item = ItemBuilder.EasyInit<FusedAmmolet>("items/bombammolet", "sprites/fused_ammolet_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, 344, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);
            //setup bomb
            GameObject bombObject = AssetBundleManager.Load<GameObject>("assets/itemeffects/blombk.prefab");
            tk2dSprite bombSprite = tk2dSprite.AddComponent(bombObject, SpriteBuilder.itemCollection, ItemBuilder.AddSpriteToCollection(AssetBundleManager.Load<Texture2D>("assets/sprites/blankbomb_idle_001.png"), SpriteBuilder.itemCollection));
            int id2 = ItemBuilder.AddSpriteToCollection(AssetBundleManager.Load<Texture2D>("assets/sprites/blankbomb_idle_002.png"), SpriteBuilder.itemCollection);
            SpriteBuilder.itemCollection.spriteDefinitions[id2].ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, null, false, true);
            bombSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, null, false, true);
            tk2dSpriteAnimator bombAnimator = bombObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation bombAnimation = bombObject.AddComponent<tk2dSpriteAnimation>();
            bombAnimator.Library = bombAnimation;
            bombAnimation.clips = new tk2dSpriteAnimationClip[]
            {
                new tk2dSpriteAnimationClip()
                {
                    fps = 5,
                    frames = new tk2dSpriteAnimationFrame[]
                    {
                        new tk2dSpriteAnimationFrame
                        {
                            spriteId = bombSprite.spriteId,
                            spriteCollection = SpriteBuilder.itemCollection
                        },
                        new tk2dSpriteAnimationFrame
                        {
                            spriteId = id2,
                            spriteCollection = SpriteBuilder.itemCollection
                        }
                    },
                    wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
                }
            };
            bombAnimator.playAutomatically = true;
            BlombkBehaviour bombBehaviour = bombObject.AddComponent<BlombkBehaviour>();
            bombBehaviour.ExplosionDelay = 1;
            bombBehaviour.ExplosionData = new ExplosionData
            {
                useDefaultExplosion = false,
                doDamage = true,
                forceUseThisRadius = false,
                damageRadius = 3f,
                damageToPlayer = 0f,
                damage = 250f,
                breakSecretWalls = true,
                secretWallsRadius = 4.5f,
                forcePreventSecretWallDamage = false,
                doDestroyProjectiles = true,
                doForce = true,
                pushRadius = 6f,
                force = 100f,
                debrisForce = 50f,
                preventPlayerForce = true,
                explosionDelay = 0.1f,
                usesComprehensiveDelay = false,
                comprehensiveDelay = 0f,
                effect = LoadHelper.LoadAssetFromAnywhere<GameObject>("VFX_Ring_Explosion_001"),
                doScreenShake = true,
                ss = new ScreenShakeSettings { },
                doStickyFriction = true,
                doExplosionRing = true,
                isFreezeExplosion = false,
                freezeRadius = 5f,
                freezeEffect = null,
                IsChandelierExplosion = false,
                ignoreList = new List<SpeculativeRigidbody>(),
                playDefaultSFX = true,
                overrideRangeIndicatorEffect = null,
                rotateEffectToNormal = false
            };
            item.BlankBombPrefab = bombObject;
            item.AddToOldRedShop();
            new Hook(
                typeof(PlayerController).GetMethod("DoConsumableBlank", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(FusedAmmolet).GetMethod("LookItsBlombkFromOMITB", BindingFlags.Public | BindingFlags.Static)
            );
        }

        public static void LookItsBlombkFromOMITB(Action<PlayerController> orig, PlayerController p)
        {
            FusedAmmolet bombItem = null;
            foreach(PassiveItem p2 in p.passiveItems)
            {
                if(p2 is FusedAmmolet)
                {
                    bombItem = p2 as FusedAmmolet;
                    break;
                }
            }
            if (bombItem != null)
            {
                bombItem.PlaceBomb(p);
            }
            else
            {
                orig(p);
            }
        }

        public void PlaceBomb(PlayerController p)
        {
            if (p.Blanks > 0)
            {
                p.Blanks--;
                GameObject bomb = Instantiate(BlankBombPrefab, p.sprite.WorldBottomCenter, Quaternion.identity);
                BlombkBehaviour bombBehaviour = bomb.GetComponent<BlombkBehaviour>();
                bombBehaviour.SetOwner(p);
                if(p.PlayerHasActiveSynergy("Super Blasts"))
                {
                    bombBehaviour.DamageMult *= 2;
                }
                int random = -1;
                if (p.PlayerHasActiveSynergy("Random Blasts"))
                {
                    random = UnityEngine.Random.Range(0, 4);
                }
                if (p.PlayerHasActiveSynergy("Stunning Blasts") || random == 0)
                {
                    bombBehaviour.ForceMult *= 2;
                }
                if(p.PlayerHasActiveSynergy("Poison Blasts") || random == 1)
                {
                    bombBehaviour.DamageType |= CoreDamageTypes.Poison;
                }
                if (p.PlayerHasActiveSynergy("Hot Blasts") || random == 2)
                {
                    bombBehaviour.DamageType |= CoreDamageTypes.Fire;
                }
                if (p.PlayerHasActiveSynergy("Freezing Blasts") || random == 3)
                {
                    bombBehaviour.DamageType |= CoreDamageTypes.Ice;
                }
            }
        }

        public GameObject BlankBombPrefab;
    }
}
