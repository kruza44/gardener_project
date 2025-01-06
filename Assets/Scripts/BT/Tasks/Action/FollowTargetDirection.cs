using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class FollowTargetDirection : Action
    {
        public SharedTransform target;
        public SharedTransform startTransform;  // 눈알 위치
        public SharedVector2 sightDirection;
        public SharedFloat followSpeed; // 각도(Degree) 단위
        private Vector2 targetDirection;

        public override void OnStart()
        {
            sightDirection.Value = Vector2.down;
        }

        public override TaskStatus OnUpdate()
        {
            targetDirection = target.Value.position - this.transform.position;
            float angle = Vector2.SignedAngle(sightDirection.Value, targetDirection);
            float speed = followSpeed.Value * Time.deltaTime;

            Debug.Log("angle: "+angle);
            
            if(angle > speed)
            {
                sightDirection.Value = RotateAngle(sightDirection.Value, speed);
            } 
            else if(angle > 0 && angle <= speed)
            {
                sightDirection.Value = RotateAngle(sightDirection.Value, angle);
            }
            else if (angle < 0 && -angle <= speed)
            {
                sightDirection.Value = RotateAngle(sightDirection.Value, angle);
            }
            else if (angle < 0 && -angle > speed)
            {
                sightDirection.Value = RotateAngle(sightDirection.Value, -speed);
            }

            Debug.DrawLine(transform.position, transform.position + (Vector3)sightDirection.Value * 5f, Color.red);
            Debug.DrawLine(transform.position, transform.position + (Vector3)targetDirection * 5f, Color.blue);

            return TaskStatus.Running;
        }

        Vector2 RotateAngle(Vector2 vec, float angle)
        {
            return new Vector2
            (
                vec.x * Mathf.Cos(angle * Mathf.Deg2Rad) - vec.y * Mathf.Sin(angle * Mathf.Deg2Rad),
                vec.x * Mathf.Sin(angle * Mathf.Deg2Rad) + vec.y * Mathf.Cos(angle * Mathf.Deg2Rad)
            );
        }
    }
}