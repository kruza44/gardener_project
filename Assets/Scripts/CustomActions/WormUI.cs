using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    (임시) 지렁이 방향 표시를 위한 FSM 스크립트
    후일 지렁이 머리가 해당 방향을 바라보는 것으로 수정 예정 (SmoothDamp 사용)
*/
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
    public class WormUI : FsmStateAction
    {
        public GameObject _wormUI;
		[UIHintAttribute(UIHint.Variable)]
        [RequiredField]
        public FsmVector2 axisDirection_R;
        public Transform centerTransform;
        public override void OnEnter()
        {
            _wormUI.SetActive(true);
            _wormUI.transform.position = centerTransform.position + (Vector3)(axisDirection_R.Value * 2);
        }
        
        public override void OnUpdate()
        {
            _wormUI.transform.position = centerTransform.position + (Vector3)(axisDirection_R.Value * 2);
        }

        public override void OnExit()
        {
            _wormUI.SetActive(false);
        }
    }
}
