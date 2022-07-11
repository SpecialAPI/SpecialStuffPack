using HutongGames.PlayMaker.Actions;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class BinaryGunProjectile : HelixProjectile
    {
        public override HandleDamageResult HandleDamage(SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget, PlayerController player, bool alreadyPlayerDelayed = false)
        {
            if(rigidbody != null)
            {
                if (rigidbody.GetComponent<BinaryGunMarkedBehaviour>() != null && rigidbody.aiActor != null)
                {
                    if (rigidbody.GetComponent<BinaryGunMarkedBehaviour>().IsOne == !IsOne)
                    {
                        if (Owner != null && Owner is PlayerController && !string.IsNullOrEmpty(InstakillSynergy) && (Owner as PlayerController).PlayerHasActiveSynergy(InstakillSynergy))
                        {
                            if (rigidbody.healthHaver != null && rigidbody.aiActor != null && !rigidbody.healthHaver.IsBoss)
                            {
                                rigidbody.GetComponent<BinaryGunMarkedBehaviour>().Mark(IsOne);
                                rigidbody.aiActor.EraseFromExistenceWithRewards(true);
                                killedTarget = true;
                                return HandleDamageResult.HEALTH_AND_KILLED;
                            }
                            else if(rigidbody.healthHaver != null && rigidbody.aiActor != null && rigidbody.healthHaver.IsBoss)
                            {
                                base.HandleDamage(rigidbody, hitPixelCollider, out var _, player, alreadyPlayerDelayed);
                            }
                        }
                        base.HandleDamage(rigidbody, hitPixelCollider, out var _, player, alreadyPlayerDelayed);
                    }
                }
                else if (rigidbody.GetComponent<BinaryGunMarkedBehaviour>() == null && rigidbody.aiActor != null)
                {
                    BinaryGunMarkedBehaviour mark = rigidbody.AddComponent<BinaryGunMarkedBehaviour>();
                    mark.MarkVFX = MarkVFX;
                    mark.VFXOffset = VFXOffset;
                    mark.ZeroId = ZeroId;
                    mark.OneId = OneId;
                    mark.Mark(IsOne);
                }
            }
            return base.HandleDamage(rigidbody, hitPixelCollider, out killedTarget, player, alreadyPlayerDelayed);
        }

        public bool IsOne;
        public GameObject MarkVFX;
        public Vector2 VFXOffset;
        public int ZeroId;
        public int OneId;
        public string InstakillSynergy;
    }
}
