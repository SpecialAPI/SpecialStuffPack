using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public static class MoreLogging
    {
        public static void DebugLog(string message)
        {
            Debug.Log(message);
        }

        public static void ConsoleLog(string message)
        {
            ETGModConsole.Log(message);
        }

        public static void LogComponents(this GameObject obj, bool logNormalComponents = true, bool logComponentsInChildren = true, bool logComponentsInParent = true, bool withObjectNames = true, bool isDebugLog = false, Action<string> overrideLog = null)
        {
            Action<string> log = overrideLog ?? (isDebugLog ? DebugLog : (Action<string>)ConsoleLog);
            if (logNormalComponents)
            {
                log("---------------------COMPONENTS:-------------------");
                foreach (Component component in obj.GetComponents<Component>())
                {
                    log(component.GetType().ToString() + "<color=#ff0000> " + (withObjectNames ? component.gameObject.name : "") + "</color>");
                }
            }
            if (logComponentsInChildren)
            {
                log("---------------------IN CHILDREN:-------------------");
                foreach (Component component in obj.GetComponentsInChildren<Component>())
                {
                    log(component.GetType().ToString() + "<color=#ff0000> " + (withObjectNames ? component.gameObject.name : "") + "</color>");
                }
            }
            if (logComponentsInParent)
            {
                log("---------------------IN PARENT:-------------------");
                foreach (Component component in obj.GetComponentsInParent<Component>())
                {
                    log(component.GetType().ToString() + "<color=#ff0000> " + (withObjectNames ? component.gameObject.name : "") + "</color>");
                }
            }
        }

        public static void LogComponents(this Component obj, bool logNormalComponents = true, bool logComponentsInChildren = true, bool logComponentsInParent = true, bool withObjectNames = true, bool isDebugLog = false, Action<string> overrideLog = null)
        {
            LogComponents(obj.gameObject, logNormalComponents, logComponentsInChildren, logComponentsInParent, withObjectNames, isDebugLog, overrideLog);
        }
    }
}
