using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class CrownBullets : PassiveItem
    {
        public static void Init()
        {
            var name = "Crown Bullets";
            var shortdesc = "Dark and Evil";
            var longdesc = "Bullets possess the Gundead. Survivors can be indoctrinated to become companions permanently... or almost.\n\nThese bullets come from the mysterious and elusive cult of the Blam.";
            var item = EasyItemInit<CrownBullets>("CrownBullets", "crown_bullets_idle_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.chance = 0.05f;
            var yellowChamberCharm = YellowChamberObject.CharmEffect;
            indoctrinateEffect = new CultCharmEffect()
            {
                AffectsEnemies = true,
                AffectsPlayers = false,
                effectIdentifier = "charm",
                resistanceType = EffectResistanceType.Charm,
                stackMode = GameActorEffect.EffectStackingMode.Ignore,
                duration = -1f,
                maxStackedDuration = -1f,
                AppliesTint = true,
                TintColor = yellowChamberCharm.TintColor,
                AppliesDeathTint = true,
                DeathTintColor = yellowChamberCharm.DeathTintColor,
                AppliesOutlineTint = false,
                OutlineTintColor = Color.black,
                PlaysVFXOnActor = true,
                OverheadVFX = yellowChamberCharm.OverheadVFX,
                indoctrinateInteractable = RedGun.globalIndoctrinateInteractable,
                usesHealthAsCurrency = true,
                healthRequired = 0.5f
            };
            item.AddToCursulaShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += PostProcessProjectile;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if(Random.value < chance.Scale(f))
            {
                proj.statusEffectsToApply.Add(indoctrinateEffect);
                proj.AdjustPlayerProjectileTint(Color.red, 0, 0f);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
        }

        public float chance;
        public static GameActorEffect indoctrinateEffect; //static because unity is bad and doesnt want to serialize my stuff :(
    }
}
