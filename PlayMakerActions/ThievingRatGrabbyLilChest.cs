using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.PlayMakerActions
{
	public class ThievingRatGrabbyLilChest : FsmStateAction
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
			m_talkDoer.aiAnimator.PlayUntilFinished("grabby", true, null, -1f, false);
			yield return new WaitForSeconds(0.55f);
			if (Fsm.ActiveState != State)
			{
				yield break;
			}
			Fsm.SuppressGlobalTransitions = true;
			if (TargetObject == null || TargetObject.pickedUp || TargetObject.IsOpen)
			{
				yield break;
			}
			RoomHandler.unassignedInteractableObjects.Remove(TargetObject);
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
				UnityEngine.Object.Destroy(TargetObject.gameObject);
				GameStatsManager.Instance.RegisterStatChange(TrackedStats.ITEMS_TAKEN_BY_RAT, 1f);
				yield return new WaitForSeconds(0.9f);
			}
			m_talkDoer.aiAnimator.PlayUntilFinished("grab_laugh", true, null, -1f, false);
			yield return new WaitForSeconds(1f);
			if (m_talkDoer)
			{
				NoteDoer component = notePrefab.InstantiateObject(m_talkDoer.GetAbsoluteParentRoom(), m_talkDoer.transform.position.IntXY(VectorConversions.Floor) - m_talkDoer.GetAbsoluteParentRoom().area.basePosition, false).GetComponent<NoteDoer>();
				component.stringKey = StringTableManager.GetLongString("#RESRAT_NOTE_ITEM").Replace("%ITEM", "chest");
				m_talkDoer.GetAbsoluteParentRoom().RegisterInteractable(component);
				component.alreadyLocalized = true;
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

		public LilChest TargetObject;
		public NoteDoer notePrefab;
		private Vector2 m_lastPosition;
		private TalkDoerLite m_talkDoer;
		private bool m_grabby;
	}
}
