using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using HutongGames.PlayMaker;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Shake")]
    public float shakeIntensity = 3.5f;    // 카메라 흔들리 세기
    [HideInInspector] public Transform lockOnTarget = null;
    [Header("Sight Offset")]
    [SerializeField] float sightOffsetDistance;
    [SerializeField] float maxOffsetDistance;
    [SerializeField] float lockOnSmoothTime; // 카메라가 최종 위치까지 도달하는 데 걸리는 시간
    [SerializeField] float sightSmoothTime;
    private CinemachineVirtualCamera vCam;   // 메인 카메라
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    private CinemachineCameraOffset _offset;    // 락온 등 카메라 움직임을 위한 offset
    private Coroutine shakeShake;
    private PlayMakerFSM _fsm;
    private FsmVector2 sightDirection;
    private Vector3 smoothVel;  // 카메라 움직임 smoothDamp용

    // * R 스틱 카메라 이동은 나중에 추가 예정 *

    // 싱글턴
    private static CameraManager instance = null;

    public static CameraManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }

        vCam = GameObject.FindGameObjectWithTag("LiveCamera").GetComponent<CinemachineVirtualCamera>();
        channelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _offset = vCam.GetComponent<CinemachineCameraOffset>();

        _fsm = GameObject.FindGameObjectWithTag("Gardener").GetComponent<PlayMakerFSM>();
    }

    void Start()
    {
        sightDirection = _fsm.FsmVariables.GetFsmVector2("sightDirection");

        StartCoroutine("CameraOffset");
    }

    public void ShakeCamera(float time)
    {
        DOTween.Kill("Shake");
        if(shakeShake != null) { StopCoroutine(shakeShake); }

        // channelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        channelPerlin.m_AmplitudeGain = shakeIntensity;
        shakeShake = StartCoroutine(ShakeCoroutine(time));
    }

    IEnumerator ShakeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        float temp = shakeIntensity;

        // 서서히 카메라 흔들림 중지
        DOTween.To(()=> temp, x=> temp = x, 0, 0.2f).SetEase(Ease.OutCubic).SetId("Shake");
        while(temp != 0)
        {
            channelPerlin.m_AmplitudeGain = temp;
            yield return null;
        }

        yield break;
    }

    IEnumerator CameraOffset()
    {
        Vector3 cameraPosition;
        Vector3 tempOffset;

        while(true)
        {
            if(lockOnTarget != null)    // 락온 타겟을 향해 바라봄
            {
                cameraPosition = (lockOnTarget.position + vCam.Follow.position) / 2f;   // 중간지점
                tempOffset = cameraPosition - vCam.Follow.position;
                Vector3 currentOffset = _offset.m_Offset;
                Vector3 nextOffset = Vector3.ClampMagnitude(tempOffset, maxOffsetDistance);
                _offset.m_Offset = Vector3.SmoothDamp(currentOffset, nextOffset, ref smoothVel, lockOnSmoothTime);
                yield return null;
            }
            else    // 락온 타겟이 없다면 sightDirection 바라봄
            {
                Vector3 currentOffset = _offset.m_Offset;
                Vector3 nextOffset = sightDirection.Value * sightOffsetDistance;
                 _offset.m_Offset = Vector3.SmoothDamp(currentOffset, nextOffset, ref smoothVel, sightSmoothTime);
                yield return null;
            }
        }
    }
}
