using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class RandomSuccess : Action
    {
        public SharedInt successRate;    // 1 ~ 100

        public override TaskStatus OnUpdate()
        {
            int randomNum = Random.Range(1, 100);   // 1 ~ 99

            if(successRate.Value > randomNum)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}