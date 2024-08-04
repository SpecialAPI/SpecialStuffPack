using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class Immortal : BraveBehaviour
    {
        public void Start()
        {
            healthHaver.Ext().ModifyDamageContextLater += DivineProtect;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            healthHaver.Ext().ModifyDamageContextLater -= DivineProtect;
        }

        public void DivineProtect(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args, Vector2 direction, string source, CoreDamageTypes type, DamageCategory category, bool ignoreInvulnerability, bool ignoreDPSCaps)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (hh.aiActor != null)
            {
                if (DivineProtectionStatusEffect.DivineProtectionEffect(hh.aiActor, args.ModifiedDamage, direction, source, type))
                {
                    args.ModifiedDamage = 0f;
                }
            }
        }
    }
}
