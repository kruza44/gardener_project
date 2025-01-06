using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class CheckAIStunState : Conditional
    {
        public SharedAIStunState currentState;
        public SharedAIStunState checkState;

        public override TaskStatus OnUpdate()
        {
            if(currentState.Value == checkState.Value) { return TaskStatus.Success; }
            else { return TaskStatus.Failure; }
        }
    }
}