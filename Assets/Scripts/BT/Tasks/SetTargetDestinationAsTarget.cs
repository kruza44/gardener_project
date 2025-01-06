using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class SetTargetDestinationAsTarget : Action
    {
        [RequiredField] public SharedTransform target;
        [RequiredField] public SharedVector2 targetDestination;
        public bool isUpdate;

        public override void OnStart()
        {
            targetDestination.Value = target.Value.position;
        }

        public override TaskStatus OnUpdate()
        {
            if(isUpdate)
            {
                targetDestination.Value = target.Value.position;
                return TaskStatus.Running;
            } 
            else { return TaskStatus.Success; }
        }
    }
}