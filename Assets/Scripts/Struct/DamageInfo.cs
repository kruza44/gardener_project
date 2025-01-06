using System;
using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    public GameObject causer;   // 공격자
    public int baseDamage;  // 기본 데미지
    public float pushPower; // 피격자가 Staggered 됐을 때 밀려날 세기
    public int poiseDamage; // Poise가 0이 되면 Staggered 상태가 된다
    public int staminaDamage;   // 가드 시 스태미나 소모량 (적 -> 플레이어 상대로만 해당)
    public float renewTime;   // 같은 히트박스에 다시 피격당할 수 있는 시간
    public bool doWallSlam;  // 벽쿵 여부 (GuardBreak)
    public bool doNonStunPush;  // 체크 시 Staggered, GuardBreak 상태가 아닐 때에도 1/3 세기로 밀려남
    public DamageEffectType damageEffectType; // 피격 시 이펙트 (예: flashwhite)
    [HideInInspector] public Vector2 pushDirection;   // 피격자가 Staggered 됐을 때 밀려날 방향
    [HideInInspector] public Vector2 sightDirection;  // 피격자가 Staggered 됐을 때 바라볼 방향
    [HideInInspector] public Vector2 attackDirection; // 가드 판정을 위한 공격 방향

    /*
        hitTarget
        - 공격 대상(팀, 적, 자신) 구분할 수 있어야 함
        - 기본적인 공격들은 Unit 특성을 따라야 할 것임
        - 현재 기본적인 Unit의 hitTarget을 어디에 저장하고, 어떻게 받아와야 할지 해결 못함
        - Tag만 따로 모아서 변수로 지정할 수 없다는 문제도 있음 (Using Cinemachine으로 해결할 수는 있다고 함)
        - 일단 제대로 구현 전까지는 공격마다 일일이 지정해줘야 함
    */
    public string[] hitTarget; // 공격할 대상들 (Tag로 구분)

    /*
        피격자가 Guid를 읽어올 수 있어야 하므로 public으로 선언했으나
        공격자 외에는 Guid 수정이 불가능해야 한다
        DamageInfo를 class로 바꿔 private Guid를 읽는 함수를 넣어야 할까...?
    */
    public Guid attackID;   // 중복히트 방지, 다단히트 구현용
}
