using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public class CultCharmEffect : GameActorCharmEffect
    {
        public GameObject indoctrinateInteractable;

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if(actor is AIActor aiactor)
            {
                if(aiactor.ParentRoom != null && aiactor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) == 1 && aiactor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All)[0] == aiactor)
                {
                    var go = Object.Instantiate(indoctrinateInteractable, actor.transform.position, Quaternion.identity);
                    var interactable = go.GetComponent<IndoctrinateFollowerInteractable>();
                    interactable.enemyGuid = aiactor.EnemyGuid;
                    interactable.sprite.SetSprite(aiactor.sprite.Collection, aiactor.sprite.spriteId);
                    if (aiactor.optionalPalette != null)
                    {
                        interactable.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
                        interactable.sprite.renderer.material.SetTexture("_PaletteTex", aiactor.optionalPalette);
                    }
                    interactable.sprite.IsPerpendicular = aiactor.sprite.IsPerpendicular;
                    interactable.sprite.HeightOffGround = aiactor.sprite.HeightOffGround;
                    if (aiactor.forceUsesTrimmedBounds)
                    {
                        interactable.sprite.depthUsesTrimmedBounds = true;
                    }
                    interactable.sprite.UpdateZDepth();
                    SpriteOutlineManager.AddOutlineToSprite(interactable.sprite, Color.black);
                    if (aiactor.spriteAnimator != null)
                    {
                        interactable.spriteAnimator.Library = aiactor.spriteAnimator.Library;
                        if(aiactor.aiAnimator != null && aiactor.aiAnimator.IdleAnimation != null && aiactor.aiAnimator.IdleAnimation.HasAnimation && aiactor.aiAnimator.IdleAnimation.GetInfo(Vector2.down) != null)
                        {
                            interactable.spriteAnimator.Play(aiactor.aiAnimator.IdleAnimation.GetInfo(Vector2.down).name);
                        }
                        else if(aiactor.spriteAnimator.CurrentClip != null)
                        {
                            interactable.spriteAnimator.Play(aiactor.spriteAnimator.CurrentClip.name);
                        }
                        else if(aiactor.spriteAnimator.DefaultClip != null)
                        {
                            interactable.spriteAnimator.Play(aiactor.spriteAnimator.DefaultClip.name);
                        }
                    }
                    aiactor.ParentRoom.RegisterInteractable(interactable);
                    var onDeathBehaviours = aiactor.GetComponents<OnDeathBehavior>().ToArray();
                    foreach(var behav in onDeathBehaviours)
                    {
                        if(behav != null)
                        {
                            Object.DestroyImmediate(behav);
                        }
                    }
                    aiactor.EraseFromExistence();
                }
            }
        }

        public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
        {
            return false;
        }
    }
}
