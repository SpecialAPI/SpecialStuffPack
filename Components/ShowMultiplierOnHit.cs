using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ShowMultiplierOnHit : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += ShowMultiplier;
        }

        public void ShowMultiplier(Projectile proj, SpeculativeRigidbody rigidbody, bool fatal)
        {
            if(rigidbody != null && rigidbody.aiActor != null)
            {
                var pos = rigidbody.UnitCenter.ToVector3ZisY(0f);
                DoCustomTextPopup(pos, pos.y - rigidbody.UnitBottomCenter.y, $"{mult}x", colorGradient.Evaluate(mult));
            }
        }

        public float mult;
        public Gradient colorGradient;
    }
}
