using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Action")]
    public class SetBoolEnterExit : FsmStateAction
    {
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmBool boolToSet;
        public bool enterFalseExitTrue;

        public override void OnEnter()
        {
            boolToSet.Value = enterFalseExitTrue ? false : true;
        }

        public override void OnExit()
        {
            boolToSet.Value = enterFalseExitTrue ? true : false;
        }
    }
}
