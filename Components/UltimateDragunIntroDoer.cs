using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using Dungeonator;
using System.Collections;

namespace SpecialStuffPack.Components
{
    [RequireComponent(typeof(GenericIntroDoer))]
    public class UltimateDraGunIntroDoer : SpecificIntroDoer
    {
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public override IntVector2 OverrideExitBasePosition(DungeonData.Direction directionToWalk, IntVector2 exitBaseCenter)
		{
			return exitBaseCenter + new IntVector2(0, DraGunRoomPlaceable.HallHeight);
		}

		public override Vector2? OverrideIntroPosition
		{
			get
			{
				return new Vector2?(specRigidbody.UnitCenter + new Vector2(0f, 4f));
			}
		}

		public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
		{
			m_introDummy = transform.Find("RoarDummy").GetComponent<tk2dSpriteAnimator>();
			//m_introDummy.aiAnimator = aiAnimator;
			m_neck = transform.Find("Neck").gameObject;
			m_wings = transform.Find("Wings").gameObject;
			m_leftArm = transform.Find("LeftArm").gameObject;
			m_rightArm = transform.Find("RightArm").gameObject;
			m_introDummy.gameObject.SetActive(true);
			renderer.enabled = false;
			m_neck.SetActive(false);
			m_wings.SetActive(false);
			m_leftArm.SetActive(false);
			m_rightArm.SetActive(false);
			StartCoroutine(RunEmbers());
		}

		private IEnumerator RunEmbers()
		{
			DraGunRoomPlaceable emberDoer = aiActor.ParentRoom.GetComponentsAbsoluteInRoom<DraGunRoomPlaceable>()[0];
			emberDoer.UseInvariantTime = true;
			while (!m_isFinished)
			{
				yield return null;
			}
			emberDoer.UseInvariantTime = false;
			yield break;
		}

		public override void StartIntro(List<tk2dSpriteAnimator> animators)
		{
			animators.Add(m_introDummy);
			GetComponent<DragunCracktonMap>().ConvertToGold();
			StartCoroutine(DoIntro());
		}

		public IEnumerator DoIntro()
		{
			foreach(tk2dSpriteAnimator anim in m_neck.GetComponentsInChildren<tk2dSpriteAnimator>())
            {
				anim.ignoreTimeScale = true;
			}
			foreach (tk2dSpriteAnimator anim in m_wings.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = true;
			}
			foreach (tk2dSpriteAnimator anim in m_leftArm.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = true;
			}
			foreach (tk2dSpriteAnimator anim in m_rightArm.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = true;
			}
			yield return new WaitForSecondsRealtime(1f);
			m_introDummy.AnimateDuringBossIntros = true;
			m_introDummy.Play(m_introDummy.GetClipByName("roar"), 0f, 6, false);
			while(m_introDummy.sprite.GetCurrentSpriteDef().name != "dragun_gold_roar_012")
            {
				yield return null;
			}
			AkSoundEngine.PostEvent("Play_WPN_grenade_blast_01", m_introDummy.gameObject);
			AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", m_introDummy.gameObject);
			AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", m_introDummy.gameObject);
			float screamTime = 0.1f;
			float screamTimer = screamTime;
			while (m_introDummy.IsPlaying("roar"))
			{
				screamTimer += GameManager.INVARIANT_DELTA_TIME;
				if(screamTimer >= screamTime)
                {
					Instantiate((GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab"), Vector2.Lerp(m_introDummy.sprite.WorldCenter, m_introDummy.sprite.WorldTopCenter, 0.5f).ToVector3ZUp(0f), Quaternion.identity).GetComponentInChildren<tk2dSpriteAnimator>().AnimateDuringBossIntros = true;
					screamTimer = 0f;
				}
				yield return null;
			}
			m_introDummy.AnimateDuringBossIntros = false;
			m_introDummy.gameObject.SetActive(false);
			renderer.enabled = true;
			foreach (tk2dSpriteAnimator anim in m_neck.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = false;
			}
			foreach (tk2dSpriteAnimator anim in m_wings.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = false;
			}
			foreach (tk2dSpriteAnimator anim in m_leftArm.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = false;
			}
			foreach (tk2dSpriteAnimator anim in m_rightArm.GetComponentsInChildren<tk2dSpriteAnimator>())
			{
				anim.ignoreTimeScale = false;
			}
			m_neck.SetActive(true);
			m_wings.SetActive(true);
			m_leftArm.SetActive(true);
			m_rightArm.SetActive(true);
			aiAnimator.EndAnimation();
			GetComponent<DraGunController>().ModifyCamera(true);
			m_isFinished = true;
			AkSoundEngine.PostEvent("Play_ENM_gorgun_gaze_01", gameObject);
			yield break;
		}

		public override bool IsIntroFinished
		{
			get
			{
				return m_isFinished;
			}
		}

		public override void EndIntro()
		{
			m_introDummy.gameObject.SetActive(false);
			renderer.enabled = true;
			m_neck.SetActive(true);
			m_wings.SetActive(true);
			m_leftArm.SetActive(true);
			m_rightArm.SetActive(true);
			aiAnimator.EndAnimation();
			DraGunController component = GetComponent<DraGunController>();
			component.ModifyCamera(true);
			component.BlockPitTiles(true);
			component.HasDoneIntro = true;
			m_isFinished = true;
		}

		private bool m_isFinished;
		private tk2dSpriteAnimator m_introDummy;
		private GameObject m_neck;
		private GameObject m_wings;
		private GameObject m_leftArm;
		private GameObject m_rightArm;
	}
}
