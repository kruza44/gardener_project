using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    지렁이나 채찍의 몸통 움직임을 위한 스크립트
    참고 영상 : https://youtu.be/9hTnlp9_wX8 (Tentacle)
        https://youtu.be/dnNCVcVS6uw (그래플링 훅)
*/
public class WormAndVine : MonoBehaviour
{
    [SerializeField] private float maxDistance;   // head가 움직일 수 있는 최대 거리
    [SerializeField] private float bodyDistance;   // 각 bodyPart 끼리 유지할 거리
    [SerializeField] private float smoothTime;    // 각 bodyPart가 목표 위치 도달에 걸리는 시간 (짧을수록 빠르게 움직인다)
    [SerializeField] private bool instantMove = false;  // 즉시 해당 위치로 움직이길 바라는 경우
    [SerializeField] private Transform[] bodyParts; // bodyParts[0]은 head
    [SerializeField] private Transform owner;    // 중심점 역할
    [Header("Hide When Close")]
    [SerializeField] private bool hideWhenClose  = false;   // Owner, Ball 등과 가까울 때 숨기기를 바라는 경우
    [SerializeField] private float hideDist = 0.001f;
    [HideInInspector] public Transform hideFrom; // 가까우면 숨길 대상 (타 스크립트에서 할당)
    [HideInInspector] public bool isFinishing;
    [HideInInspector] public bool maxReached;
    private int bodyCount;  // maxDistance에 따라 사용될 bodyParts의 수가 달라진다
    private Vector3[] currVelo;
    private Vector3[] partPos;

    void Awake()
    {
        // owner = GetComponentInParent<Transform>();
        currVelo = new Vector3[bodyParts.Length];
        SetMaxDistance(maxDistance);
    }

    void Start()
    {
        if(hideWhenClose)
        {
            for(int i = 1; i < bodyParts.Length; i++) // 머리를 제외한 bodyParts 비활성화
            {
                bodyParts[i].gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        for(int i = 1; i < bodyCount; i++)
        {
            Vector3 targetPos = bodyParts[i-1].position + (bodyParts[i].position - bodyParts[i-1].position).normalized * bodyDistance;
            if(instantMove)
            {
                bodyParts[i].position = targetPos;
            } else
            {
                bodyParts[i].position = Vector3.SmoothDamp(bodyParts[i].position, targetPos, ref currVelo[i], smoothTime);
            }

            if(hideWhenClose)
            {
                if((!isFinishing && (hideFrom.position - bodyParts[i].position).sqrMagnitude >= hideDist))
                {
                    bodyParts[i].gameObject.SetActive(true);    // owner와 특정 거리 이상 멀어져야 보이도록
                }

                if(isFinishing && (hideFrom.position - bodyParts[i].position).sqrMagnitude < hideDist)
                {
                    bodyParts[i].gameObject.SetActive(false);   // owner와 가까워지면 숨기도록 *상당히 어색함... 수정 필요*
                }

                if(maxReached && (bodyParts[0].position - bodyParts[i].position).sqrMagnitude < hideDist)
                {
                    bodyParts[i].gameObject.SetActive(false);   // WormHead와 가까워지면 숨기도록   * 구현 실패, 보류 *
                }
            }
        }
    }

    public void InitializePositions()  // 본 위치로 초기화
    {
        for(int i = 0; i < bodyCount; i++)
        {
            bodyParts[i].position = owner.position;
        }

        if(hideWhenClose)
        {
            for(int i = 1; i < bodyCount; i++) // 머리를 제외한 bodyParts 비활성화
            {
                bodyParts[i].gameObject.SetActive(false);
            }

            isFinishing = false;
            maxReached = false;
        }
    }

    public void SetMaxDistance(float max)
    {
        maxDistance = max;

        bodyCount = (int)(maxDistance / bodyDistance);
        for(int i = bodyCount; i < bodyParts.Length; i++) // 사용하지 않는 bodyparts 비활성화
        {
            bodyParts[i].gameObject.SetActive(false);
        }
    }
    
}
