using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class WaitAnimStart : Action
    {
        [Tooltip("Animator State to check if it's started.")]
		public string animState;

		public Animator _animator;
        public override TaskStatus OnUpdate()
		{
            if(_animator == null)
            {
                Debug.LogWarning("Animator null error");
                return TaskStatus.Failure;
            }
            
			AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo (0);

			if (!animInfo.IsName (animState))
			{
				return TaskStatus.Running;
			} else
            {
                return TaskStatus.Success;
            }
		}
    }
}
