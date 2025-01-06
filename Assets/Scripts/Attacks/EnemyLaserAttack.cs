using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserAttack : Attack
{
    // 방향은 호출자가 DoAttack 함수에 건네줘서 받아옴
    [SerializeField] private HitBox_CircleCast _hitBox;
    [SerializeField] private Transform startTransform;  // 레이저 시작 위치
    [SerializeField] private float hitBoxRadius;
    [SerializeField] private float hitBoxDuration = 0.3f;
    [SerializeField] private bool lineDebug;

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        _hitBox.SetHitBoxUser(this);
        
        base.DoAttack(targetDir, targetLoc);
        
        SetLaserHitBox(targetDirection, targetLocation);

        _hitBox.EnableHitBox();
        StartCoroutine(AttackHitBox());
    }

    public override void DetectedHurtBox(Collider2D other, Transform hitBoxTR)
    {
        // 호출자가 건네준 targetDirection으로 밀도록
        damageInfo.attackDirection = targetDirection;
        damageInfo.pushDirection = targetDirection;
        damageInfo.sightDirection = -targetDirection;

        // 상대 HurtBox 인터페이스 받아와서 GetHit 호출, DamageInfo 전달
        IHasHurtBox hurtBox = other.transform.root.GetComponent<IHasHurtBox>();
        hurtBox.GetHit(damageInfo);
    }

    void SetLaserHitBox(Vector2 dir, Vector2 endPosition)
    {
        float dist = Vector2.Distance(startTransform.position, endPosition);
        _hitBox.SetHitBox(startTransform.position, dir, hitBoxRadius, dist);
    }

    IEnumerator AttackHitBox()
    {
        float timer = 0;

        while(timer <= hitBoxDuration)
        {
            SetLaserHitBox(targetDirection, targetLocation);
            _hitBox.UpdateHitBox();
            yield return null;
            timer += Time.deltaTime;
        }

        _hitBox.DisableHitBox();
        yield break;
    }
}
