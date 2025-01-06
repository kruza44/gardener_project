using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SetCoolDownTime : Action
    {
        // 처음 실행된 경우 CoolDownConditional이 -1임을 확인하고 success를 반환함
        public float coolDownStartTime = -1f;

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            coolDownStartTime= Time.time;
        }
    }
}