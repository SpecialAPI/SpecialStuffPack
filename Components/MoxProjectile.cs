using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class MoxProjectile : BraveBehaviour
    {
        public void Start()
        {
            radialIndicator = ((GameObject)Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), specRigidbody.UnitCenter, Quaternion.identity, transform)).GetComponent<HeatIndicatorController>();
            radialIndicator.IsFire = false;
            radialIndicator.CurrentColor = auraColor;
            radialIndicator.CurrentRadius = auraRadius;
            playerOwner = projectile.Owner as PlayerController;
        }

        public void Update()
        {
            radialIndicator.CurrentRadius = Radius;
            foreach(var proj in StaticReferenceManager.AllProjectiles)
            {
                if(proj.Owner != null && proj.Owner is PlayerController && Vector2.Distance(specRigidbody.UnitCenter, proj.specRigidbody.UnitCenter) <= radialIndicator.CurrentRadius && proj.GetComponent<MoxProjectile>() == null)
                {
                    if(proj.GetComponent<AffectedByMox>() == null || !proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Contains(type))
                    {
                        proj.GetOrAddComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Add(type);
                        proj.AdjustPlayerProjectileTint(auraColor, 0, 0f);
                        switch (type)
                        {
                            case MoxType.Green:
                                proj.GetOrAddPierce().penetration += 1;
                                break;
                            case MoxType.Orange:
                                proj.baseData.damage *= 1.25f;
                                break;
                            case MoxType.Blue:
                                var home = proj.GetOrAddComponent<HomingModifier>();
                                home.HomingRadius += 5f;
                                home.AngularVelocity += 210f;
                                break;
                            case MoxType.Magnum:
                                if (!proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Contains(MoxType.Green))
                                {
                                    proj.GetOrAddPierce().penetration += 1;
                                    proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Add(MoxType.Green);
                                }
                                if (!proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Contains(MoxType.Orange))
                                {
                                    proj.baseData.damage *= 1.25f;
                                    proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Add(MoxType.Orange);
                                }
                                if (!proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Contains(MoxType.Blue))
                                {
                                    var home2 = proj.GetOrAddComponent<HomingModifier>();
                                    home2.HomingRadius += 5f;
                                    home2.AngularVelocity += 210f;
                                    proj.GetComponent<AffectedByMox>().moxThisHasBeenAffectedByYes.Add(MoxType.Blue);
                                }
                                break;
                        }
                    }
                }
            }
            if(playerOwner != null && playerOwner.CurrentRoom != null && HasSynergy)
            {
                playerOwner.CurrentRoom.ApplyActionToNearbyEnemies(specRigidbody.UnitCenter, radialIndicator.CurrentRadius, SynergyAuraAction);
            }
            if(projectile.m_currentSpeed <= 0.03125)
            {
                destroyTimer += BraveTime.DeltaTime;
                if(destroyTimer >= (HasSynergy ? destroyTimeSynergy : destroyTime))
                {
                    projectile.DieInAir(false, false, false, false);
                }
            }
        }

        public void SynergyAuraAction(AIActor ai, float dist)
        {
            switch (type)
            {
                case MoxType.Green:
                    ai.ApplyEffect(slowness);
                    break;
                case MoxType.Orange:
                    ai.ApplyEffect(burn);
                    break;
                case MoxType.Blue:
                    if(ai.behaviorSpeculator != null)
                    {
                        ai.behaviorSpeculator.Stun(0.1f, true);
                    }
                    break;
                case MoxType.Magnum:
                    ai.ApplyEffect(slowness);
                    ai.ApplyEffect(burn);
                    if (ai.behaviorSpeculator != null)
                    {
                        ai.behaviorSpeculator.Stun(0.1f, true);
                    }
                    break;
            }
        }

        public bool HasSynergy => (playerOwner != null) && (playerOwner.PlayerHasActiveSynergy(synergy) ||
            (type == MoxType.Magnum && (playerOwner.PlayerHasActiveSynergy(otherSynergy) || playerOwner.PlayerHasActiveSynergy(otherOtherSynergy))));
        public float Radius => HasSynergy ? synergyRadius : auraRadius;

        public float destroyTimer;
        public float auraRadius;
        public float destroyTime;
        public float destroyTimeSynergy;
        public Color auraColor;
        public MoxType type;
        public string synergy;
        public string otherSynergy;
        public string otherOtherSynergy;
        public float synergyRadius;
        public PlayerController playerOwner;
        public HeatIndicatorController radialIndicator;
        public GameActorSpeedEffect slowness;
        public GameActorFireEffect burn;

        public enum MoxType
        {
            Green,
            Orange,
            Blue,
            Magnum
        }
    }
}
