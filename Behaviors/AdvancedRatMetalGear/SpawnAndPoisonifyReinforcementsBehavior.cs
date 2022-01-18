using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Behaviors.AdvancedRatMetalGear
{
    public class SpawnAndPoisonifyReinforcementsBehavior : SpawnAndModifyReinforcementsBehavior
    {
        public override void ModifyEnemy(AIActor enemy)
        {
            GoopDoer gooper = enemy.AddComponent<GoopDoer>();
            gooper.updateTiming = GoopDoer.UpdateTiming.Always;
            gooper.goopDefinition = GoopDatabase.DefaultPoisonGoop;
            gooper.positionSource = GoopDoer.PositionSource.GroundCenter;
            gooper.defaultGoopRadius = 1f;
            gooper.isTimed = false;
            gooper.suppressSplashes = true;
            GoopDoer deathGooper = enemy.AddComponent<GoopDoer>();
            deathGooper.updateTiming = GoopDoer.UpdateTiming.TriggerOnly;
            deathGooper.updateOnDeath = true;
            deathGooper.goopDefinition = GoopDatabase.DefaultPoisonGoop;
            deathGooper.isTimed = true;
            deathGooper.suppressSplashes = false;
            deathGooper.defaultGoopRadius = 3.5f;
            enemy.SetResistance(EffectResistanceType.Poison, 1f);
        }
    }
}
