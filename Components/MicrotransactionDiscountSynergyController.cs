using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class MicrotransactionDiscountSynergyController : MonoBehaviour
    {
        public static void MaybeReduceCost(Action<Gun, ProjectileModule> orig, Gun self, ProjectileModule proj)
        {
            int cachedCost = self.CurrencyCostPerShot;
            bool changedCost = false;
            if (self.GetComponent<MicrotransactionDiscountSynergyController>() != null && Random.value < 0.25f && self.RequiresFundsToShoot && self.CurrentOwner != null && self.CurrentOwner is PlayerController && 
                (self.CurrentOwner as PlayerController).PlayerHasActiveSynergy(self.GetComponent<MicrotransactionDiscountSynergyController>().SynergyToCheck))
            {
                changedCost = true;
                self.CurrencyCostPerShot = 0;
            }
            orig(self, proj);
            if (changedCost)
            {
                self.CurrencyCostPerShot = cachedCost;
            }
        }

        public string SynergyToCheck;
    }
}
