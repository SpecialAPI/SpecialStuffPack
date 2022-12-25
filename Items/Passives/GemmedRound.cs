using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class GemmedRound : PassiveItem
    {
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.IncrementFlag<GemmedRound>();
            player.healthHaver.ModifyDamage += Overpower;
        }

        public void Overpower(HealthHaver h, HealthHaver.ModifyDamageEventArgs a)
        {
            if(a == EventArgs.Empty)
            {
                return;
            }
            if(!PickedUp || Owner == null || Owner.passiveItems == null || this != Owner.passiveItems.OfType<GemmedRound>().FirstOrDefault())
            {
                return;
            }
            var increase = GetGemmedRounds();
            if(increase <= 0)
            {
                return;
            }
            a.ModifiedDamage *= increase;
        }

        public override void DisableEffect(PlayerController player)
        {
            if (player == null)
            {
                return;
            }
            base.DisableEffect(player);
            player.DecrementFlag<GemmedRound>();
            player.healthHaver.ModifyDamage -= Overpower;
        }

        public int GetGemmedRounds()
        {
            return Mathf.Max(0, Owner.GetFlagCount<GemmedRound>() - Owner.GetFlagCount<DiamondItem>());
        }
    }
}
