using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    public class DoAttackTask : Action
    {
        [RequiredField] public Attack _attack;
        [RequiredField] public SharedVector2 targetDirection;
        [RequiredField] public SharedVector2 targetPosition;
        private float timer;

        public override void OnStart()
        {
            _attack.DoAttack(targetDirection.Value, targetPosition.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
