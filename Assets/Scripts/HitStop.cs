using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    protected string tweenID;
    protected Coroutine cor;
    protected float prevSpeed;    // 디폴트 애니메이터 속도

    virtual protected void Awake()
    {
        // 트윈 ID 할당
        tweenID = this.gameObject.GetInstanceID().ToString(); // 트윈 ID 할당
    }

    virtual public void StartHitStop(float duration)
    {
        if(cor != null) { StopCoroutine(cor); }

        cor = StartCoroutine(HitStopCoroutine(duration));
    }

    // *** 아직 어떻게 제대로 써야할지 생각 못함 ***
    virtual public void InterruptHitStop()
    {
        if(cor != null)
        {
            StopCoroutine(cor);
            PlayThings();   // HitStop 중지
        }
    }

    IEnumerator HitStopCoroutine(float duration)
    {
        yield return null;  // 애니메이터 transition 기다리기 위함

        prevSpeed = _animator.speed;

        // 일시정지
        PauseThings();

        yield return new WaitForSeconds(duration);

        // 재생
        PlayThings();

        cor = null;
        yield break;
    }

    virtual protected void PauseThings()
    {
        // 상속에서 구현 (플레이어 / 적 구분)
    }

    virtual protected void PlayThings()
    {
        // 상속에서 구현
    }

}
