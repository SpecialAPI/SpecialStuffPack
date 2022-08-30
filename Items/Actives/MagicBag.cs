using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class MagicBag : PlayerItem
    {
        public static void Init()
        {
            var name = "Magic Bag";
            var shortdesc = "Dedication";
            var longdesc = "Use near an item to put it in the bag. Can be used again after recharging to spit out 2 duplicates of that item.\n\nUsing magic and dedication this bag can duplicate anything! It's mostly magic. " +
                "Ok, it's all magic.";
            var item = EasyInitItem<MagicBag>("MagicBag", "magic_sack_idle_001", name, shortdesc, longdesc, ItemQuality.B, null, null);
			item.SetCooldownType(CooldownType.Damage, item.damageBasedRecharge = 250f);
			item.timedRecharge = 0.5f;
			item.spawnLaunchForce = 1f;
			item.minItemDistance = 2f;
        }

        public override void DoOnCooldownEffect(PlayerController user)
        {
            base.DoOnCooldownEffect(user);
			if(contents != Contents.NONE)
            {
				contents = Contents.NONE;
				AkSoundEngine.PostEvent("Play_CHR_muncher_eat_01", user.gameObject);
				ClearCooldowns();
            }
        }

		private IEnumerator HandleSuck(tk2dSprite targetSprite)
		{
			float elapsed = 0f;
			float duration = 0.25f;
			PlayerController owner = LastOwner;
			if (targetSprite)
			{
				Vector3 startPosition = targetSprite.transform.position;
				while (elapsed < duration && owner)
				{
					elapsed += BraveTime.DeltaTime;
					if (targetSprite)
					{
						targetSprite.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), elapsed / duration);
						targetSprite.transform.position = Vector3.Lerp(startPosition, owner.CenterPosition.ToVector3ZisY(0f), elapsed / duration);
					}
					yield return null;
				}
			}
			Destroy(targetSprite.gameObject);
			yield break;
		}

		public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
			if(contents != Contents.NONE)
            {
				int id = contents switch
                {
					Contents.KEY => KeyId,
					Contents.HALF_HEART => HalfHeartId,
					Contents.FULL_HEART => HeartId,
					Contents.ARMOR => ArmorId,
					Contents.AMMO => AmmoId,
					Contents.SPREAD_AMMO => SpreadAmmoId,
					Contents.BLANK => BlankId,
					Contents.GLASS_GUON => GlassGuonStoneId,
					_ => -1
                };
				if(id >= 0)
                {
					for(int i = 0; i < 2; i++)
                    {
						LootEngine.SpawnItem(GetItemById(id).gameObject, user.CenterPosition, Random.insideUnitCircle, spawnLaunchForce, true, true, false);
                    }
				}
				contents = Contents.NONE;
				AkSoundEngine.PostEvent("Play_CHR_muncher_eat_01", user.gameObject);
				damageCooldown = 0f;
				timeCooldown = timedRecharge;
			}
            else
			{
				if (user)
				{
					IPlayerInteractable lastInteractable = user.GetLastInteractable();
					if (lastInteractable is HeartDispenser)
					{
						HeartDispenser exists = lastInteractable as HeartDispenser;
						if (exists && HeartDispenser.CurrentHalfHeartsStored > 0)
						{
							if (HeartDispenser.CurrentHalfHeartsStored > 1)
							{
								HeartDispenser.CurrentHalfHeartsStored -= 2;
								contents = Contents.FULL_HEART;
							}
							else
							{
								HeartDispenser.CurrentHalfHeartsStored--;
								contents = Contents.HALF_HEART;
							}
							return;
						}
					}
				}
				List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
				if (allDebris != null)
				{
					DebrisObject closestDebris = null;
					var closestDistance = minItemDistance;
					foreach (var debris in allDebris)
					{
						if (debris != null && debris.IsPickupObject)
						{
							float dist = Vector2.Distance(debris.transform.position.XY(), user.CenterPosition);
							if (dist <= closestDistance)
							{
								HealthPickup health = debris.GetComponent<HealthPickup>();
								AmmoPickup ammo = debris.GetComponent<AmmoPickup>();
								KeyBulletPickup key = debris.GetComponent<KeyBulletPickup>();
								SilencerItem blank = debris.GetComponent<SilencerItem>();
								IounStoneOrbitalItem guon = debris.GetComponent<IounStoneOrbitalItem>();
								if ((health != null && (health.armorAmount == 1 || health.healAmount == 0.5f || health.healAmount == 1f)) || ammo != null || key != null || blank != null || (guon != null &&
									guon.PickupObjectId == GlassGuonStoneId))
								{
									closestDebris = debris;
								}
							}
						}
					}
					if(closestDebris != null)
					{
						HealthPickup health = closestDebris.GetComponent<HealthPickup>();
						AmmoPickup ammo = closestDebris.GetComponent<AmmoPickup>();
						KeyBulletPickup key = closestDebris.GetComponent<KeyBulletPickup>();
						SilencerItem blank = closestDebris.GetComponent<SilencerItem>();
						IounStoneOrbitalItem guon = closestDebris.GetComponent<IounStoneOrbitalItem>();
						if (closestDebris.sprite != null)
						{
							tk2dSprite targetSprite = tk2dSprite.AddComponent(new GameObject("sucked sprite")
							{
								transform =
												{
													position = closestDebris.transform.position
												}
							}, closestDebris.sprite.Collection, closestDebris.sprite.spriteId);
							AkSoundEngine.PostEvent("Play_NPC_BabyDragun_Munch_01", user.gameObject);
							GameManager.Instance.Dungeon.StartCoroutine(HandleSuck(targetSprite));
						}
						if (health != null && health.armorAmount == 1 && health.healAmount == 0f)
						{
							contents = Contents.ARMOR;
						}
						else if (health != null && health.healAmount == 0.5f && health.armorAmount == 0)
						{
							contents = Contents.HALF_HEART;
						}
						else if (health != null && health.healAmount == 1f && health.armorAmount == 0)
						{
							contents = Contents.FULL_HEART;
						}
						else if (ammo != null && ammo.mode == AmmoPickup.AmmoPickupMode.FULL_AMMO)
						{
							contents = Contents.AMMO;
						}
						else if (ammo != null && ammo.mode == AmmoPickup.AmmoPickupMode.SPREAD_AMMO)
						{
							contents = Contents.SPREAD_AMMO;
						}
						else if (key != null)
						{
							contents = Contents.KEY;
						}
						else if (blank != null)
						{
							contents = Contents.BLANK;
						}
						else if (guon != null && guon.PickupObjectId == GlassGuonStoneId)
						{
							contents = Contents.GLASS_GUON;
						}
						Destroy(closestDebris.gameObject);
					}
					if(contents != Contents.NONE)
                    {
						damageCooldown = damageBasedRecharge;
						timeCooldown = 0f;
                    }
				}
			}
        }

		public override bool CanBeUsed(PlayerController user)
		{
			if (!user)
			{
				return false;
			}
            if (user.IsInCombat)
            {
				return false;
            }
			if (contents != Contents.NONE)
			{
				return true;
			}
			List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
			if (allDebris != null)
			{
				foreach(var debris in allDebris)
                {
					if (debris != null && debris.IsPickupObject)
					{
						float dist = Vector2.Distance(debris.transform.position.XY(), user.CenterPosition);
						if (dist <= minItemDistance)
						{
							HealthPickup health = debris.GetComponent<HealthPickup>();
							AmmoPickup ammo = debris.GetComponent<AmmoPickup>();
							KeyBulletPickup key = debris.GetComponent<KeyBulletPickup>();
							SilencerItem blank = debris.GetComponent<SilencerItem>();
							IounStoneOrbitalItem guon = debris.GetComponent<IounStoneOrbitalItem>();
							if ((health != null && (health.armorAmount == 1 || health.healAmount == 0.5f || health.healAmount == 1f)) || ammo != null || key != null || blank != null || (guon != null && 
								guon.PickupObjectId == GlassGuonStoneId))
							{
								return true;
							}
						}
					}
				}
			}
			if (user)
			{
				IPlayerInteractable lastInteractable = user.GetLastInteractable();
				if (lastInteractable is HeartDispenser)
				{
					HeartDispenser exists = lastInteractable as HeartDispenser;
					if (exists && HeartDispenser.CurrentHalfHeartsStored > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void CopyStateFrom(PlayerItem other)
        {
            base.CopyStateFrom(other);
            if(other is MagicBag bag)
            {
                contents = bag.contents;
            }
        }

		public float minItemDistance;
		public float timedRecharge;
        public float damageBasedRecharge;
		public float spawnLaunchForce;
		private Contents contents;
		public enum Contents
		{
			NONE,
			HALF_HEART,
			FULL_HEART,
			AMMO,
			SPREAD_AMMO,
			BLANK,
			KEY,
			ARMOR,
			GLASS_GUON
		}
	}
}
