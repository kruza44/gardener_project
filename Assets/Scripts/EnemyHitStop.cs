using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using DG.Tweening;

public class EnemyHitStop : HitStop
{
    [SerializeField] private BehaviorTree _bt;

    protected override void PauseThings()
    {
        _bt.DisableBehavior(true);
        _animator.speed = 0;
        DOTween.Pause(tweenID);
    }

    protected override void PlayThings()
    {
        _bt.EnableBehavior();
        _animator.speed = prevSpeed;
        DOTween.Play(tweenID);
    }
}
