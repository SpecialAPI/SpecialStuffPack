using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dungeonator;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunGrenade2 : Script
    {
		public DraGunGrenade2()
		{
			NumRockets = 11;
			Magnitude = 4.5f;
		}

		protected override IEnumerator Top()
		{
			bool reverse = BraveUtility.RandomBool();
			StartTask(FireWave(reverse, false, 0f));
			yield return Wait(55);
			StartTask(FireWave(reverse, true, 0.25f));
			yield return Wait(80);
			for(int i = 0; i < 5; i++)
			{
				DraGunController dragunController = BulletBank.GetComponent<DraGunController>();
				if(BulletBank.aiActor.TargetRigidbody != null)
				{
					FireRocket(dragunController.skyRocket, BulletBank.aiActor.TargetRigidbody.UnitCenter);
				}
				yield return Wait(15);
            }
			yield break;
		}

		private IEnumerator FireWave(bool reverse, bool offset, float sinOffset)
		{
			DraGunController dragunController = BulletBank.GetComponent<DraGunController>();
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			Vector2 start = area.UnitBottomLeft + new Vector2(1f, 25.5f);
			for (int i = 0; i < ((!offset) ? NumRockets : (NumRockets - 1)); i++)
			{
				float t = ((!offset) ? ((float)i) : ((float)i + 0.5f)) / ((float)NumRockets - 1f);
				float dx = 34f * t;
				float dy = Mathf.Sin((t * 2.5f + sinOffset) * 3.1415927f) * Magnitude;
				if (reverse)
				{
					dx = 34f - dx;
				}
				FireRocket(dragunController.skyRocket, start + new Vector2(dx, dy));
				FireRocket(dragunController.skyRocket, start + new Vector2(dx, -dy));
				if (Mathf.Abs(dy) < 1f)
				{
					FireRocket(dragunController.skyRocket, start + new Vector2(dx, Magnitude));
					FireRocket(dragunController.skyRocket, start + new Vector2(dx, -Magnitude));
				}
				yield return Wait(10);
			}
			yield break;
		}

		private void FireRocket(GameObject skyRocket, Vector2 target)
		{
			SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, Position, Quaternion.identity, true).GetComponent<SkyRocket>();
			component.TargetVector2 = target;
			component.DescentTime *= 0.75f;
			component.AscentTime *= 0.75f;
			tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
			component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
			component.ExplosionData.ignoreList.Add(BulletBank.specRigidbody);
		}

		public int NumRockets;
		public float Magnitude;
	}
}
