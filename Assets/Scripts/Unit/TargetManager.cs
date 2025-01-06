using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

/*
    Target이 장애물에 가려 보이지 않는 경우 등
    여러 다른 상황에 대한 반응 구현 아직 못함

    처음에는 sightDirection에 따라 시야각을 판별하는 방식이었지만
    스프라이트 방향에 따라 시야각 90, 135도로 바꿔가며 고정하는 방식으로 수정
    조금 이상하긴 하지만 최소한 플레이어가 예측 가능함
*/

public class TargetManager : MonoBehaviour
{
    public enum ReactionType
    {
        Attack, Dodge, Nothing
    }

    // Debug용 에디터 handle을 위해 public 선언함
    public float sightRadius;   // 시야 범위
    // [Range(0, 360)] public float sightAngle;    // 시야각
    [SerializeField] private LayerMask targetMask;    // Target으로 설정할 LayerMask
    [SerializeField] private LayerMask obstacleMask;  // 시야를 가리게 하는 LayerMask

    // private List<Transform> foundTargets = new List<Transform>();    // 발견한 타겟들을 저장

    /*
        상대의 행동에 공격 / 회피 / 무반응 중 어떤 반응을 할지 결정 여부
    */
    [SerializeField] private ReactionType meleeAttackReactionType;
    [SerializeField] private ReactionType rangeAttackReactionType;
    [SerializeField] private ReactionType healingReactionType;
    private BehaviorTree _bt;
    private SharedTransform sharedTarget;
    private SharedAIState sharedState;
    // private SharedVector2 sightDirection;    // 스프라이트 방향을 따르도록 변경
    private SharedFloat targetDistance;
    private SharedVector2 targetDirection;
    private SharedBool isTargetInSight;
    private SharedSpriteDirection spriteDirection;
    private Vector2 spriteSightDir;
    private float spriteSightAngle;

    void Awake()
    {
        _bt = GetComponent<BehaviorTree>();
    }

    void Start()
    {
        if(_bt != null)
        {
            sharedTarget = (SharedTransform)_bt.GetVariable("target");
            sharedState = (SharedAIState)_bt.GetVariable("state");
            // sightDirection = (SharedVector2)_bt.GetVariable("sightDirection");
            targetDistance = (SharedFloat)_bt.GetVariable("targetDistance");
            targetDirection = (SharedVector2)_bt.GetVariable("targetDirection");
            isTargetInSight = (SharedBool)_bt.GetVariable("isTargetInSight");
            spriteDirection = (SharedSpriteDirection)_bt.GetVariable("spriteDirection");

            StartCoroutine ("FindTargetsWithDelay", .2f);
        } else { Debug.LogWarning("TargetManager BT null"); }
    }

    void Update()
    {
        if(sharedState.Value == AIState.TargetFound)
        {
            Vector2 targetVec = (sharedTarget.Value.position - transform.position);
            targetDirection.Value = targetVec.normalized;
            targetDistance.Value = targetVec.magnitude;
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            FindTargets();
            yield return new WaitForSeconds(delay);
        }
    }

