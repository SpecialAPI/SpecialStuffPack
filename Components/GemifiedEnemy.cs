using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class GemifiedEnemy : BraveBehaviour
    {
        public void Awake()
        {
            type = (GemType)Random.Range(0, 3);
            if(aiActor != null)
            {
				vfx = aiActor.PlayEffectOnActor(AssetBundleManager.Load<GameObject>($"magicgem{type}"), new Vector3(0f, specRigidbody.HitboxPixelCollider.UnitDimensions.y, 0f));
			}
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(vfx != null)
            {
                Destroy(vfx);
            }
        }

        public GemType type;
        public GameObject vfx;

        public enum GemType
        {
            Teleport,
            Destroy,
            Mirror
        }
    }
}
