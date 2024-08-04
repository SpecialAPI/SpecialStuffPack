using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
	public class GunBehaviourFormeSynergyProcessor : GunBehaviour
	{
		public override void Update()
		{
			if (gun && !gun.CurrentOwner && CurrentForme != 0)
			{
				ChangeForme(Formes[0]);
				CurrentForme = 0;
			}
		}

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
		{
			if (manual && !gun.IsReloading)
			{
				int nextValidForme = GetNextValidForme(player);
				if (nextValidForme != CurrentForme)
				{
					ChangeForme(Formes[nextValidForme]);
					CurrentForme = nextValidForme;
				}
			}
		}

		protected int GetNextValidForme(PlayerController ownerPlayer)
		{
			for (int i = 0; i < Formes.Length; i++)
			{
				int num = (i + CurrentForme) % Formes.Length;
				if (num != CurrentForme)
				{
					if (Formes[num].IsValid(ownerPlayer))
					{
						return num;
					}
				}
			}
			return CurrentForme;
		}

		protected void ChangeForme(AdvancedGunFormeData targetForme)
		{
			Gun gun = PickupObjectDatabase.GetById(targetForme.FormeID) as Gun;
			this.gun.TransformToTargetGun(gun);
			if (encounterTrackable && gun.encounterTrackable)
			{
				this.gun.encounterTrackable.journalData.PrimaryDisplayName = gun.encounterTrackable.journalData.PrimaryDisplayName;
				this.gun.encounterTrackable.journalData.ClearCache();
				PlayerController playerController = this.gun.CurrentOwner as PlayerController;
				if (playerController)
				{
					GameUIRoot.Instance.TemporarilyShowGunName(playerController.IsPrimaryPlayer);
				}
			}
		}

		public static void AssignTemporaryOverrideGun(PlayerController targetPlayer, int gunID, float duration)
		{
			if (targetPlayer && !targetPlayer.IsGhost)
			{
				targetPlayer.StartCoroutine(HandleTransformationDuration(targetPlayer, gunID, duration));
			}
		}

		public static IEnumerator HandleTransformationDuration(PlayerController targetPlayer, int gunID, float duration)
		{
			float elapsed = 0f;
			if (targetPlayer && targetPlayer.inventory.GunLocked.Value && targetPlayer.CurrentGun)
			{
				MimicGunController component = targetPlayer.CurrentGun.GetComponent<MimicGunController>();
				if (component)
				{
					component.ForceClearMimic(false);
				}
			}
			targetPlayer.inventory.GunChangeForgiveness = true;
			Gun limitGun = PickupObjectDatabase.GetById(gunID) as Gun;
			Gun m_extantGun = targetPlayer.inventory.AddGunToInventory(limitGun, true);
			m_extantGun.CanBeDropped = false;
			m_extantGun.CanBeSold = false;
			targetPlayer.inventory.GunLocked.SetOverride("override gun", true, null);
			elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			ClearTemporaryOverrideGun(targetPlayer, m_extantGun);
			yield break;
		}

		protected static void ClearTemporaryOverrideGun(PlayerController targetPlayer, Gun m_extantGun)
		{
			if (!targetPlayer || !m_extantGun)
			{
				return;
			}
			if (targetPlayer)
			{
				targetPlayer.inventory.GunLocked.RemoveOverride("override gun");
				targetPlayer.inventory.DestroyGun(m_extantGun);
				m_extantGun = null;
			}
			targetPlayer.inventory.GunChangeForgiveness = false;
		}

		/// <summary>
		/// Gun forms.
		/// </summary>
		public AdvancedGunFormeData[] Formes;
		protected int CurrentForme;
		public AdvancedGunFormeData CurrentFormeData => Formes[CurrentForme];
		public string CurrentFormeSynergy => CurrentFormeData.RequiredSynergyString;
	}
}
