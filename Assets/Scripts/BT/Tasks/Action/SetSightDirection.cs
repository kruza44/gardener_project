using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    [TaskDescription("Set sighDirection and change animator float value accordingly")]
    public class SetSightDirection : Action
    {
        [RequiredField] public Animator _animator;
        [RequiredField] public SharedVector2 sightDirection;
        [RequiredField] public SharedVector2 targetDirection;
        [RequiredField] public SharedVector2 moveDirection;
        [RequiredField] public SharedSpriteDirection spriteDir; // 다른 스크립트에서 사용하기 위해 저장함

        public bool isUpdate;
        public bool watchTargetDir;    // check 시 sightDirection = targetDirection이 됨 (isTargetInSight = true인 경우)
        public bool watchMoveDir;  // check 시 sightDirection = moveDirection이 됨

        public override void OnStart()
        {
            SetSightDir();
        }

        public override TaskStatus OnUpdate()
        {
            if(isUpdate)
            {
                SetSightDir();
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }

        void SetSightDir()
        {
            if(watchTargetDir)
            {
                sightDirection.Value = targetDirection.Value;
            } else if(watchMoveDir) { sightDirection.Value = moveDirection.Value; }

            // 아무것도 체크되어있지 않은 경우 현재 sightDirection 값 유지
            
            if(sightDirection.Value != Vector2.zero)
            {
                // sightDirection이 (0, 0)인 경우, 이전의 animation layer 유지
                SetAnimatorLayer(); 
            }
        }

        void SetAnimatorLayer()
		{
            // 각도에 따라 animator의 float 값을 바꿔 blend tree가 sprite의 방향을 바꾸도록 함
			float angle = Mathf.Atan2(sightDirection.Value.x, sightDirection.Value.y) * Mathf.Rad2Deg;

			if(angle <= 0f && angle >= -45f) spriteDir.Value = SpriteDirection.BackLeft;
			else if(angle >= 0f && angle <= 45f) spriteDir.Value = SpriteDirection.BackRight;
			else if(angle <= -45f && angle >= -135f) spriteDir.Value = SpriteDirection.Left;
			else if(angle >= 45f && angle <= 135) spriteDir.Value = SpriteDirection.Right;
            else if(angle < -135f && angle >= -180f) spriteDir.Value = SpriteDirection.FrontLeft;
			else if(angle > 135f && angle <= 180f) spriteDir.Value = SpriteDirection.FrontRight;

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
