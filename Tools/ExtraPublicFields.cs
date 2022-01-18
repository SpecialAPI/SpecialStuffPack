using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Reflection;
using MonoMod.Utils;

namespace SpecialStuffPack
{
    public static class ExtraPublicFields
    {
        public static List<ShopItemController> ItemControllers(this BaseShopController shop)
        {
            return (List<ShopItemController>)typeof(BaseShopController).GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shop);
        }

        public static BaseShopController BaseParentShop(this ShopItemController shop)
        {
            return (BaseShopController)typeof(ShopItemController).GetField("m_baseParentShop", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shop);
        }

        public static bool IsFlipped(this FireplaceController fireplace)
        {
            return (bool)typeof(FireplaceController).GetField("m_flipped", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(fireplace);
        }

		public static void DropReward(this PunchoutAIActor self, bool isLeft, int exactItemId)
		{
			self.StartCoroutine(self.DropRewardCR(isLeft, exactItemId));
		}

		public static IEnumerator DropRewardCR(this PunchoutAIActor self, bool isLeft, int exactItemId)
		{
			if (exactItemId >= 0)
			{
				self.DroppedRewardIds.Add(exactItemId);
				while (self.state is PunchoutAIActor.ThrowAmmoState)
				{
					yield return null;
				}
				GameObject droppedItem = SpawnManager.SpawnVFX(self.DroppedItemPrefab, self.transform.position + new Vector3(-0.25f, 2.5f), Quaternion.identity);
				tk2dSprite droppedItemSprite = droppedItem.GetComponent<tk2dSprite>();
				tk2dSprite rewardSprite = PickupObjectDatabase.GetById(exactItemId).GetComponent<tk2dSprite>();
				droppedItemSprite.SetSprite(rewardSprite.Collection, rewardSprite.spriteId);
				droppedItem.GetComponent<PunchoutDroppedItem>().Init(isLeft);
			}
			yield break;
		}

		public static AttackBehaviorBase CurrentBehavior(this SequentialAttackBehaviorGroup group)
        {
			return (AttackBehaviorBase)typeof(SequentialAttackBehaviorGroup).GetProperty("currentBehavior", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(group, new object[0]);
        }

		public static int CurrentBehaviorIndex(this SequentialAttackBehaviorGroup group)
		{
			return (int)typeof(SequentialAttackBehaviorGroup).GetField("m_currentIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(group);
		}

		public static void QueueSpecificAnimation(this PlayerController self, tk2dSpriteAnimationClip clip)
		{
			typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, true);
			tk2dSpriteAnimator spriteAnimator = self.spriteAnimator;
			self.spriteAnimator.Play(clip);
		}

		public static List<CutsceneMotion> ActiveMotions(this GenericIntroDoer self)
        {
			return (List<CutsceneMotion>)typeof(GenericIntroDoer).GetField("activeMotions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
        }

		public static Vector2 AdjustInputVector(this PlayerController play, Vector2 rawInput, float cardinalMagnetAngle, float ordinalMagnetAngle)
		{
			float num = BraveMathCollege.ClampAngle360(BraveMathCollege.Atan2Degrees(rawInput));
			float num2 = num % 90f;
			float num3 = (num + 45f) % 90f;
			float num4 = 0f;
			if (cardinalMagnetAngle > 0f)
			{
				if (num2 < cardinalMagnetAngle)
				{
					num4 = -num2;
				}
				else if (num2 > 90f - cardinalMagnetAngle)
				{
					num4 = 90f - num2;
				}
			}
			if (ordinalMagnetAngle > 0f)
			{
				if (num3 < ordinalMagnetAngle)
				{
					num4 = -num3;
				}
				else if (num3 > 90f - ordinalMagnetAngle)
				{
					num4 = 90f - num3;
				}
			}
			num += num4;
			return (Quaternion.Euler(0f, 0f, num) * Vector3.right).XY() * rawInput.magnitude;
		}
	}
}
