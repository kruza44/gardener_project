using UnityEngine;

namespace HutongGames.PlayMaker.Actions

{
	[ActionCategory("Custom")]

	public class SetAnimatorByAngleNum : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt angleNum;
		public bool isUpdate;
        [RequiredField]
        public Animator unitAnimator;
        [RequiredField]
        public Animator weaponAnimator;

		public override void Reset()
		{
			isUpdate = false;
		}

		public override void OnEnter()
		{		
			SetAnimatorLayer();
			if(!isUpdate)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			SetAnimatorLayer();
		}

		void SetAnimatorLayer()
		{
            int angleNumber = angleNum.Value;

            switch(angleNumber)
            {
                case 1 :    // Back (BL)
                    unitAnimator.SetFloat("Horizontal", 0);
                    unitAnimator.SetFloat("Vertical", 1);
                    weaponAnimator.SetFloat("Horizontal", 0);
                    weaponAnimator.SetFloat("Vertical", 1);
                    break;
                case 2 :    // Right (수정 예정, 실제로는 BR)
                    unitAnimator.SetFloat("Horizontal", 1);
                    unitAnimator.SetFloat("Vertical", 0);
                    weaponAnimator.SetFloat("Horizontal", 1);
                    weaponAnimator.SetFloat("Vertical", 0);
                    break;
                case 3 :    // Front (FL)
                    unitAnimator.SetFloat("Horizontal", 0);
                    unitAnimator.SetFloat("Vertical", -1);
                    weaponAnimator.SetFloat("Horizontal", 0);
                    weaponAnimator.SetFloat("Vertical", -1);
                    break;
                case 4 :    // Left (수정 예정. 실제로는 FR)
                    unitAnimator.SetFloat("Horizontal", -1);
                    unitAnimator.SetFloat("Vertical", 0);
                    weaponAnimator.SetFloat("Horizontal", -1);
                    weaponAnimator.SetFloat("Vertical", 0);
                    break;
                default :
                    Debug.LogWarning("angleNum error");
                    break;
		    }
	    }
    }
}