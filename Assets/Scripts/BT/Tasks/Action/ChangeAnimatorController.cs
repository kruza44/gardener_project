using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class ChangeAnimatorController : Action
    {
        public SharedObject animatorController;
        public Animator _animator;

        public override void OnStart()
        {
            _animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorController.Value;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
