using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class OrbitProjectile : BraveBehaviour
    {
        public void Start()
        {
            if (projectile != null && (projectile.OverrideMotionModule == null || projectile.OverrideMotionModule is not OrbitProjectileMotionModule))
            {
                OrbitProjectileMotionModule orbitProjectileMotionModule = new();
                orbitProjectileMotionModule.lifespan = 15f;
                if (projectile.OverrideMotionModule != null && projectile.OverrideMotionModule is HelixProjectileMotionModule helix)
                {
                    orbitProjectileMotionModule.StackHelix = true;
                    orbitProjectileMotionModule.ForceInvert = helix.ForceInvert;
                }
                orbitProjectileMotionModule.MinRadius = orbitProjectileMotionModule.MaxRadius = 4.5f;
                projectile.OverrideMotionModule = orbitProjectileMotionModule;
                projectile.PreMoveModifiers += (x) => LateUpdate();
            }
            if(projectile?.OverrideMotionModule != null && projectile.OverrideMotionModule is OrbitProjectileMotionModule orbit)
            {
                if(orbit.MaxRadius > 0.025f)
                {
                    m_cachedMaxRadius = orbit.MaxRadius - 0.025f;
                    orbit.MaxRadius = 0.025f;
                }
                if (orbit.MinRadius > 0.025f)
                {
                    m_cachedMinRadius = orbit.MinRadius - 0.025f;
                    orbit.MinRadius = 0.025f;
                }
            }
        }

        public void LateUpdate()
        {
            if(projectile?.OverrideMotionModule != null && projectile.OverrideMotionModule is OrbitProjectileMotionModule orbit)
            {
                if (orbit.m_initialized)
                {
                    if (!m_appliedCachedValues)
                    {
                        orbit.MaxRadius += m_cachedMaxRadius;
                        orbit.MinRadius += m_cachedMinRadius;
                        m_appliedCachedValues = true;
                    }
                    currentOrbitDistance ??= Random.Range(orbit.MinRadius, orbit.MaxRadius);
                    orbit.m_radius = Mathf.Lerp(0.025f, Mathf.Max(currentOrbitDistance.GetValueOrDefault(), 0.025f), Mathf.Min(projectile.ElapsedTime / expandTime, 1));
                }
            }
        }

        public float expandTime;
        private float? currentOrbitDistance;
        private float m_cachedMinRadius;
        private float m_cachedMaxRadius;
        private bool m_appliedCachedValues;
    }
}
