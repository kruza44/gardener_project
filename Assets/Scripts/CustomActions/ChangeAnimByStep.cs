using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /*
        플레이어 공격 + 이동 시 발걸음을 맞추기 위한 스크립트
        SetSightDirection 대신 사용

        다시 Attack state가 되었을 경우(OnExit 사용 불가능), timer와 step이 유지되어야 한다
        멈춘 경우, 다른 state가 된 경우 초기화 된다 
            -> 연속공격이 아닌 경우 반드시 Idle을 거치게 하여 Idle에서 초기화하는 방법으로 구현할 수 있다
            -> 움직이는 스크립트에서 isWalking bool을 받아 수정하도록 한다
    */
	[ActionCategory("Custom")]

    public class ChangeAnimByStep : FsmStateAction
    {
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmVector2 sightDirection;   // 바라볼 최종 방향
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 lockOnDirection;  // LockOn 한 상대의 방향
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 axisDirection;    // 가장 최근에 움직인 방향
        [UIHint(UIHint.Variable)]
        [RequiredField]
		public FsmInt angleNum;	//이곳에 angleNum Int값을 저장
        
        [UIHintAttribute(UIHint.Variable)]
        public FsmBool isLockOn;    // LockOn 여부
        public FsmBool isRunning;   // 달리고 있는지 여부 (LockOn 무시)
        public FsmBool isUsingWorm; // 지렁이 사용 중인지 여부 (LockOn 무시)
        [RequiredField]
        public Animator unitAnimator;
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool isWalking;   

        //  연속공격의 경우에 유지되어야 하는 값이므로 Fsm 변수를 받아 저장시킴 (Idle state에서 초기화)
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmFloat walkTimer;
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmInt step; // 0: 걷는 모션, 1: 멈춘 모션
        public float stepTime = 0.6f;  // 걸음이 바뀌는 데에 걸리는 시간

        public override void OnEnter()
        {
            if(!isWalking.Value)
            {
                walkTimer.Value = 0f;
                step.Value = 1;
            }


            SetSightDirection();
            GetAngleInt();
            SetAnimatorLayer();
        }

        // IsUpdate를 체크한 경우에만 실시간 적용
        public override void OnUpdate()
        {
            if(!isWalking.Value)
            {
                walkTimer.Value = 0f;
                step.Value = 1;
            } else  // 걷고 있다면
            {
                walkTimer.Value += Time.deltaTime;
                step.Value = ((int)(walkTimer.Value / stepTime) % 2);
            }

            SetAnimatorLayer(); // step 관련만 변경
        }

        void SetSightDirection()
        {
            if(isLockOn.Value && !isRunning.Value && !isUsingWorm.Value)  // LockOn 상태 + 달리고 있지 않은 경우 + 지렁이 사용 중이지 않은 경우
            {
                sightDirection.Value = lockOnDirection.Value;
            } else  // 달리고 있거나, LockOn 상태가 아닌 경우
            {
                if(axisDirection.Value != Vector2.zero)
                {
                    sightDirection.Value = axisDirection.Value;
                }
            }
        }

        void GetAngleInt()
		{
            Vector2 direction = sightDirection.Value;
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
//			Debug.Log(angle);
			if(angle >= -45f && angle <= 0f) angleNum.Value = 1;
			else if(angle >= 0f && angle <= 45f) angleNum.Value = 2;
			else if(angle >= -180f && angle <= -45f) angleNum.Value = 3;
			else if(angle >= 45f && angle <= 180) angleNum.Value = 4;

//			Debug.Log(angleNum.Value);
		}

        void SetAnimatorLayer()
		{
            int angleNumber = angleNum.Value;

            switch(angleNumber)
            {
                case 1 :    // Back (BL)
                    if(step.Value == 1) // 걷고 있지 않을 때
                    {
                        unitAnimator.SetFloat("Horizontal", 0f);
                        unitAnimator.SetFloat("Vertical", 1f);
                    } else if((step.Value == 0))
                    {
                        unitAnimator.SetFloat("Horizontal", 0.7f);
                        unitAnimator.SetFloat("Vertical", 0.7f);
                    }
                    break;
                case 2 :    // Right (수정 예정, 실제로는 BR)
                    if(step.Value == 1)
                    {
                        unitAnimator.SetFloat("Horizontal", 1f);
                        unitAnimator.SetFloat("Vertical", 0f);
                    } else if((step.Value == 0))
                    {
                        unitAnimator.SetFloat("Horizontal", 0.7f);
                        unitAnimator.SetFloat("Vertical", -0.7f);
                    }
                    break;
                case 3 :    // Front (FL)
                    if(step.Value == 1)
                    {
                        unitAnimator.SetFloat("Horizontal", 0);
                        unitAnimator.SetFloat("Vertical", -1);
                    } else if((step.Value == 0))
                    {
                        unitAnimator.SetFloat("Horizontal", -0.7f);
                        unitAnimator.SetFloat("Vertical", -0.7f);
                    }
                    break;
                case 4 :    // Left (수정 예정. 실제로는 FR)
                    if(step.Value == 1)
                    {
                        unitAnimator.SetFloat("Horizontal", -1);
                        unitAnimator.SetFloat("Vertical", 0);
                    } else if((step.Value == 0))
                    {
                        unitAnimator.SetFloat("Horizontal", -0.7f);
                        unitAnimator.SetFloat("Vertical", 0.7f);
                    }
                    break;
                default :
                    Debug.LogWarning("angleNum error");
                    break;
		    }
	    }
    }
}
