using System.Collections;
using UnityEngine;
using System;   // GUID
using DG.Tweening;

/*
    2. WormReady 중 지렁이 얼굴 보이고 움직이도록 해야 함
    3. 지렁이 방향에 따라 얼굴 방향도 바뀌도록 해야 함
    4. 
*/

public enum ShieldBashImpact
{
    Weak, Adequate, Strong   // 거리에 따라 실드배시 강도가 달라진다
}

public class Worm : MonoBehaviour
{
    [HideInInspector] public bool isFinished = true; // FSM에 현 state가 끝났음을 알림
    [Header("Required fields")]
    [SerializeField] private GameObject worm;   // 활성, 비활성화 시킬 대상
    [SerializeField] private Transform wormHead;   // 이동시킬 지렁이 머리
    [SerializeField] private GameObject owner;    // 이동시킬 주인
    [SerializeField] private LayerMask mask;    // 지렁이 머리가 감지할 대상 (Pushbox)
    [SerializeField] private float maxDistance;   // 지렁이가 움직일 수 있는 최대 거리
    [SerializeField] private Vector2 boxCastSize = new Vector2(0.25f, 0.25f);
    [SerializeField] private Ease easeType; // DOTween 이동 타입, Owner가 이동할 때 쓰임 *Animation curve로 대체?*
    [SerializeField] private float wormMoveSpeed = 4f;  // 지렁이 이동 속도
    [SerializeField] private float ownerMoveDuration = 1f;    // 플레이어 이동 시간
    [Header("Required Stamina")]
    [SerializeField] private float wormStamina = 20f;    // Worm 스태미나 소비량
    [SerializeField] private float shieldBashStamina = 40f;    // 실드배시 스태미나 소비량
    [Header("Test parameters")]
    [SerializeField] private float recoilDuration;  // 지렁이가 목표물에 닿은 후 잠깐 멈추는 시간
    [SerializeField] private float recoilSpeed;
    [SerializeField] private AnimationCurve wormSpeedCurve;
    [SerializeField] private float offsetDistance = 0.6f;  // Owner가 최종 위치에 도착했다고 판단할 offset 거리
    [SerializeField] private float wormMaxTime = 0.3f;  // 지렁이가 최대 거리에 도달하는 시간
    // [SerializeField] private float wormTotalTime = 0.5f;    // 지렁이 발사 후 복귀까지 걸리는 총 시간 (AnimationCurve에 time을 반환하는 기능이 없음)
    [Header("Inertia Test")]
    [SerializeField] private float inertiaSpeed;
    [SerializeField] private float inertiaMinSpeed;
    [SerializeField] private float inertiaTime;
    [Header("Shield Bash Test")]
    [SerializeField] private DamageInfo shieldBashInfo;
    private ShieldBashImpact shieldBashImpact;
    [SerializeField] private float hitRecoilTime;   // 실드 배시 후 반동 조작불가 시간
    [SerializeField] private float weakDist;    // 실드 배시가 제대로 발동하지 않는 최대 거리
    [SerializeField] private float adeqDist;    // 적절한 실드 배시가 발동하는 최대 거리 (넘어가면 강한 실드 배시가 발동됨)
    [HideInInspector] public bool isShieldBash;  // true 시 실드 배시 발동
    [Header("Game Feel Test Parameters")]
    [SerializeField] private float shakeTime;
    [SerializeField] private float hitStopTime = 0.2f;
    private Rigidbody2D ownerRB;
    private IEnumerator wormCoroutine;
    private WormAndVine wormScript;
    private MoveManager _move;
    private PlayerStaminaManager _stamina;  // 스태미나 감소, 체크
    private string tweenID; // 일단 Worm instanceID로... (Gardener가 아님)

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.yellow;
        // // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        // Gizmos.DrawWireCube(wormHead.transform.position, new Vector3(boxCastSize.x, boxCastSize.y, 1f));
    }

    void Awake()
    {
        ownerRB = owner.GetComponent<Rigidbody2D>();
        _move = owner.GetComponent<MoveManager>();
        wormScript = wormHead.GetComponent<WormAndVine>();
        _stamina = owner.GetComponent<PlayerStaminaManager>();

        tweenID = this.gameObject.GetInstanceID().ToString();
    }

    void Start()
    {
        wormScript.SetMaxDistance(maxDistance);
        wormScript.InitializePositions();   // 지렁이 위치 초기화
    }

    public void DoWorm(Vector2 targetDirection)
    {
        isFinished = false;
        wormScript.InitializePositions();   // 지렁이 위치 초기화
        wormScript.hideFrom = this.transform;   // 중심점에 너무 가까우면 숨김
        worm.SetActive(true);   // 지렁이 활성화

        Vector3 targetPos = this.transform.position + (Vector3)targetDirection * maxDistance;  // 지렁이가 목표로 할 최종 지점
        wormCoroutine = WormSequence(targetPos);
        StartCoroutine(wormCoroutine);
    }

    IEnumerator WormSequence(Vector3 targetPos)
    {
        _stamina.ReduceCurrentParam(wormStamina);

        float wormMoveTimer = 0f;
        float recoilTimer = 0f;
        float tempSpeed = wormMoveSpeed;   // 변화하는 worm 움직임 속도

        Collider2D hitCol = null;
        Vector2 hitPosition = Vector2.zero;
        Vector2 hitCenter = Vector2.zero;
        bool isHit = false;

        while(true)
        {
            wormMoveTimer += Time.fixedDeltaTime;

            Vector3 nextPos = this.transform.position + (targetPos - this.transform.position) * wormSpeedCurve.Evaluate(wormMoveTimer);

            // 다음에 있을 위치를 미리 boxcast하여 벽 통과 방지
            if(wormMoveTimer <= wormMaxTime)    // 지렁이가 복귀하는 동안에는 충돌 감지 x
            {
                Vector3 step = nextPos - wormHead.position;
                RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)wormHead.position, boxCastSize, 0f, step.normalized, step.magnitude, mask);
                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.transform.root.CompareTag("Gardener")) { continue; }  // 플레이어 PushBox 무시
                    hitCol = hit.collider;  // 실드 배시에 사용됨
                    hitPosition = hit.point;    // 최초로 감지된 point 저장 후 탈출
                    hitCenter = hit.centroid;   // BoxCast 중심점
                    isHit = true;
                    break;
                }
            }
            else    // 지렁이가 복귀하는 동안
            {
                // * 현재는 구현 보류, 구현 시 복귀하는 동안은 플레이어가 자유롭게 행동할 수 있도록 (<Worm> 제외) *
                wormScript.maxReached = true;
            }

            if(!isHit)
            {
                wormHead.position = nextPos;
            }
            else   // 충돌 감지 시
            {
                // Owner와 worm Head 사이 거리 측정 (소 중 대)
                // ** 실드 배시 차지 효과음, 이펙트 등 고려하면 owner 이동거리를 측정하면서 차츰 바꿔가는 형태로 수정 필요함 **
                float headDist = (wormHead.position - this.transform.position).magnitude;
                if(headDist <= weakDist) { shieldBashImpact = ShieldBashImpact.Weak; }
                else if(headDist > weakDist && headDist <= adeqDist) { shieldBashImpact = ShieldBashImpact.Adequate; }
                else { shieldBashImpact = ShieldBashImpact.Strong; }

                float temp = recoilSpeed; 
                wormHead.position = hitCenter;
                isHit = false;

                // Owner 반동
                Vector2 recoilDir = ((Vector2)this.transform.position - (Vector2)targetPos).normalized;
                DOTween.To(()=> temp, x=> tempSpeed = x, 0.5f, recoilDuration/2f).SetEase(easeType).SetId(tweenID);
                while(recoilTimer <= recoilDuration)
                {
                    // LB 인풋 받아서 실드 배시 준비 
                    if(Input.GetButton("LB_Button"))
                    {
                        if(_stamina.CheckStaminaAvailable(shieldBashStamina))
                        {
                            isShieldBash = true;
                        }
                    }
                    recoilTimer += Time.fixedDeltaTime;
                    _move.AddMove(recoilDir * temp);
                    yield return new WaitForFixedUpdate();
                }

                wormScript.isFinishing = true;  // 지렁이 body에 owner가 가까워지면 숨기도록

                // 플레이어 이동 시작
                Vector2 movePosition = (Vector2)this.transform.position;
                DOTween.To(()=> movePosition, x=> movePosition = x, (Vector2)wormHead.position, ownerMoveDuration).SetEase(easeType).SetId(tweenID);
                while(true)
                {
                    _move.AddMove((movePosition - (Vector2)this.transform.position) * (1f/Time.fixedDeltaTime));    // ownerMoveDuration 안에 이동하도록

                    if(Vector2.Distance((Vector2)this.transform.position, wormHead.position) <= offsetDistance) // 위치 도착 시 종료 *플레이어 벽 끼임 문제 개선 필요*
                    {
                        Vector2 dir;    // 관성 or 반동 방향

                        if(isShieldBash)    // !! 실드배시 모드!!
                        {

                            if(LayerMask.LayerToName(hitCol.gameObject.layer) == "Wall") // 레이어가 Wall 이라면
                            {
                                // 실드배시 (벽 관련)
                                switch(shieldBashImpact)
                                {
                                    case ShieldBashImpact.Weak:
                                        //
                                        break;
                                    case ShieldBashImpact.Adequate:
                                    case ShieldBashImpact.Strong:
                                        _stamina.ReduceCurrentParam(shieldBashStamina); // 스태미나 소비
                                        
                                        CameraManager.Instance.ShakeCamera(shakeTime);
                        
                                        // ** 플레이어 HitStop 구현이 힘들어 임시 조치 **
                                        yield return new WaitForSeconds(hitStopTime);
                                        break;
                                } 
                            } else
                            {
                                // 실드배시 (유닛 관련)
                                switch(shieldBashImpact)
                                {
                                    case ShieldBashImpact.Weak:
                                        break;
                                    case ShieldBashImpact.Adequate:
                                    case ShieldBashImpact.Strong:
                                        _stamina.ReduceCurrentParam(shieldBashStamina); // 스태미나 소비
                                        
                                        CameraManager.Instance.ShakeCamera(shakeTime);  // 카메라 흔들기

                                        SetShieldBashDirection((Vector2)hitCol.transform.position);   // DamageInfo 방향 설정

                                        Unit bashedUnit = hitCol.transform.root.GetComponentInChildren<Unit>();
                                        bashedUnit.GetHit(shieldBashInfo);
                                        bashedUnit.shieldBashedInfo = shieldBashInfo;   // Unit-Unit 간 충돌에 필요
                                        bashedUnit.Shake(0.3f);    // 스프라이트 흔들기
                                        bashedUnit.GetComponent<HitStop>().StartHitStop(hitStopTime);   // 적 HitStop

                                        // ** 플레이어 HitStop 구현이 힘들어 임시 조치 **
                                        yield return new WaitForSeconds(hitStopTime);
                                        break;
                                } 
                            }
                            // 반대 방향으로 반동(inertia 코루틴 사용)
                            dir = ((Vector2)this.transform.position - (Vector2)wormHead.position).normalized;
                        } else  // !!일반 worm 모드!!
                        {          
                            // 관성 방향              
                            dir = ((Vector2)wormHead.position - (Vector2)this.transform.position).normalized;
                        }
                        
                        StartCoroutine(OwnerInertia(dir));   
                        StartCoroutine(EndWormSequence());
                        yield break;            
                    }

                    yield return new WaitForFixedUpdate();
                }
            }

            if(wormMoveTimer > wormMaxTime)   // * 일단은 복귀 없이 최대 거리 도달 시 끝나는 것으로 *
            {
                StartCoroutine(EndWormSequence());
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator EndWormSequence()
    {
        StopCoroutine(wormCoroutine);
        DOTween.Kill(tweenID);
        // DOTween.Kill("Recoil"); // Recoil 중단

        /*
            지렁이가 주인 품으로 돌아오도록 추가해야 함(안 돌아오는 경우도 있으므로 나눠야 함)
        */
        worm.SetActive(false);  // 지렁이 비활성화

        if(isShieldBash)
        {
            isShieldBash = false;
            yield return new WaitForSeconds(hitRecoilTime); // 실드 배시 후 잠시 조작불가
        }

        isFinished = true;  // FSM에 끝났음을 알림
        yield break;
    }

    IEnumerator OwnerInertia(Vector2 targetDir)  // 목표 지점에 도달 후 잠시 동안 약한 관성 유지
    {
        float timer = 0f;
        float moveSpeed = inertiaSpeed;

        DOTween.To(()=> moveSpeed, x=> moveSpeed = x, inertiaMinSpeed, inertiaTime).SetEase(Ease.OutCubic);

        while(timer <= inertiaTime)
        {
            timer += Time.fixedDeltaTime;
            _move.AddMove(targetDir * moveSpeed);            
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public void SetWormMaxDistacne(float max)
    {
        maxDistance = max;
        wormScript.SetMaxDistance(maxDistance);
    }

    void SetShieldBashDirection(Vector2 otherPos)
    {
        // GUID 생성
        shieldBashInfo.attackID = Guid.NewGuid();

        // 밀치는 방향으로 설정
        Vector2 dir = (Vector2)(wormHead.position - this.transform.position).normalized;
        shieldBashInfo.attackDirection = dir;
        shieldBashInfo.pushDirection = dir;
        shieldBashInfo.sightDirection = -dir;
    }
}
