using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    public class ForceSetSpriteDirection : Action
    {
        [RequiredField] public Animator _animator;
        [RequiredField] public SharedSpriteDirection spriteDir;
        public SpriteDirection setDirection;

        public override void OnStart()
        {
            spriteDir.Value = setDirection;
            SetAnimatorLayer();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        void SetAnimatorLayer()
		{
            switch(spriteDir.Value)
            {
                case SpriteDirection.BackLeft :
                    _animator.SetFloat("Horizontal", -0.7f);
                    _animator.SetFloat("Vertical", 0.7f);
                    break;
                case SpriteDirection.BackRight :
                    _animator.SetFloat("Horizontal", 0.7f);
                    _animator.SetFloat("Vertical", 0.7f);
                    break;
                case SpriteDirection.Left :
                    _animator.SetFloat("Horizontal", -1f);
                    _animator.SetFloat("Vertical", 0);
                    break;
                case SpriteDirection.Right :  
                    _animator.SetFloat("Horizontal", 1f);
                    _animator.SetFloat("Vertical", 0);
                    break;
                case SpriteDirection.FrontLeft :
                    _animator.SetFloat("Horizontal", -0.7f);
                    _animator.SetFloat("Vertical", -0.7f);
                    break;
                case SpriteDirection.FrontRight :  
                    _animator.SetFloat("Horizontal", 0.7f);
                    _animator.SetFloat("Vertical", -0.7f);
                    break;
                default :
                    Debug.LogWarning("SpriteDirection error");
                    break;
		    }
	    }
    }
}
