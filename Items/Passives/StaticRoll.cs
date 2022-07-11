using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class StaticRoll : PassiveItem
    {
        public static void Init()
        {
            string name = "Static Roll";
            string shortdesc = "Publically Available";
            string longdesc = "Allows the owner to dodgeroll without moving. Those dodgerolls will not move the owner anywhere but will give some time of invulnerability.\n\nA proof of concept device created by a crazy scientist to prove that you can, in" +
                " fact, dodgeroll without moving.\n\nThe words \"BindingFlags.Public | BindingFlags.Static\" can be seen from inside.";
            StaticRoll item = ItemBuilder.EasyInit<StaticRoll>("items/staticroll", "sprites/static_roll_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, null, null);
			item.voidSynergyProjectile = CodeShortcuts.GetItemById<Gun>(593).DefaultModule.projectiles[0];
        }

        public override void Update()
        {
            base.Update();
            if (Owner != null && PickedUp)
            {
				Vector2 vector = Vector2.zero;
				if (Owner.CurrentInputState != PlayerInputState.NoMovement)
				{
					vector = Owner.AdjustInputVector(BraveInput.GetInstanceForPlayer(Owner.PlayerIDX).ActiveActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal);
				}
				if (vector.magnitude > 1f)
				{
					vector.Normalize();
				}
				if(vector == Vector2.zero)
				{
					HandleStartDodgeRoll(Owner, vector);
				}
            }
        }

		public bool HandleStartDodgeRoll(PlayerController self, Vector2 direction)
		{
			Type playerType = typeof(PlayerController);
			playerType.GetField("m_handleDodgeRollStartThisFrame", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, true);
			if (self.WasPausedThisFrame)
			{
				return false;
			}
			if (!(bool)playerType.GetMethod("CheckDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[0]))
			{
				return false;
			}
			if ((PlayerController.DodgeRollState)playerType.GetField("m_dodgeRollState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) == PlayerController.DodgeRollState.AdditionalDelay)
			{
				return false;
			}
			self.rollStats.blinkDistanceMultiplier = 1f;
			if (self.IsFlying && !(self.AdditionalCanDodgeRollWhileFlying.Value || IsFlagSetForCharacter(self, typeof(WingsItem))))
			{
				return false;
			}
            if (self.DodgeRollIsBlink)
            {
				return false;
            }
			bool flag = false;
			BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(self.PlayerIDX);
			/*GenericFieldInfo<float> timeHeldBlinkButtonInfo = playerType.GetField<float>("m_timeHeldBlinkButton", BindingFlags.NonPublic | BindingFlags.Instance);
			GenericFieldInfo<Vector2> cachedBlinkPositionInfo = playerType.GetField<Vector2>("m_cachedBlinkPosition", BindingFlags.NonPublic | BindingFlags.Instance);
			if (self.DodgeRollIsBlink)
			{
				bool flag2 = false;
				if (instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.DodgeRoll))
				{
					flag2 = true;
					self.healthHaver.TriggerInvulnerabilityPeriod(0.001f);
					instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.DodgeRoll);
				}
				if (instanceForPlayer.ActiveActions.DodgeRollAction.IsPressed)
				{
					timeHeldBlinkButtonInfo.SetValue(self, timeHeldBlinkButtonInfo.GetValue(self) + BraveTime.DeltaTime);
					if (timeHeldBlinkButtonInfo.GetValue(self) < 0.2f)
					{
						cachedBlinkPositionInfo.SetValue(self, self.specRigidbody.UnitCenter);
					}
					else
					{
						Vector2 cachedBlinkPosition = cachedBlinkPositionInfo.GetValue(self);
						if (BraveInput.GetInstanceForPlayer(self.PlayerIDX).IsKeyboardAndMouse(false))
						{
							cachedBlinkPositionInfo.SetValue(self, self.unadjustedAimPoint.XY() - (self.CenterPosition - self.specRigidbody.UnitCenter));
						}
						else
						{
							cachedBlinkPositionInfo.SetValue(self, cachedBlinkPositionInfo.GetValue(self) + BraveInput.GetInstanceForPlayer(self.PlayerIDX).ActiveActions.Aim.Vector.normalized * BraveTime.DeltaTime * 15f);
						}
						self.m_cachedBlinkPosition = BraveMathCollege.ClampToBounds(self.m_cachedBlinkPosition, GameManager.Instance.MainCameraController.MinVisiblePoint, GameManager.Instance.MainCameraController.MaxVisiblePoint);
						self.UpdateBlinkShadow(self.m_cachedBlinkPosition - cachedBlinkPosition, self.CanBlinkToPoint(self.m_cachedBlinkPosition, self.transform.position.XY() - self.specRigidbody.UnitCenter));
					}
				}
				else if (instanceForPlayer.ActiveActions.DodgeRollAction.WasReleased || flag2)
				{
					if (direction != Vector2.zero || self.m_timeHeldBlinkButton >= 0.2f)
					{
						flag = true;
					}
				}
				else
				{
					self.m_timeHeldBlinkButton = 0f;
				}
			}
			else*/ if (instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.DodgeRoll))
			{
				instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.DodgeRoll);
				flag = true;
			}
			if (flag)
			{
				self.DidUnstealthyAction();
				if (GameManager.Instance.InTutorial)
				{
					GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgeRoll");
				}
				return StartDodgeRoll(self, direction);
			}
			return false;
		}

		private bool StartDodgeRoll(PlayerController self, Vector2 direction)
		{
			Type playerType = typeof(PlayerController);
			if (!(bool)playerType.GetMethod("CheckDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[0]))
			{
				return false;
			}
			if (self.IsFlying && !(self.AdditionalCanDodgeRollWhileFlying.Value || IsFlagSetForCharacter(self, typeof(WingsItem))))
			{
				return false;
			}
			self.GetEventDelegate<Action<PlayerController>>("OnPreDodgeRoll")?.Invoke(self);
			if (self.IsStationary)
			{
				return false;
			}
			if (self.knockbackDoer)
			{
				self.knockbackDoer.ClearContinuousKnockbacks();
			}
			Vector2 rollDir = Vector2.zero;
			if (BraveInput.GetInstanceForPlayer(self.PlayerIDX).IsKeyboardAndMouse(false))
			{
				rollDir = self.unadjustedAimPoint.XY() - self.CenterPosition;
			}
			else
			{
				rollDir =  BraveInput.GetInstanceForPlayer(self.PlayerIDX).ActiveActions.Aim.Vector.normalized;
			}
			rollDir = rollDir.normalized;
			playerType.GetField("lockedDodgeRollDirection", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, direction);
			((List<AIActor>)playerType.GetField("m_rollDamagedEnemies", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self)).Clear();
			self.spriteAnimator.Stop();
			playerType.GetField("m_dodgeRollTimer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, 0f);
			playerType.GetField("m_dodgeRollState", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, (!self.rollStats.hasPreDodgeDelay) ? PlayerController.DodgeRollState.InAir : PlayerController.DodgeRollState.PreRollDelay);
			playerType.GetField("m_currentDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, (int)playerType.GetField("m_currentDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) + 1);
			self.GetEventDelegate<Action<PlayerController, Vector2>>("OnRollStarted")?.Invoke(self, rollDir);
			playerType.GetMethod("PlayDodgeRollAnimation", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { rollDir });
			if (self.CurrentGun != null)
			{
				self.CurrentGun.HandleDodgeroll(self.rollStats.GetModifiedTime(self));
			}
			if (self.CurrentGun == null || string.IsNullOrEmpty(self.CurrentGun.dodgeAnimation))
			{
				self.ToggleGunRenderers(false, "dodgeroll");
			}
			self.ToggleHandRenderers(false, "dodgeroll");
			if (self.CurrentFireMeterValue > 0f)
			{
				self.CurrentFireMeterValue = Mathf.Max(0f, self.CurrentFireMeterValue -= 0.5f);
				if (self.CurrentFireMeterValue == 0f)
				{
					self.IsOnFire = false;
				}
			}
			if(self.PlayerHasActiveSynergy("static readonly"))
			{
				self.StartCoroutine(RollInvulnerability(self));
			}
			if(self.PlayerHasActiveSynergy("static void"))
            {
				CodeShortcuts.OwnedShootProjectile(voidSynergyProjectile, self.CenterPosition, 0f, self);
				CodeShortcuts.OwnedShootProjectile(voidSynergyProjectile, self.CenterPosition, 90f, self);
				CodeShortcuts.OwnedShootProjectile(voidSynergyProjectile, self.CenterPosition, 180f, self);
				CodeShortcuts.OwnedShootProjectile(voidSynergyProjectile, self.CenterPosition, 270f, self);
			}
			if (self.PlayerHasActiveSynergy("private static") && !self.IsStealthed)
            {
				CodeShortcuts.HandleStealth(self, "being private");
            }
			return true;
		}

		public static IEnumerator RollInvulnerability(PlayerController player)
        {
            while (!player.IsDodgeRolling)
            {
				yield return null;
            }
            while (player.IsDodgeRolling)
            {
				player.healthHaver.IsVulnerable = false;
				yield return null;
            }
			player.healthHaver.IsVulnerable = true;
			yield break;
        }

		public Projectile voidSynergyProjectile;
	}
}
