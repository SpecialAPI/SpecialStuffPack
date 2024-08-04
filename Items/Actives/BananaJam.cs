using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class BananaJam : PlayerItem
    {
        public static void Init()
        {
            var name = "Banana Jam";
            var shortdesc = "Slippery";
            var longdesc = "Makes the user throw the current gun, getting a new one.\n\nA personal favorite of the Cultist";
            var item = EasyItemInit<BananaJam>("BananaJam", "banana_jam_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.SetCooldownType(CooldownType.Damage, 300);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            if (user.CharacterUsesRandomGuns)
            {
				user.ChangeToRandomGun();
				return;
			}
			var gun = user.CurrentGun;
			var quality = gun.quality - 1;
			gun.PrepGunForThrow();
			gun.m_prepThrowTime = 1.3f;
            user.inventory.RemoveGunFromInventory(gun);
            ThrowAndDestroyGun(gun);
			if(quality < ItemQuality.D)
			{
				LootEngine.SpawnCurrency(user.CenterPosition, 10, false);
				return;
			}
			var reward = GameManager.Instance.RewardManager;
			LootEngine.GivePrefabToPlayer(reward.GetItemForPlayer(user, reward.GunsLootTable, quality, null).gameObject, user);
		}

		public static void ThrowAndDestroyGun(Gun self)
		{
			self.m_isThrown = true;
			self.m_thrownOnGround = false;
			self.gameObject.SetActive(true);
			AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", self.gameObject);
			Vector3 vector = self.ThrowPrepTransform.parent.TransformPoint((self.ThrowPrepPosition * -1f).WithX(0f));
			Vector2 vector2 = self.m_localAimPoint - vector.XY();
			float z = BraveMathCollege.Atan2Degrees(vector2);
			GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector, Quaternion.Euler(0f, 0f, z));
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Shooter = self.m_owner.specRigidbody;
			component.DestroyMode = Projectile.ProjectileDestroyMode.Destroy;
			component.baseData.damage *= (self.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ThrownGunDamage);
			SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
			component2.sprite = self.sprite;
			self.m_sprite.scale = Vector3.one;
			self.transform.parent = gameObject.transform;
			self.transform.localRotation = Quaternion.identity;
			self.m_sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
			if (self.m_sprite.FlipY)
			{
				self.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), self.transform.localPosition);
			}
			Bounds bounds = self.sprite.GetBounds();
			component2.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
			component2.PrimaryPixelCollider.ManualOffsetX = -Mathf.RoundToInt(bounds.extents.x / 0.0625f);
			component2.PrimaryPixelCollider.ManualOffsetY = -Mathf.RoundToInt(bounds.extents.y / 0.0625f);
			component2.PrimaryPixelCollider.ManualWidth = Mathf.RoundToInt(bounds.size.x / 0.0625f);
			component2.PrimaryPixelCollider.ManualHeight = Mathf.RoundToInt(bounds.size.y / 0.0625f);
			component2.UpdateCollidersOnRotation = true;
			component2.UpdateCollidersOnScale = true;
			component.Reawaken();
			component.Owner = self.CurrentOwner;
			component.Start();
			component.SendInDirection(vector2, true, false);
			component.angularVelocity = (vector2.x <= 0f) ? 1080f : -1080f;
			if (RoomHandler.unassignedInteractableObjects.Contains(self))
			{
				RoomHandler.unassignedInteractableObjects.Remove(self);
			}
			component2.ForceRegenerate(null, null);
			if (self.m_owner)
			{
				(self.m_owner as PlayerController).DoPostProcessThrownGun(component);
			}
			self.m_owner = null;
		}

		public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && user.CurrentGun != null && (user.CurrentGun.CanActuallyBeDropped(user) || user.CharacterUsesRandomGuns);
        }
    }
}
