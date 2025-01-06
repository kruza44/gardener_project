using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
    public class SetSightDirection : FsmStateAction
    {
        [UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmVector2 sightDirection;
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 lockOnDirection;
        [UIHintAttribute(UIHint.Variable)]
        public FsmVector2 axisDirection;
        public Animator unitAnimator;
        public Animator weaponAnimator;
        
        [UIHintAttribute(UIHint.Variable)]
        public FsmBool isLockOn;
        public FsmBool isRunning;
        public FsmBool isUpdate;

        public override void OnEnter()
        {
            if(isLockOn.Value && !isRunning.Value)
            {
                sightDirection.Value = lockOnDirection.Value;
            } else
            {
                sightDirection.Value = axisDirection.Value;
            }
        }

        public override void OnUpdate()
        {
            if(isUpdate.Value)
            {
             if(isLockOn.Value && !isRunning.Value)
            {
                sightDirection.Value = lockOnDirection.Value;
            } else
            {
                sightDirection.Value = axisDirection.Value;
            }
            }

        }
    }
}