using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using System.Text;
using SpecialStuffPack.ItemAPI;

namespace SpecialStuffPack.Items
{
    public class LightningHornController : BraveBehaviour
    {
		public static void Init()
        {
			string name = "Storm Horn";
			string shortdesc = "Crackles with Power";
			string longdesc = "Summons gusts of wind from the horn and a lightning at the aim point.\n\nThis horn was used by ancient shamans to summon thunderstorms.";
			Gun gun = GunBuilder.EasyGunInit("guns/lightning_horn", name, shortdesc, longdesc, "lightning_horn_idle_001", "gunsprites/ammonomicon/lightning_horn_idle_001", "gunsprites/lightninghorn", 75, 2f, new Vector3(), null,
				"Kthulu", PickupObject.ItemQuality.A, GunClass.ICE, SpecialStuffModule.globalPrefix, out var finish, null, null, null);
			LightningHornController horn = gun.AddComponent<LightningHornController>();
			horn.LightningRadialDamage = 15f;
			horn.LightningRadialDamageRadius = 1.5f;
			horn.LightningRadialKnockback = 50f;
			horn.LightningRadialKnockbackRadius = 3f;
			Projectile proj = GunBuilder.EasyProjectileInit<Projectile>("projectiles/wind_projectile", "projectilesprites/wind_projectile_001", 1f, 20f, 99999f, 50f, true, false, true, null, tk2dBaseSprite.Anchor.MiddleCenter, 0, 0, null, null, 
				null, null);
			ProjectileModule module = new ProjectileModule
			{
				shootStyle = ProjectileModule.ShootStyle.Burst,
				ammoType = GameUIAmmoType.AmmoType.CUSTOM,
				customAmmoType = "white",
				projectiles = new List<Projectile>
				{proj
				},
				orderedGroupCounts = new List<int>(),
				numberOfFinalProjectiles = 0,
				angleVariance = 0f,
				cooldownTime = 1f,
				burstShotCount = 5,
				burstCooldownTime = 0.1f,
				numberOfShotsInClip = 5
			};
			gun.RawSourceVolley.projectiles.Add(module);
			finish();
        }

        public void Start()
        {
            m_gun = GetComponent<Gun>();
            m_gun.PostProcessProjectile += HandleLightningStrike;
			m_gun.OnInitializedWithOwner += InitializedWithOwner;
			m_gun.OnDropped += OnDropped;
			if (m_gun.CurrentOwner != null)
            {
				InitializedWithOwner(m_gun.CurrentOwner);
            }
        }

		public void InitializedWithOwner(GameActor owner)
        {
			if(owner is PlayerController)
            {
				m_playerOwner = owner as PlayerController;
				m_playerOwner.GunChanged += HandleGunChanged;
				CheckFlightStatus(m_playerOwner.CurrentGun);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
			OnDropped();
        }

        public void OnDropped()
        {
			if(m_playerOwner != null)
			{
				m_playerOwner.GunChanged -= HandleGunChanged;
				m_playerOwner = null;
            }
        }

		private void CheckFlightStatus(Gun currentGun)
		{
			if (m_playerOwner && currentGun)
			{
				LightningHornController component = currentGun.GetComponent<LightningHornController>();
				if (component)
				{
					m_playerOwner.SetIsFlying(true, "storm horn", false, false);
					m_playerOwner.AdditionalCanDodgeRollWhileFlying.SetOverride("storm horn", true, null);
				}
				else
				{
					m_playerOwner.SetIsFlying(false, "storm horn", false, false);
					m_playerOwner.AdditionalCanDodgeRollWhileFlying.RemoveOverride("storm horn");
				}
			}
		}

		public void HandleGunChanged(Gun previous, Gun current, bool isNew)
		{
			CheckFlightStatus(current);
		}

		public void Update()
        {
			m_gun.PreventNormalFireAudio = true;
        }

        public void HandleLightningStrike(Projectile projectile)
        {
            if (!m_gun.MidBurstFire && m_gun.CurrentOwner != null && m_gun.CurrentOwner is PlayerController)
            {
				AkSoundEngine.PostEvent("Play_OBJ_charmhorn_use_01", m_gun.gameObject);
                PlayerController player = (m_gun.CurrentOwner as PlayerController);
                IntVector2 position = player.unadjustedAimPoint.XY().ToIntVector2(VectorConversions.Floor);
                if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.data != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(position) &&
                    GameManager.Instance.Dungeon.data[position].parentRoom != null && GameManager.Instance.Dungeon.data[position].parentRoom == player.CurrentRoom && !GameManager.Instance.Dungeon.data[position].isExitCell &&
                    GameManager.Instance.Dungeon.data[position].type != CellType.WALL)
                {
					Exploder.DoRadialDamage(LightningRadialDamage, position.ToCenterVector3(0f), LightningRadialDamageRadius, false, true, false, null);
					Exploder.DoRadialKnockback(position.ToCenterVector3(0f), LightningRadialKnockback, LightningRadialKnockbackRadius);
					if(GameManager.Instance.Dungeon.data[position].type == CellType.FLOOR)
					{
						GameManager.Instance.Dungeon.data[position].type = CellType.PIT;
						RoomHandler parentRoom = player.CurrentRoom;
						tk2dTileMap tk2dTileMap = (parentRoom == null || !(parentRoom.OverrideTilemap != null)) ? GameManager.Instance.Dungeon.MainTilemap : parentRoom.OverrideTilemap;
						for (int i = -1; i < 2; i++)
						{
							for (int j = -2; j < 4; j++)
							{
								CellData cellData = GameManager.Instance.Dungeon.data.cellData[position.x + i][position.y + j];
								if (cellData != null)
								{
									cellData.hasBeenGenerated = false;
									if (cellData.parentRoom != null)
									{
										List<GameObject> list = new List<GameObject>();
										for (int k = 0; k < cellData.parentRoom.hierarchyParent.childCount; k++)
										{
											Transform child = cellData.parentRoom.hierarchyParent.GetChild(k);
											if (child.name.StartsWith("Chunk_"))
											{
												list.Add(child.gameObject);
											}
										}
										for (int l = list.Count - 1; l >= 0; l--)
										{
											Destroy(list[l]);
										}
									}
									TK2DDungeonAssembler assembler = (TK2DDungeonAssembler)typeof(Dungeon).GetField("assembler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(GameManager.Instance.Dungeon);
									assembler.ClearTileIndicesForCell(GameManager.Instance.Dungeon, tk2dTileMap, cellData.position.x, cellData.position.y);
									assembler.BuildTileIndicesForCell(GameManager.Instance.Dungeon, tk2dTileMap, cellData.position.x, cellData.position.y);
									cellData.HasCachedPhysicsTile = false;
									cellData.CachedPhysicsTile = null;
								}
							}
						}
						GameManager.Instance.Dungeon.RebuildTilemap(GameManager.Instance.Dungeon.MainTilemap);
					}
				}
            }
        }

		public float LightningRadialDamage;
		public float LightningRadialDamageRadius;
		public float LightningRadialKnockback;
		public float LightningRadialKnockbackRadius;
		private Gun m_gun;
		private PlayerController m_playerOwner;
    }
}
