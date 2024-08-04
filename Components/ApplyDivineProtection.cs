using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ApplyDivineProtection : BraveBehaviour
    {
        public void Start()
        {
            if(specRigidbody != null)
            {
                specRigidbody.OnPreRigidbodyCollision += MaybeApplyDivineProtection;
            }
        }

        public void MaybeApplyDivineProtection(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody != null && otherRigidbody.aiActor != null)
            {
                otherRigidbody.aiActor.ApplyEffect(divineProtect);
            }
        }

        public static DivineProtectionStatusEffect divineProtect = DivineProtectionStatusEffect.DivineProtectionGenerator(1f);
    }
}
