// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

/*
    * Worm 애니메이션 등이 끝났음을 알리기 위한 액션 스크립트

    * Animator trigger 통한 구현에 문제가 발생해 만든 대안
         - Trigger은 해당 transition이 없을 때 실행되면, 무시되지 않고 trigger가 keep 됨

    * Worm 스크립트 자체에서 애니메이터를 받아와 set trigger 하는 편이 더 안전해 보이나,
     일단 bool 조절이 더 간편하기 때문에 이쪽을 선택함
*/

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of a bool parameter on Exit")]
	public class SetAnimatorBoolExit : FsmStateAction
	{
		[RequiredField]
        [Tooltip("Animator Component.")]
        public Animator _animator;
		
        [RequiredField]
        [UIHint(UIHint.AnimatorBool)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;
		
		[Tooltip("The Bool value to assign to the animator parameter")]
		public FsmBool Value;

		public override void OnExit()
		{
            SetParameter();
		}


        private void SetParameter()
		{
            _animator.SetBool(parameter.Value, Value.Value) ;
		}

	}
}