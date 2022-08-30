using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ChanceBasedDropItemOnKill : BraveBehaviour
    {
        public void Start()
        {
            projectile.OnHitEnemy += ChanceToSpawnItem;
        }

        public void ChanceToSpawnItem(Projectile sourceProjectile, SpeculativeRigidbody hitRigidbody, bool fatal)
        {
            if (hitRigidbody == null)
            {
                return;
            }
            if (fatal && Random.value <= chance)
            {
                LootEngine.SpawnItem(GetItemById(itemId).gameObject, hitRigidbody.transform.position, Vector2.down, 0f, true, false, false);
            }
        }

        public float chance;
        public int itemId;
    }
}
