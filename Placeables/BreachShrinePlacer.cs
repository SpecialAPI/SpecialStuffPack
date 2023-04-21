using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables
{
    [HarmonyPatch]
    public static class BreachShrinePlacer
    {
        public static void RegisterBreachShrine(GameObject shrine, Vector2 pos)
        {
            stuff.Add(new(shrine, pos));
        }

        [HarmonyPatch(typeof(Foyer), nameof(Foyer.Awake))]
        [HarmonyPostfix]
        public static void PlaceStuff(Foyer __instance)
        {
            if (__instance.name != "Foyer_Base") // why is this even a thing
            {
                __instance.GetOrAddComponent<Placer>();
            }
        }

        public class Placer : MonoBehaviour
        {
            public void Start()
            {
                foreach (var kvp in stuff)
                {
                    var room = GameManager.Instance.Dungeon.data.GetRoomFromPosition(kvp.Value.ToIntVector2());
                    if (room != null)
                    {
                        var obj = Instantiate(kvp.Key, room.hierarchyParent);
                        obj.transform.position = kvp.Value;
                        var interactables = obj.GetComponentsInChildren<IPlayerInteractable>();
                        foreach (var interactable in interactables)
                        {
                            room.RegisterInteractable(interactable);
                        }
                        var sprites = obj.GetComponentsInChildren<tk2dSprite>();
                        foreach(var sprite in sprites)
                        {
                            sprite.UpdateZDepth();
                        }
                    }
                }
            }
        }

        public static List<KeyValuePair<GameObject, Vector2>> stuff = new();
    }
}
