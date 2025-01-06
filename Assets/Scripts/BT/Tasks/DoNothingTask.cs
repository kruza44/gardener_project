using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class DoNothingTask : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}