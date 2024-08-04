using SpecialStuffPack.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class TarotCardRoomPlaceable : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!sprite)
            {
                return 1000f;
            }
            Bounds bounds = sprite.GetBounds();
            bounds.SetMinMax(bounds.min + transform.position, bounds.max + transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            MaybeAssignTarot();
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            sprite.UpdateZDepth();
            TextBoxManager.ShowTextBox(sprite.WorldTopCenter + Vector2.up / 4f, transform, -1f, $"{card.EncounterNameOrDisplayName}\n{card.encounterTrackable.journalData.GetAmmonomiconFullEntry(false, false)}", "", true,
                TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            sprite.UpdateZDepth();
            TextBoxManager.ClearTextBox(transform);
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            MaybeAssignTarot();
            SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
            TextBoxManager.ClearTextBoxImmediate(transform);
            LootEngine.GivePrefabToPlayer(card.gameObject, interactor);
            LootEngine.DoDefaultItemPoof(sprite.WorldCenter, false, false);
            if(room != null && room.GetRoomInteractables() != null)
            {
                var toRemove = room.GetRoomInteractables().Where(x => x != null && x is TarotCardRoomPlaceable).ToArray();
                foreach(var car in toRemove)
                {
                    room.DeregisterInteractable(car);
                    LootEngine.DoDefaultItemPoof((car as TarotCardRoomPlaceable).sprite.WorldCenter, false, false);
                    Destroy((car as TarotCardRoomPlaceable).gameObject);
                }
            }
            Destroy(gameObject);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public void MaybeAssignTarot()
        {
            if(card == null)
            {
                card = TarotCards.GetTarotCardForPlayer();
                TarotCards.seenTarotCards.Add(card.type);
            }
        }

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.room = room;
            room.Entered += RegisterSeen;
        }

        public void RegisterSeen(PlayerController p)
        {
            MaybeAssignTarot();
        }

        public RoomHandler room;
        public TarotCards card;
    }
}
