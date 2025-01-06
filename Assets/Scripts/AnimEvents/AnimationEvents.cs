using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;


/*
    애니메이션 이벤트용 함수들.
    애니메이션이 재생될 때 호출됨.
*/
public class AnimationEvents : MonoBehaviour
{
    private PlayMakerFSM _fsm;
	private FsmBool canActionBuffer;
	private FsmBool canAction;
	private FsmBool attackTiming;
	private FsmBool healthPotionTiming;

	void Awake()
	{
		_fsm = this.transform.root.GetComponent<PlayMakerFSM>();
	}

	void Start()
	{
		canActionBuffer = _fsm.FsmVariables.GetFsmBool("canActionBuffer");
		canAction = _fsm.FsmVariables.GetFsmBool("canAction");
		attackTiming = _fsm.FsmVariables.GetFsmBool("attackTiming");
		healthPotionTiming = _fsm.FsmVariables.GetFsmBool("healthPotionTiming");
	}

    // 액션 버퍼가 가능해지는 시점
    void CanActionBuffer()
	{
		canActionBuffer.Value = true;
	}

    // 행동이 가능해지는 시점
	void CanAction()
	{
		canAction.Value = true;
	}

	void AttackTiming()
	{
		attackTiming.Value = true;
	}

	void PotionTiming()
	{
		healthPotionTiming.Value = true;
	}

}
