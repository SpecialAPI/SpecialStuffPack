using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecialStuffPack
{
    public class GenericFieldInfo<T>
    {
        public GenericFieldInfo(FieldInfo inf)
        {
            capsuledInfo = inf;
        }

        public T GetValue(object obj)
        {
            return (T)capsuledInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            capsuledInfo.SetValue(obj, value);
        }

        public static implicit operator FieldInfo(GenericFieldInfo<T> inf)
        {
            return inf.capsuledInfo;
        }

        public FieldInfo capsuledInfo;
    }
}
