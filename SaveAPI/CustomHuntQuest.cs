using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.SaveAPI
{
	[Serializable]
	public class CustomHuntQuest : MonsterHuntQuest
	{
		public new bool IsQuestComplete()
		{
			if (CustomQuestFlag != CustomDungeonFlags.NONE && AdvancedGameStatsManager.Instance.GetFlag(CustomQuestFlag))
            {
				return true;
            }
			return GameStatsManager.Instance.GetFlag(QuestFlag);
		}

		public bool IsEnemyValid(AIActor enemy, MonsterHuntProgress progress)
        {
			if(ValidTargetCheck != null && !ValidTargetCheck(enemy, progress))
            {
				return false;
            }
			return SaveTools.IsEnemyStateValid(enemy, RequiredEnemyState);
        }

		public void Complete()
        {
			if(QuestFlag != GungeonFlags.NONE)
            {
				GameStatsManager.Instance.SetFlag(QuestFlag, true);
			}
			if(CustomQuestFlag != CustomDungeonFlags.NONE)
            {
				AdvancedGameStatsManager.Instance.SetFlag(CustomQuestFlag, true);
			}
        }

		public new void UnlockRewards()
		{
			for (int i = 0; i < FlagsToSetUponReward.Count; i++)
			{
				GameStatsManager.Instance.SetFlag(FlagsToSetUponReward[i], true);
			}
			for (int i = 0; i < CustomFlagsToSetUponReward.Count; i++)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomFlagsToSetUponReward[i], true);
			}
		}

		[LongEnum]
		[SerializeField]
		public CustomDungeonFlags CustomQuestFlag;
		[LongEnum]
		[SerializeField]
		public List<CustomDungeonFlags> CustomFlagsToSetUponReward;
		[SerializeField]
		public Func<AIActor, MonsterHuntProgress, bool> ValidTargetCheck;
		[SerializeField]
		public JammedEnemyState RequiredEnemyState;
	}
}
