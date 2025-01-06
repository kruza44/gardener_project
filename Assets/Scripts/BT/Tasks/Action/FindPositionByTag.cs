using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class FindPositionByTag : Action
    {
        public SharedVector2 targetDestination;
        public string tagName;

        public override void OnStart()
        {
            targetDestination.Value = GameObject.FindGameObjectWithTag(tagName).transform.position;            
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}