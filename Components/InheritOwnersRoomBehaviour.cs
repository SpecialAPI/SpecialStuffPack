using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class InheritOwnersRoomBehaviour : BraveBehaviour
    {
        public void Start()
        {
            companion = GetComponent<CompanionController>();
            if(companion != null && companion.m_owner != null)
            {
                companion.m_owner.OnEnteredCombat += WarpToPlayer;
            }
            if(bulletBank != null)
            {
                bulletBank.OnProjectileCreated += MakeProjectilesDealIntendedDamage;
            }
            if (aiShooter != null)
            {
                aiShooter.PostProcessProjectile += MakeProjectilesDealIntendedDamage;
            }
        }

        public void MakeProjectilesDealIntendedDamage(Projectile obj)
        {
            obj.baseData.damage = 5;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (companion != null && companion.m_owner != null)
            {
                companion.m_owner.OnEnteredCombat -= WarpToPlayer;
            }
        }

        public void WarpToPlayer()
        {
            if (companion != null && companion.m_owner != null)
            {
                aiActor.CompanionWarp(companion.m_owner.transform.position);
            }
        }

        public void Update()
        {
            if(companion != null && aiActor != null)
            {
                aiActor.ParentRoom = companion.m_owner.CurrentRoom;
            }
        }

        public CompanionController companion;
    }
}
