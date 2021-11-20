using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class BinaryGunMarkedBehaviour : BraveBehaviour
    {
        public void Mark(bool isOne)
        {
            if(aiActor == null)
            {
                aiActor = GetComponentInParent<AIActor>() ?? GetComponentInChildren<AIActor>();
            }
            if(aiActor != null)
            {
                if (m_currentMarkVfx == null)
                {
                    m_currentMarkVfx = aiActor.PlayEffectOnActor(MarkVFX, VFXOffset, true, false, true);
                }
                m_currentMarkVfx.GetComponent<tk2dBaseSprite>().SetSprite(isOne ? OneId : ZeroId);
            }
            m_isOne = isOne;
        }

        public bool IsOne
        {
            get
            {
                return m_isOne;
            }
        }

        public GameObject MarkVFX;
        public Vector2 VFXOffset;
        public int ZeroId;
        public int OneId;
        private bool m_isOne;
        private GameObject m_currentMarkVfx;
    }
}
