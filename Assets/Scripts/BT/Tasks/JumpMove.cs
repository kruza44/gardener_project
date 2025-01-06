using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class JumpMove : Action
    {
        [RequiredField] public SharedBool isJumping;    // 점프 여부를 상대 공격에게 알려주기 위함
        [RequiredField] public SharedVector2 destination;  // 목적지 (Transform 옵션도 추가해야 할 수 있음..?)
        public float jumpHeight;
        public float arriveTime;    // 목표 도착에 걸리는 시간
        public float maxHeightInterval; // 최고 높이를 유지하는 시간 (적절한 값 찾는 테스트용)
        [RequiredField] public SharedTransform spriteTransform;
        public bool targetFollow;   // 타겟을 따라가는 방식의 움직임인지 여부
        public float targetFollowMaxSpeed = 6.5f;
        public Ease jumpEaseType;
        public Ease fallEaseType;
        public Ease horizontalEaseType;
        public bool shakeCamera;
        private MoveManager _move;
        private JumpColliderManager _jump;
        private float tempSpeed;
        private bool jumpFinished;
        private string tweenID;
        private Vector3 movePosition;
        private float moveSpeed;    // targetFollow가 true인 경우에만 사용

        public override void OnAwake()
        {
            _move = GetComponent<MoveManager>();
            _jump = GetComponent<JumpColliderManager>();

            tweenID = this.gameObject.GetInstanceID().ToString(); // 트윈 ID 할당
        }

        public override void OnStart()
        {
            jumpFinished = false;
            isJumping = true;
            moveSpeed = 0f;
            _jump.DisableColiders();


            // Sprite vertical 움직임           
            DG.Tweening.Sequence jumpSequence = DOTween.Sequence();

            jumpSequence.Append(spriteTransform.Value.DOLocalMoveY(jumpHeight, (arriveTime - maxHeightInterval)/2f).SetEase(jumpEaseType))
            .AppendInterval(maxHeightInterval)
            .Append(spriteTransform.Value.DOLocalMoveY(0, (arriveTime - maxHeightInterval)/2f).SetEase(fallEaseType)).SetId(tweenID)
            .OnComplete(()=>
            {
                jumpFinished = true;
                isJumping = false;
                _jump.EnableColliders();
                if(shakeCamera)
                {
                    CameraManager.Instance.ShakeCamera(0.1f);
                }
            });

            jumpSequence.Play();

            // Unit horizontal 움직임
            if(!targetFollow)   // 타겟 위치가 움직이지 않는 경우
            {
                movePosition = this.transform.position;
                DOTween.To(()=> movePosition, x=> movePosition = x, (Vector3)destination.Value, arriveTime).SetEase(horizontalEaseType).SetId(tweenID);
            }
            else  // 타겟 위치가 움직이는 경우
            {
                moveSpeed = Vector3.Distance(destination.Value, this.transform.position) / (arriveTime * (0.7f));
                moveSpeed = moveSpeed > targetFollowMaxSpeed ? targetFollowMaxSpeed : moveSpeed;    // 최대속도 넘지 않도록
            }
        }

        public override TaskStatus OnUpdate()
        {
            if(jumpFinished)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }

        public override void OnFixedUpdate()
        {
            // Unit horizontal 움직임
            if(!targetFollow)   // 타겟 위치가 움직이지 않는 경우
            {
                _move.AddMove(((Vector2)movePosition - (Vector2)this.transform.position) * (1f/Time.fixedDeltaTime));    // ownerMoveDuration 안에 이동하도록
            }
            else    // 타겟 위치가 움직이는 경우
            {
                // MoveTowards 주의 (Time.deltaTime)
                tempSpeed = moveSpeed * Time.fixedDeltaTime;
                _move.AddMove(((Vector2)((Vector3.MoveTowards(this.transform.position, destination.Value, tempSpeed) - this.transform.position)) * (1f/Time.fixedDeltaTime)));
            }
        }

        public override void OnEnd()
        {
            // DOTween.Kill(tweenID);
            jumpFinished = false;
            isJumping = false;
            _jump.EnableColliders();
        }
    }
}
