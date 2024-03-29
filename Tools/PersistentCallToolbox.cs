﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace SpecialStuffPack
{
    public static class PersistentCallToolbox
    {
        public static void AddPersistentCall(this UnityEventBase self, UnityEngine.Object obj, string methodName)
        {
            if(self == null || obj == null || string.IsNullOrEmpty(methodName))
            {
                return;
            }
            var method = obj.GetType().GetMethod(methodName);
            if(method == null)
            {
                return;
            }
            /*if(method.GetGenericArguments() != null && method.GetGenericArguments().Length > 0)
            {
                return;
            }
            var eventGenericArguments = 0;
            var genericParams = self.GetType().GetGenericArguments();
            var param = method.GetParameters();
            if (genericParams != null)
            {
                eventGenericArguments = genericParams.Length;
            }
            if (param == null || (param != null && param.Length != eventGenericArguments))
            {
                return;
            }
            if (genericParams != null && param != null)
            {
                for (int i = 0; i < eventGenericArguments; i++)
                {
                    if(param[i].ParameterType != genericParams[i])
                    {
                        return;
                    }
                }
            }*/
            var group = self.GetType().GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
            var callList = group.GetType().GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance).FieldType;
            var callType = callList.GetGenericArguments()[0];
            var call = Activator.CreateInstance(callType);
            call.GetType().GetMethod("RegisterPersistentListener").Invoke(call, new object[] { obj, methodName });
            group.GetType().GetMethod("AddListener").Invoke(group, new object[] { call });
        }
    }
}
