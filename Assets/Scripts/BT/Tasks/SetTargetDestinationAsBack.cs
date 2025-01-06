using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    // 타겟으로부터 멀리 떨어지도록 
    public class SetTargetDestinationAsBack : Action
    {
        [RequiredField] public SharedTransform target;
        [RequiredField] public SharedVector3 targetDestination;
        public float distance;

        public override void OnStart()
        {
            targetDestination.Value = this.transform.position + (this.transform.position - target.Value.position).normalized * distance;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}