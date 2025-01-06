using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class PlayerStaminaManager : StaminaManager
{
    [HideInInspector] public float attackStamina; // 앞으로 할 공격에 요구되는 스태미나, attack이 미리 이 스크립트에 저장함
    private FsmBool isPerfectGuard;
    [SerializeField] private float perfectGuardPlusValue; // 퍼펙트 가드가 줄여주는 스태미나 소비량

    protected override void Start()
    {
        base.Start();

        isPerfectGuard = GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("isPerfectGuard");
    }

    void Update()
    {
        /*
            현재는 가드 상황만 고려
            이후 다른 스태미나 리젠 정지 상황 고려하여 수정 필요
            코루틴으로 최적화 필요
        */
        if(_unit.GetIsGuarding()) { briefStopRegen = true; }
    }

    public bool CheckStaminaAvailable(float required)
    {
        if((float)currentParam >= required) { return true; }
        else { return false; }
    }

    public bool CheckAttackAvailable()
    {
        if((float)currentParam >= attackStamina) { return true; }
        else { return false; }
    }

    public override void ReduceCurrentParam(float value)    // 퍼펙트 가드 고려
    {
        if(value < 0) { return; }

        // 퍼펙트 가드의 경우 소태미나 소비 감소
        float reduceValue = (_unit.GetIsGuarding() && isPerfectGuard.Value) ? value - perfectGuardPlusValue : value;

        floatParam -= (reduceValue >= 0) ? reduceValue : 0;    // 감소량이 0보다 낮아지지 않도록
        currentParam = (int)floatParam;
        briefStopRegen = true;

        if(floatParam < 0f) // 0보다 아래로 내려가지 않도록
        {
            floatParam = 0f;
            currentParam = 0;
        }

        if(_bar != null)    // UI
        {
            _bar.SetBar(currentParam);
        }
    }
}
