using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    public class WaitAttackTiming : Action
    {
        [RequiredField] public SharedBool attackTiming;
        [RequiredField] public Attack _attack;
        [RequiredField] public SharedVector2 attackDirection;
        [RequiredField] public SharedTransform target;

        public override TaskStatus OnUpdate()
        {
            if(!attackTiming.Value)
            {
                return TaskStatus.Running;  // attackTiming이 false일 시 대기
            } else
            {
                attackTiming.Value = false;
                _attack.DoAttack(attackDirection.Value, target.Value.position);
                return TaskStatus.Success;  // attackTiming 초기화 후 다음 task로 넘어감
            }
        }

        public override void OnEnd()
        {
            attackTiming.Value = false;
        }
    }
}
