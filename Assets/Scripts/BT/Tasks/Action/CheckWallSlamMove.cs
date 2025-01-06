using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    public class CheckWallSlamMove : Action
    {
        public SharedBool checkWallSlam;
        public SharedVector2 slideDirection;
        public Ease easeType;   // 트윈 타입
        public float easeDuration;  // 속도가 최소 속도까지 도달하는 시간
        public SharedFloat slideSpeed;    // 최대 속도
        public float minSpeed;      // 최소 속도
        private string tweenID;     // 도중에 트윈을 중지시킬 수 있도록 ID를 저장
        private MoveManager _move;
        private float tempSpeed;
        private float wallCheckTimer;
        private bool wallDetected;
        private float stopTimer;    // 스턴이 오래 지속되면 속도가 0이 됨
    

        public override void OnAwake()
        {
            _move = GetComponent<MoveManager>();
            tweenID = this.gameObject.GetInstanceID().ToString();   // 트윈 ID 할당
        }

        public override void OnStart()
        {
            wallCheckTimer = 0;
            tempSpeed = slideSpeed.Value;
            wallDetected = false;
            stopTimer = 0;

            // 속도가 트윈 easeType 그래프를 따르며 minSpeed까지 감소
            DOTween.To(()=> tempSpeed, x=> tempSpeed = x, minSpeed, easeDuration).SetEase(easeType).SetId(tweenID);
        }

        public override TaskStatus OnUpdate()
        {
            stopTimer += Time.deltaTime;
            if(stopTimer >= 0.5f)
            {
                tempSpeed = 0;
            }

            return TaskStatus.Running;
        }

        public override void OnFixedUpdate()
        {
            if(checkWallSlam.Value) // 벽쿵 체크
            {
                wallCheckTimer += Time.fixedDeltaTime;

                // 벽에 밀착한 상태에서 벽쿵 시 여러번 발생하는 문제 해결을 위해 wallDetected 조건 추가
                // 속도가 많이 붙어있을 시간 0.2f 동안만 벽쿵 체크
                if((wallCheckTimer <= 0.2f) && !wallDetected)
                {
                    // 벽에 밀착한 상태에서 벽쿵 시 BT Stunned -> Stunned state가 제대로 작동하지 않는 문제
                    // 해결을 위해 0.1초간은 벽에 밀착시키기만 하고
                    // 이후부터 벽쿵 효과가 나타나도록 함 (이 편이 효과가 더 자연스러울 것 같기도 함)
                    if(wallCheckTimer <= 0.1f)
                    {
                        _move.CheckWallMove(slideDirection.Value * tempSpeed);
                    } else
                    {
                        wallDetected = _move.CheckWallMove(slideDirection.Value * tempSpeed);
                        if(wallDetected)
                        {
                            _move.wallDetected = true;  // MoveManger에서 wallSlammed 이벤트 전달
                        }
                    }
                } else
                {
                    _move.AddMove(slideDirection.Value * tempSpeed);
                }
            }
            else    // 일반적인 SlideMove
            {
                _move.AddMove(slideDirection.Value * tempSpeed);
            }
        }

        public override void OnEnd()
        {
            DOTween.Kill(tweenID);
        }
    }
}
