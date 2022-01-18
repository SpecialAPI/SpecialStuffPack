using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Controls
{
    public class SpecialGungeonActions : PlayerActionSet
    {
        public SpecialGungeonActions()
        {
            HoldSecondPlayerAction = CreatePlayerAction("Hold Second Player");
        }

        public void ReinitializeDefaults()
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].ResetBindings();
            }
        }

        public void IgnoreBindingsOfType(BindingSourceType sourceType)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].IgnoreBindingsOfType(sourceType);
            }
        }

        public PlayerAction GetActionFromType(SpecialGungeonActionType type)
        {
            switch (type)
            {
                case SpecialGungeonActionType.HoldSecondPlayer:
                    return HoldSecondPlayerAction;
            }
            return null;
        }

        public void InitializeDefaults()
        {
            HoldSecondPlayerAction.AddDefaultBinding(Mouse.MiddleButton);
            HoldSecondPlayerAction.AddDefaultBinding(InputControlType.RightStickButton);
        }

        public PlayerAction HoldSecondPlayerAction;

        public enum SpecialGungeonActionType
        {
            HoldSecondPlayer
        }
    }
}
