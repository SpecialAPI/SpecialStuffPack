using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    [HarmonyPatch]
    public class SynergyCompletionGun : MonoBehaviour
    {
        public static void Init()
        {
            string name = "Lich's Old Gun";
            string shortdesc = "Empty";
            string longdesc = "The Lich's old gun. Weathered with time, this gun might not shoot, but its strong magic energy might prove useful somehow...";
            var gun = EasyGunInit("lichgun", name, shortdesc, longdesc, "lichgun_idle_001", "lichgun_idle_001", "gunsprites/lichgun/", 1, 1f, new(24, 12), GetGunById(537).CriticalMuzzleFlashEffects, "BlackRevolver", 
				PickupObject.ItemQuality.A, GunClass.NONE, out var finish);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                cooldownTime = 0.15f,
                projectiles = new()
                {
                    GetProjectile(210)
                },
                numberOfShotsInClip = 1,
                angleVariance = 0f
            });
            var volley = gun.AddComponent<AdvancedVolleyReplacementSynergyProcessor>();
            volley.RequiredSynergy =
                gun.AddComponent<NoShootyShootyGun>().enableShootyShootySynergy =
                gun.AddComponent<AdvancedInfiniteAmmoSynergyProcessor>().RequiredSynergy = "Gun and Bullets";
            volley.SynergyVolley = new()
            {
                projectiles = new()
                {
                    new()
                    {
                        cooldownTime = 0.15f,
                        projectiles = new()
                        {
							GetProjectile(210)
						},
                        numberOfShotsInClip = 3,
                        angleVariance = 0f
                    }
                }
            };
			gun.AddComponent<SynergyCompletionGun>();
			gun.AddComponent<ExcludeGunInBlessedMode>();
            finish();
			gun.SetupUnlockOnFlag(GungeonFlags.GUNSLINGER_PAST_KILLED, true);
		}

        [HarmonyPatch(typeof(AdvancedSynergyEntry), nameof(AdvancedSynergyEntry.SynergyIsAvailable))]
        [HarmonyPrefix]
        public static bool AddLichGun(AdvancedSynergyEntry __instance, ref bool __result, PlayerController p, PlayerController p2, int additionalID = -1)
        {
			if (__instance.ActivationStatus == SynergyEntry.SynergyActivation.INACTIVE)
			{
				return false;
			}
			if (__instance.ActivationStatus == SynergyEntry.SynergyActivation.DEMO)
			{
				return false;
			}
			bool hasLichEye = __instance.PlayerHasSynergyCompletionItem(p) || __instance.PlayerHasSynergyCompletionItem(p2);
			bool hasLichGun = __instance.PlayerHasSynergyCompletionGun(p) || __instance.PlayerHasSynergyCompletionGun(p2);
			if (__instance.IgnoreLichEyeBullets)
			{
				hasLichEye = false;
				hasLichGun = false;
			}
			int mandatoryGunsOwned = 0;
			int mandatoryItemsOwned = 0;
			for (int i = 0; i < __instance.MandatoryGunIDs.Count; i++)
			{
				if (__instance.PlayerHasPickup(p, __instance.MandatoryGunIDs[i]) || __instance.PlayerHasPickup(p2, __instance.MandatoryGunIDs[i]) || __instance.MandatoryGunIDs[i] == additionalID)
				{
					mandatoryGunsOwned++;
				}
			}
			for (int j = 0; j < __instance.MandatoryItemIDs.Count; j++)
			{
				if (__instance.PlayerHasPickup(p, __instance.MandatoryItemIDs[j]) || __instance.PlayerHasPickup(p2, __instance.MandatoryItemIDs[j]) || __instance.MandatoryItemIDs[j] == additionalID)
				{
					mandatoryItemsOwned++;
				}
			}
			int optionalGunsOwned = 0;
			int optionalItemsOwned = 0;
			for (int k = 0; k < __instance.OptionalGunIDs.Count; k++)
			{
				if (__instance.PlayerHasPickup(p, __instance.OptionalGunIDs[k]) || __instance.PlayerHasPickup(p2, __instance.OptionalGunIDs[k]) || __instance.OptionalGunIDs[k] == additionalID)
				{
					optionalGunsOwned++;
				}
			}
			for (int l = 0; l < __instance.OptionalItemIDs.Count; l++)
			{
				if (__instance.PlayerHasPickup(p, __instance.OptionalItemIDs[l]) || __instance.PlayerHasPickup(p2, __instance.OptionalItemIDs[l]) || __instance.OptionalItemIDs[l] == additionalID)
				{
					optionalItemsOwned++;
				}
			}
			bool onlyOptionalGuns = __instance.MandatoryItemIDs.Count > 0 && __instance.MandatoryGunIDs.Count == 0 && __instance.OptionalGunIDs.Count > 0 && __instance.OptionalItemIDs.Count == 0;
			bool onlyOptionalItems = __instance.MandatoryGunIDs.Count > 0 && __instance.MandatoryItemIDs.Count == 0 && __instance.OptionalItemIDs.Count > 0 && __instance.OptionalGunIDs.Count == 0;
			if (((__instance.MandatoryGunIDs.Count > 0 && mandatoryGunsOwned > 0) || (onlyOptionalGuns && optionalGunsOwned > 0)) && hasLichEye)
			{
				mandatoryGunsOwned++;
				mandatoryItemsOwned++;
			}
			if (((__instance.MandatoryItemIDs.Count > 0 && mandatoryItemsOwned > 0) || (onlyOptionalItems && optionalGunsOwned > 0)) && hasLichGun)
			{
				mandatoryGunsOwned++;
				mandatoryItemsOwned++;
			}
			if (mandatoryGunsOwned < __instance.MandatoryGunIDs.Count || mandatoryItemsOwned < __instance.MandatoryItemIDs.Count)
			{
				return false;
			}
			int allItemsOwned = __instance.MandatoryItemIDs.Count + __instance.MandatoryGunIDs.Count + optionalGunsOwned + optionalItemsOwned;
			int gunsOwned = __instance.MandatoryGunIDs.Count + optionalGunsOwned;
			int itemsOwned = __instance.MandatoryItemIDs.Count + optionalItemsOwned;
			if (gunsOwned > 0 && (__instance.MandatoryGunIDs.Count > 0 || onlyOptionalGuns || (__instance.RequiresAtLeastOneGunAndOneItem && gunsOwned > 0)) && hasLichEye)
			{
				itemsOwned++;
				gunsOwned++;
				allItemsOwned += 2;
			}
			if (itemsOwned > 0 && (__instance.MandatoryItemIDs.Count > 0 || onlyOptionalItems || (__instance.RequiresAtLeastOneGunAndOneItem && itemsOwned > 0)) && hasLichGun)
			{
				itemsOwned++;
				gunsOwned++;
				allItemsOwned += 2;
			}
			if (__instance.RequiresAtLeastOneGunAndOneItem && __instance.OptionalGunIDs.Count + __instance.MandatoryGunIDs.Count > 0 && __instance.OptionalItemIDs.Count + __instance.MandatoryItemIDs.Count > 0 && 
				(gunsOwned < 1 || itemsOwned < 1))
			{
				return false;
			}
			int itemsRequired = Mathf.Max(2, __instance.NumberObjectsRequired);
			__result |= allItemsOwned >= itemsRequired;
			return false;
		}
    }
}
