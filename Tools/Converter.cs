using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public static class Converter
    {
        public static GameObject GetGameObject(this PrototypePlacedObjectData placeable)
        {
            if (placeable.unspecifiedContents != null)
            {
                return placeable.unspecifiedContents;
            }
            else if (placeable.nonenemyBehaviour != null)
            {
                return placeable.nonenemyBehaviour.gameObject;
            }
            else if(placeable.placeableContents != null && placeable.placeableContents.variantTiers != null)
            {
                return placeable.placeableContents.variantTiers.ConvertAll((DungeonPlaceableVariant vari) => vari.GetOrLoadPlaceableObject).Where((GameObject go) => go != null).ToList()[0];
            }
            return null;
        }

        public static GameObject GetGameObject(this DungeonPlaceable placeable)
        {
            if (placeable != null && placeable.variantTiers != null)
            {
                return placeable.variantTiers.ConvertAll((DungeonPlaceableVariant vari) => vari.GetOrLoadPlaceableObject).Where((GameObject go) => go != null).ToList()[0];
            }
            return null;
        }

        public static Dictionary<T, T2> ToDictionary<T, T2>(this List<KeyValuePair<T, T2>> list)
        {
            Dictionary<T, T2> dict = new Dictionary<T, T2>();
            if(list != null)
            {
                foreach(KeyValuePair<T, T2> p in list)
                {
                    if (p.Key != null && p.Value != null && !dict.ContainsKey(p.Key))
                    {
                        try
                        {
                            dict.Add(p.Key, p.Value);
                        }
                        catch { } 
                    }
                }
            }
            return dict;
        }
    }
}
