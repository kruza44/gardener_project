using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class LockOnManager : MonoBehaviour
{
    [SerializeField] private float maxSqrDistance = 250f; // 최대거리 넘어가면 락온 풀림
    [SerializeField] private LayerMask targetMask;  // HurtBox가 있는 오브젝트만 감지
    [SerializeField] private float deadzone = 0.4f;  // 게임패드 스틱 데드존 (스냅백 방지)
    private GameObject lockOnUI;   // 락온 상대를 따라다닐 UI
    private Transform lockOnTarget;
    private Camera _camera;
    private Vector2 axisDirection_R;
    private Vector2 tempDir;
    private bool axisInputLocked;   // 타겟 변경 후 스틱을 정위치에 위치해야 풀림
    private PlayMakerFSM fsm;
    private FsmVector2 lockOnDirection; // FSM variable
    private FsmVector2 sightDirection;  // FSM variable
    private FsmBool isLockOn;   // FSM variable
    private FsmBool isWormReadyState;    // FSM <WormReady> state에서는 R스틱 조작 중지
    private float lockOnSqrDist; // 락온한 상대와의 제곱거리

    void Awake()
    {
        fsm = GetComponent<PlayMakerFSM>();
        if(fsm == null) { Debug.LogWarning("LockOnManager fsm null"); }

        lockOnUI = GameObject.FindWithTag("LockOnUI");
        if(lockOnUI == null) { Debug.LogWarning("LockOnManager UI null"); }
        else { lockOnUI.SetActive(false); }
    }

    void Start()
    {
        _camera = Camera.main;

        lockOnDirection = fsm.FsmVariables.GetFsmVector2("lockOnDirection");
        sightDirection = fsm.FsmVariables.GetFsmVector2("sightDirection");
        isLockOn = fsm.FsmVariables.GetFsmBool("isLockOn");
        isWormReadyState = fsm.FsmVariables.GetFsmBool("isWormReadyState");
    }

    void Update()
    {
        if(Input.GetButtonDown("R_StickPress"))
        {
            if(!isLockOn.Value)
            {
                DoLockOn();
            }
            else
            {
                UnDoLockOn();
            }
        }

        if(isLockOn.Value)
        {
            lockOnSqrDist = (lockOnTarget.position - transform.position).sqrMagnitude;
            lockOnDirection.Value = (lockOnTarget.position - transform.position).normalized;
            
            if(lockOnSqrDist >= maxSqrDistance) // 락온 타겟이 너무 멀리 떨어지면 취소
            {
                UnDoLockOn();
            }

            if(isWormReadyState.Value == false) // FSM <WormReadyState>인 경우에는 R스틱 조작 불가
            {
                GetRStickAxis();
                if(axisDirection_R != Vector2.zero)
                {
                    ChangeTarget(axisDirection_R);  // R스틱 조작으로 타겟 변경
                }
            }
        }
    }

    public void DoLockOn()
    {
        List<Collider2D> targets = new List<Collider2D>();

        // 우선 스크린 전체를 감지
        Vector2 pointA = _camera.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 pointB = _camera.ScreenToWorldPoint(new Vector2(_camera.pixelWidth, _camera.pixelHeight));
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, targetMask);

        foreach(Collider2D col in colliders)
        {
            if(col.transform.root.CompareTag("Gardener")) { continue; } // Ignore self

            targets.Add(col);
        }

        // sightDirection과 거리에 따라 최우선 락온 순위가 결정됨
        if(targets.Count >= 1)  // 무언가 잡혔다면
        {
            float[] dotAndDist = new float[targets.Count];
            int maxIndex = 0;
            for(int i=0; i < targets.Count; i++)
            {
                // (DotProduct / SqrDistance)
                Vector2 targetDir = (targets[i].transform.position - this.transform.position).normalized;
                float targetSqrDist = Vector2.SqrMagnitude(targets[i].transform.position - this.transform.position);
                dotAndDist[i] = (Vector2.Dot(sightDirection.Value, targetDir) / targetSqrDist);
                if(i != 0)
                {
                    if(dotAndDist[i] < 0)
                    {
                        dotAndDist[i] = dotAndDist[i] * (-1f/5000f);    // 반대 방향에 있을 시
                    }
                    if(dotAndDist[i] > dotAndDist[i-1])
                    {
                        maxIndex = i;
                    }
                }
            }
            lockOnTarget = FindCenterTransform(targets[maxIndex]);
            CameraManager.Instance.lockOnTarget = lockOnTarget;
            isLockOn.Value = true;
            lockOnTarget.root.GetComponent<NonPlayerUnit>().isLockedOn = true;

            // 락온 UI 표시
            StartCoroutine(UIFollowTarget());
        }
    }

    public void UnDoLockOn()
    {
        StopAllCoroutines();
        lockOnTarget.root.GetComponent<NonPlayerUnit>().isLockedOn = false;
        lockOnTarget = null;
        CameraManager.Instance.lockOnTarget = null;
        lockOnUI.SetActive(false);  // UI 비활성화
        isLockOn.Value = false;
    }

    IEnumerator UIFollowTarget()
    {
        lockOnUI.SetActive(true);

        while(true)
        {
            if(lockOnTarget != null)
            {
                lockOnUI.transform.position = lockOnTarget.position;
            }
            else { Debug.LogWarning("LockOnTarget Null!!"); }

            yield return new WaitForFixedUpdate();
        }
    }

    // R스틱 조작 방향에 가장 가까운 타겟으로 변경 
    void ChangeTarget(Vector2 changeDir)
    {
        if(lockOnTarget != null)
        {
            List<Collider2D> targets = new List<Collider2D>();

            // 우선 스크린 전체를 감지
            Vector2 pointA = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            Vector2 pointB = _camera.ScreenToWorldPoint(new Vector2(_camera.pixelWidth, _camera.pixelHeight));
            Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, targetMask);
            

            foreach(Collider2D col in colliders)
            {
                Transform root = col.transform.root;
                if(root.CompareTag("Gardener")) { continue; } // Ignore self
                if(root == lockOnTarget)    { continue; }   // Igonre already locked on target

                Vector2 targetDir = (col.transform.position - this.transform.position).normalized;
                targets.Add(col);
            }

            // changeDir에 가장 근접한 대상을 추려내기 위함 (가장 큰 것)
            if(targets.Count >= 1)  // 무언가 잡혔다면
            {
                float[] dotAndDist = new float[targets.Count];
                int maxIndex = 0;
                for(int i=0; i < targets.Count; i++)
                {
                    // (DotProduct / SqrDistance)
                    Vector2 targetDir = (targets[i].transform.position - lockOnTarget.position).normalized;
                    float targetSqrDist = Vector2.SqrMagnitude(targets[i].transform.position - lockOnTarget.position);
                    dotAndDist[i] = (Vector2.Dot(changeDir, targetDir) / targetSqrDist);
                    if(i != 0)
                    {
                        if(dotAndDist[i] < 0)
                        {
                            dotAndDist[i] = dotAndDist[i] * (-1f/5000f);    // 반대 방향에 있을 시
                        }
                        if(dotAndDist[i] > dotAndDist[i-1])
                        {
                            maxIndex = i;
                        }
                    }
                }
                lockOnTarget.root.GetComponent<NonPlayerUnit>().isLockedOn = false;  // 이전 타겟

                lockOnTarget = FindCenterTransform(targets[maxIndex]); // 타겟 변경
                CameraManager.Instance.lockOnTarget = lockOnTarget;
                lockOnTarget.root.GetComponent<NonPlayerUnit>().isLockedOn = true;   // 새로운 타겟
                axisInputLocked = true;
            }
        }
    }

    void GetRStickAxis()
    {
        // Input 컨트롤러 R_스틱 받아오기
        tempDir.x = Input.GetAxisRaw("R_Horizontal");
        tempDir.y = Input.GetAxisRaw("R_Vertical");

        if(tempDir.magnitude < deadzone) { tempDir = Vector2.zero; }    // 데드존
        tempDir.Normalize();

        // 인풋 잠긴 경우 스틱을 (0,0)에 위치해야 풀림
        if(axisInputLocked)
        {
            if(tempDir == Vector2.zero)
            {
                axisInputLocked = false;
            }
            else
            {
                axisDirection_R = Vector2.zero;
            }

            return;
        }

        axisDirection_R = tempDir;
    }

    // Sprite의 Pivot이 발에 있는 관계로
    // Center을 찾도록 하는 함수
    // 비효율적... 고쳐야할 것 같음
    Transform FindCenterTransform(Collider2D target)
    {
        Transform[] trs = target.transform.root.GetComponentsInChildren<Transform>();
        
        foreach(Transform tr in trs)
        {
            if(tr.CompareTag("Center"))
            {
                return tr;
            }
        }

        Debug.LogWarning("Couldn't find center");
        return target.transform;
    }
}