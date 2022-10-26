using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Components
{
    public class ChanceBasedExplodeOnEnemyHit : BraveBehaviour
    {
        public void Start()
        {
            if(projectile != null)
            {
				if(projectile.Owner != null && projectile.Owner is PlayerController player)
                {
                    if (!string.IsNullOrEmpty(increaseDamageSynergy) && player.PlayerHasActiveSynergy(increaseDamageSynergy))
                    {
						explosion.damage *= synergyDamageMultiplier;
                    }
					if (!string.IsNullOrEmpty(increaseRangeSynergy) && player.PlayerHasActiveSynergy(increaseRangeSynergy))
					{
						explosion.damageRadius *= synergyRangeMultiplier;
						explosion.pushRadius *= synergyRangeMultiplier;
					}
					if(!string.IsNullOrEmpty(increaseChanceSynergy) && player.PlayerHasActiveSynergy(increaseChanceSynergy))
                    {
						chance = synergyChanceOverride;
                    }
				}
                projectile.OnHitEnemy += ChanceToExplode;
            }
        }

        public void ChanceToExplode(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
			if (Random.value < chance)
			{
				if(proj.Owner != null)
				{
					if (proj.Owner is PlayerController)
					{
						for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
						{
							PlayerController playerController = GameManager.Instance.AllPlayers[i];
							if (playerController && playerController.specRigidbody)
							{
								explosion.ignoreList.Add(playerController.specRigidbody);
							}
						}
					}
					else
					{
						explosion.ignoreList.Add(proj.Owner.specRigidbody);
					}
				}
				CoreDamageTypes coreDamageTypes = CoreDamageTypes.None;
				if (explosion.doDamage && explosion.damageRadius < 10f && proj)
				{
					if (proj.AppliesFreeze)
					{
						coreDamageTypes |= CoreDamageTypes.Ice;
					}
					if (proj.AppliesFire)
					{
						coreDamageTypes |= CoreDamageTypes.Fire;
					}
					if (proj.AppliesPoison)
					{
						coreDamageTypes |= CoreDamageTypes.Poison;
					}
					if (proj.statusEffectsToApply != null)
					{
						for (int j = 0; j < proj.statusEffectsToApply.Count; j++)
						{
							GameActorEffect gameActorEffect = proj.statusEffectsToApply[j];
							if (gameActorEffect is GameActorFreezeEffect)
							{
								coreDamageTypes |= CoreDamageTypes.Ice;
							}
							else if (gameActorEffect is GameActorFireEffect)
							{
								coreDamageTypes |= CoreDamageTypes.Fire;
							}
							else if (gameActorEffect is GameActorHealthEffect)
							{
								coreDamageTypes |= CoreDamageTypes.Poison;
							}
						}
					}
				}
				Exploder.Explode(specRigidbody.UnitCenter, explosion, Vector2.zero, null, ignoreQueues, coreDamageTypes, proj.ignoreDamageCaps);
			}
		}

        public ExplosionData explosion;
        public float chance;
        public bool ignoreQueues = true;
		public string increaseDamageSynergy;
		public float synergyDamageMultiplier = 1f;
		public string increaseRangeSynergy;
		public float synergyRangeMultiplier = 1f;
		public string increaseChanceSynergy;
		public float synergyChanceOverride;
    }
}
