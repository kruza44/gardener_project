using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    [TaskDescription("If the animator state is over, return Failure. Else, return Success.")]
    public class CheckAnimatorState : Conditional
    {
        [Tooltip("Animator State to check if it's finished.")]
		public string animState;

		public Animator _animator;
        private bool checkStart;    // 우선 원하는 animState가 시작되었는지부터 확인 후 check 시작

        public override void OnStart()
        {
            checkStart = false;

            AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo (0);
            if (animInfo.IsName (animState)) { checkStart = true; }
        }

        public override TaskStatus OnUpdate()
		{
            if(_animator == null)
            {
                Debug.LogWarning("BT Animator null error");
                return TaskStatus.Failure;
            }
            
			AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo (0);
            if (animInfo.IsName (animState)) { checkStart = true; }
            
            if(checkStart)
            {
                if (!animInfo.IsName (animState))
                {
                    return TaskStatus.Failure;  // 현 state abort
                } else
                {
                    return TaskStatus.Success;  // 현 state 유지
                }
            } else { return TaskStatus.Success; }
		}
    }
}
