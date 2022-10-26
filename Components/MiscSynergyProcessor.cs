using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class MiscSynergyProcessor : BraveBehaviour
    {
        public void Awake()
        {
            gun = GetComponent<Gun>();
            passive = GetComponent<PassiveItem>();
            active = GetComponent<PlayerItem>();
            if(gun != null)
            {
                cachedFundsToShoot = gun.RequiresFundsToShoot;
            }
        }

        public void Update()
        {
            if (SynergyActive)
            {
                if (toggleFundsToShoot)
                {
                    gun.RequiresFundsToShoot = !cachedFundsToShoot;
                }
            }
            else
            {
                if(gun != null)
                {
                    if (toggleFundsToShoot)
                    {
                        gun.RequiresFundsToShoot = cachedFundsToShoot;
                    }
                }
            }
        }

        public bool SynergyActive
        {
            get
            {
                var player = Player;
                if (player != null)
                {
                    return player.PlayerHasActiveSynergy(synergy);
                }
                return false;
            }
        }

        public PlayerController Player => passive != null ? passive.Owner : active != null ? active.LastOwner : gun != null ? gun.CurrentOwner as PlayerController : null;

        public string synergy;

        public bool toggleFundsToShoot;

        public Gun gun;
        public PassiveItem passive;
        public PlayerItem active;

        private bool cachedFundsToShoot;
    }
}
