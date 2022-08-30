using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class IndoctrinateJammedEnemies : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
                projectile.OnHitEnemy += TryIndoctrinate;
            }
        }

        public void TryIndoctrinate(Projectile proj, SpeculativeRigidbody rigidbody, bool fatal)
        {
            if(rigidbody.aiActor != null && rigidbody.aiActor.IsBlackPhantom && Random.value < chanceToApply)
            {
                rigidbody.aiActor.ApplyEffect(indoctrinateEffect);
            }
        }

        public GameActorEffect indoctrinateEffect;
        public float chanceToApply;
    }
}
