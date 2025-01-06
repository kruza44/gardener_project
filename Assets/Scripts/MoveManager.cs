using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Script Execution Order 설정을 통해
    마지막에 실행되어 최종적으로 move 벡터값을 취합할 수 있도록 했음
*/
public class MoveManager : MonoBehaviour
{
    [HideInInspector] public Vector2 previousMove;   // 초기화 되기 전의 속도 (점프 등에서 사용)
    private Vector2 move;
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    [SerializeField] private LayerMask mask;    // Wall
    [HideInInspector] public bool wallDetected;  // 벽 통과 방지
    private Vector2 wallHitPos;    // 벽 충돌 지점
    private Unit _unit; // WallSlammed 발동시키기 위함
    private int objectID;    // Boxcast가 무시하도록 하기 위해 필요
    // private Wall slammedWall;   // 벽쿵한 벽 컴포넌트

    void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _col = this.GetComponent<CircleCollider2D>();
        _unit = this.GetComponent<Unit>();

        objectID = this.gameObject.GetInstanceID();
    }

    // void Start()
    // {
    //     StartCoroutine(Move());
    // }

    void FixedUpdate()
    {
        if(!wallDetected)
        {
            _rb.MovePosition(_rb.position + move * Time.fixedDeltaTime);
        } else  // CheckWallMove에서 벽 감지한 경우 통과하지 않도록 충돌 지점으로 이동
        {
            wallDetected = false;
            _rb.MovePosition(wallHitPos);
            // slammedWall.WallCrumbleEffect();
            // _unit.WallSlammed(slammedWall.wallDmgInfo);
        }
        
        previousMove = move;
        move = Vector2.zero;    // 지속적으로 0으로 초기화
    }

    public void AddMove(Vector2 m)
    {
        // 여러 스크립트에서 받아온 움직임 벡터 값을 취합한다
        move += m;
    }

    public bool CheckWallMove(Vector2 m)
    {
        float colSize = _col.radius;
        Vector2 colOffset = _col.offset;
        Vector2 step = m * Time.fixedDeltaTime; // 다음에 이동할 벡터

        // Owner 콜라이더 무시하도록 하면 딱 달라붙어 있는 콜라이더도 무시하는 문제 때문에 이런 방식으로 구현
        RaycastHit2D[] hits = Physics2D.CircleCastAll(_rb.position + _col.offset, colSize, m.normalized, step.magnitude, mask);

        if(hits.Length >= 2)    // 벽이나 Unit 감지한 경우
        {
            Debug.DrawLine(hits[1].centroid, hits[1].point, Color.blue, 10f);

            // 벽면과 25도 이하 각도라면 벽쿵 무시
            if(Vector2.Dot(hits[1].normal, -m.normalized) <= 0.42f)
            {
                Debug.Log(Vector2.Dot(hits[1].normal, -m.normalized));
                Debug.Log(hits[1].collider.gameObject);
                AddMove(m);
                return false;
            }

            // 벽 통과하지 않도록 충돌 위치로 이동
            wallHitPos = hits[1].centroid - colOffset;
            
            if(LayerMask.LayerToName(hits[1].collider.gameObject.layer) == "Wall")  // 벽 충돌
            {
                Wall slammedWall = hits[1].collider.GetComponent<Wall>();
                if(slammedWall == null) { Debug.Log("**Wall null error**"); }

                slammedWall.WallCrumbleEffect();
                _unit.WallSlammed(slammedWall.wallDmgInfo);
            }
            else    // Unit 충돌
            {
                Unit slammedUnit = hits[1].collider.transform.root.GetComponent<Unit>();
                if(slammedUnit == null) { Debug.Log("**Wall Unit null error**"); }

                // 약화된 pushPower, poiseDamage, staminaDamage
                DamageInfo unitBashedInfo = _unit.shieldBashedInfo;
                unitBashedInfo.pushPower *= 0.8f;
                unitBashedInfo.poiseDamage = (int)(unitBashedInfo.poiseDamage * 0.8f);
                unitBashedInfo.staminaDamage = (int)(unitBashedInfo.staminaDamage * 0.8f);
                
                // 부딫힌 Unit
                Debug.Log(slammedUnit);
                slammedUnit.shieldBashedInfo = unitBashedInfo;  // DamageInfo 전달
                slammedUnit.GetHit(unitBashedInfo);
                slammedUnit.Shake(0.3f);
                slammedUnit.GetComponent<HitStop>().StartHitStop(0.2f);

                // 자신은 wallSlammed
                DamageInfo unitSlammedInfo = _unit.shieldBashedInfo;
                unitSlammedInfo.baseDamage = 60;    // 기본 벽쿵 데미지
                unitSlammedInfo.pushDirection = -m.normalized;  // 튕겨져 나가도록
                unitSlammedInfo.pushPower = 7f;   // 반동 세기
                unitSlammedInfo.doWallSlam = false;
                unitSlammedInfo.doNonStunPush = false;
                _unit.GetComponent<HitStop>().StartHitStop(0.2f);
                _unit.WallSlammed(unitSlammedInfo); // * 스프라이트 변화 없이 hitstop 될 것 같다... 고쳐야 함 *
            }

            return true;    // CheckWallSalmMove BT task에 전달
        }
        else
        {
            AddMove(m);
            return false;
        }
    }

    // IEnumerator Move()
    // {
    //     while(true)
    //     {
    //         yield return new WaitForFixedUpdate();

    //         if(!wallDetected)
    //         {
    //             _rb.MovePosition(_rb.position + move * Time.fixedDeltaTime);
    //         } else  // CheckWallMove에서 벽 감지한 경우 통과하지 않도록 충돌 지점으로 이동
    //         {
    //             wallDetected = false;
    //             yield return new WaitForFixedUpdate();
    //             _rb.MovePosition(wallHitPos);
    //             slammedWall.WallCrumbleEffect();
    //             _unit.WallSlammed(slammedWall.wallDmgInfo);
    //         }
    //         move = Vector2.zero;    // 지속적으로 0으로 초기화

    //         yield return null;
    //     }
    // }
}
