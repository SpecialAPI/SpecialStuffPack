using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class IndoctrinateFollowerInteractable : BraveBehaviour, IPlayerInteractable
    {
		public string noRedgunKey;
		public string notEnoughAmmoKey;
		public string validKey;
		public string yesKey;
		public string yesHealthKey;
		public string noKey;
		public string enemyGuid;
		public bool usesHealthAsCurrency;
		public float healthRequired;

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
			shouldBeFlipped = false;
			return "";
        }

        public float GetDistanceToPoint(Vector2 point)
		{
			if (!gameObject.activeSelf)
			{
				return 10000f;
			}
			if (!sprite)
			{
				return 1000f;
			}
			Bounds bounds = sprite.GetBounds();
			Vector2 vector = transform.position.XY() + (base.transform.rotation * bounds.min).XY();
			Vector2 a = vector + (transform.rotation * bounds.size).XY();
			return BraveMathCollege.DistToRectangle(point, vector, a - vector);
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

        public void Interact(PlayerController interactor)
        {
			if (TextBoxManager.HasTextBox(transform) || interactor.IsInCombat)
			{
				return;
			}
			StartCoroutine(HandleShrineConversation(interactor));
		}

		public IEnumerator HandleShrineConversation(PlayerController interactor)
		{
			var key = validKey;
			var isValid = false;
            if (usesHealthAsCurrency)
			{
				isValid = interactor.healthHaver.GetCurrentHealth() > healthRequired;
                if (!isValid)
                {
					key = notEnoughAmmoKey;
                }
			}
            else
            {
				if (interactor == null || interactor.CurrentGun == null || interactor.CurrentGun.GetComponent<RedGun>() == null)
				{
					key = noRedgunKey;
				}
				else if (interactor.CurrentGun.ammo < interactor.CurrentGun.GetComponent<RedGun>().indoctrinateAmmoCost)
				{
					key = notEnoughAmmoKey;
				}
				else
				{
					isValid = true;
				}
			}
			TextBoxManager.ShowTextBox(sprite.WorldTopCenter + Vector2.up / 2f, transform, -1f, StringTableManager.GetString(key), "", false, TextBoxManager.BoxSlideOrientation.FORCE_LEFT, false, false);
			interactor.SetInputOverride("shrineConversation");
			yield return null;
			if (isValid)
			{
				GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, usesHealthAsCurrency ?
					StringTableManager.GetString(yesHealthKey) :
					StringTableManager.GetString(yesKey).Replace("%AMMO", interactor.CurrentGun.GetComponent<RedGun>().indoctrinateAmmoCost.ToString()), 
					StringTableManager.GetString(noKey));
			}
			else
			{
				GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(noKey), string.Empty);
			}
			int selectedResponse;
			while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
			{
				yield return null;
			}
			TextBoxManager.ClearTextBox(transform);
			if (isValid && selectedResponse == 0)
			{
                if (usesHealthAsCurrency)
                {
					interactor.healthHaver.ForceSetCurrentHealth(Mathf.Max(interactor.healthHaver.GetCurrentHealth() - healthRequired, 0.5f));
                }
				else if(interactor != null && interactor.CurrentGun != null && interactor.CurrentGun.GetComponent<RedGun>() != null)
				{
					interactor.CurrentGun.LoseAmmo(interactor.CurrentGun.GetComponent<RedGun>().indoctrinateAmmoCost);
				}
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);
				GameObject companion = Instantiate(orLoadByGuid.gameObject, transform.position, Quaternion.identity);
				CompanionController orAddComponent = companion.GetOrAddComponent<CompanionController>();
				orAddComponent.CanInterceptBullets = true;
				if(orAddComponent.specRigidbody != null)
                {
					orAddComponent.specRigidbody.PixelColliders.ForEach(x => x.CollisionLayer = x.CollisionLayer == CollisionLayer.EnemyCollider ? CollisionLayer.PlayerCollider : x.CollisionLayer == CollisionLayer.EnemyHitBox ? CollisionLayer.PlayerHitBox : x.CollisionLayer);
                }
				orAddComponent.Initialize(interactor);
                companion.GetOrAddComponent<BetterCompanions>();
				var onDeathBehaviours = orAddComponent.GetComponents<OnDeathBehavior>().ToArray();
				foreach (var behav in onDeathBehaviours)
				{
					if (behav != null)
					{
						DestroyImmediate(behav);
					}
				}
				orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                if (orAddComponent.specRigidbody)
				{
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
				}
				Destroy(gameObject);
			}
			interactor.ClearInputOverride("shrineConversation");
			yield break;
        }

        public void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, false);
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.white);
			sprite.UpdateZDepth();
		}

		public void OnExitRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(sprite, true);
			SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black);
			sprite.UpdateZDepth();
		}
	}
}
