using SpecialStuffPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public class TCultistController : PlayerController
    {
        public override void Update()
        {
            base.Update();
            if(specRigidbody != null && GameManager.HasInstance && GameManager.Instance.GetOtherPlayer(this) != null && GameManager.Instance.GetOtherPlayer(this).specRigidbody != null && PhysicsEngine.HasInstance)
            {
                List<SpeculativeRigidbody> overlaps = PhysicsEngine.Instance.GetOverlappingRigidbodies(specRigidbody);
                if(overlaps != null && overlaps.Contains(GameManager.Instance.GetOtherPlayer(this).specRigidbody))
                {
                    GameManager.Instance.GetOtherPlayer(this).specRigidbody.RegisterGhostCollisionException(specRigidbody);
                    specRigidbody.RegisterGhostCollisionException(GameManager.Instance.GetOtherPlayer(this).specRigidbody);
                }
            }
            if(canHold && BraveInput.HasInstanceForPlayer(0) && BraveInput.GetInstanceForPlayer(0) != null && BraveInput.GetInstanceForPlayer(0).SpecialInput() != null && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions != null
                && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions.HoldSecondPlayerAction.IsPressed)
            {
                IsStationary = true;
            }
            else if(canHold && BraveInput.HasInstanceForPlayer(0) && BraveInput.GetInstanceForPlayer(0) != null && BraveInput.GetInstanceForPlayer(0).SpecialInput() != null && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions != null
                && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions.HoldSecondPlayerAction.WasReleased)
            {
                IsStationary = false;
            }
        }

        public override Vector2 HandlePlayerInput()
        {
            Vector2 result = base.HandlePlayerInput();
            if ((canHold && BraveInput.HasInstanceForPlayer(0) && BraveInput.GetInstanceForPlayer(0) != null && BraveInput.GetInstanceForPlayer(0).SpecialInput() != null && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions != null
                && BraveInput.GetInstanceForPlayer(0).SpecialInput().ActiveActions.HoldSecondPlayerAction.IsPressed) || (specRigidbody != null && GameManager.HasInstance && GameManager.Instance.GetOtherPlayer(this) != null 
                && GameManager.Instance.GetOtherPlayer(this).specRigidbody != null && specRigidbody.GhostCollisionExceptions != null && specRigidbody.GhostCollisionExceptions.Contains(GameManager.Instance.GetOtherPlayer(this).specRigidbody)))
            {
                return Vector2.zero;
            }
            return result;
        }

        public bool canHold;
    }
}
