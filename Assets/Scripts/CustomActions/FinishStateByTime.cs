using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class FinishStateByTime : FsmStateAction
    {
        public FsmFloat endTime;   // ~초 후 현 state를 끝냄
        public GameObject animatorObject;
        private Animator _animator; // Animator을 직접 public으로 받으면 out of range 에러... 이유불명
        private bool isCoroutineRunning = false; // EndCurrentState 코루틴이 실행되는 도중에 끝났는지 확인 (예: <GuardStunned> -> <GuardStunned>)
        private Coroutine endCoroutine;

        public override void Awake()
        {
            _animator = animatorObject.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            isCoroutineRunning = false;
            endCoroutine = StartCoroutine(EndCurrentState());
        }

        public override void OnExit()
        {
            if(isCoroutineRunning)   // 도중에 끝나 다시 리셋해야 한다면
            {
                StopCoroutine(endCoroutine);
            } else  // 제대로 시간이 지나서 끝났다면
            {
                _animator.SetTrigger("Finished");
            }
        }

        IEnumerator EndCurrentState()
        {
            isCoroutineRunning = true;
            yield return new WaitForSeconds(endTime.Value);    // 오작동 시 해당 instance의 inspector에서 endTime 값 확인해볼 것
            isCoroutineRunning = false;
            Fsm.Event("FINISHED");
            yield break;
        }
    }
}
