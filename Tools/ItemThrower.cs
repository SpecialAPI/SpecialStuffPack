using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeonator;
using System.Text;

namespace SpecialStuffPack
{
    public static class ItemThrower
    {
        public static Projectile ThrowActive(this PlayerItem item, PlayerController player, bool consumable)
        {
            if (!item)
            {
				return null;
            }
			player = player ?? item.LastOwner;
            if (!player)
            {
				return null;
            }
			DebrisObject deb = player.DropActiveItem(item);
			UnityEngine.Object.Destroy(deb);
			deb.GetComponent<PlayerItem>().ForceAsExtant = true;
			GameObject active = deb.gameObject;
			AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", active);
			Vector3 vector = player.CenterPosition;
			Vector2 vector2 = player.unadjustedAimPoint.XY() - vector.XY();
			float z = BraveMathCollege.Atan2Degrees(vector2);
			GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector, Quaternion.Euler(0f, 0f, z));
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Shooter = player.specRigidbody;
			component.DestroyMode = consumable ? Projectile.ProjectileDestroyMode.Destroy : Projectile.ProjectileDestroyMode.BecomeDebris;
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
						UnityEngine.Object.Destroy(componentInChildren);
					}
					SpeculativeRigidbody component3 = a.GetComponent<SpeculativeRigidbody>();
					if (component3)
					{
						component3.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.EnemyHitBox));
					}
				}
			};
			Projectile projectile2 = component; 
			void HandleThrownGunGrounded(DebrisObject obj)
			{
				obj.OnGrounded -= HandleThrownGunGrounded;
				obj.inertialMass = 10f;
				SpriteOutlineManager.RemoveOutlineFromSprite(obj.sprite, true);
				SpriteOutlineManager.AddOutlineToSprite(obj.sprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
				obj.sprite.UpdateZDepth();
			}
			projectile2.OnBecameDebrisGrounded += HandleThrownGunGrounded;
			component.angularVelocity = ((vector2.x <= 0f) ? 1080 : -1080);
			if (!RoomHandler.unassignedInteractableObjects.Contains(deb.GetComponent<PlayerItem>()))
			{
				RoomHandler.unassignedInteractableObjects.Add(deb.GetComponent<PlayerItem>());
			}
			void OnPickup(PlayerController player2)
			{
				if (gameObject != null)
				{
					gameObject.transform.DetachChildren();
					UnityEngine.Object.Destroy(gameObject);
				}
				if (deb != null && deb.GetComponent<PlayerItem>() != null)
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
			return projectile;
		}
    }
}
