using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class RenderLine : Action
    {
        public LineRenderer _lineRenderer;
        public Transform startTransform;    // 선이 시작하는 위치
        public SharedVector2 lineDirection;
        public LayerMask mask;  // 선이 막히게 될 마스크
        public float duration;  // 선을 그릴 시간
        public bool updatePosition; // True 시 duration에 상관없이 계속 실행됨
        public SharedVector2 targetDestination; // 레이저가 끝나는 위치 저장 용
        private Vector2 endPosition;    // 선이 끝나는 위치
        private float timer;

        public override void OnStart()
        {
            timer = 0f;
            _lineRenderer.gameObject.SetActive(true);
            SetLinePositions();
        }

        public override TaskStatus OnUpdate()
        {
            if(updatePosition)
            {
                SetLinePositions();
                return TaskStatus.Running;
            }

            timer += Time.deltaTime;

            if(timer >= duration)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            _lineRenderer.gameObject.SetActive(false);
        }

        void SetLinePositions()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lineDirection.Value, 100f, mask);

            if(hit.collider != null)
            {
                endPosition = hit.point;
            }
            else
            {
                endPosition = (Vector2)transform.position + lineDirection.Value * 100f;
            }

            targetDestination.Value = endPosition;
            _lineRenderer.SetPosition(0, startTransform.position);
            _lineRenderer.SetPosition(1, endPosition);
        }
    }
}