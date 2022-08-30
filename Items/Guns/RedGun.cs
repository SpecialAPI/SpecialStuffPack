using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    [HarmonyPatch]
    public class RedGun : GunBehaviourFormeSynergyProcessor
	{
		public int shiftStartFrame;
		public IntVector2 carryOverheadOffset;
		public float shadowTimeDelay;
		public float shadowLifetime;
		public float damageMultiplierPerOwnerlessCompanion;
		public float damageMultiplierPerCompanionTier;
		public float sacrificeOwnerlessBonus;
		public float sacrificeBaseBonus;
		public float sacrificeBonusPerTier;
		public float multiplierFromSacrifice;
		public static GameActorEffect indoctrinateEffect; //static because unity is bad and doesnt want to serialize my stuff :(
		public float chanceToApplyEffect;
		public int ammoReqForSacrifice;
		public int indoctrinateAmmoCost;
		private bool isSacrificingSomeone;
		public BraveBehaviour lastCrownTarget;
		public GameObject tentaclevfx;
		public static GameObject globalIndoctrinateInteractable;

		public string PoisonSynergy;
		public float PoisonChance;
		public GameActorHealthEffect PoisonEffect;

		public string VampiricSynergy;
		public float HeartChance;
		public float MaxHeartChance;

		public string GhostSynergy;
		public float GhostChance;
		public Projectile GhostProjectile;
		public float GhostTime;

		public string DoubleActiveSynergy;
		public float ActiveMultiplier;

		public string CritSynergy;
		public float CritChance;
		public Projectile CritReplacementProjectile;

		public string GodlySynergy;
		public float GodlyDamageMult;

		public static void Init()
        {
            string name = "The Red Gun";
            string shortdesc = "Start a Cult";
            string longdesc = "Gets stronger with every follower, charms the Jammed. Reload with a full clip to sacrifice the nearest follower for permanent strength.\n\nThis gun once belonged to the One Who Shoots, " +
                "before he was banished by Kaliber.";
            var gun = EasyGunInit("red_gun", name, shortdesc, longdesc, "red_gun_idle_001", "red_gun_idle_001", "gunsprites/redgun/", 250, 1f, new(22, 9), VoidMarshalObject.muzzleFlashEffects, "Kthulu", PickupObject.ItemQuality.S,
                GunClass.CHARM, out var finish);
            gun.RawSourceVolley.projectiles.Add(new()
            {
                projectiles = new()
                {
                    MagnumObject.GetProjectile()
                },
                shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
                cooldownTime = 0.15f,
                ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER,
                numberOfShotsInClip = 6
            });
            var redgun = gun.AddComponent<RedGun>();
			redgun.shiftStartFrame = 7;
			redgun.carryOverheadOffset = new(3, 16);
			redgun.shadowTimeDelay = 0.015f;
			redgun.shadowLifetime = 0.075f;
			redgun.shadowLifetime = 0.075f;
			redgun.sacrificeOwnerlessBonus = 0.2f;
			redgun.sacrificeBaseBonus = 0.2f;
			redgun.sacrificeBonusPerTier = 0.1f;
			redgun.damageMultiplierPerOwnerlessCompanion = 0.1f;
			redgun.damageMultiplierPerCompanionTier = 0.25f;
			redgun.tentaclevfx = YellowChamberObject.EraseVFX;
			redgun.ammoReqForSacrifice = 50;
			redgun.indoctrinateAmmoCost = 25;
			redgun.chanceToApplyEffect = 0.33f;

			redgun.PoisonSynergy = "Bane Gun";
			redgun.PoisonChance = 0.25f;
			redgun.PoisonEffect = IrradiatedLeadObject.HealthModifierEffect;

			redgun.VampiricSynergy = "Vampiric Gun";
			redgun.HeartChance = 0.05f;
			redgun.MaxHeartChance = 0.1f;

			redgun.GhostSynergy = "Necromantic Gun";
			redgun.GhostChance = 0.5f;
			redgun.GhostProjectile = GunslingersAshesObject.GetProjectile();
			redgun.GhostTime = 3f;

			redgun.DoubleActiveSynergy = "Zealous Gun";
			redgun.ActiveMultiplier = 2f;

			redgun.CritSynergy = "Merciless Gun";
			redgun.CritChance = 0.05f;
			redgun.CritReplacementProjectile = VorpalGunObject.CriticalReplacementProjectile;

			redgun.GodlySynergy = "Godly Gun";
			redgun.GodlyDamageMult = 1.5f;

			var yellowChamberCharm = YellowChamberObject.CharmEffect;
			indoctrinateEffect = new CultCharmEffect()
			{
				AffectsEnemies = true,
				AffectsPlayers = false,
				effectIdentifier = "charm",
				resistanceType = EffectResistanceType.Charm,
				stackMode = GameActorEffect.EffectStackingMode.Ignore,
				duration = -1f,
				maxStackedDuration = -1f,
				AppliesTint = true,
				TintColor = yellowChamberCharm.TintColor,
				AppliesDeathTint = true,
				DeathTintColor = yellowChamberCharm.DeathTintColor,
				AppliesOutlineTint = false,
				OutlineTintColor = Color.black,
				PlaysVFXOnActor = true,
				OverheadVFX = yellowChamberCharm.OverheadVFX,
				indoctrinateInteractable = globalIndoctrinateInteractable
			};
			gun.gunHandedness = GunHandedness.NoHanded;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f);
            gun.carryPixelOffset = new(18, 0);
			gun.dodgeAnimation = "frogs 🐸";
            finish();
			Dictionary<string, int> formesD = new()
			{
				{ "", gun.PickupObjectId },
				{ redgun.PoisonSynergy, InitForm("bane_gun", "RedGunBane", redgun.PoisonSynergy) },
				{ redgun.VampiricSynergy, InitForm("vamp_gun", "RedGunVampiric", redgun.VampiricSynergy) },
				{ redgun.GhostSynergy, InitForm("redghost_gun", "RedGunNecromantic", redgun.GhostSynergy) },
				{ redgun.DoubleActiveSynergy, InitForm("redeye_gun", "RedGunEyes", redgun.DoubleActiveSynergy) },
				{ redgun.CritSynergy, InitForm("redcrit_gun", "RedGunCrit", redgun.CritSynergy) },
				{ redgun.GodlySynergy, InitForm("redgodly_gun", "RedGunGodly", redgun.GodlySynergy) }
			};
			var formes = formesD.ToList();
			redgun.Formes = new AdvancedGunFormeData[formes.Count];
			for(int i = 0; i < formes.Count; i++)
            {
				redgun.Formes[i] = new()
				{
					FormeID = formes[i].Value,
					RequiredSynergy = formes[i].Key,
					RequiresSynergy = !string.IsNullOrEmpty(formes[i].Key)
				};
            }
        }

		public static int InitForm(string objectName, string gunSpriteContainer, string synergyName)
		{
			string name = synergyName;
			string shortdesc = "Start a Cult";
			string longdesc = "Gets stronger with every follower, charms the Jammed. Reload with a full clip to sacrifice the nearest follower for permanent strength.\n\nThis gun once belonged to the One Who Shoots, " +
				"before he was banished by Kaliber.";
			var gun = EasyGunInit(objectName, name, shortdesc, longdesc, $"{objectName}_idle_001", "red_gun_idle_001", $"gunsprites/{gunSpriteContainer.ToLower()}/", 250, 1f, new(22, 9), VoidMarshalObject.muzzleFlashEffects, "Kthulu", 
				PickupObject.ItemQuality.EXCLUDED, GunClass.CHARM, out var finish, null, $"spapi:the_red_gun+{synergyName.ToMTGId()}");
			gun.RawSourceVolley.projectiles.Add(new()
			{
				projectiles = new()
				{
					MagnumObject.GetProjectile()
				},
				shootStyle = ProjectileModule.ShootStyle.SemiAutomatic,
				cooldownTime = 0.15f,
				ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER,
				numberOfShotsInClip = 6
			});
			gun.gunHandedness = GunHandedness.NoHanded;
			gun.carryPixelOffset = new(18, 0);
			gun.dodgeAnimation = "frogs 🐸";
			finish();
			return gun.PickupObjectId;
		}

		public override void Update()
        {
            base.Update();
			if(PlayerOwner != null)
            {
				BraveBehaviour closestObject = null;
                if (!PlayerOwner.IsInCombat && !isSacrificingSomeone)
				{
					var playerCenter = PlayerOwner.CenterPosition;
					var closestDistance = 3f;
					foreach (var comp in PlayerOwner.companions)
					{
						if (comp.GetComponent<SpeculativeRigidbody>() != null)
						{
							var dist = Vector2.Distance(comp.GetComponent<SpeculativeRigidbody>().UnitCenter, playerCenter);
							if (dist <= closestDistance)
							{
								closestDistance = dist;
								closestObject = comp;
							}
						}
					}
					if(PlayerOwner.GetLastInteractable() != null && PlayerOwner.GetLastInteractable() is TalkDoerLite && (PlayerOwner.GetLastInteractable() as TalkDoerLite).name.Contains("ResourcefulRat_Beaten"))
                    {
						closestObject = PlayerOwner.GetLastInteractable() as TalkDoerLite;
					}
				}
				if(closestObject != lastCrownTarget)
                {
					if(lastCrownTarget != null)
                    {
						var targetSprite = lastCrownTarget.sprite;
						if(targetSprite != null)
						{
							SpriteOutlineManager.RemoveOutlineFromSprite(targetSprite);
							SpriteOutlineManager.AddOutlineToSprite(targetSprite, Color.black);
						}
						if (TextBoxManager.HasTextBox(lastCrownTarget.transform))
                        {
							TextBoxManager.ClearTextBox(lastCrownTarget.transform);
                        }
                    }
					lastCrownTarget = closestObject;
					if(lastCrownTarget != null)
					{
						var targetSprite2 = lastCrownTarget.sprite;
						if (targetSprite2 != null)
						{
							SpriteOutlineManager.RemoveOutlineFromSprite(targetSprite2);
							SpriteOutlineManager.AddOutlineToSprite(targetSprite2, Color.red);
						}
						var sprite = lastCrownTarget.sprite;
						if (!TextBoxManager.HasTextBox(lastCrownTarget.transform) && sprite != null)
						{
							if(lastCrownTarget is TalkDoerLite)
							{
								TextBoxManager.ShowThoughtBubble(sprite.WorldTopCenter + Vector2.up / 2f, lastCrownTarget.transform, -1f, $"+???", true, false, "");
							}
                            else
							{
								CompanionItem companionItem = null;
								foreach (var item in PlayerOwner.passiveItems)
								{
									if (item is CompanionItem companion && companion.ExtantCompanion == lastCrownTarget.gameObject)
									{
										companionItem = companion;
										break;
									}
								}
								TextBoxManager.ShowThoughtBubble(sprite.WorldTopCenter + Vector2.up / 2f, lastCrownTarget.transform, -1f, $"+{CalculateDMGUpForCompanion(companionItem) * 100}%", true, false, "");
							}
						}
					}
				}
			}
        }

		public float CalculateDMGUpForCompanion(CompanionItem item)
        {
			if(item != null && item.quality > 0)
            {
				return sacrificeBaseBonus + sacrificeBonusPerTier * (int)item.quality;
            }
            else
			{
				return sacrificeOwnerlessBonus;
			}
        }

		public int ActualAmmoRequiredForSacrifice => (PlayerOwner != null && PlayerOwner.PlayerHasActiveSynergy("Cheap Rituals")) ? Mathf.FloorToInt(ammoReqForSacrifice / 2f) : ammoReqForSacrifice;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
			if(manual && !gun.IsReloading && player != null && !player.IsInCombat && lastCrownTarget != null && gun.ammo >= ActualAmmoRequiredForSacrifice)
			{
				if(lastCrownTarget is AIActor aiactor)
				{
					if (player.IsPetting && player.m_pettingTarget)
					{
						player.m_pettingTarget.StopPet();
						player.ToggleGunRenderers(true, "petting");
						player.ToggleHandRenderers(true, "petting");
						player.m_pettingTarget = null;
					}
					CompanionItem companionItem = null;
					foreach (var item in PlayerOwner.passiveItems)
					{
						if (item is CompanionItem companion && companion.ExtantCompanion == aiactor.gameObject)
						{
							companionItem = companion;
							break;
						}
					}
					multiplierFromSacrifice += CalculateDMGUpForCompanion(companionItem);
					if (companionItem != null)
					{
						companionItem.m_extantCompanion = null;
						int num = player.passiveItems.FindIndex(p => p == companionItem);
						if (num >= 0)
						{
							player.RemovePassiveItemAt(num);
						}
					}
					isSacrificingSomeone = true;
					player.StartCoroutine(SacrificeCR(aiactor));
					gun.LoseAmmo(ActualAmmoRequiredForSacrifice);
				}
				else if(lastCrownTarget is TalkDoerLite doer && doer.name.Contains("ResourcefulRat_Beaten"))
                {
					player.StartCoroutine(EatRatCR(doer));
                }
			}
        }

		public IEnumerator EatRatCR(TalkDoerLite targetCorpse)
        {
			AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", PlayerOwner.gameObject);
            var sprite = targetCorpse.sprite;
			var go = targetCorpse.gameObject;
            Destroy(targetCorpse);
			Destroy(targetCorpse.specRigidbody);
			RenderSettings.ambientLight = Color.red;
			var duration = 1f;
			var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                RenderSettings.ambientLight = Color.red;
                sprite.scale = Vector2.Lerp(Vector2.one, Vector2.zero, elapsed / duration);
                yield return null;
            }
			var pos = go.transform.position;
			Destroy(go);
			LootEngine.SpawnCurrency(pos, 10, true);
			LootEngine.SpawnCurrency(pos, 45, false);
			yield break;
		}

		public IEnumerator SacrificeCR(AIActor target)
        {
			target.behaviorSpeculator.Stun(1000f);
			target.knockbackDoer.SetImmobile(true, "tentacle sacrifice");
			var tentacle = target.PlayEffectOnActor(tentaclevfx, new Vector3(0f, -1f, 0f), false, false, false);
			var vfxAnimator = tentacle.GetComponent<tk2dSpriteAnimator>();
			if (vfxAnimator)
			{
				vfxAnimator.sprite.IsPerpendicular = false;
				vfxAnimator.sprite.HeightOffGround = -1f;
			}
			while (target && vfxAnimator && vfxAnimator.sprite.GetCurrentSpriteDef().name != "kthuliber_tentacles_010")
			{
				vfxAnimator.sprite.UpdateZDepth();
				yield return null;
			}
			if (vfxAnimator)
			{
				vfxAnimator.sprite.IsPerpendicular = true;
				vfxAnimator.sprite.HeightOffGround = 1.5f;
				vfxAnimator.sprite.UpdateZDepth();
			}
			if (target)
			{
				target.EraseFromExistence(false);
			}
			isSacrificingSomeone = false;
			yield break;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.AdjustPlayerProjectileTint(Color.red, 0, 0f);
			float multiply = 1f + multiplierFromSacrifice;
			if(PlayerOwner != null)
			{
				foreach (var comp in PlayerOwner.companions)
				{
					foreach(var item in PlayerOwner.passiveItems)
                    {
						if(item is CompanionItem companion && companion.ExtantCompanion == comp.gameObject)
                        {
							multiply += damageMultiplierPerCompanionTier;
							break;
                        }
                    }
					multiply += damageMultiplierPerOwnerlessCompanion;
				}
				foreach(var orb in PlayerOwner.trailOrbitals)
				{
					foreach (var item in PlayerOwner.passiveItems)
					{
						if (item is IounStoneOrbitalItem companion && companion.m_extantOrbital == orb.gameObject)
						{
							multiply += damageMultiplierPerCompanionTier;
							break;
						}
					}
					multiply += damageMultiplierPerOwnerlessCompanion;
				}
			}
			projectile.baseData.damage *= multiply;
            if (projectile.GetComponent<IndoctrinateJammedEnemies>() == null)
            {
                var indoctrinateJammedEnemies = projectile.AddComponent<IndoctrinateJammedEnemies>();
                indoctrinateJammedEnemies.indoctrinateEffect = indoctrinateEffect;
                indoctrinateJammedEnemies.chanceToApply = chanceToApplyEffect;
            }
			var synregy = CurrentFormeSynergy;
			if (synregy == PoisonSynergy)
            {
				if(Random.value < PoisonChance * multiply)
                {
					projectile.statusEffectsToApply.Add(PoisonEffect);
                }
            }
			else if(synregy == VampiricSynergy)
            {
				var vampire = projectile.AddComponent<ChanceBasedDropItemOnKill>();
				vampire.chance = Mathf.Min(HeartChance * multiply, MaxHeartChance);
				vampire.itemId = HalfHeartId;
            }
			else if(synregy == GhostSynergy)
            {
				var ghost = projectile.AddComponent<ChanceBasedSpawnTimedProjectileOnKill>();
				ghost.chance = GhostChance;
				ghost.time = GhostTime * multiply;
				ghost.toShoot = GhostProjectile;
            }
			else if(synregy == DoubleActiveSynergy)
            {
				var active = projectile.AddComponent<DoubleChargeActive>();
				active.cooldownMultiplier = ActiveMultiplier;
            }
			else if(synregy == GodlySynergy)
            {
				projectile.baseData.damage *= GodlyDamageMult;
				projectile.ignoreDamageCaps = true;
            }
        }

        public override Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule module)
        {
			float multiply = 1f + multiplierFromSacrifice;
			if (PlayerOwner != null)
			{
				foreach (var comp in PlayerOwner.companions)
				{
					foreach (var item in PlayerOwner.passiveItems)
					{
						if (item is CompanionItem companion && companion.ExtantCompanion == comp.gameObject)
						{
							multiply += damageMultiplierPerCompanionTier;
							break;
						}
					}
					multiply += damageMultiplierPerOwnerlessCompanion;
				}
				foreach (var orb in PlayerOwner.trailOrbitals)
				{
					foreach (var item in PlayerOwner.passiveItems)
					{
						if (item is IounStoneOrbitalItem companion && companion.m_extantOrbital == orb.gameObject)
						{
							multiply += damageMultiplierPerCompanionTier;
							break;
						}
					}
					multiply += damageMultiplierPerOwnerlessCompanion;
				}
			}
            if (CurrentFormeSynergy == CritSynergy && Random.value < CritChance * multiply)
            {
				return CritReplacementProjectile;
            }
			return projectile;
		}

        [HarmonyPatch(typeof(Gun), nameof(Gun.GetCarryPixelOffset))]
		[HarmonyPostfix]
		public static void LerpOffset(Gun __instance, ref IntVector2 __result)
		{
			var redgun = __instance.GetComponent<RedGun>();
			if (redgun != null)
			{
				if (__instance.spriteAnimator.IsPlaying(__instance.shootAnimation))
				{
					var clip = __instance.spriteAnimator.CurrentClip;
					if (clip != null)
					{
						var time = __instance.spriteAnimator.clipTime - redgun.shiftStartFrame;
						var total = clip.frames.Length - redgun.shiftStartFrame;
						if (total > 1 && time >= 0f)
						{
							__result = Vector2.Lerp(__result.ToVector2(), redgun.carryOverheadOffset.ToVector2(), Mathf.Min(time / total, 1f)).ToIntVector2();
						}
					}
				}
                else
                {
					__result = redgun.carryOverheadOffset;
				}
			}
		}

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.HandleGunDepthInternal))]
        [HarmonyPrefix]
        public static bool OverrideGunDepth(Gun targetGun, float gunAngle)
        {
            if(targetGun.GetComponent<RedGun>() != null)
			{
				tk2dBaseSprite sprite = targetGun.GetSprite();
				if (!targetGun.spriteAnimator.IsPlaying(targetGun.shootAnimation))
				{
					sprite.HeightOffGround = 0.4f;
				}
				else if (gunAngle > 0f && gunAngle <= 155f && gunAngle >= 25f)
				{
					sprite.HeightOffGround = -0.075f;
				}
				else
				{
					sprite.HeightOffGround = 0.075f;
				}
				return false;
            }
            return true;
        }

		[HarmonyPatch(typeof(Gun), nameof(Gun.ShootSingleProjectile))]
		[HarmonyPrefix]
		public static void RotateGunBeforehand(Gun __instance)
        {
			var redgun = __instance.GetComponent<RedGun>();
			if (redgun != null)
			{
				if(__instance.CurrentOwner is PlayerController player)
                {
					Vector3 vector = player.m_startingAttachPointPosition;
					Vector3 vector2 = player.downwardAttachPointPosition;
					if (__instance.IsForwardPosition)
					{
						vector = vector.WithX(player.m_spriteDimensions.x - vector.x);
						vector2 = vector2.WithX(player.m_spriteDimensions.x - vector2.x);
					}
					if (player.SpriteFlipped)
					{
						vector = vector.WithX(player.m_spriteDimensions.x - vector.x);
						vector2 = vector2.WithX(player.m_spriteDimensions.x - vector2.x);
					}
					float num = (float)((!player.SpriteFlipped) ? 1 : -1);
					Vector3 a = __instance.carryPixelOffset.ToVector3();
					vector += Vector3.Scale(a, new Vector3(num, 1f, 1f)) * 0.0625f;
					vector2 += Vector3.Scale(a, new Vector3(num, 1f, 1f)) * 0.0625f;
					if (__instance.Handedness == GunHandedness.NoHanded && player.SpriteFlipped)
					{
						vector += Vector3.Scale(__instance.leftFacingPixelOffset.ToVector3(), new Vector3(num, 1f, 1f)) * 0.0625f;
						vector2 += Vector3.Scale(__instance.leftFacingPixelOffset.ToVector3(), new Vector3(num, 1f, 1f)) * 0.0625f;
					}
					if (player.IsFlying)
					{
						vector += new Vector3(0f, 0.1875f, 0f);
						vector2 += new Vector3(0f, 0.1875f, 0f);
					}
					if (__instance == player.CurrentSecondaryGun)
					{
						if (__instance.transform.parent != player.SecondaryGunPivot)
						{
							__instance.transform.parent = player.SecondaryGunPivot;
							__instance.transform.localRotation = Quaternion.identity;
							__instance.HandleSpriteFlip(player.SpriteFlipped);
							__instance.UpdateAttachTransform();
						}
						player.SecondaryGunPivot.position = player.gunAttachPoint.position + num * new Vector3(-0.75f, 0f, 0f);
					}
					else
					{
						if (__instance.transform.parent != player.gunAttachPoint)
						{
							__instance.transform.parent = player.gunAttachPoint;
							__instance.transform.localRotation = Quaternion.identity;
							__instance.HandleSpriteFlip(player.SpriteFlipped);
							__instance.UpdateAttachTransform();
						}
						if (__instance.IsHeroSword)
						{
							float t = 1f - Mathf.Abs(player.m_currentGunAngle - 90f) / 90f;
							player.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector, vector2, t), 16f);
						}
						else if (__instance.Handedness == GunHandedness.TwoHanded)
						{
							float t2 = Mathf.PingPong(Mathf.Abs(1f - Mathf.Abs(player.m_currentGunAngle + 90f) / 90f), 1f);
							Vector2 vector3 = Vector2.zero;
							if (player.m_currentGunAngle > 0f)
							{
								vector3 = Vector2.Scale(__instance.carryPixelUpOffset.ToVector2(), new Vector2(num, 1f)) * 0.0625f;
							}
							else
							{
								vector3 = Vector2.Scale(__instance.carryPixelDownOffset.ToVector2(), new Vector2(num, 1f)) * 0.0625f;
							}
							if (__instance.LockedHorizontalOnCharge)
							{
								vector3 = Vector3.Slerp(vector3, Vector2.zero, __instance.GetChargeFraction());
							}
							if (player.m_currentGunAngle < 0f)
							{
								player.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector, vector2 + vector3.ToVector3ZUp(0f), t2), 16f);
							}
							else
							{
								player.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector, vector + vector3.ToVector3ZUp(0f), t2), 16f);
							}
						}
						else
						{
							player.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(vector, 16f);
						}
					}
				}
				var discard = 0f;
				HandleRedGunRotation(redgun, __instance, ref discard, __instance.m_localAimPoint, false, 1f, true);
			}
        }

		//[HarmonyPatch(typeof(Gun), nameof(Gun.HandleSpriteFlip))]
		//[HarmonyPrefix]
		public static void NoMoreFlippy(Gun __instance, ref bool flipped)
        {
			if(__instance.GetComponent<RedGun>() != null && __instance.CurrentOwner is PlayerController player && player.IsDodgeRolling)
            {
				flipped = false;
            }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.HandleAimRotation))]
        [HarmonyPrefix]
        public static bool OverrideRotation(Gun __instance, ref float __result, Vector3 ownerAimPoint, bool limitAimSpeed, float aimTimeScale)
        {
			var redgun = __instance.GetComponent<RedGun>();
			if (redgun != null)
            {
				HandleRedGunRotation(redgun, __instance, ref __result, ownerAimPoint, limitAimSpeed, aimTimeScale, false);
				return false;
            }
            return true;
        }

        public static void HandleRedGunRotation(RedGun redgun, Gun __instance, ref float __result, Vector3 ownerAimPoint, bool limitAimSpeed, float aimTimeScale, bool forceFiringAnimation)
        {
			if (__instance.m_isThrown)
			{
				__result = __instance.prevGunAngleUnmodified;
				return;
			}
			Vector2 b;
			if (__instance.usesDirectionalIdleAnimations)
			{
				b = (__instance.m_transform.position + Quaternion.Euler(0f, 0f, -__instance.m_attachTransform.localRotation.z) * __instance.barrelOffset.localPosition).XY();
				b = __instance.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter;
			}
			else if (__instance.LockedHorizontalOnCharge)
			{
				b = __instance.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter;
			}
			else
			{
				b = (__instance.m_transform.position + Quaternion.Euler(0f, 0f, __instance.gunAngle) * Quaternion.Euler(0f, 0f, -__instance.m_attachTransform.localRotation.z) * __instance.barrelOffset.localPosition).XY();
			}
			float num = Vector2.Distance(ownerAimPoint.XY(), b);
			float t = Mathf.Clamp01((num - 2f) / 3f);
			b = Vector2.Lerp(__instance.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter, b, t);
			__instance.m_localAimPoint = ownerAimPoint.XY();
			Vector2 vector = __instance.m_localAimPoint - b;
			float num2 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (__instance.OverrideAngleSnap != null)
			{
				num2 = BraveMathCollege.QuantizeFloat(num2, __instance.OverrideAngleSnap.Value);
			}
			if ((limitAimSpeed && aimTimeScale != 1f) || __instance.m_activeBeams.Count > 0)
			{
				float num3 = float.MaxValue * BraveTime.DeltaTime * aimTimeScale;
				if (__instance.m_activeBeams.Count > 0 && __instance.Volley && __instance.Volley.UsesBeamRotationLimiter)
				{
					num3 = __instance.Volley.BeamRotationDegreesPerSecond * BraveTime.DeltaTime * aimTimeScale;
				}
				float value = BraveMathCollege.ClampAngle180(num2 - __instance.prevGunAngleUnmodified);
				num2 = BraveMathCollege.ClampAngle180(__instance.prevGunAngleUnmodified + Mathf.Clamp(value, -num3, num3));
				__instance.m_localAimPoint = (__instance.transform.position + (Quaternion.Euler(0f, 0f, num2) * Vector3.right).normalized * vector.magnitude).XY();
			}
			__instance.prevGunAngleUnmodified = num2;
			__instance.gunAngle = num2;
			__instance.m_attachTransform.localRotation = Quaternion.Euler(__instance.m_attachTransform.localRotation.x, __instance.m_attachTransform.localRotation.y, num2);
			__instance.m_unroundedBarrelPosition = __instance.barrelOffset.position;
			float num4 = (float)((!__instance.forceFlat) ? (Mathf.RoundToInt(num2 / 10f) * 10) : (Mathf.RoundToInt(num2 / 3f) * 3));
			if (__instance.IgnoresAngleQuantization)
			{
				num4 = num2;
			}
			bool flag = __instance.sprite.FlipY;
			float num5 = 75f;
			float num6 = 105f;
			if (num4 <= 155f && num4 >= 25f)
			{
				num5 = 75f;
				num6 = 105f;
			}
			if (!__instance.sprite.FlipY && Mathf.Abs(num4) > num6)
			{
				flag = true;
			}
			else if (__instance.sprite.FlipY && Mathf.Abs(num4) < num5)
			{
				flag = false;
			}
			if (__instance.spriteAnimator.IsPlaying(__instance.shootAnimation) || forceFiringAnimation)
			{
				var clip = __instance.spriteAnimator.CurrentClip;
				if (clip != null)
				{
					var time = __instance.spriteAnimator.clipTime - redgun.shiftStartFrame;
					var total = clip.frames.Length - redgun.shiftStartFrame;
					if (redgun.PlayerOwner != null)
					{
						if (!redgun.PlayerOwner.IsSlidingOverSurface)
						{
							if (redgun.PlayerOwner.IsDodgeRolling)
							{
								if (redgun.PlayerOwner.lockedDodgeRollDirection.x < -0.1f)
								{
									if (!flag)
									{
										num4 -= 180;
									}
									flag = true;
								}
								else if (redgun.PlayerOwner.lockedDodgeRollDirection.x > 0.1f)
								{
                                    if (flag)
                                    {
										num4 -= 180;
                                    }
									flag = false;
								}
							}
						}
					}
					if (total > 1 && time >= 0f)
					{
						num4 = Mathf.LerpAngle(num4, (!flag) ? 0 : 180, Mathf.Min(time / total, 1f));
					}
				}
			}
			if (__instance.m_isPreppedForThrow)
			{
				if (__instance.m_prepThrowTime < 1.2f)
				{
					num4 = (float)Mathf.FloorToInt(Mathf.LerpAngle(num4, -90f, Mathf.Clamp01(__instance.m_prepThrowTime / 1.2f)));
				}
				else
				{
					num4 = (float)Mathf.FloorToInt(Mathf.PingPong(__instance.m_prepThrowTime * 15f, 10f) + -95f);
				}
			}
			if (!__instance.spriteAnimator.IsPlaying(__instance.shootAnimation))
			{
                if (redgun.PlayerOwner != null)
                {
					if (!redgun.PlayerOwner.IsSlidingOverSurface)
					{
						if (redgun.PlayerOwner.IsDodgeRolling)
						{
							if (redgun.PlayerOwner.lockedDodgeRollDirection.x < -0.1f)
							{
								flag = true;
							}
							else if (redgun.PlayerOwner.lockedDodgeRollDirection.x > 0.1f)
							{
								flag = false;
							}
						}
					}
				}
				num4 = flag ? 180 : 0;
			}
			if (__instance.usesDirectionalIdleAnimations)
			{
				int num7 = BraveMathCollege.AngleToOctant(num4 + 90f);
				float num8 = (float)(num7 * -45);
				Debug.Log(num8);
				float z = (num4 + 360f) % 360f - num8;
				__instance.m_attachTransform.localRotation = Quaternion.Euler(__instance.m_attachTransform.localRotation.x, __instance.m_attachTransform.localRotation.y, z);
			}
			else
			{
				__instance.m_attachTransform.localRotation = Quaternion.Euler(__instance.m_attachTransform.localRotation.x, __instance.m_attachTransform.localRotation.y, num4);
			}
			if (__instance.m_currentlyPlayingChargeVFX != null)
			{
				__instance.UpdateChargeEffectZDepth(vector);
			}
			if (__instance.m_sprite != null)
			{
				__instance.m_sprite.ForceRotationRebuild();
			}
			if (__instance.ShouldDoLaserSight())
			{
				if (__instance.m_extantLaserSight == null)
				{
					string path = "Global VFX/VFX_LaserSight";
					if (!(__instance.m_owner is PlayerController))
					{
						path = ((!__instance.LaserSightIsGreen) ? "Global VFX/VFX_LaserSight_Enemy" : "Global VFX/VFX_LaserSight_Enemy_Green");
					}
					__instance.m_extantLaserSight = SpawnManager.SpawnVFX((GameObject)BraveResources.Load(path, ".prefab"), false).GetComponent<tk2dTiledSprite>();
					__instance.m_extantLaserSight.IsPerpendicular = false;
					__instance.m_extantLaserSight.HeightOffGround = __instance.CustomLaserSightHeight;
					__instance.m_extantLaserSight.renderer.enabled = __instance.m_meshRenderer.enabled;
					__instance.m_extantLaserSight.transform.parent = __instance.barrelOffset;
					if (__instance.m_owner is AIActor)
					{
						__instance.m_extantLaserSight.renderer.enabled = false;
					}
				}
				__instance.m_extantLaserSight.transform.localPosition = Vector3.zero;
				__instance.m_extantLaserSight.transform.rotation = Quaternion.Euler(0f, 0f, num2);
				if (__instance.m_extantLaserSight.renderer.enabled)
				{
					Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
					bool flag2 = false;
					float num9 = float.MaxValue;
					if (__instance.DoubleWideLaserSight)
					{
						CollisionLayer layer = (!(__instance.m_owner is PlayerController)) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox;
						int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, layer, CollisionLayer.BulletBreakable);
						Vector2 b2 = BraveMathCollege.DegreesToVector(vector.ToAngle() + 90f, 0.0625f);
						RaycastResult raycastResult;
						if (PhysicsEngine.Instance.Raycast(__instance.barrelOffset.position.XY() + b2, vector, __instance.CustomLaserSightDistance, out raycastResult, true, true, rayMask, null, false, rigidbodyExcluder, null))
						{
							flag2 = true;
							num9 = Mathf.Min(num9, raycastResult.Distance);
						}
						RaycastResult.Pool.Free(ref raycastResult);
						if (PhysicsEngine.Instance.Raycast(__instance.barrelOffset.position.XY() - b2, vector, __instance.CustomLaserSightDistance, out raycastResult, true, true, rayMask, null, false, rigidbodyExcluder, null))
						{
							flag2 = true;
							num9 = Mathf.Min(num9, raycastResult.Distance);
						}
						RaycastResult.Pool.Free(ref raycastResult);
					}
					else
					{
						CollisionLayer layer2 = (!(__instance.m_owner is PlayerController)) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox;
						int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, layer2, CollisionLayer.BulletBreakable);
						RaycastResult raycastResult2;
						if (PhysicsEngine.Instance.Raycast(__instance.barrelOffset.position.XY(), vector, __instance.CustomLaserSightDistance, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
						{
							flag2 = true;
							num9 = raycastResult2.Distance;
							if (raycastResult2.SpeculativeRigidbody && raycastResult2.SpeculativeRigidbody.aiActor)
							{
								__instance.HandleEnemyHitByLaserSight(raycastResult2.SpeculativeRigidbody.aiActor);
							}
						}
						RaycastResult.Pool.Free(ref raycastResult2);
					}
					__instance.m_extantLaserSight.dimensions = new Vector2((!flag2) ? 480f : (num9 / 0.0625f), 1f);
					__instance.m_extantLaserSight.ForceRotationRebuild();
					__instance.m_extantLaserSight.UpdateZDepth();
				}
			}
			else if (__instance.m_extantLaserSight != null)
			{
				SpawnManager.Despawn(__instance.m_extantLaserSight.gameObject);
				__instance.m_extantLaserSight = null;
			}
			if (!__instance.OwnerHasSynergy(CustomSynergyType.PLASMA_LASER) && __instance.m_extantLockOnSprite)
			{
				SpawnManager.Despawn(__instance.m_extantLockOnSprite);
			}
			if (__instance.usesDirectionalAnimator)
			{
				__instance.aiAnimator.LockFacingDirection = true;
				__instance.aiAnimator.FacingDirection = num2;
			}
			__result = num2;
		}

        [HarmonyPatch(typeof(AIActor), nameof(AIActor.CheckForBlackPhantomness))]
        [HarmonyPrefix]
        public static bool ReplaceBlackPhantomnessCheck(AIActor __instance)
        {
            if (__instance.CompanionOwner != null || !__instance.IsNormalEnemy)
            {
                return true;
            }
            if (__instance.PreventBlackPhantom || __instance.ForceBlackPhantom)
            {
                return true;
            }
            if (GameManager.HasInstance)
            {
                var shouldDoDoublePhantomness = false;
                if (!shouldDoDoublePhantomness)
                {
                    foreach(var play in GameManager.Instance.AllPlayers)
                    {
                        if (play != null && play.inventory != null && play.inventory.AllGuns != null)
                        {
                            foreach(var gun in play.inventory.AllGuns)
                            {
                                if(gun.GetComponent<RedGun>() != null)
                                {
                                    shouldDoDoublePhantomness = true;
                                    break;
                                }
                            }
                            if (shouldDoDoublePhantomness)
                            {
                                break;
                            }
                        }
                    }
                }
                if (shouldDoDoublePhantomness)
                {
                    int totalCurse = PlayerStats.GetTotalCurse();
                    float num;
                    if (totalCurse <= 2)
                    {
                        num = 0.05f;
                    }
                    else if (totalCurse <= 4)
                    {
                        num = 0.1f;
                    }
                    else if (totalCurse <= 6)
                    {
                        num = 0.25f;
                    }
                    else if (totalCurse <= 8)
                    {
                        num = 0.5f;
                    }
                    else if (totalCurse == 9)
                    {
                        num = 0.75f;
                    }
                    else
                    {
                        num = 1f;
                    }
                    if (__instance.healthHaver.IsBoss)
                    {
                        if (totalCurse < 7)
                        {
                            num = 0.05f;
                        }
                        else if (totalCurse < 9)
                        {
                            num = 0.25f;
                        }
                        else if (totalCurse < 10)
                        {
                            num = 0.5f;
                        }
                        else
                        {
                            num = 0.75f;
                        }
                    }
                    if (__instance.ForceBlackPhantom || Random.value < num)
                    {
                        __instance.BecomeBlackPhantom();
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
