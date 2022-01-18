using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;

namespace SpecialStuffPack.Components
{
	public class UniqueOutlineItem : PassiveItem, IPlayerInteractable
	{
		public void Start()
		{
			if (!m_pickedUp)
			{
				SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
                if (!OverrideAddBlackOutline())
				{
					SpriteOutlineManager.AddOutlineToSprite(sprite, blackReplacement, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
				}
			}
		}

		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
            if (!OverrideAddBlackOutline())
			{
				SpriteOutlineManager.AddOutlineToSprite(sprite, blackReplacement, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			return result;
		}

		public new void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			if (!RoomHandler.unassignedInteractableObjects.Contains(this))
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
            if (!OverrideAddWhiteOutline())
			{
				SpriteOutlineManager.AddOutlineToSprite(sprite, whiteReplacement, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			sprite.UpdateZDepth();
			SquishyBounceWiggler component = GetComponent<SquishyBounceWiggler>();
			if (component != null)
			{
				component.WiggleHold = true;
			}
		}

		public new void OnExitRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
			if (m_pickedUp)
			{
				return;
			}
            if (!OverrideAddBlackOutline())
            {
				SpriteOutlineManager.AddOutlineToSprite(sprite, blackReplacement, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			sprite.UpdateZDepth();
			SquishyBounceWiggler component = GetComponent<SquishyBounceWiggler>();
			if (component != null)
			{
				component.WiggleHold = false;
			}
		}

		protected virtual bool OverrideAddWhiteOutline()
        {
			return false;
        }

		protected virtual bool OverrideAddBlackOutline()
		{
			return false;
		}

		public Color blackReplacement;
		public Color whiteReplacement;
	}
}
