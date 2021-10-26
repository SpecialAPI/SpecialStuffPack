using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack.Components
{
    public class BlombkBehaviour : BraveBehaviour
    {
        public BlombkBehaviour()
        {
            DamageMult = 1f;
            ForceMult = 1f;
        }

        public IEnumerator Start()
        {
            float ela = 0;
            float ela2 = 0;
            float dura = 0.2f;
            bool b = false;
            while(ela < ExplosionDelay)
            {
                ela += BraveTime.DeltaTime;
                ela2 += BraveTime.DeltaTime;
                if(ela2 >= dura)
                {
                    b = !b;
                    sprite.usesOverrideMaterial = true;
                    if (b)
                    {
                        renderer.material.SetColor("_OverrideColor", Color.white);
                    }
                    else
                    {
                        renderer.material.SetColor("_OverrideColor", new Color(0f, 0f, 0f, 0f));
                    }
                    ela2 = 0;
                    dura -= 0.025f;
                }
                yield return null;
            }
            ExplosionData.damage *= DamageMult;
            ExplosionData.force *= ForceMult;
            Exploder.Explode(sprite.WorldCenter, ExplosionData, Vector2.zero, null, true, DamageType, true);
            m_owner.ForceBlank(25f, 0.5f, false, true, sprite.WorldCenter, true, -1f);
            if (!m_owner.IsInCombat)
            {
                for (int i = 0; i < StaticReferenceManager.AllAdvancedShrineControllers.Count; i++)
                {
                    if (StaticReferenceManager.AllAdvancedShrineControllers[i].IsBlankShrine && StaticReferenceManager.AllAdvancedShrineControllers[i].transform.position.GetAbsoluteRoom() == transform.position.GetAbsoluteRoom())
                    {
                        StaticReferenceManager.AllAdvancedShrineControllers[i].OnBlank();
                    }
                }
            }
            for (int j = 0; j < StaticReferenceManager.AllRatTrapdoors.Count; j++)
            {
                if (StaticReferenceManager.AllRatTrapdoors[j])
                {
                    StaticReferenceManager.AllRatTrapdoors[j].OnBlank();
                }
            }
            m_owner.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            m_owner.RaiseEvent2("OnUsedBlank", m_owner, m_owner.Blanks);
            Destroy(gameObject);
            yield break;
        }

        public void SetOwner(PlayerController owner)
        {
            m_owner = owner;
        }

        public ExplosionData ExplosionData;
        public float ExplosionDelay;
        public CoreDamageTypes DamageType;
        public float DamageMult;
        public float ForceMult;
        private PlayerController m_owner;
    }
}
