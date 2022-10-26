using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class DontLoseDamageOnPierce : BraveBehaviour
    {
        public void Awake()
        {
            specRigidbody.OnRigidbodyCollision += HasntPierced;
        }

        public void HasntPierced(CollisionData data)
        {
            if(data != null && data.MyRigidbody != null && data.MyRigidbody.projectile != null)
            {
                data.MyRigidbody.projectile.m_hasPierced = false;
            }
        }
    }
}
