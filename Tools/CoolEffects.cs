using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace SpecialStuffPack
{
    public static class CoolEffects
    {
        public static void DoFadeFlash(this tk2dBaseSprite sprite, float flashTime = 0.5f, float destination = 2f, bool useInvariantTime = false)
        {
            GameObject fadeObject = new GameObject(sprite.name + "_fade");
            tk2dSprite fade = tk2dSprite.AddComponent(fadeObject, sprite.Collection, sprite.spriteId);
            fade.usesOverrideMaterial = true;
            fade.renderer.material = new Material(fade.renderer.material);
            fade.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            fade.renderer.material.SetFloat("_VertexColor", 1f);
            fade.renderer.material.SetColor("_OverrideColor", new Color(99, 99, 99, 1f));
            fade.color = new Color(99, 99, 99, 1f);
            fade.HeightOffGround = sprite.HeightOffGround + 10f;
            fade.PlaceAtPositionByAnchor(sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            GameManager.Instance.Dungeon.StartCoroutine(HandleFadeSprite(fade, flashTime, destination, useInvariantTime));
        }

        public static IEnumerator HandleFadeSprite(tk2dSprite s, float flashTime = 0.5f, float destination = 2f, bool useInvariantTime = false)
        {
            float ela = 0f;
            float dura = flashTime;
            while (ela < dura)
            {
                ela += useInvariantTime ? GameManager.INVARIANT_DELTA_TIME : BraveTime.DeltaTime;
                Vector2 center = s.WorldCenter;
                s.scale = Vector2.one * Mathf.Lerp(1f, destination, ela / dura);
                s.sprite.color = s.sprite.color.WithAlpha(Mathf.Lerp(1f, 0f, ela / dura));
                s.PlaceAtPositionByAnchor(center, tk2dBaseSprite.Anchor.MiddleCenter);
                yield return null;
            }
            UnityEngine.Object.Destroy(s.gameObject);
            yield break;
        }

        public static void BreakChestWithoutDrops(this Chest c)
        {
            if(c != null && c.majorBreakable != null && c.majorBreakable.GetCurrentHealthPercentage() > 0f)
            {
                if (!c.pickedUp)
                {
                    c.pickedUp = true;
                    c.GetAbsoluteParentRoom()?.DeregisterInteractable(c);
                    c.majorBreakable.Break(Vector2.zero);
                    c.sprite.HeightOffGround -= 0.5f;
                    c.sprite.UpdateZDepth();
                }
                c.majorBreakable.Break(Vector2.zero);
            }
        }

        public static GameObject ScarfPoof = GetItemById<BlinkPassiveItem>(436).BlinkpoofVfx;
        public static GameObject smokePoofVFX = GetItemById<ConsumableStealthItem>(462).poofVfx;
        public static GameObject ScarfPrefab = GetItemById<BlinkPassiveItem>(436).ScarfPrefab.gameObject;
    }
}
