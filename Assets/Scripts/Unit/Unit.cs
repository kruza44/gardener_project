using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour, IHasHurtBox
{
    /*.
        현재는 임시적으로 inspector에서 스탯을 설정할 수 있도록 했지만,
        후일 excel 파일 등에서 받아오도록 수정 필요.
    */
    [Header("Unit Stat")]
    [SerializeField] private int maxHP;
    [SerializeField] private int maxStamina;
    [SerializeField] private int maxPoise;
    [SerializeField] private int strength;
    [SerializeField] private int defense;
    [SerializeField] protected bool isHeavy;  // 무거운 경우, pushPower가 반감된다
    /////////////////////////////////////

    // [HideInInspector] public UnitStat baseStat;  // 스탯을 어떻게 받아올지??
    [Header("Else")]
    // public HitWhiteEffect _hitEffect;   // 피격 시 하얗게 번쩍이는 이펙트 /// *EffectManager 싱글턴 형태로 개선 필요*
    public SpriteRenderer _sprite;    // FlashWhite 등 이펙트를 위함
    [HideInInspector] public bool isInvincible = false;  // 회피 등 무적판정인 경우
    [HideInInspector] public bool isInPhaseTransition = false;  // 무적이지만 피격 이펙트는 나타나야 하는 경우

    // 실드배시 등으로 날아갈 때 필요한 정보
    // 다른 Unit과 충돌할 시 해당 Unit에게 전달하기 위해 필요
    [HideInInspector] public DamageInfo shieldBashedInfo;

    protected HealthManager _health;
    protected StaminaManager _stamina;
    protected PoiseManager _poise;
    protected MoveManager _move;
    /*
        hitBoxList : 해당 공격의 attackID를 등록
            - renewTime이 지나면 해당 공격은 hitBoxList에서 Invoke로 제거
            - 제거된 후부터는 다시 공격받을 수 있게 됨 (재등록)
            - 최적화를 위해 List 대신 HashSet 사용
    */
    private HashSet<Guid> hitBoxList = new HashSet<Guid>();
    protected bool isDead = false;

    // private UnitStat appliedStat;   // *버프, 장비 효과 등 구현을 위해 후일 추가 필요*

    virtual protected void Awake()
    {
        _health = GetComponent<HealthManager>();
        _stamina = GetComponent<StaminaManager>();
        _poise = GetComponent<PoiseManager>();
        _move = GetComponent<MoveManager>();

        _health.InitializeParam(maxHP, maxHP);  // Current param 불러와서 할당해야 하나 아직 미완성
        _stamina.InitializeParam(maxStamina, maxStamina);
        _poise.InitializeParam(maxPoise, maxPoise);
    }


    /*
        피격 당할 시 호출되는 함수
    */
    virtual public void GetHit(DamageInfo dmgInfo)
    {
        // 무적일 시 무시
        if(isInvincible) { return; }

        // 보스 등 페이즈 전환 시
        // 무적이지만 피격 효과는 나타남
        // 더 나은 방법으로 구현해야 할 수도 있음
        if(isInPhaseTransition)
        {
            EffectManager.Instance.DamageEffect(_sprite, dmgInfo.damageEffectType);
            return;
        }

        // 죽은 경우 밀려나기만 함
        if(isDead)
        {
            hitBoxList.Add(dmgInfo.attackID);
            StartCoroutine(DeleteAttackID(dmgInfo.attackID, dmgInfo.renewTime));
            
            DeadPushed(dmgInfo);
            return;
        }

        // hitTarget 대상인지 확인
        bool isHitTarget = false;
        foreach(string tag in dmgInfo.hitTarget)
        {
            if(transform.root.CompareTag(tag))
            {
                isHitTarget = true;
            }
        }
        if(!isHitTarget) { return; }

        // Guid 이미 등록되었는지 확인
        if(hitBoxList.Contains(dmgInfo.attackID)) { return; } 

        // 위 조건들 충족 시 피격 시작
        if (GetIsGuarding())  // 가드 중일 시 가드 판정 확인
        {
            Vector2 sightDir = GetSightDirection();
            float dot = Vector2.Dot(-sightDir, dmgInfo.attackDirection);    // 시야 반대 방향, 공격 방향 간 내적

            if(dot <= 0f)   // 각도차가 90 이상이라면 (옆, 뒤에서 공격)
            {
                hitBoxList.Add(dmgInfo.attackID);
                StartCoroutine(DeleteAttackID(dmgInfo.attackID, dmgInfo.renewTime));    // renewTime 지나면 저장된 Guid 제거
                GetDamaged(dmgInfo);
                return;
            }
            /*
                * 가드 피격 *
                // 스태미나 소모
                // <GuardStun>
                // 전부 소진 시 <GuardBroke>
            */
            ShieldDamaged(dmgInfo);
        } else  // 가드 중이 아닐 시 일반적인 데미지
        {
            GetDamaged(dmgInfo);
        }
    }

    // 다른 유닛이 sprite에 접근할 수 있도록
    public void Shake(float shakeDuration)
    {
        EffectManager.Instance.SpriteShake(_sprite, shakeDuration);
    }

    virtual public void IsDead()
    {
        // 사망 관련 효과 (상속에서 구현)
    }

    virtual public void DeadPushed(DamageInfo dmgInfo)
    {
        // 사망 후 공격 받으면 시체가 밀려남
    }

    virtual public void GetStaggered(DamageInfo dmgInfo)
    {
        // FSM이나 Tree에 Staggered 이벤트 전달 (상속에서 구현)
    }

    virtual public void GuardBroke(DamageInfo dmgInfo)
    {
        // FSM이나 Tree에 GuardBroke 이벤트 전달 (상속에서 구현)
    }

    virtual public void GuardStunned(DamageInfo dmgInfo)
    {
        // FSM이나 Tree에 GuardStunned 이벤트 전달 (상속에서 구현)
    }

    virtual public void WallSlammed(DamageInfo wallDmgInfo)
    {
        // FSM이나 Tree에 WallSalmmed 이벤트 전달 (상속에서 구현)
    }

    IEnumerator DeleteAttackID(Guid id, float time)
    {
        yield return new WaitForSeconds(time);
        hitBoxList.Remove(id);
    }

    virtual public bool GetIsGuarding()  // 플레이어, 적에 따라 FSM, BT로 나뉨
    {
        return false;
    }

    virtual protected Vector2 GetSightDirection()   // 플레이어, 적에 따라 FSM, BT로 나뉨
    {
        return Vector2.zero;
    }

    protected void GetDamaged(DamageInfo dmgInfo)
    {
        hitBoxList.Add(dmgInfo.attackID);
        StartCoroutine(DeleteAttackID(dmgInfo.attackID, dmgInfo.renewTime));    // renewTime 지나면 저장된 Guid 제거

        // 피격 시 FlashWhite 이펙트
        EffectManager.Instance.DamageEffect(_sprite, dmgInfo.damageEffectType);

        /*
            int defense = appliedStat.GetDefense();
            방어력에 의한 데미지 경감 나중에 구현
        */

        // HP, Poise 감소
        _health.ReduceCurrentParam(dmgInfo.baseDamage);
        _poise.ReduceCurrentParam(dmgInfo.poiseDamage);
        // Debug.Log("Current Poise: " + _poise.currentParam);

        // Poise 0 이하 시 Staggered state
        if(_poise.currentParam <= 0f)
        {
            GetStaggered(dmgInfo);
            _poise.SetCurrentParam(_poise.maxParam);   // Poise 다시 최대치로 회복
        } else if(dmgInfo.doNonStunPush)    // Staggered, Guard 상태가 아닌 경우에만 non stun push 한다
        {
            StartCoroutine(NonStunPushed(dmgInfo));
        }
    }

    virtual protected void ShieldDamaged(DamageInfo dmgInfo)
    {
        hitBoxList.Add(dmgInfo.attackID);
        StartCoroutine(DeleteAttackID(dmgInfo.attackID, dmgInfo.renewTime));    // renewTime 지나면 저장된 Guid 제거

        _stamina.ReduceCurrentParam(dmgInfo.staminaDamage);
        if(_stamina.currentParam > 0)
        {
            GuardStunned(dmgInfo);
        } else  // 스태미나 소진 시 <GuardBroke> state 
        {
            GuardBroke(dmgInfo);    // 가드 상황 뿐만 다른 상황에서도 쓰이므로 따로 함수를 만듬
        }
    }

    IEnumerator NonStunPushed(DamageInfo dmgInfo)
    {
        float timer = 0f;
        float moveSpeed = (dmgInfo.pushPower / 3f);

        DOTween.To(()=> moveSpeed, x=> moveSpeed = x, 0.1f, 0.2f).SetEase(Ease.OutQuad);

        while(timer <= 0.4f)
        {
            timer += Time.fixedDeltaTime;
            _move.AddMove(dmgInfo.pushDirection * moveSpeed);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }


}