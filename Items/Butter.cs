using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using System.Collections;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class Butter : PassiveItem
    {
        public static void Init()
        {
            string name = "Butter";
            string shortdesc = "It's Too Slippery!";
            string longdesc = "Makes the owner throw active items when using them. Active items that take longer to recharge deal more damage.\n\nA regular bar of butter. It's very difficult to hold, and now your items are covered in it.";
            ItemBuilder.EasyInit<Butter>("items/butter", "sprites/butter_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, null, null);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnUsedPlayerItem += Throw;
        }

        public override DebrisObject Drop(PlayerController player)
        {
			player.OnUsedPlayerItem -= Throw;
            return base.Drop(player);
        }

        protected override void OnDestroy()
        {
			if(m_owner != null)
            {
				m_owner.OnUsedPlayerItem -= Throw;
            }
            base.OnDestroy();
        }

        public void Throw(PlayerController player, PlayerItem item)
        {
            if (item.CanBeDropped && (!item.consumable || item.numberOfUses > 1))
            {
				item.StartCoroutine(DelayedThrow(player, item));
            }
        }

        public IEnumerator DelayedThrow(PlayerController player, PlayerItem item)
        {
            yield return new WaitForEndOfFrame();
			if (item.IsCurrentlyActive)
			{
                while (item.IsCurrentlyActive)
                {
					yield return null;
				}
				yield return new WaitForEndOfFrame();
			}
			DebrisObject deb = player.DropActiveItem(item);
			Destroy(deb);
			deb.GetComponent<PlayerItem>().ForceAsExtant = true;
			GameObject active = deb.gameObject;
			AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", active);
			Vector3 vector = player.CenterPosition;
			Vector2 vector2 = player.unadjustedAimPoint.XY() - vector.XY();
			float z = BraveMathCollege.Atan2Degrees(vector2);
			GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector, Quaternion.Euler(0f, 0f, z));
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Shooter = player.specRigidbody;
			component.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
			component.baseData.damage *= player.stats.GetStatValue(PlayerStats.StatType.ThrownGunDamage);
			SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
			component2.sprite = deb.sprite;
			deb.sprite.scale = Vector3.one;
			active.transform.parent = gameObject.transform;
			active.transform.localRotation = Quaternion.identity;
			deb.sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
			Bounds bounds = deb.sprite.GetBounds();
			component2.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
			component2.PrimaryPixelCollider.ManualOffsetX = -Mathf.RoundToInt(bounds.extents.x / 0.0625f);
			component2.PrimaryPixelCollider.ManualOffsetY = -Mathf.RoundToInt(bounds.extents.y / 0.0625f);
			component2.PrimaryPixelCollider.ManualWidth = Mathf.RoundToInt(bounds.size.x / 0.0625f);
			component2.PrimaryPixelCollider.ManualHeight = Mathf.RoundToInt(bounds.size.y / 0.0625f);
			component2.UpdateCollidersOnRotation = true;
			component2.UpdateCollidersOnScale = true;
			component.Reawaken();
			component.Owner = player;
			component.Start();
			component.SendInDirection(vector2, true, false);
			Projectile projectile = component;
			projectile.OnBecameDebris += delegate (DebrisObject a)
			{
				if (a)
				{
					a.FlagAsPickup();
					a.Priority = EphemeralObject.EphemeralPriority.Critical;
					TrailRenderer componentInChildren = a.gameObject.GetComponentInChildren<TrailRenderer>();
					if (componentInChildren)
					{
						Destroy(componentInChildren);
					}
					SpeculativeRigidbody component3 = a.GetComponent<SpeculativeRigidbody>();
					if (component3)
					{
						component3.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.EnemyHitBox));
					}
				}
			};
			Projectile projectile2 = component;
			projectile2.OnBecameDebrisGrounded += HandleThrownGunGrounded;
			component.angularVelocity = ((vector2.x <= 0f) ? 1080 : -1080);
			if (!RoomHandler.unassignedInteractableObjects.Contains(deb.GetComponent<PlayerItem>()))
			{
				RoomHandler.unassignedInteractableObjects.Add(deb.GetComponent<PlayerItem>());
			}
			void OnPickup(PlayerController player2)
			{
				if(gameObject != null)
				{
					gameObject.transform.DetachChildren();
					Destroy(gameObject);
				}
				if(deb != null && deb.GetComponent<PlayerItem>() != null)
				{
					deb.GetComponent<PlayerItem>().OnPickedUp -= OnPickup;
				}
			}
			deb.GetComponent<PlayerItem>().OnPickedUp += OnPickup;
			component2.ForceRegenerate(null, null);
			if (player)
			{
				player.DoPostProcessThrownGun(component);
			}
			PostThrowModifier(projectile);
			yield break;
        }

		public void PostThrowModifier(Projectile proj)
        {
			if(m_owner != null)
            {
                if (m_owner.PlayerHasActiveSynergy("Bouncy Throws"))
                {
					proj.GetOrAddComponent<BounceProjModifier>().numberOfBounces += 1;
				}
				if (m_owner.PlayerHasActiveSynergy("Piercing Throws"))
				{
					proj.GetOrAddComponent<PierceProjModifier>().penetration += 1;
				}
				if (m_owner.PlayerHasActiveSynergy("Homing Boomerang Throws"))
				{
					HomingModifier home = proj.AddComponent<HomingModifier>();
					home.HomingRadius = 10;
					home.AngularVelocity = 420;
					proj.OnBecameDebrisGrounded += HandleReturnLikeBoomerang;
				}
			}
        }

		private void HandleReturnLikeBoomerang(DebrisObject obj)
		{
			obj.OnGrounded -= HandleReturnLikeBoomerang;
			PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
			if (pickupMover.specRigidbody)
			{
				pickupMover.specRigidbody.CollideWithTileMap = false;
			}
			pickupMover.minRadius = 1f;
			pickupMover.moveIfRoomUnclear = true;
			pickupMover.stopPathingOnContact = true;
		}

		private void HandleThrownGunGrounded(DebrisObject obj)
		{
			obj.OnGrounded -= HandleThrownGunGrounded;
			obj.inertialMass = 10f;
			SpriteOutlineManager.RemoveOutlineFromSprite(obj.sprite, true);
			SpriteOutlineManager.AddOutlineToSprite(obj.sprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
			obj.sprite.UpdateZDepth();
		}
	}
}
