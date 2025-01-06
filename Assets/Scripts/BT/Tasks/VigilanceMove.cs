using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    /*
        현재는 오직 방패 없는 망자만을 위한 스크립트
        방패를 든 망자나 기사 등 적마다 다른 움직임을 보여야 하기 때문
    */
    public class VigilanceMove : Action
    {
        enum MoveType { Backward, Clock, CounterClock, Forward };

        [RequiredField] public SharedTransform target;
        [RequiredField] public SharedVector2 targetDirection;
        [RequiredField] public SharedFloat targetDistance;
        [RequiredField] public SharedFloat walkSpeed;   // 유닛의 기본 걷는 속도... 여기서는 1/3 속도로 움직임
        [RequiredField] public SharedVector2 moveDirection; // 움직이는 방향을 저장하여 다른 스크립트에서 사용하도록
        [RequiredField] public SharedBool isWalking;
        public float farDistance;   // 상대가 (적당히) 멀다고 느껴지는 거리
        public float closeDistance; // 상대가 너무 가깝다고 느껴지는 거리
        [SerializeField] private float slowWalkMultiplier = (1/3f);   // 얼마나 느리게 걷도록 할 것인지
        private MoveType moveType;
        private MoveManager _move;
        private AnimatorManager _animManager;
        // private bool randomDone;    // 랜덤 움직임 선택 update에서 한번만 실행되도록

        public override void OnAwake()
        {
            _move = this.GetComponent<MoveManager>();
            _animManager = this.GetComponent<AnimatorManager>();
        }

        public override void OnStart()
        {
            // randomDone = false;
            SetMoveType();

            isWalking.Value = true;
            _animManager._animator.speed = slowWalkMultiplier;
        }

        public override TaskStatus OnUpdate()
        {
            switch(moveType)
            {
                case MoveType.Backward:
                    moveDirection.Value = -targetDirection.Value;
                    break;
                case MoveType.Clock:
                /*
                    2. 감지해서 움직임 바꾸도록 추가 예정
                */
                    moveDirection.Value = Vector2.Perpendicular(targetDirection.Value);
                    break;
                case MoveType.CounterClock:
                    moveDirection.Value = -Vector2.Perpendicular(targetDirection.Value);
                    break;
                case MoveType.Forward:
                    moveDirection.Value = targetDirection.Value;
                    if(targetDistance.Value <= closeDistance)
                    {
                        int randomNum = Random.Range(0,3);
                        moveType = (MoveType)randomNum;
                    }
                    break;
                default:
                    break;
            }
            return TaskStatus.Running;
        }

        public override void OnFixedUpdate()
        {
            _move.AddMove(moveDirection.Value * (walkSpeed.Value * slowWalkMultiplier));
        }

        public override void OnEnd()
        {
            isWalking.Value = false;
            _animManager._animator.speed = 1f;
        }

        void SetMoveType()
        {   
            int randomNum = Random.Range(1,4);
            moveType = (MoveType)randomNum; // 상대가 중간 거리에 있으면 네가지 움직임 중 랜덤으로 하나를 택한다
        }
    }
}
