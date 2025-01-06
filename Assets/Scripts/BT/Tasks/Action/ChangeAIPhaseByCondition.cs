using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class ChangeAIPhaseByCondition : Action
    {
        public HealthManager _health;
        public SharedFloat changeHealthPercentage;
        public SharedAIPhaseState phaseStateVariable;
        public AIPhaseState changePhase;

        public override void OnStart()
        {
            float maxHealth = _health.maxParam;
            float currentHealth = _health.currentParam;

            if(currentHealth <= maxHealth * changeHealthPercentage.Value)
            {
                phaseStateVariable.Value = changePhase;
            }            
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}
