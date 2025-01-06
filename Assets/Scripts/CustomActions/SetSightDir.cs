using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /*
        플레이어 스프라이트가 바라볼 방향을 설정하는 스크립트
        LockOn 상태인 경우, LockOn 상태가 아닌 경우로 나뉨
    */
	[ActionCategory("Custom")]
    public class SetSightDir : FsmStateAction
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

        /*
            현재는 이 스크립트에서 Animator의 State를 바꾸지 않음
            "Get Angle Num" + "Set Animator By Angle Num"에서 따로 처리하는 중
            굳이 나눈 이유는 기억이 안 남...
        */

//        public Animator unitAnimator;
//        public Animator weaponAnimator; // 무기, 유닛 애니메이터는 따로
        
        public FsmBool isLockOn;    // LockOn 여부
        public FsmBool isRunning;   // 달리고 있는지 여부 (LockOn 무시)
        public FsmBool isUsingWorm; // 지렁이 사용 중인지 여부 (LockOn 무시)
        public FsmBool forceSightDir;   // 어떤 조건이든 sightDirection을 보도록
        public FsmBool isUpdate;    // State가 유지되는 동안 계속 SightDirection을 바꿀지 여부
        [RequiredField]
        public Animator unitAnimator;
//        [RequiredField]
//        public Animator weaponAnimator;


        public override void Reset()
		{
			isUpdate = false;
		}

        public override void OnEnter()
        {
            SetSightDirection();
            GetAngleInt();
            SetAnimatorLayer();
        }

        // IsUpdate를 체크한 경우에만 실시간 적용
        public override void OnUpdate()
        {
            if(isUpdate.Value)
            {
                SetSightDirection();
                GetAngleInt();
                SetAnimatorLayer();
            }
        }

        void SetSightDirection()
        {
            if(forceSightDir.Value)
            {
                return;
            }
            else if(isLockOn.Value && !isRunning.Value && !isUsingWorm.Value)  // LockOn 상태 + 달리고 있지 않은 경우 + 지렁이 사용 중이지 않은 경우
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
                    unitAnimator.SetFloat("Horizontal", 0);
                    unitAnimator.SetFloat("Vertical", 1);
//                    weaponAnimator.SetFloat("Horizontal", 0);
//                    weaponAnimator.SetFloat("Vertical", 1);
                    break;
                case 2 :    // Right (수정 예정, 실제로는 BR)
                    unitAnimator.SetFloat("Horizontal", 1);
                    unitAnimator.SetFloat("Vertical", 0);
//                    weaponAnimator.SetFloat("Horizontal", 1);
//                    weaponAnimator.SetFloat("Vertical", 0);
                    break;
                case 3 :    // Front (FL)
                    unitAnimator.SetFloat("Horizontal", 0);
                    unitAnimator.SetFloat("Vertical", -1);
//                    weaponAnimator.SetFloat("Horizontal", 0);
//                    weaponAnimator.SetFloat("Vertical", -1);
                    break;
                case 4 :    // Left (수정 예정. 실제로는 FR)
                    unitAnimator.SetFloat("Horizontal", -1);
                    unitAnimator.SetFloat("Vertical", 0);
                    // weaponAnimator.SetFloat("Horizontal", -1);
                    // weaponAnimator.SetFloat("Vertical", 0);
                    break;
                default :
                    Debug.LogWarning("angleNum error");
                    break;
		    }
	    }
    }
}