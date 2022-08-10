using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class WeightedFishingRewardCollection : ScriptableObject
    {
		public void Add(FishingReward item, float weight)
		{
			if (elements == null)
			{
				elements = new List<WeightedFishingReward>();
			}
			elements.Add(new WeightedFishingReward(item, weight));
		}

		public FishingReward SelectByWeight()
		{
			if (elements == null || elements.Count == 0)
			{
				return null;
			}
			float num = 0f;
			for (int i = 0; i < elements.Count; i++)
			{
				num += elements[i].weight;
			}
			float num2 = UnityEngine.Random.value * num;
			float num3 = 0f;
			for (int j = 0; j < elements.Count; j++)
			{
				num3 += elements[j].weight;
				if (num3 > num2)
				{
					return elements[j].value;
				}
			}
			return elements[elements.Count - 1].value;
		}

		public List<WeightedFishingReward> elements;
	}

	public class WeightedFishingReward : ScriptableObject
	{
		public WeightedFishingReward(FishingReward v, float w)
		{
			value = v;
			weight = w;
		}

		public FishingReward value;

		public float weight;
	}

}
