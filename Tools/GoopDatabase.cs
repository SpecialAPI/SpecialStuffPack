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
            GoopDefinition goopDefinition;
            string text = "assets/data/goops/water goop.asset";
            try
            {
                GameObject gameObject2 = LoadHelper.LoadAssetFromAnywhere(text) as GameObject;
                goopDefinition = gameObject2.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition = (LoadHelper.LoadAssetFromAnywhere(text) as GoopDefinition);
            }
            DefaultWaterGoop = goopDefinition;
            GoopDefinition goopDefinition2;
            text = "assets/data/goops/poison goop.asset";
            try
            {
                GameObject gameObject2 = LoadHelper.LoadAssetFromAnywhere(text) as GameObject;
                goopDefinition2 = gameObject2.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition2 = (LoadHelper.LoadAssetFromAnywhere(text) as GoopDefinition);
            }
            goopDefinition2.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            DefaultPoisonGoop = goopDefinition2;
            GoopDefinition goopDefinition3;
            text = "assets/data/goops/napalmgoopquickignite.asset";
            try
            {
                GameObject gameObject2 = LoadHelper.LoadAssetFromAnywhere(text) as GameObject;
                goopDefinition3 = gameObject2.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition3 = (LoadHelper.LoadAssetFromAnywhere(text) as GoopDefinition);
            }
            goopDefinition3.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            DefaultFireGoop = goopDefinition3;
            PickupObject byId = PickupObjectDatabase.GetById(310);
            GoopDefinition item;
            if (byId == null)
            {
                item = null;
            }
            else
            {
                WingsItem component = byId.GetComponent<WingsItem>();
                item = ((component != null) ? component.RollGoop : null);
            }
            DefaultCharmGoop = item;
            DefaultCheeseGoop = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect.CheeseGoop;
            DefaultBlobulonGoop = EnemyDatabase.GetOrLoadByGuid("0239c0680f9f467dbe5c4aab7dd1eca6").GetComponent<GoopDoer>().goopDefinition;
            DefaultPoopulonGoop = EnemyDatabase.GetOrLoadByGuid("116d09c26e624bca8cca09fc69c714b3").GetComponent<GoopDoer>().goopDefinition;
            DefaultGreenFireGoop = GetItemById<Gun>(698).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
            DefaultWebGoop = EnemyDatabase.GetOrLoadByGuid("98ca70157c364750a60f5e0084f9d3e2").GetComponent<GoopDoer>().goopDefinition;
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
    }
}
