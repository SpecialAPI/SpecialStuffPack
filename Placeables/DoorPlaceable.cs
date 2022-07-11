using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables
{
    public class DoorPlaceable : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

		public float GetDistanceToPoint(Vector2 point)
		{
			if (!gameObject.activeSelf)
			{
				return 10000f;
			}
			if (!sprite)
			{
				return 1000f;
			}
			Bounds bounds = sprite.GetBounds();
			Vector2 vector = transform.position.XY() + (transform.rotation * bounds.min).XY();
			Vector2 a = vector + (transform.rotation * bounds.size).XY();
			return BraveMathCollege.DistToRectangle(point, vector, a - vector);
		}

		public float GetOverrideMaxDistance()
        {
            return -1;
        }

        public void Interact(PlayerController interactor)
        {
            if (TextBoxManager.HasTextBox(talkpoint))
            {
                return;
            }
			TextBoxManager.ShowStoneTablet(talkpoint.position, talkpoint, -1f, StringTableManager.GetString("#STRANGE_DOOR_DESC"), true, false);
        }

		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white);
		}

		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
			TextBoxManager.ClearTextBox(talkpoint);
		}

		public Transform talkpoint;
    }
}
