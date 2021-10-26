﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.SaveAPI
{
    public class CustomTrackedStatsComparer : IEqualityComparer<CustomTrackedStats>
    {
        public bool Equals(CustomTrackedStats x, CustomTrackedStats y)
        {
            return x == y;
        }

        public int GetHashCode(CustomTrackedStats obj)
        {
            return (int)obj;
        }
    }
}
