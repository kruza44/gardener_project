using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
	[Tooltip("Send FSM event based on input")]
    public class InputFSMEvent : FsmStateAction
    {
        // Input 별 FSM에 보낼 이벤트
        // 해당 state에서 보낼 이벤트 없을 시 비워두면 됨
        public FsmEvent con_A_Button;
        public FsmEvent con_B_Button;
        public FsmEvent con_X_Button;
        public FsmEvent con_Y_Button;
        public FsmEvent con_LB_Button;
        public FsmEvent con_LB_Released;
        public FsmEvent con_R_Axis;
        public FsmEvent con_R_Axis_Released;

        // 게임패드 스틱 input
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 axisDirection_L;  // L 스틱 axis
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 axisDirection_R;  // R 스틱 axis

        // Action buffer 사용 여부
        public FsmBool useActionBuffer;
        [UIHint(UIHint.Variable)]
		public FsmBool canAction;
		[UIHint(UIHint.Variable)]
		public FsmBool canActionBuffer;

        public float deadzone = 0.4f;  // 게임패드 스틱 데드존 (스냅백 방지)

        private float timer;    // 버튼 입력 / 유지 구분을 위한 타이머
        private Vector2 dir;    // axisDirection_R에 (0,0)이 저장되지 않도록

        public override void OnUpdate()
        {
            GetLStickAxis();
            GetRStickAxis();

            if(useActionBuffer.Value)
            {

            } else
            {
                
            }

            if(Input.GetButton("B_Button")) // B 버튼 입력 / 유지 구분을 위한 타이머
            {
                timer += Time.deltaTime;
            }

            if(Input.GetButton("A_Button"))
            {
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
                Fsm.Event(con_X_Button);
            } else if(Input.GetButtonDown("Y_Button"))
            {
                Fsm.Event(con_Y_Button);
            }

            if(dir != Vector2.zero) // R 스틱 입력 감지
            {
                Fsm.Event(con_R_Axis);
            } else  // R 스틱 release 감지 (axisDirection_R이 아닌 dir로 zero 구분을 한다)
            {
                Fsm.Event(con_R_Axis_Released);
            }

            if (Input.GetButton("LB_Button"))
            {
                Fsm.Event(con_LB_Button);
            } else
            {
                Fsm.Event(con_LB_Released);
            }
        }

        void GetLStickAxis()
		{
			dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");
            if(dir.magnitude < deadzone) { dir = Vector2.zero; }    // 데드존
            dir.Normalize();
		}

        void GetRStickAxis()
        {
            // Input 컨트롤러 R_스틱 받아오기
            dir.x = Input.GetAxisRaw("R_Horizontal");
            dir.y = Input.GetAxisRaw("R_Vertical");
            if(dir.magnitude < deadzone) { dir = Vector2.zero; }    // 데드존
            dir.Normalize();
            if(dir != Vector2.zero) // dir에만 zero가 저장되고, 다음 state에서 사용되는 axisDirection_R에는 zero가 저장되지 않음
            {
                axisDirection_R.Value = dir;
            }
        }
		
	}
}