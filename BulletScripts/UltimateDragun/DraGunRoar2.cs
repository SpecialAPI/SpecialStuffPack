using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using Dungeonator;
using System.Collections;
using SpecialStuffPack.Components;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunRoar2 : Script
    {
		public DraGunRoar2()
		{
			NumRockets = 4;
		}

		public override IEnumerator Top()
		{
			if (s_xValues == null || s_yValues == null)
			{
				s_xValues = new int[NumRockets];
				s_yValues = new int[NumRockets];
				for (int j = 0; j < NumRockets; j++)
				{
					s_xValues[j] = j;
					s_yValues[j] = j;
				}
			}
			DraGunController dragunController = BulletBank.GetComponent<DraGunController>();
			CellArea area = BulletBank.aiActor.ParentRoom.area;
			Vector2 roomLowerLeft = area.UnitBottomLeft + new Vector2(2f, 21f);
			Vector2 dimensions = new Vector2(32f, 6f);
			Vector2 delta = new Vector2(dimensions.x / (float)NumRockets, dimensions.y / (float)NumRockets);
			BraveUtility.RandomizeArray(s_xValues, 0, -1);
			BraveUtility.RandomizeArray(s_yValues, 0, -1);
			for (int i = 0; i < NumRockets; i++)
			{
				int baseX = s_xValues[i];
				int baseY = s_yValues[i];
				Vector2 goalPos = roomLowerLeft + new Vector2(((float)baseX + UnityEngine.Random.value) * delta.x, ((float)baseY + UnityEngine.Random.value) * delta.y);
				FireRocket(dragunController.skyBoulder, goalPos);
				yield return Wait(10);
			}
			yield break;
		}

		private void FireRocket(GameObject skyRocket, Vector2 target)
		{
			SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, Position, Quaternion.identity, true).GetComponent<SkyRocket>();
			component.TargetVector2 = target;
			tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
			component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
			component.ExplosionData.ignoreList.Add(BulletBank.specRigidbody);
			SpawnBulletScriptOnDestroy scriptOnDestroy = component.AddComponent<SpawnBulletScriptOnDestroy>();
			scriptOnDestroy.owner = BulletBank.aiActor;
			scriptOnDestroy.script = new CustomBulletScriptSelector(typeof(CircleBurst12));
		}

		public int NumRockets;
		private static int[] s_xValues;
		private static int[] s_yValues;
	}
}
