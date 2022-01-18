using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class LobberGun : MonoBehaviour
    {
        public void Start()
        {
            GetComponent<Gun>().PostProcessProjectile += DirectLobProjectile; 
        }

        public void DirectLobProjectile(Projectile proj)
        {
            if(proj is LobbedProjectile)
            {
                (proj as LobbedProjectile).SetDestination((GetComponent<Gun>().CurrentOwner as PlayerController).unadjustedAimPoint);
            }
        }
    }
}
