using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables
{
    public class LilChestPlacer : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
		public void ConfigureOnPlacement(RoomHandler room)
		{
			IntVector2 positionInRoom = transform.position.IntXY(VectorConversions.Round) - room.area.basePosition;
			LilChest chest;
			if (UseOverrideChest && (OverrideChestPrereq == null || OverrideChestPrereq.CheckConditionsFulfilled()))
			{
				chest = Chest.Spawn(OverrideChestPrefab, transform.position.IntXY(VectorConversions.Round)) as LilChest;
			}
			else
			{
				chest = GameManager.Instance.RewardManager.GenerationSpawnLilChestAt(positionInRoom, room, (!OverrideItemQuality) ? null : new PickupObject.ItemQuality?(ItemQuality));
			}
			chest.transform.position += new Vector3(xPixelOffset / 16f, yPixelOffset / 16f);
			if (OverrideLockChance && chest)
			{
				if (Random.value < LockChance || (ForceUnlockedIfWooden && chest.lootTable.D_Chance == 1f))
				{
					chest.ForceUnlock();
				}
				else
				{
					chest.IsLocked = true;
				}
			}
			if(brokenLockChance > 0 && Random.value < brokenLockChance)
            {
				chest.BreakLock();
			}
			if (secretRainbowChance > 0 && Random.value < secretRainbowChance)
			{
				chest.forceHiddenRainbow = true;
			}
			if (glitchChance > 0 && Random.value < glitchChance)
			{
				chest.BecomeGlitchChest();
			}
			Destroy(gameObject);
		}

		public bool OverrideItemQuality;
		public PickupObject.ItemQuality ItemQuality;
		public int xPixelOffset;
		public int yPixelOffset;
		//public bool CenterChestInRegion;
		public bool OverrideLockChance;
		public bool ForceUnlockedIfWooden;
		public float LockChance = 0.5f;
		public float brokenLockChance;
		public float secretRainbowChance;
		public float glitchChance;
		public bool UseOverrideChest;
		public DungeonPrerequisite OverrideChestPrereq;
		public LilChest OverrideChestPrefab;
	}
}
