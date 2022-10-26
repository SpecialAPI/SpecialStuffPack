using SpecialStuffPack.SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    public class SpecialDungeonPrerequisite : CustomDungeonPrerequisite
    {
        public override bool CheckConditionsFulfilled()
        {
            if(specialPrerequisiteType == SpecialPrerequisiteType.ITEM_FLAG)
            {
                return PassiveItem.IsFlagSetAtAll(flagToCheck);
            }
            else if(specialPrerequisiteType == SpecialPrerequisiteType.SYNERGY)
            {
                return AnyoneHasActiveSynergy(synergyToCheck) == shouldHaveSynergy;
            }
            return base.CheckConditionsFulfilled();
        }

        public SpecialPrerequisiteType specialPrerequisiteType;
        public Type flagToCheck;
        public string synergyToCheck;
        public bool shouldHaveSynergy = true;

        public enum SpecialPrerequisiteType
        {
            NONE,
            ITEM_FLAG,
            SYNERGY
        }
    }
}
