using SpecialStuffPack.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables
{
    public class FreeGunCrate : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return "";
        }

        public float GetDistanceToPoint(Vector2 point)
        {
			Bounds bounds = sprite.GetBounds();
			Vector2 vector = transform.position.XY() + (transform.rotation * bounds.min).XY();
			Vector2 a = vector + (transform.rotation * bounds.size).XY();
			return BraveMathCollege.DistToRectangle(point, vector, a - vector);
		}

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public void Interact(PlayerController interactor)
        {
            LootEngine.GivePrefabToPlayer(itemToGive, interactor);
            LootEngine.DoDefaultItemPoof(sprite.WorldCenter, false, false);
            Destroy(gameObject);
            if (unsealRoom)
            {
                theRoomThatWillBeUnsealed.area.runtimePrototypeData.roomEvents.AddRange(unsealEvents);
                theRoomThatWillBeUnsealed.UnsealRoom();
            }
            if(startGiandoCountdown && interactor is MrGiando gnd)
            {
                gnd.ActivateCountdown();
            }
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
        }

        public GameObject itemToGive;
        public bool unsealRoom;
        public bool startGiandoCountdown;
        public RoomHandler theRoomThatWillBeUnsealed;
        public List<RoomEventDefinition> unsealEvents;
    }
}
