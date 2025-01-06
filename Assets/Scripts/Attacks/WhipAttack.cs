using System.Collections;
using UnityEngine;
using DG.Tweening;

/*
    일반적인 채찍 공격
*/

/*
    0. Ball, Stem 위치 초기화
    1. 장애물 감지 : targetDirection과 whipRange 만큼 raycast
    2. Ball 위치 결정 : 장애물에 닿았는지 여부에 따라 달라짐
    3. Sprite 활성화, Ball 이동 : DOTween 사용하여 빠르고 부드럽게 이동
    4. 히트박스 활성화 : 우선은 하나의 히트박스만. 스윗스팟 추가는 후일 예정.
    5. 히트박스 비활성화 : AnimEvent 혹은 Animation 프레임에 따라 타이밍 결정
    6. Ball 회수 : DOTween으로 빠르고 부드럽게
    7. Sprite 비활성화
*/
public class WhipAttack : Attack
{
    public float whipRange; // 채찍 길이
    public GameObject weapon;   // 활성화, 비활성화 할 무기
    public Transform whipBall;  // 볼 이동을 위함
    // public Ease easeType; // 채찍 볼 DOTween 이동 타입

    public HitBox ballHitBox;   // 볼 히트박스
    public HitBox_CircleCast stemHitBox; // 줄기 히트박스

    public Vector2 ballHitBoxSize;
    public float stemHitBoxRadius;
    public float ballMoveDuration = 0.15f;  // 볼이 최종위치에 도달할 때까지 걸리는 시간
    public float ballHitBoxDuration;    // 볼 히트박스 지속시간
    public int staminaDecrease; // 스태미나 소비량

    private Vector2 finalBallPos;   // 볼의 최종 위치
    private float attackTimer;   // 타이머
    private PlayerStaminaManager _stamina;  // 플레이어 스태미나 매니저
    private WormAndVine vineScript;

    void Awake()
    {
        _stamina = this.transform.root.GetComponent<PlayerStaminaManager>();
        vineScript = whipBall.GetComponent<WormAndVine>();
    }
    void Start()
    {
        _stamina.attackStamina = staminaDecrease;   // 공격에 사용되는 스태미나 수치 미리 inform
        vineScript.SetMaxDistance(whipRange);
    }

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        base.DoAttack(targetDir, targetLoc);
        
        vineScript.SetMaxDistance(whipRange);
        _stamina.ReduceCurrentParam(staminaDecrease);

        // 사용하는 HitBox들에게 사용자 등록
        ballHitBox.SetHitBoxUser(this);
        stemHitBox.SetHitBoxUser(this);

        // 타이머 초기화
        attackTimer = 0f;

        // 장애물 감지 (현재 생략, 장애물 없다고 가정)
        // Ball 최종 위치 결정
        finalBallPos = (Vector2)transform.position + targetDirection * whipRange;

        // 활성화
        weapon.SetActive(true);

        // 히트박스 활성화
        ballHitBox.EnableHitBox();
        stemHitBox.EnableHitBox();

        // 볼 움직임 시작
        // whipBall.DOMove(finalBallPos, ballMoveDuration, false).SetEase(easeType);
        whipBall.position = finalBallPos;

        StartCoroutine("AttackSequence");
    }

    IEnumerator AttackSequence()
    {      
        while(true)
        {
            attackTimer += Time.deltaTime;

            // 히트박스 세팅
            ballHitBox.SetHitBox(whipBall.position, ballHitBoxSize);
            float dist = Vector2.Distance(transform.position, whipBall.position); // 볼이 나아간 거리
            stemHitBox.SetHitBox(this.transform.position, targetDirection, stemHitBoxRadius, dist);

            // 히트박스 업데이트
            ballHitBox.UpdateHitBox();  // 볼 히트박스를 먼저 처리해야 스윗스팟 구현이 가능함
            stemHitBox.UpdateHitBox();

            if(attackTimer >= 0.25f)  // 오브젝트(스프라이트) 비활성화
            {            
                // 비활성화
                weapon.SetActive(false);
                // Ball, Stem 위치 초기화
                vineScript.InitializePositions();
                yield break;
            } else if(attackTimer >= ballHitBoxDuration) // 히트박스 비활성화
            {
                ballHitBox.DisableHitBox();
                stemHitBox.DisableHitBox();
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
