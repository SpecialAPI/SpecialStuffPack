using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;

namespace SpecialStuffPack.Components
{
	public class NoOutlineItem : PassiveItem, IPlayerInteractable
	{
		public new void Start()
		{
			if (!m_pickedUp)
			{
				SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
			}
		}

		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite);
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
			sprite.UpdateZDepth();
			SquishyBounceWiggler component = GetComponent<SquishyBounceWiggler>();
			if (component != null)
			{
				component.WiggleHold = false;
			}
		}

		public Color blackReplacement;
		public Color whiteReplacement;
	}
}
