using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.EnumExtensions
{
    public class EnumExtensionAttribute : Attribute
    {
        public EnumExtensionAttribute(Type extensiontype)
        {
            type = extensiontype;
        }

        public Type type;
    }
}
