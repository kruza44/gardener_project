using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class CoolDownConditional : Conditional
    {
        public SharedFloat duration;
        // 대상 "SetCoolDownTime" task에 기록된 시간을 읽어온다
        [RequiredField] public SetCoolDownTime coolDownTime;


        public override TaskStatus OnUpdate()
        {
            // 첫 실행 시 쿨타임 x
            if(coolDownTime.coolDownStartTime == -1)
            {
                return TaskStatus.Success;
            }

            if(coolDownTime.coolDownStartTime + duration.Value < Time.time)
            {
                return TaskStatus.Success;
            } else { return TaskStatus.Failure; }
        }
    }
}