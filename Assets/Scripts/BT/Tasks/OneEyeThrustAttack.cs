using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class OneEyeThrustAttack : Action
    {
        [RequiredField] public EnemyCircleCastAttack _attack;
        [RequiredField] public SharedTransform target;
        [RequiredField] public Animator _animator;
        [RequiredField] public SharedSpriteDirection spriteDirection;   // attack 스크립트에 알리기 위함 (히트박스 위치)
        public float attackTiming = 0.5f;
        public float arriveTime = 0.4f;
        public float mediumDistance = 3f;
        public Ease readyEaseType;
        public Ease thrustEaseType;
        private float tempSpeed;
        private string tweenID;
        private Vector2 readyPosition;  // 이동 후 돌진을 시작할 위치
        private ThrustMode thrustMode;
        private bool isFinished;
        private Vector2 tempPosition;
        private MoveManager _move;

        private enum ThrustMode
        {
            LeftUp, Left, LeftDown,
            RightUp, Right, RightDown
        }

        public override void OnAwake()
        {
            _move = GetComponent<MoveManager>();

            tweenID = this.gameObject.GetInstanceID().ToString();
        }

        public override void OnStart()
        {
            isFinished = false;

            Vector3 reverseTargetDirection = (this.transform.position - target.Value.position).normalized;

            float angle = Mathf.Atan2(reverseTargetDirection.x, reverseTargetDirection.y) * Mathf.Rad2Deg;
            
            if(angle <= 0f && angle >= -65.614f) thrustMode = ThrustMode.RightDown;
			else if(angle > -114.386f && angle < -65.614f) thrustMode = ThrustMode.Right;
			else if(angle >= -180 && angle <= -114.386f) thrustMode = ThrustMode.RightUp;
			else if(angle > 0f && angle <= 65.614f) thrustMode = ThrustMode.LeftDown;
            else if(angle > 65.614f && angle < 114.386f) thrustMode = ThrustMode.Left;
			else if(angle >= 114.386f && angle < 180f) thrustMode = ThrustMode.LeftUp;

            SetAnimatorLayer();
            _animator.SetTrigger("DoThrustAttack1");

            Vector2 thrustDirection = Vector2.left;

            switch(thrustMode)
            {
                case ThrustMode.LeftUp :
                    thrustDirection = new Vector2(-2.25f, 1.02f).normalized;
                    break;
                case ThrustMode.Left :
                    thrustDirection = Vector2.left;
                    break;
                case ThrustMode.LeftDown :
                    thrustDirection = new Vector2(-2.25f, -1.02f).normalized;
                    break;
                case ThrustMode.RightUp :
                    thrustDirection = new Vector2(2.25f, 1.02f).normalized;
                    break;
                case ThrustMode.Right :
                    thrustDirection = Vector2.right;
                    break;
                case ThrustMode.RightDown :
                    thrustDirection = new Vector2(2.25f, -1.02f).normalized;
                    break;
            }

            float targetDistance = Vector3.Distance(target.Value.position, this.transform.position);

            // if(targetDistance > mediumDistance)
            // {
            //     readyPosition = (Vector2)target.Value.position + (-thrustDirection * (targetDistance));
            // }
            // else
            // {
                readyPosition = (Vector2)target.Value.position + (-thrustDirection * (targetDistance + 1f));
            // }

            tempPosition = this.transform.position;
            Vector2 arrivePosition = (Vector2)target.Value.position - thrustDirection;
            DG.Tweening.Sequence thrustAttackSequence = DOTween.Sequence();

            thrustAttackSequence.Append(DOTween.To(()=> tempPosition, x=> tempPosition = x, readyPosition, attackTiming).SetEase(readyEaseType))
            .AppendCallback(()=>_attack.DoAttack(thrustDirection, Vector2.zero))
            .Append(DOTween.To(()=> tempPosition, x=> tempPosition = x, arrivePosition, arriveTime).SetEase(thrustEaseType))
            .OnComplete(()=> isFinished = true);
        }

        public override TaskStatus OnUpdate()
        {
            if(isFinished)
            {
                _animator.SetTrigger("Finished");
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }

        public override void OnFixedUpdate()
        {
            _move.AddMove((tempPosition - (Vector2)this.transform.position) * (1f/Time.fixedDeltaTime));
        }

        public override void OnEnd()
        {
            DOTween.Kill(tweenID);
        }

        void SetAnimatorLayer()
		{
            switch(thrustMode)
            {
                case ThrustMode.LeftUp :
                    spriteDirection.Value = SpriteDirection.BackLeft;
                    _animator.SetFloat("Horizontal", -0.7f);
                    _animator.SetFloat("Vertical", 0.7f);
                    break;
                case ThrustMode.RightUp :
                    spriteDirection.Value = SpriteDirection.BackRight;
                    _animator.SetFloat("Horizontal", 0.7f);
                    _animator.SetFloat("Vertical", 0.7f);
                    break;
                case ThrustMode.Left :
                    spriteDirection.Value = SpriteDirection.Left;
                    _animator.SetFloat("Horizontal", -1f);
                    _animator.SetFloat("Vertical", 0);
                    break;
                case ThrustMode.Right :  
                    spriteDirection.Value = SpriteDirection.Right;
                    _animator.SetFloat("Horizontal", 1f);
                    _animator.SetFloat("Vertical", 0);
                    break;
                case ThrustMode.LeftDown :
                    spriteDirection.Value = SpriteDirection.FrontLeft;
                    _animator.SetFloat("Horizontal", -0.7f);
                    _animator.SetFloat("Vertical", -0.7f);
                    break;
                case ThrustMode.RightDown :  
                    spriteDirection.Value = SpriteDirection.FrontRight;
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