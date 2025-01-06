using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class PlayerUnit : Unit
{
    private PlayMakerFSM _fsm;
    private FsmVector2 fsmSightDir;
    private FsmVector2 fsmPushedDir;
    private FsmFloat fsmPushedPow;
    private FsmBool fsmIsGuarding;

    protected override void Awake()
    {
        base.Awake();

        _fsm = GetComponent<PlayMakerFSM>();
        if(_fsm == null) { Debug.LogWarning("PlayerUnit script fsm null error"); }
    }

    void Start()
    {
        fsmSightDir = _fsm.FsmVariables.GetFsmVector2("sightDirection");
        fsmPushedDir = _fsm.FsmVariables.GetFsmVector2("pushedDirection");
        fsmPushedPow = _fsm.FsmVariables.GetFsmFloat("pushedPower");
        fsmIsGuarding = _fsm.FsmVariables.GetFsmBool("isGuarding");
    }

    public override void IsDead()
    {
        // FSM에 죽었다고 이벤트 전달
    }

    public override void GetStaggered(DamageInfo dmgInfo)
    {
        fsmSightDir.Value = dmgInfo.sightDirection;
        fsmPushedDir.Value = dmgInfo.pushDirection;
        fsmPushedPow.Value = dmgInfo.pushPower;

        _fsm.SendEvent("Staggered");
    }

    public override void GuardBroke(DamageInfo dmgInfo)
    {
        GetDamaged(dmgInfo);
        
        fsmSightDir.Value = dmgInfo.sightDirection;
        fsmPushedDir.Value = dmgInfo.pushDirection;
        fsmPushedPow.Value = dmgInfo.pushPower;

        _fsm.SendEvent("GuardBroke");
    }

    public override void GuardStunned(DamageInfo dmgInfo)
    {
        fsmSightDir.Value = dmgInfo.sightDirection;
        fsmPushedDir.Value = dmgInfo.pushDirection;
        fsmPushedPow.Value = (dmgInfo.pushPower / 2f);  // 가드 스턴 시 약하게 밀려남

        _fsm.SendEvent("GuardStunned");
    }

    public override bool GetIsGuarding()
    {
        return fsmIsGuarding.Value;
    }

    protected override Vector2 GetSightDirection()
    {
        return fsmSightDir.Value;
    }
}
