using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class SlideMoveTask : Action
    {
        public SharedVector2 slideDirection;
        public Ease easeType;   // 트윈 타입
        public float easeDuration;  // 속도가 최소 속도까지 도달하는 시간
        public SharedFloat slideSpeed;    // 최대 속도
        public float minSpeed;      // 최소 속도
        private string tweenID;     // 도중에 트윈을 중지시킬 수 있도록 ID를 저장
        private MoveManager _move;
        private float tempSpeed;
        private float timer;    // 스턴이 오래 지속되면 속도가 0이 됨

        public override void OnAwake()
        {
            _move = GetComponent<MoveManager>();
            tweenID = this.gameObject.GetInstanceID().ToString(); // 트윈 ID 할당
        }

        public override void OnStart()
        {
            tempSpeed = slideSpeed.Value;
            timer = 0f;

            // 속도가 트윈 easeType 그래프를 따르며 minSpeed까지 감소
            DOTween.To(()=> tempSpeed, x=> tempSpeed = x, minSpeed, easeDuration).SetEase(easeType).SetId(tweenID);
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if(timer >= 0.5f)
            {
                tempSpeed = 0;
            }
            return TaskStatus.Running;
        }

        public override void OnFixedUpdate()
        {
            _move.AddMove(slideDirection.Value * tempSpeed);
        }

        public override void OnEnd()
        {
            DOTween.Kill(tweenID);
        }
    }
}
