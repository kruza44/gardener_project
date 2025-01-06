using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /*
        현재는 스태미나 소비만 줄어들도록 구현
        가드 스턴 시간까지 줄여야 할지는 나중에 결정
    */
    public class PerfectGuardTiming : FsmStateAction
    {
        [RequiredField] public FsmFloat perfectTiming;
        [RequiredField] public FsmBool isPerfectGuard;
        [RequiredField] public FsmFloat guardStunTime;
        [RequiredField] public Animator _animator;
        private float timer;

        public override void OnEnter()
        {
            timer = 0f;
            isPerfectGuard.Value = true;
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if(timer > perfectTiming.Value)
            {
                isPerfectGuard.Value = false;
                _animator.SetBool("IsPerfectGuard", false);
            }
            else
            {
                isPerfectGuard.Value = true;
                _animator.SetBool("IsPerfectGuard", true);
            }
        }
    }
}
