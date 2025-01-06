using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    [TaskDescription("Check time to finish current state")]
    public class CheckTimer : Conditional
    {
        public float endTime;
        public Animator _animator;
        // private bool isfinished;
        private float timer;

        public override void OnStart()
        {
            Debug.Log("Start");
            timer = 0;
            // isfinished = false;
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if(timer >= endTime)
            {
                Debug.Log("TimerFini");
                _animator.SetTrigger("Finished");
                return TaskStatus.Failure;
            } else { return TaskStatus.Success; }
        }

        // public override void OnEnd()
        // {
        //     if(isCoroutineRunning)   // 도중에 끝나 다시 리셋해야 한다면
        //     {
        //         Debug.Log("Aborted");
        //         StopCoroutine(endCoroutine);
        //     } else  // 제대로 시간이 지나서 끝났다면
        //     {
        //         Debug.Log("Finished");
        //         _animator.SetTrigger("Finished");
        //     }
        // }

        // // endTime 후 현재 state를 끝냄
        // IEnumerator EndCurrentState()
        // {
        //     isCoroutineRunning = true;
        //     yield return new WaitForSeconds(endTime);
        //     Debug.Log("TimerFini");   
        //     isCoroutineRunning = false;
        //     isfinished = true;
        //     _animator.SetTrigger("Finished");
        //     yield break;
        // }
    }
}
