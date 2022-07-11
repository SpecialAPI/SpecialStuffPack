using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.BulletScripts.UltimateDragun
{
    public class DraGunFlameBreath3 : Script
    {
		public override IEnumerator Top()
		{
			StopYHeight = BulletBank.aiActor.ParentRoom.area.UnitBottomLeft.y + 21f;
			int pocketResetTimer = 0;
			float pocketAngle = 0f;
			float pocketSign = BraveUtility.RandomSign();
			for (int i = 0; i < 120; i++)
			{
				if (i % 40 == 27)
				{
					for (int j = 0; j < 12; j++)
					{
						Fire(new Direction(SubdivideArc(-30f, 60f, 12, j, false), DirectionType.Aim, -1f), new Speed(14f, SpeedType.Absolute), new FlameBullet(true));
					}
				}
				for(int j = 0; j < 1 + i % 2; j++)
				{
					float direction = UnityEngine.Random.Range(-45f, 45f);
					if (pocketResetTimer == 0)
					{
						pocketAngle = pocketSign * UnityEngine.Random.Range(0f, 22.5f);
						pocketSign *= -1f;
						pocketResetTimer = 30;
					}
					pocketResetTimer--;
					if (direction >= pocketAngle - 5f && direction <= pocketAngle)
					{
						direction -= 5f;
					}
					else if (direction <= pocketAngle + 5f && direction >= pocketAngle)
					{
						direction += 5f;
					}
					Fire(new Direction(direction, DirectionType.Aim, -1f), new Speed(14f, SpeedType.Absolute), new DraGunFlameBreath2.FlameBullet());
				}
				yield return Wait(2);
			}
			yield break;
		}

		protected static float StopYHeight;

		public class FlameBullet : Bullet
		{
			public FlameBullet(bool doesBounce = false) : base("Breath", false, false, false)
			{
				bouncy = doesBounce;
			}

			public override IEnumerator Top()
			{
                if (bouncy)
                {
					Projectile.GetOrAddComponent<BounceProjModifier>().numberOfBounces = 3;
                }
				while (Position.y > StopYHeight && (!bouncy || Projectile.GetOrAddComponent<BounceProjModifier>().numberOfBounces > 0))
				{
					yield return Wait(1);
				}
				ChangeSpeed(new Speed(0.33f, SpeedType.Absolute), 12);
				yield return Wait(60);
				Vanish(false);
				yield break;
			}

            public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
            {
                base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
				if(Projectile.GetComponent<BounceProjModifier>() != null && bouncy)
                {
					UnityEngine.Object.Destroy(Projectile.GetComponent<BounceProjModifier>());
                }
            }

            private bool bouncy;
		}
	}
}
