using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class CheckAIState : Conditional
    {
        public SharedAIState currentState;
        public SharedAIState checkState;

        public override TaskStatus OnUpdate()
        {
            if(currentState.Value == checkState.Value) { return TaskStatus.Success; }
            else { return TaskStatus.Failure; }
        }
    }
}
