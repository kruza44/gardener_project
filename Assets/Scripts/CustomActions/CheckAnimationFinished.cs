using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	/*
		Playmaker state과 Animator state의 박자가 어긋나는 상황을 방지하기 위해
		우선 Animator state가 원하는 state인지 먼저 확인부터 하는 과정을 추가함
	*/
	[ActionCategory("Custom")]
    public class CheckAnimationFinished : FsmStateAction
    {
        [Tooltip("Event to send when animation has finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Animation to check if finished.")]
		public FsmString animState;

		private bool isStarted;

        [RequiredField]
		public Animator _animator;

		public override void OnEnter()
		{
			isStarted = false;
		}

        public override void OnUpdate()
		{
			AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo (0);

			if (animInfo.IsName (animState.Value))
			{
				isStarted = true;
			}

			if(isStarted)
			{
				if (!animInfo.IsName (animState.Value))
				{
					Fsm.Event (finishEvent);
				}
			}
		}
    }
}