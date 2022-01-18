using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    public static class ListTools
    {
        public static void SplitDictionary<T, T2>(this Dictionary<T, T2> dictionary, out List<T> keys, out List<T2> values)
        {
            keys = dictionary.Keys.ToList();
            values = dictionary.Values.ToList();
        }
    }
}
