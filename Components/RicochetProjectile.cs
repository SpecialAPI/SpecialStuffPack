using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class RicochetProjectile : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
                projectile.OnHitEnemy += Ricochet;
            }
        }

        public void Ricochet(Projectile proj, SpeculativeRigidbody rb, bool fatal)
        {
            if(proj.GetComponent<BounceProjModifier>() != null && proj.GetComponent<BounceProjModifier>().numberOfBounces > 0 && rb != null && rb.aiActor != null)
            {
                proj.GetComponent<BounceProjModifier>().numberOfBounces--;
                proj.GetOrAddPierce().penetration++;
                var dir = Random.insideUnitCircle;
                if(rb.aiActor.ParentRoom != null && rb.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    var t = rb.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).FindAll(x => x != null && x != rb.aiActor && x.HasBeenEngaged && x.healthHaver != null && x.healthHaver.IsVulnerable);
                    if(t.Count > 0)
                    {
                        dir = t.RandomElement().CenterPosition - proj.specRigidbody.UnitCenter;
                    }
                }
                proj.SendInDirection(dir, false);
            }
        }
    }
}
