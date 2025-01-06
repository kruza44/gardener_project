using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class SetDestinationBasedOnTarget : Action
    {
        public enum DestinationType
        {
            FromHereAway, FromTargetAway, TargetLeft, TargetRight, TargetBack
        }
        
        [RequiredField] public SharedTransform target;
        [RequiredField] public SharedVector2 targetDestination; // 결과를 저장할 변수
        public float distance;
        public DestinationType destinationType;

        public override void OnStart()
        {
            Vector2 direction = Vector3.zero;

            switch(destinationType)
            {
                case DestinationType.FromHereAway :
                    direction = (this.transform.position - target.Value.position).normalized;
                    targetDestination.Value = this.transform.position + (Vector3)direction * distance;
                    break;
                case DestinationType.FromTargetAway :
                    direction = (this.transform.position - target.Value.position).normalized;
                    targetDestination.Value = target.Value.transform.position + (Vector3)direction * distance;
                    break;
                case DestinationType.TargetLeft :
                    direction = Vector2.Perpendicular((this.transform.position - target.Value.position).normalized);
                    targetDestination.Value = target.Value.transform.position + (Vector3)direction * distance;
                    break;
                case DestinationType.TargetRight :
                    direction = -Vector2.Perpendicular((this.transform.position - target.Value.position).normalized);
                    targetDestination.Value = target.Value.transform.position + (Vector3)direction * distance;
                    break;
                case DestinationType.TargetBack :
                    direction = (target.Value.position - this.transform.position).normalized;
                    targetDestination.Value = target.Value.transform.position + (Vector3)direction * distance;
                    break;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
