using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using DG.Tweening;

public class PlayerHitStop : HitStop
{
    private PlayMakerFSM _fsm;

    protected override void Awake()
    {
        base.Awake();
        
        _fsm = this.GetComponent<PlayMakerFSM>();
        // ** fsm을 일시정지 할 방법이... **
    }

    protected override void PauseThings()
    {
        _animator.speed = 0;
        DOTween.Pause(tweenID);
    }

    protected override void PlayThings()
    {
        _animator.speed = prevSpeed;
        DOTween.Play(tweenID);
    }
}
