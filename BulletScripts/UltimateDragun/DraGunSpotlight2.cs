using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunSpotlight2 : Script
    {
		protected override IEnumerator Top()
		{
			GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = true;
			DraGunController dragunController = BulletBank.GetComponent<DraGunController>();
			dragunController.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_darken_world_01");
			dragunController.HandleDarkRoomEffects(true, 3f);
			yield return Wait(30);
			dragunController.SpotlightPos = BulletBank.aiActor.transform.position + new Vector3(4f, 1f);
			dragunController.SpotlightSpeed = 8f;
			dragunController.SpotlightSmoothTime = 0.2f;
			dragunController.SpotlightVelocity = Vector2.zero;
			dragunController.SpotlightEnabled = true;
			StartTask(UpdateSpotlightShrink());
			while (Tick < 480)
			{
				float dist = Vector2.Distance(BulletManager.PlayerPosition(), dragunController.SpotlightPos);
				dragunController.SpotlightSpeed = Mathf.Lerp(6f, 14f, Mathf.InverseLerp(3f, 10f, dist));
				if (dist <= dragunController.SpotlightRadius)
				{
					float t = UnityEngine.Random.value;
					float speed = Mathf.Lerp(8f, 14f, t);
					Vector2 target = (!BraveUtility.RandomBool()) ? GetPredictedTargetPositionExact(1f, speed) : BulletManager.PlayerPosition();
                    if (BraveUtility.RandomBool())
					{
						Fire(new Direction((target - Position).ToAngle(), DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), new DraGunSpotlight1.ArcBullet(target, t));
					}
                    else
					{
						Fire(new Direction((GetPredictedTargetPositionExact(1f, speed) - Position).ToAngle(), DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), new DraGunSpotlight1.ArcBullet(target, t));
						Fire(new Direction((BulletManager.PlayerPosition() - Position).ToAngle(), DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), new DraGunSpotlight1.ArcBullet(target, t));
					}
					yield return Wait(3);
				}
				yield return Wait(1);
			}
			dragunController.SpotlightEnabled = false;
			dragunController.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
			dragunController.HandleDarkRoomEffects(false, 3f);
			yield return Wait(30);
			GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = false;
			yield break;
		}

		private IEnumerator UpdateSpotlightShrink()
		{
			DraGunController dragunController = BulletBank.GetComponent<DraGunController>();
			int startTick = Tick;
			while (Tick < 480)
			{
				if (Tick - startTick < 10)
				{
					dragunController.SpotlightShrink = (float)(Tick - startTick) / 9f * 1.35f;
				}
				else if (Tick > 470)
				{
					int num = 480 - Tick - 1;
					dragunController.SpotlightShrink = (float)num / 9f * 1.35f;
				}
				yield return Wait(1);
			}
			yield break;
		}

		public override void OnForceEnded()
		{
			DraGunController component = BulletBank.GetComponent<DraGunController>();
			component.SpotlightEnabled = false;
			component.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
			component.HandleDarkRoomEffects(false, 3f);
		}

		public class ArcBullet : Bullet
		{
			public ArcBullet(Vector2 target, float t) : base("triangle", false, false, false)
			{
				m_target = target;
				m_t = t;
			}

			protected override IEnumerator Top()
			{
				Vector2 toTarget = m_target - Position;
				float travelTime = toTarget.magnitude / Speed * 60f - 1f;
				float magnitude = BraveUtility.RandomSign() * (1f - m_t) * 8f;
				Vector2 offset = magnitude * toTarget.Rotate(90f).normalized;
				ManualControl = true;
				Vector2 truePosition = Position;
				Vector2 lastPosition = Position;
				int i = 0;
				while ((float)i < travelTime)
				{
					UpdateVelocity();
					truePosition += Velocity / 60f;
					lastPosition = Position;
					Position = truePosition + offset * Mathf.Sin((float)Tick / travelTime * 3.1415927f);
					yield return Wait(1);
					i++;
				}
				Vector2 v = (Position - lastPosition) * 60f;
				Speed = v.magnitude;
				Direction = v.ToAngle();
				ManualControl = false;
				yield break;
			}

			private Vector2 m_target;
			private float m_t;
		}
	}
}
