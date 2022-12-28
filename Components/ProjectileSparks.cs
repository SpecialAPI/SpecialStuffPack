using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ProjectileSparks : BraveBehaviour
    {
        public void Update()
        {
            sparkTimer += BraveTime.DeltaTime;
            if(sparkTimer >= sparkCooldown)
            {
                GlobalSparksDoer.DoSingleParticle(specRigidbody.UnitCenter + offset.Rotate(projectile.Direction.ToAngle()), BraveMathCollege.DegreesToVector(-projectile.Direction.ToAngle() + Random.Range(-angleVariance, angleVariance)),
                    size, startingLifetime, color, type);
                sparkTimer = 0f;
            }
        }

        public float sparkCooldown;
        public Vector2 offset;
        public float size;
        public float startingLifetime;
        public float angleVariance;
        public Color color;
        public GlobalSparksDoer.SparksType type;
        private float sparkTimer;
    }
}