    void FindTargets()
    {
        if(_bt == null)
        {
            Debug.LogWarning("TargetManager BT null");
            return;
        }

        if(sharedState.Value == AIState.Dead)
        {
            this.enabled = false;
            return;
        }

        SetSpriteSight();   // 스프라이트 방향에 맞춰 시야 방향, 시야각 설정

        if(sharedState.Value == AIState.TargetFound)
        {
            // 아직 타겟에서 벗어나는 조건은 제대로 구현하지 못함
            // 일단 거리가 너무 멀어지면 벗어나는 것으로 간단하게 구현
            if(targetDistance.Value >= 100f)
            {
                UnSetTarget();
            }

            // * IsTargetInSight 제대로 작동하도록 수정 필요하나 일단 보류... *
            // if(Vector2.Angle(spriteSightDir, targetDirection.Value) < (spriteSightAngle / 2)) // 시야각 체크
            // {
            //     if(!Physics.Raycast(transform.position, targetDirection.Value, targetDistance.Value, obstacleMask)) // 장애물 체크
            //     {
            //         isTargetInSight.Value = true;
            //     }
            // } else { isTargetInSight.Value = false; }

            return;
        }

        Collider2D[] targetsInSightRadius = Physics2D.OverlapCircleAll(transform.position, sightRadius, targetMask);

        if(targetsInSightRadius.Length < 2) return;
        for(int i=0 ; i<targetsInSightRadius.Length ; i++)
        {
            if(targetsInSightRadius[i].transform == transform) { continue; } // Ignore self

            // 일시적으로 태그로 플레이어만 인식하도록 함
            // 플레이어를 제외한 다른 타겟도 인식하도록 추가해야 함
            // TeamID?
            if(targetsInSightRadius[i].transform.root.CompareTag("Gardener"))
            {
                Transform target = targetsInSightRadius[i].transform;
                Vector2 dirToTarget = (target.position - transform.position).normalized;

                if(Vector2.Angle(spriteSightDir, dirToTarget) < (spriteSightAngle / 2)) // 시야각 체크
                {
                    float distToTarget = Vector2.Distance(target.position, transform.position);
                    if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask)) // 장애물 체크
                    {
                        // foundTargets.Add(target);    // 여러 타겟을 인식할 때 사용 (아직 구현 못함)
                        SetTarget(target);
                        Debug.Log("Target found");
                    }
                }
            }
        }
    }

    void SetSpriteSight()
    {
        switch(spriteDirection.Value)
            {
                case SpriteDirection.BackLeft :
                case SpriteDirection.Left :
                case SpriteDirection.FrontLeft :
                    spriteSightDir = new Vector2(-1f, 0f);
                    spriteSightAngle = 180f;
                    break;
                case SpriteDirection.BackRight :
                case SpriteDirection.Right :  
                case SpriteDirection.FrontRight :
                    spriteSightDir = new Vector2(1f, 0f);
                    spriteSightAngle = 180f;  
                    break;
                default :
                    Debug.LogWarning("SpriteDirection error");
                    break;
		    }
    }

    public void SetTarget(Transform target)
    {
        if(sharedState.Value == AIState.Peace)
        {
            sharedTarget.Value = target;
            sharedState.Value = AIState.TargetFound;
            AddReactionEvent(target);
        }
    }

    void UnSetTarget()
    {
        DeleteReactionEvent(sharedTarget.Value);
        sharedState.Value = AIState.Peace;
        sharedTarget.Value = null;
    }

    /*
        이것들 모두 타겟을 보고 있을 때만
        실행되도록 할 수도 있을 것
        추가할지 말지는 나중에 결정
    */
    void AddReactionEvent(Transform target)
    {
        ReactionEventManager _reaction = target.root.GetComponent<ReactionEventManager>();

        switch(meleeAttackReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeMeleeAttack += AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeMeleeAttack += DodgeReactionEvent;
                break;
        }

        switch(rangeAttackReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeRangeAttack += AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeRangeAttack += DodgeReactionEvent;
                break;
        }

        switch(healingReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeHealing += AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeHealing += DodgeReactionEvent;
                break;
        }
    }

    void DeleteReactionEvent(Transform target)
    {
        ReactionEventManager _reaction = target.root.GetComponent<ReactionEventManager>();

        switch(meleeAttackReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeMeleeAttack -= AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeMeleeAttack -= DodgeReactionEvent;
                break;
        }

        switch(rangeAttackReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeRangeAttack -= AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeRangeAttack -= DodgeReactionEvent;
                break;
        }

        switch(healingReactionType)
        {
            case ReactionType.Attack :
                _reaction.noticeHealing -= AttackReactionEvent;
                break;
            case ReactionType.Dodge :
                _reaction.noticeHealing -= DodgeReactionEvent;
                break;
        }
    }

    public void AttackReactionEvent(float attackRange)
    {
        if(targetDistance.Value <= attackRange)
            _bt.SendEvent("AttackReaction");
    }

    public void AttackReactionEvent()
    {
        _bt.SendEvent("AttackReaction");
    }

    public void DodgeReactionEvent(float attackRange)
    {
        if(targetDistance.Value <= attackRange)
            _bt.SendEvent("DodgeReaction");
        // 방향 정보도 전달할 수 있어야 함
    }
    public void DodgeReactionEvent()
    {
        _bt.SendEvent("DodgeReaction");
        // 방향 정보도 전달할 수 있어야 함
    }
}