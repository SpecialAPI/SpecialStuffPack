using System;
using System.Collections.Generic;
using System.Collections;
using Dungeonator;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;

namespace SpecialStuffPack.PlayMakerActions
{
	public class FriendlyRatGrabby : FsmStateAction
	{
		public override void Awake()
		{
			base.Awake();
			m_talkDoer = Owner.GetComponent<TalkDoerLite>();
		}

		public override void OnEnter()
		{
			m_lastPosition = m_talkDoer.specRigidbody.UnitCenter;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (m_talkDoer.CurrentPath != null)
			{
				if (!m_talkDoer.CurrentPath.WillReachFinalGoal)
				{
					m_talkDoer.transform.position = TargetObject.sprite.WorldCenter + new Vector2(0f, 1f);
					m_talkDoer.specRigidbody.Reinitialize();
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(m_talkDoer.specRigidbody, new int?(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider)), false);
					m_talkDoer.talkDoer.CurrentPath = null;
				}
				else
				{
					m_talkDoer.specRigidbody.Velocity = m_talkDoer.GetPathVelocityContribution(m_lastPosition, 32);
					m_lastPosition = m_talkDoer.specRigidbody.UnitCenter;
				}
			}
		}

		private IEnumerator HandleGrabby()
		{
			m_grabby = true;
			m_talkDoer.aiAnimator.PlayUntilFinished("laugh", false, null, -1f, false);
			yield return new WaitForSeconds(2f);
			if(TargetObject != null)
			{
				m_talkDoer.aiAnimator.PlayUntilFinished("grabby", true, null, -1f, false);
				yield return new WaitForSeconds(0.55f);
			}
			if (Fsm.ActiveState != State)
			{
				yield break;
			}
			Fsm.SuppressGlobalTransitions = true;
			if (TargetObject && TargetObject.GetComponentInParent<PlayerController>())
			{
				yield break;
			}
			if (TargetObject is IPlayerInteractable)
			{
				RoomHandler.unassignedInteractableObjects.Remove(TargetObject as IPlayerInteractable);
			}
			float elapsed = 0f;
			float duration = 0.25f;
			if (TargetObject && TargetObject.transform)
			{
				Vector3 startPosition = TargetObject.transform.position;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime;
					if (TargetObject && TargetObject.transform && TargetObject.sprite != null)
					{
						TargetObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), elapsed / duration);
						TargetObject.transform.position = Vector3.Lerp(startPosition, m_talkDoer.transform.position + new Vector3(0.4375f, 0.375f, 0f), elapsed / duration);
					}
					yield return null;
				}
				if (TargetObject is Gun)
				{
					(TargetObject as Gun).GetRidOfMinimapIcon();
				}
				if (TargetObject is PlayerItem)
				{
					(TargetObject as PlayerItem).GetRidOfMinimapIcon();
				}
				if (TargetObject is PassiveItem)
				{
					(TargetObject as PassiveItem).GetRidOfMinimapIcon();
				}
				if (TargetObject is Gun && TargetObject.transform.parent != null)
				{
					UnityEngine.Object.Destroy(TargetObject.transform.parent.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(TargetObject.gameObject);
				}
				GameStatsManager.Instance.RegisterStatChange(TrackedStats.ITEMS_TAKEN_BY_RAT, 1f);
				yield return new WaitForSeconds(0.9f);
			}
			m_talkDoer.aiAnimator.PlayUntilFinished("grab_laugh", true, null, -1f, false);
			yield return new WaitForSeconds(1f);
			if (m_talkDoer)
			{
				UnityEngine.Object.Instantiate(bombObject, m_talkDoer.transform.position, Quaternion.identity);
			}
            else
            {
            }
			Fsm.SuppressGlobalTransitions = false;
			Finish();
			yield break;
		}

		public override void OnLateUpdate()
		{
			if (m_talkDoer.CurrentPath != null)
			{
				return;
			}
			if (!m_grabby)
			{
				m_talkDoer.StartCoroutine(HandleGrabby());
			}
		}

		public PickupObject TargetObject;
		public GameObject bombObject;
		private Vector2 m_lastPosition;
		private TalkDoerLite m_talkDoer;
		private bool m_grabby;
	}
}
