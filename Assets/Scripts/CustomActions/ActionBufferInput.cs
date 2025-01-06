using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
	public class ActionBufferInput : FsmStateAction
	{
        [UIHint(UIHint.Variable)]
		public FsmBool canAction;
		[UIHint(UIHint.Variable)]
		public FsmBool canActionBuffer;
		[UIHint(UIHint.Variable)]
		public FsmBool bufferCommanded;
		[RequiredField]
		[UIHintAttribute(UIHint.Variable)]
        public FsmVector2 axisDirection_L;    // L 스틱 axis
		[RequiredField]
		[UIHintAttribute(UIHint.Variable)]
		public FsmFloat deadZone;

		private string command;
		private Vector2 dir;
		private PlayerStaminaManager _stamina;  // 해당 행동을 하기에 스태미나가 충분한지 체크

		public override void Awake()
		{
			_stamina = Owner.GetComponent<PlayerStaminaManager>();
		}

        public override void OnEnter()
        {
            command = string.Empty;
        }

		public override void OnUpdate()
		{
			GetLStickAxis();

			if(canActionBuffer.Value == true)
			{			
				if (Input.GetButtonDown ("A_Button"))	
				{
					axisDirection_L.Value = dir;
                    command = "JumpPressed";
				} else if (Input.GetButtonDown ("X_Button")) 
				{
					if(_stamina.CheckAttackAvailable()) // 스태미나 충분할 경우
					{
						axisDirection_L.Value = dir;
						command = "AttackPressed";
					}
				}
			}

            if(canAction.Value == true)
            {
                if(command != string.Empty)
                {
                    Fsm.Event(command);
                }
            }
		}

        public override void OnExit()
        {
            command = string.Empty;
			canAction.Value = false;
			canActionBuffer.Value = false;
        }

		void GetLStickAxis()
		{
			dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");
            if(dir.magnitude < deadZone.Value) { dir = Vector2.zero; }    // 데드존
            dir.Normalize();
		}
	}
}
