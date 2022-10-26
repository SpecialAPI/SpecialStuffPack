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
            EasyItemInit<Butter>("items/butter", "sprites/butter_idle_001", name, shortdesc, longdesc, ItemQuality.D, 306, null);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnUsedPlayerItem += Throw;
        }

        public override void DisableEffect(PlayerController player)
        {
			player.OnUsedPlayerItem -= Throw;
        }

        public void Throw(PlayerController player, PlayerItem item)
        {
            if (item.CanBeDropped)
            {
				bool consumable = item.consumable && item.numberOfUses > 1;
				item.StartCoroutine(DelayedThrow(player, item, consumable));
            }
        }

        public IEnumerator DelayedThrow(PlayerController player, PlayerItem item, bool consumable)
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
			Projectile projectile = ItemThrower.ThrowActive(item, player, consumable);
			if(projectile != null)
			{
				PostThrowModifier(projectile);
			}
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
	}
}
