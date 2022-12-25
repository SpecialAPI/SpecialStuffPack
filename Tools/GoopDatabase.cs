using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialStuffPack
{
    public class GoopDatabase
    {
        public static void Init()
        {
            DefaultWaterGoop = LoadHelper.LoadAssetFromAnywhere("assets/data/goops/water goop.asset") as GoopDefinition;
            DefaultPoisonGoop = LoadHelper.LoadAssetFromAnywhere("assets/data/goops/poison goop.asset") as GoopDefinition;
            DefaultFireGoop = LoadHelper.LoadAssetFromAnywhere("assets/data/goops/napalmgoopquickignite.asset") as GoopDefinition;
            DefaultCharmGoop = FairyWingsObject.RollGoop;
            DefaultCheeseGoop = ElimentalerObject.GetProjectile().cheeseEffect.CheeseGoop;
            DefaultBlobulonGoop = BlobulonEnemy.GetComponent<GoopDoer>().goopDefinition;
            DefaultPoopulonGoop = PoopulonEnemy.GetComponent<GoopDoer>().goopDefinition;
            DefaultGreenFireGoop = MaximizeSpellSynergyObject.GetProjectile().GetComponent<GoopModifier>().goopDefinition;
            DefaultWebGoop = PhaserSpiderEnemy.GetComponent<GoopDoer>().goopDefinition;

            LambIchor = CreateGoopBase("ichor");
            LambIchor.AppliesDamageOverTime = true;
            LambIchor.goopTexture = AssetBundleManager.Load<Texture2D>("ichor");
            LambIchor.baseColor32 = Color.black;
            LambIchor.fadeColor32 = Color.black;
            LambIchor.overrideOpaqueness = 1f;
            LambIchor.usesOverrideOpaqueness = true;
            LambIchor.HealthModifierEffect = DefaultPoisonGoop.HealthModifierEffect;
        }

        public static GoopDefinition CreateGoopBase(string name)
        {
            GoopDefinition def = ScriptableObject.CreateInstance<GoopDefinition>();
            def.name = name;
            def.usesLifespan = true;
            def.fadePeriod = 2f;
            def.lifespan = 10f;
            def.damagesEnemies = false;
            def.damagesPlayers = false;
            def.damageTypes = CoreDamageTypes.None;
            def.CanBeElectrified = false;
            def.electrifiedTime = 0f;
            def.electrifiedDamagePerSecondToEnemies = 0f;
            def.electrifiedDamageToPlayer = 0f;
            def.CanBeIgnited = false;
            def.CanBeFrozen = false;
            def.fireEffect = null;
            def.CharmModifierEffect = null;
            def.CheeseModifierEffect = null;
            def.HealthModifierEffect = null;
            def.lifespanRadialReduction = 0f;
            def.goopDamageTypeInteractions = new List<GoopDefinition.GoopDamageTypeInteraction>();
            def.SpeedModifierEffect = null;
            return def;
        }

        public static GoopDefinition DefaultWaterGoop;
        public static GoopDefinition DefaultFireGoop;
        public static GoopDefinition DefaultPoisonGoop;
        public static GoopDefinition DefaultCharmGoop;
        public static GoopDefinition DefaultBlobulonGoop;
        public static GoopDefinition DefaultPoopulonGoop;
        public static GoopDefinition DefaultCheeseGoop;
        public static GoopDefinition DefaultGreenFireGoop;
        public static GoopDefinition DefaultWebGoop;
        public static GoopDefinition LambIchor;
    }
}
