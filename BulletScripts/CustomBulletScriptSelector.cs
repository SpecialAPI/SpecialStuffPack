using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;

namespace SpecialStuffPack.BulletScripts
{
    public class CustomBulletScriptSelector : BulletScriptSelector
    {
        public CustomBulletScriptSelector(Type _bulletType)
        {
            bulletType = _bulletType;
            scriptTypeName = bulletType.AssemblyQualifiedName;
        }

        public new Bullet CreateInstance()
        {
            if (bulletType == null)
            {
                ETGModConsole.Log("Unknown type! " + scriptTypeName);
                return null;
            }
            return (Bullet)Activator.CreateInstance(bulletType);
        }

        public new bool IsNull
        {
            get
            {
                return string.IsNullOrEmpty(scriptTypeName) || scriptTypeName == "null";
            }
        }

        public new BulletScriptSelector Clone()
        {
            return new CustomBulletScriptSelector(bulletType);
        }

        public Type bulletType;
    }
}
