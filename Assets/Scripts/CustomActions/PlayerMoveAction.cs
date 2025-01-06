using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
	public class PlayerMoveAction : FsmStateAction
	{
        [UIHintAttribute(UIHint.Variable)]
		[RequiredField]
        public FsmVector2 axisDirection;    // 움직이는 방향

        public FsmFloat walkSpeed;  // 걷는 속도
        public FsmFloat runValue;   // 달리기 배속
        public FsmFloat slowValue; // 느린 걸음 속도 감소율
        public FsmBool canRun;  // 공격 중에는 달릴 수 없음
        public FsmBool isRunning;   // 달리고 있는 여부. Sight Direction에 필요
        public FsmBool doSlowWalk;  // 가드, 지렁이 준비 등 상태에서 느리게 걷도록
        public Animator unitAnimator;
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField] public FsmBool isWalking;   // 다른 스크립트에 현재 걷고 있음을 알리기 위함
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField] public FsmBool isJumping;   // 점프하는 동안에는 못움직임 (점프 움직임 클래스가 따로 있음)
        [UIHintAttribute(UIHint.Variable)]
		[RequiredField] public FsmBool inputLocked;
        
        private float finalWalkSpeed;   // 느린 걷기 여부 적용한 walkspeed
        private MoveManager _move;
        private PlayerStaminaManager _stamina;
        private Vector2 moveVec;    // 움직일 방향 + 힘 벡터값, MoveManager에 전달
        private Vector2 dir;
        private float finalSpeed;


        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

        public override void Awake()
        {
            _stamina = Owner.GetComponent<PlayerStaminaManager>();
            _move = Owner.GetComponent<MoveManager>();
        }


        public override void OnUpdate()
        {
            if(isJumping.Value) { return; }

            if(inputLocked.Value)
            {
                unitAnimator.SetBool("DoingWalk", false);
                isWalking.Value = false;
                finalSpeed = 0f;
                return;
            }

            GetLStickAxis();

            if(doSlowWalk.Value)
            {
                finalWalkSpeed = (walkSpeed.Value / slowValue.Value);
            } else
            {
                finalWalkSpeed = walkSpeed.Value;
            }

            // 스틱 인풋이 없을 시 애니메이션 함수 설정, 이동 속도 0
            if(axisDirection.Value == Vector2.zero)
            {
                unitAnimator.SetBool("DoingWalk", false);
                isWalking.Value = false;
                finalSpeed = 0f;
            } else  // 스틱 인풋 있을 시 애니메이션 함수 설정, 이동 속도 설정
            {
                unitAnimator.SetBool("DoingWalk", true);
                isWalking.Value = true;

                if(canRun.Value && Input.GetButton("B_Button") && _stamina.CheckStaminaAvailable(20f * Time.deltaTime)) // B 버튼 입력 시 달리기 이동 속도, 애니메이션 속도 배속
                {
                    isRunning.Value = true;
                    finalSpeed = finalWalkSpeed * runValue.Value;
                    unitAnimator.speed = runValue.Value;
                } else  // 일반적인 걷기 이동 속도 설정
                {
                    isRunning.Value = false;
                    finalSpeed = finalWalkSpeed;
                    unitAnimator.speed = 1f;
                }

                if(isRunning.Value)
                {
                    _stamina.ReduceCurrentParam(20f * Time.deltaTime);
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if(isJumping.Value || inputLocked.Value) { return; }
            
            // MoveManager에 move 벡터값 전달
            moveVec = axisDirection.Value * finalSpeed;
            _move.AddMove(moveVec);
        }

        // 현 State 퇴장 시 애니메이션 배속 초기화
        public override void OnExit()
        {
            unitAnimator.speed = 1;
        }

        void GetLStickAxis()
		{
			dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");
            if(dir.magnitude < 0.4f) { dir = Vector2.zero; }    // 데드존
            dir.Normalize();
            axisDirection.Value = dir;
		}
    }
}
