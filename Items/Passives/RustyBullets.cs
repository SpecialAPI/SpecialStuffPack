using SpecialStuffPack.StatusEffects;
using SpecialStuffPack.SynergyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Passives
{
    public class RustyBullets : PassiveItem
    {
        public static void Init()
        {
            string name = "Rusty Bullets";
            string shortdesc = "Not Very Trusty";
            string longdesc = "Bullets have a chance to be rusty, dealing less damage but infecting enemies with Rust. Enemies infected with Rust will be slower and take more damage.\n\n" +
                "How can bullet kin rust if they aren't iron? Nobody truly knows.";
            var item = EasyItemInit<RustyBullets>("RustyBullets", "rusty_bullets_001", name, shortdesc, longdesc, ItemQuality.A, null, null);
            item.damageMultiplier = 0.5f;
            item.chance = 0.1f;
            item.synergyChance = 0.25f;
            item.tint = new Color32(135, 100, 71, 255);
            item.AddToBlacksmithShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += RustifyBullet;
            player.OnUsedPlayerItem += IronCoinSynergy;
        }

        public void IronCoinSynergy(PlayerController player, PlayerItem item)
        {
            if(item is IronCoinItem && Owner.PlayerHasActiveSynergy("Rusty Coin"))
            {
                if(player.CurrentRoom != null)
                {
                    var enemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if(enemies != null)
                    {
                        foreach(var enemy in enemies)
                        {
                            enemy.ApplyEffect(Rust);
                        }
                    }
                }
            }
        }

        public void RustifyBullet(Projectile proj, float scale)
        {
            if (ScaledChance(Owner.PlayerHasActiveSynergy("Rusty Iron") ? synergyChance : chance, scale))
            {
                proj.statusEffectsToApply.Add(Rust);
                proj.baseData.damage *= damageMultiplier;
                proj.AdjustPlayerProjectileTint(tint, 0, 0f);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            player.PostProcessProjectile -= RustifyBullet;
            player.OnUsedPlayerItem -= IronCoinSynergy;
            base.DisableEffect(player);
        }

        public float chance;
        public float synergyChance;
        public float damageMultiplier;
        public Color tint;

        public static RustStatusEffect Rust = new()
        {
            AffectsEnemies = true,
            AffectsPlayers = false,
            duration = 10f,
            AppliesDeathTint = true,
            DeathTintColor = new Color32(83, 36, 15, 255),
            AppliesTint = true,
            TintColor = new Color32(135, 100, 71, 255),
            AppliesOutlineTint = false,
            auraColor = new Color32(135, 100, 71, 255),
            auraRadius = 5f,
            damageMultiplier = 1.5f,
            speedMultiplier = 0.85f,
            effectIdentifier = "rust",
            maxStackedDuration = -1f,
            stackMode = GameActorEffect.EffectStackingMode.Ignore,
            onApplyEvent = "StatusRust",
            OutlineTintColor = Color.black,
            OverheadVFX = null,
            PlaysVFXOnActor = false,
            resistanceType = EffectResistanceTypeE.Rust
        };
    }
}
