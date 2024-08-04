using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.EnumExtensions
{
    [EnumExtension(typeof(ModifyMethod))]
    public class ModifyMethodE
    {
        public static ModifyMethod TrueMultiplicative;
        public static ModifyMethod Exponent;
    }
}
