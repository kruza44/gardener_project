using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Action")]
    public class AimAssist : FsmStateAction
    {
        public Transform centerTransform;   // 중심점
        [UIHint(UIHint.Variable)]
        public FsmVector2 axisDirection;    // 표시되는 방향
        [UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;
        public FsmFloat assistRadius = 0.5f;   // 어시스트 대상을 잡아주는 범위
        public FsmFloat assistDistance;
        public FsmFloat unCaptureAngle = 15f;  //  얼만큼의 각도를 줘야 벗어날 수 있는지
        private Vector2 actualInputAxis;    // 실제 인풋 방향
        private bool isAssistCaptured;
        private float deadzone = 0.4f;
        private Collider2D assistTarget; // 따라다닐 타겟 위치 (콜라이더 중심점이 필요하기 때문에 Transform 대신)

        public override void OnEnter()
        {
            isAssistCaptured = false;
        }

        public override void OnUpdate()
        {
            GetRStickAxis();
        }

        void GetRStickAxis()
        {
            // 어시스트 캡쳐한 타겟이 사라졌거나 콜라이더가 비활성화 된 경우 비우기
            if(assistTarget == null || assistTarget.enabled)
            {
                assistTarget = null;
                isAssistCaptured = false;
            }

            // Input 컨트롤러 R_스틱 받아오기
            actualInputAxis.x = Input.GetAxisRaw("R_Horizontal");
            actualInputAxis.y = Input.GetAxisRaw("R_Vertical");

            if(actualInputAxis.magnitude < deadzone) { actualInputAxis = Vector2.zero; }    // 데드존
            actualInputAxis.Normalize();

            // 별다른 인풋이 없으면 assist 따라가도록
            if(actualInputAxis == Vector2.zero)
            {
                if(isAssistCaptured)
                    axisDirection.Value = (assistTarget.bounds.center - centerTransform.position).normalized;
                return;
            }

            // 인풋이 있다면
            Vector2 raycastStartPosition = (Vector2)centerTransform.position + actualInputAxis * assistRadius.Value;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(raycastStartPosition, assistRadius.Value, actualInputAxis, assistDistance.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, false));
            if(hits.Length >= 2)
            {
                // 가장 가까운 각도로 고정시켜야 함
                float[] angle = new float[hits.Length];
                int minIndex = 0;

                for(int i=0; i < hits.Length; i++)
                {
                    if(hits[i].transform.root.CompareTag("Gardener"))   // 자기자신은 무시
                    {
                        angle[i] = 999f;    // 무시하도록 큰 수 할당
                        continue;
                    }
                    
                    Vector2 targetDirection = (hits[i].centroid - (Vector2)centerTransform.position).normalized;
                    angle[i] = Vector2.Angle(actualInputAxis, targetDirection);

                    if(i != 0)
                    {
                        if(angle[i] < angle[i-1])
                            minIndex = i;   // 가장 각도가 작은 index
                    }
                }

                isAssistCaptured = true;
                assistTarget = hits[minIndex].collider;
                axisDirection.Value = (assistTarget.bounds.center - centerTransform.position).normalized;
                
                Vector2 assistTargetDirection = (assistTarget.bounds.center - centerTransform.position).normalized;
                float inputAngle = Vector2.Angle(actualInputAxis, assistTargetDirection);
                if(inputAngle > unCaptureAngle.Value)    // 인풋 각도가 한계를 넘어서면 캡쳐를 벗어남
                {
                    isAssistCaptured = false;
                    assistTarget = null;
                    axisDirection.Value = actualInputAxis;
                }
            }
            else if(isAssistCaptured)   // 아무것도 잡히지 않았는데 이미 무언가 잡혀있는 상태라면
            {
                Vector2 assistTargetDirection = (assistTarget.bounds.center - centerTransform.position).normalized;
                float angle = Vector2.Angle(actualInputAxis, assistTargetDirection);
                if(angle > unCaptureAngle.Value)    // 인풋 각도가 한계를 넘어서면 캡쳐를 벗어남
                {
                    isAssistCaptured = false;
                    assistTarget = null;
                    axisDirection.Value = actualInputAxis;
                }
                else    // 벗어나지 못했으면 유지
                {
                    axisDirection.Value = assistTargetDirection;
                }
            }
            else    // 아무것도 잡히지 않았고 무언가 잡혀있지도 않은 상태라면
            {
                axisDirection.Value = actualInputAxis;
            }
        }
    }
}
