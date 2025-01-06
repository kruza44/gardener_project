using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class NonPlayerUnit : Unit
{
    [HideInInspector] public bool isLockedOn = false;
    private BehaviorTree behaviorTree;
    private SharedAIState btState;
    private SharedVector2 btPushedDir;
    private SharedFloat btPushedPow;
    private SharedVector2 btSightDir;
    private SharedBool btIsGuarding;
    private SharedFloat btStunTime; // stun 종류에 따라서 달라져야 한다
    private SharedString btAnimTrans;   // stun 종류에 따라서 animator trigger가 달라져야 한다
    private SharedBool btCheckWallSlam; // 벽쿵 감지를 할 것인지 여부 (DamageInfo에 있음)
    private TargetManager _targetManager;   // Peace 상태에서 피격 시 반응하기 위함
    [Header("AI stun time test parameters")]
    [SerializeField] private float staggeredTime = 0.5f;
    [SerializeField] private float guardStunTime = 0.4f;
    [SerializeField] private float guardBrokeTime = 0.6f;
    [SerializeField] private float wallSlammedTime = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        
        behaviorTree = GetComponent<BehaviorTree>();
        _targetManager = GetComponent<TargetManager>();

        staggeredTime = 0.5f;
        guardStunTime = 0.4f;
        guardBrokeTime = 0.6f;
        wallSlammedTime = 1.5f;
    }

    void Start()
    {
        if(behaviorTree != null)
        {
            btState = (SharedAIState)behaviorTree.GetVariable("state");
            btPushedDir = (SharedVector2)behaviorTree.GetVariable("pushedDirection");
            btSightDir = (SharedVector2)behaviorTree.GetVariable("sightDirection");
            btPushedPow = (SharedFloat)behaviorTree.GetVariable("pushedPower");
            btIsGuarding = (SharedBool)behaviorTree.GetVariable("isGuarding");
            btStunTime = (SharedFloat)behaviorTree.GetVariable("stunTime");
            btAnimTrans = (SharedString)behaviorTree.GetVariable("animatorTransition");
            btCheckWallSlam = (SharedBool)behaviorTree.GetVariable("checkWallSlam");
        } else { Debug.LogWarning("NonPlayerUnit script BT null"); }
    }

    public override void GetHit(DamageInfo dmgInfo)
    {
        // Peace 상태일 경우 피격 시 타겟 설정
        _targetManager.SetTarget(dmgInfo.causer.transform);

        base.GetHit(dmgInfo);
    }

    public override void IsDead()
    {
        isDead = true;
        btState.Value = AIState.Dead;
        
        _health._bar.gameObject.SetActive(false);   // HP 바 비활성화

        // 콜라이더 비활성화
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach(Collider2D col in cols)
        {
            col.enabled = false;
        }

        // 락온 해제 후 다른 타겟으로 재할당
        if(isLockedOn)
        {
            LockOnManager _lock = GameObject.FindGameObjectWithTag("Gardener").GetComponent<LockOnManager>();
            _lock.UnDoLockOn();
            _lock.DoLockOn();
        }
    }

    public override void DeadPushed(DamageInfo dmgInfo)
    {
        btPushedDir.Value = dmgInfo.pushDirection;
        btPushedPow.Value = isHeavy ? dmgInfo.pushPower/2f : dmgInfo.pushPower/4f;

        behaviorTree.SendEvent("Pushed");
    }

    public override void GetStaggered(DamageInfo dmgInfo)
    {
        btPushedDir.Value = dmgInfo.pushDirection;
        btPushedPow.Value = isHeavy ? dmgInfo.pushPower/2f : dmgInfo.pushPower;
        btSightDir.Value = dmgInfo.sightDirection;
        btCheckWallSlam.Value = dmgInfo.doWallSlam;
        btStunTime.Value = staggeredTime;
        btAnimTrans.Value = "Staggered";
        behaviorTree.SendEvent("Stunned");
    }

    public override void GuardBroke(DamageInfo dmgInfo)
    {
        // 적 Unit의 경우 GuardBroke 시 stamina 풀로 회복됨
        GetDamaged(dmgInfo);
        _stamina.SetCurrentParam(_stamina.maxParam);

        btPushedDir.Value = dmgInfo.pushDirection;
        btPushedPow.Value = isHeavy ? dmgInfo.pushPower/2f : dmgInfo.pushPower;
        btSightDir.Value = dmgInfo.sightDirection;
        btCheckWallSlam.Value = dmgInfo.doWallSlam;
        btStunTime.Value = guardBrokeTime;
        btAnimTrans.Value = "GuardBroke";
        behaviorTree.SendEvent("Stunned");
    }

    public override void GuardStunned(DamageInfo dmgInfo)
    {
        btPushedDir.Value = dmgInfo.pushDirection;
        btPushedPow.Value = isHeavy ? dmgInfo.pushPower/4f : dmgInfo.pushPower/2f;   // 가드 스턴은 약하게 밀려남
        btSightDir.Value = dmgInfo.sightDirection;
        btCheckWallSlam.Value = false;
        btStunTime.Value = guardStunTime;
        btAnimTrans.Value = "GuardStunned";
        behaviorTree.SendEvent("Stunned");
    }

    public override void WallSlammed(DamageInfo wallDmgInfo)
    {
        CameraManager.Instance.ShakeCamera(0.1f);
        this.Shake(0.3f);
        
        this.GetDamaged(wallDmgInfo);

        btPushedDir.Value = wallDmgInfo.pushDirection;
        btPushedPow.Value = wallDmgInfo.pushPower;
        btSightDir.Value = Vector2.zero;
        btCheckWallSlam.Value = false;
        btStunTime.Value = wallSlammedTime;
        btAnimTrans.Value = "WallSlammed";
        behaviorTree.SendEvent("Stunned");
    }

    public override bool GetIsGuarding()
    {
        return btIsGuarding.Value;
    }

    protected override Vector2 GetSightDirection()
    {
        return btSightDir.Value;
    }
}
