using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]

	public class GetAngleNum : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector2 targetDirection;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt angleNum;	//이곳에 angleNum Int값을 저장
		public bool isUpdate;

		public override void Reset()
		{
			isUpdate = false;
		}

		public override void OnEnter()
		{
			GetAngleInt();

			if(!isUpdate)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetAngleInt();
		}
		
		void GetAngleInt()
		{
            Vector2 direction = targetDirection.Value;
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
//			Debug.Log(angle);
			if(angle >= -45f && angle <= 0f) angleNum.Value = 1;
			else if(angle >= 0f && angle <= 45f) angleNum.Value = 2;
			else if(angle >= -180f && angle <= -45f) angleNum.Value = 3;
			else if(angle >= 45f && angle <= 180) angleNum.Value = 4;

//			Debug.Log(angleNum.Value);
		}
	}
}