using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using SpecialStuffPack.SynergyAPI;

namespace SpecialStuffPack.Items
{
    public class GreenCandle : PlayerItem
    {
        public static void Init()
        {
            string name = "Green Candle";
            string shortdesc = "Light It On";
            string longdesc = "A strange candle. Even though it's not lit, it still seems to emit some warmth.";
            GreenCandle item = ItemBuilder.EasyInit<GreenCandle>("items/greencandle", "sprites/green_candle_idle_002", name, shortdesc, longdesc, ItemQuality.A, SpecialStuffModule.globalPrefix, 190, null);
            item.ChanceToBurnEnemy = 1f;
            item.GreenFireEffect = CodeShortcuts.GetItemById<Gun>(722).DefaultModule.projectiles[0].fireEffect;
            item.GreenFireGoop = CodeShortcuts.GetItemById<Gun>(698).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
            item.FlamesRequired = 5;
            item.GoopRadius = 10f;
            item.GoopRadiusSynergy = 15f;
            item.GoopDuration = 0.5f;
            item.OnUseColor = Color.green;
            item.EnemyTintColor = Color.green.WithAlpha(0.25f);
            item.BossDamageMultiplier = 1.5f;
            item.EnemyDamageMultiplier = 2f;
            item.BossDamageMultiplierSynergy = 2f;
            item.EnemyDamageMultiplierSynergy = 4f;
            item.DamageUpPerFlame = 0.05f;
            item.NormalSpriteId = item.sprite.spriteId;
            item.LitSpriteId = ItemBuilder.AddSpriteToCollection(AssetBundleManager.Load<Texture2D>("sprites/green_candle_idle_001"), SpriteBuilder.itemCollection);
            item.SetCooldownType(ItemBuilder.CooldownType.Timed, 0.5f);
            item.AddToCursulaShop();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnEnteredCombat += MaybeBurnEnemy;
            if (damageMod == null)
            {
                damageMod = StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, m_flames * DamageUpPerFlame);
                LastOwner.ownerlessStatModifiers.Add(damageMod);
            }
            else
            {
                damageMod.amount = m_flames * DamageUpPerFlame;
            }
            LastOwner.stats.RecalculateStats(LastOwner, false, false);
        }

        public void MaybeBurnEnemy()
        {
            if(PickedUp && LastOwner != null && LastOwner.CurrentRoom != null && LastOwner.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > 0 && UnityEngine.Random.value < ChanceToBurnEnemy && m_flames < FlamesRequired)
            {
                AIActor randomEnemy = LastOwner.CurrentRoom.GetRandomActiveEnemy(false);
                if(randomEnemy != null && randomEnemy.healthHaver != null)
                {
                    randomEnemy.ApplyEffect(GreenFireEffect);
                    randomEnemy.healthHaver.OnDeath += AddFlame;
                    RenderSettings.ambientLight = OnUseColor;
                }
                if(LastOwner.PlayerHasActiveSynergy("BURN! BUUURRRNNNN!!!"))
                {
                    AIActor randomEnemy2 = LastOwner.CurrentRoom.GetRandomActiveEnemy(false);
                    if (randomEnemy2 != null && randomEnemy2.healthHaver != null)
                    {
                        randomEnemy2.ApplyEffect(GreenFireEffect);
                        randomEnemy2.healthHaver.OnDeath += AddFlame;
                    }
                }
            }
        }

        public void IncreaseDealtDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            if (source.IsBoss)
            {
                args.ModifiedDamage *= LastOwner.PlayerHasActiveSynergy("BURN! BUUURRRNNNN!!!") ? BossDamageMultiplierSynergy : BossDamageMultiplier;
            }
            else
            {
                args.ModifiedDamage *= LastOwner.PlayerHasActiveSynergy("BURN! BUUURRRNNNN!!!") ? EnemyDamageMultiplierSynergy : EnemyDamageMultiplier;
            }
        }

        private void AddFlame(Vector2 v)
        {
            m_flames++;
            if(m_flames > FlamesRequired)
            {
                m_flames = FlamesRequired;
            }
            else if(LastOwner != null)
            {
                if (damageMod == null)
                {
                    damageMod = StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, m_flames * DamageUpPerFlame);
                    LastOwner.ownerlessStatModifiers.Add(damageMod);
                }
                else
                {
                    damageMod.amount = m_flames * DamageUpPerFlame;
                }
                LastOwner.stats.RecalculateStats(LastOwner, false, false);
            }
        }

        public override void Update()
        {
            base.Update();
            if(m_flames >= FlamesRequired)
            {
                sprite.SetSprite(LitSpriteId);
            }
            else
            {
                sprite.SetSprite(NormalSpriteId);
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            user.OnEnteredCombat -= MaybeBurnEnemy;
            if(damageMod != null)
            {
                user.ownerlessStatModifiers.Remove(damageMod);
                damageMod = null;
            }
            base.OnPreDrop(user);
        }

        protected override void OnDestroy()
        {
            if(LastOwner != null)
            {
                LastOwner.OnEnteredCombat -= MaybeBurnEnemy;
                if (damageMod != null)
                {
                    LastOwner.ownerlessStatModifiers.Remove(damageMod);
                    damageMod = null;
                    LastOwner.stats.RecalculateStats(LastOwner, false, false);
                }
            }
            base.OnDestroy();
        }

        protected override void DoEffect(PlayerController user)
        {
            m_flames = 0;
            RenderSettings.ambientLight = OnUseColor;
            List<AIActor> enemies = user.CurrentRoom.GetActiveEnemiesUnreferenced(RoomHandler.ActiveEnemyType.All);
            foreach(AIActor aiactor in enemies)
            {
                if(aiactor != null)
                {
                    aiactor.RegisterOverrideColor(EnemyTintColor, "green candle");
                    aiactor.ApplyEffect(GreenFireEffect);
                    aiactor.healthHaver.ModifyDamage += IncreaseDealtDamage;
                }
            }
            if (damageMod == null)
            {
                damageMod = StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 0f);
                LastOwner.ownerlessStatModifiers.Add(damageMod);
            }
            else
            {
                damageMod.amount = 0f;
            }
            LastOwner.stats.RecalculateStats(LastOwner, false, false);
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GreenFireGoop).TimedAddGoopCircle(user.sprite.WorldBottomCenter, LastOwner.PlayerHasActiveSynergy("BURN! BUUURRRNNNN!!!") ? GoopRadiusSynergy : GoopRadius, GoopDuration, false);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && m_flames >= FlamesRequired && user != null && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > 0;
        }

        public int Flames
        {
            get
            {
                return m_flames;
            }
        }

        public float ChanceToBurnEnemy;
        public GameActorFireEffect GreenFireEffect;
        public GoopDefinition GreenFireGoop;
        public int FlamesRequired;
        public float GoopRadius;
        public float GoopRadiusSynergy;
        public float GoopDuration;
        public float BossDamageMultiplier;
        public float EnemyDamageMultiplier;
        public float BossDamageMultiplierSynergy;
        public float EnemyDamageMultiplierSynergy;
        public Color OnUseColor;
        public Color EnemyTintColor;
        public float DamageUpPerFlame;
        public int NormalSpriteId;
        public int LitSpriteId;
        [SerializeField]
        private int m_flames;
        private StatModifier damageMod;
    }
}
