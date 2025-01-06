using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Action")]
	[Tooltip("Send player event in idle state")]
    public class IdleInputEvent : FsmStateAction
    {
        public FsmEvent con_A_Button;
        public FsmEvent con_B_Button;
        public FsmEvent con_X_Button;
        public FsmEvent con_Y_Button;
        public FsmEvent con_LB_Button;
        public FsmEvent con_LB_Released;
        public FsmEvent con_R_Axis;
        public FsmEvent con_R_Axis_Released;
        public FsmEvent con_RB_Button;

        // public FsmVector2 axisDirection_L;
        public FsmVector2 axisDirection_R;
        public float deadzone = 0.4f;  // 게임패드 스틱 데드존 (스냅백 방지)
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool axisRLocked; // <WormReady> state 캔슬 시 필요
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool axisR_ReleaseLocked; // <WormReady> state를 위해 필요
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool jumpLocked;
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool inputLocked;

        private float timer;
        private Vector2 dir;    // axisDirection_R에 (0,0)이 저장되지 않도록
        private PlayerStaminaManager _stamina;  // 해당 행동을 하기에 스태미나가 충분한지 체크
        private HealthPotionManager _potion;    // 포션이 충분한지 체크

        public override void Awake()
        {
            _stamina = Owner.GetComponent<PlayerStaminaManager>();
            _potion = Owner.GetComponent<HealthPotionManager>();
        }

        public override void OnUpdate()
        {
            if(inputLocked.Value) { return; }

            // GetLStickAxis();
            GetRStickAxis();

            if(Input.GetButton("B_Button")) // B 버튼 입력 / 유지 구분을 위한 타이머
            {
                timer += Time.deltaTime;
            }

            if(Input.GetButtonDown("A_Button"))
            {
                if(!jumpLocked.Value)
                    Fsm.Event(con_A_Button);
            } else if(Input.GetButtonUp("B_Button"))
            {
                if(timer <= 0.14f)  // 0.14초 미만 유지 시 입력으로 취급
                {
                    timer = 0f;
                    Fsm.Event(con_B_Button);
                } else
                {
                    timer = 0f;
                }
            } else if(Input.GetButtonDown("X_Button"))
            {
                if(_stamina.CheckAttackAvailable()) // 스태미나 충분할 경우
                    Fsm.Event(con_X_Button);
            } else if(Input.GetButtonDown("Y_Button"))
            {
                if(_potion.GetCurrentCount() > 0)
                    Fsm.Event(con_Y_Button);
            }

            if(dir != Vector2.zero) // R 스틱 입력 감지
            {
                Fsm.Event(con_R_Axis);
            } else  // R 스틱 release 감지
            {
                Fsm.Event(con_R_Axis_Released);
            }

            if(Input.GetButton("LB_Button"))
            {
                Fsm.Event(con_LB_Button);
            } else
            {
                Fsm.Event(con_LB_Released);
            }

            if(Input.GetButtonDown("RB_Button"))
            {
                if(_stamina.CheckStaminaAvailable(20f)) // Worm 스태미나 충분할 경우
                {
                    Fsm.Event(con_RB_Button);
                }
            }
        }

        // void GetLStickAxis()
		// {
		// 	dir.x = Input.GetAxisRaw("Horizontal");
        //     dir.y = Input.GetAxisRaw("Vertical");
        //     if(dir.magnitude < deadzone) { dir = Vector2.zero; }    // 데드존
        //     dir.Normalize();
		// }

        // 이곳에서는 R 스틱 관련 이벤트만 처리
        void GetRStickAxis()
        {
            // Input 컨트롤러 R_스틱 받아오기
            dir.x = Input.GetAxisRaw("R_Horizontal");
            dir.y = Input.GetAxisRaw("R_Vertical");

            if(dir.magnitude < deadzone) { dir = Vector2.zero; }    // 데드존
            dir.Normalize();

            if(axisRLocked.Value)
            {
                if(dir == Vector2.zero)
                {
                    axisRLocked.Value = false;
                } else
                {
                    dir = Vector2.zero;
                    return;
                }
            }

            if(axisR_ReleaseLocked.Value)
            {
                if(dir != Vector2.zero)
                {
                    axisR_ReleaseLocked.Value = false;
                } else
                {
                    dir = axisDirection_R.Value;
                    return;
                }
            }

            // if(dir != Vector2.zero)
            // {
            //     axisDirection_R.Value = dir;    // axisDirection_R 변수는 (0, 0) 상태가 존재하지 않음
            // }
        }
		
	}
}