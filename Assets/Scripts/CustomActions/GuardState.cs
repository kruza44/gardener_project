using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
    public class GuardState : FsmStateAction
    {
        [UIHintAttribute(UIHint.Variable)]
		[RequiredField]
        public FsmBool isGuarding;
        public override void OnEnter()
        {
            isGuarding.Value = true;

        }

        public override void OnExit()
        {
            isGuarding.Value = false;
            
        }
    }
}
