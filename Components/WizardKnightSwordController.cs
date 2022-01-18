using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class WizardKnightSwordController : BraveBehaviour
    {
        public void Awake()
        {
            foreach (BraveBehaviour behav in GetComponents<BraveBehaviour>())
            {
                behav.sprite = GetComponentInChildren<tk2dBaseSprite>();
                behav.renderer = behav.sprite.renderer;
            }
        }

        public void Start()
        {
            if(Owner == null)
            {
                Owner = FindObjectOfType<WizardKnightController>();
            }
        }

        public WizardKnightController.State State
        {
            get
            {
                if(Owner == null)
                {
                    return WizardKnightController.State.None;
                }
                return Owner.CurrentState;
            }
        }

        public WizardKnightController Owner;
    }
}
