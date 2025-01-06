using System;
using UnityEngine;

public class Attack : MonoBehaviour, IHitBoxUser
{
    [SerializeField] protected DamageInfo damageInfo;

    protected Vector2 targetDirection;
    protected Vector2 targetLocation;

    virtual public void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        targetDirection = targetDir;
        targetLocation = targetLoc;

        damageInfo.attackID = Guid.NewGuid();
    }

    virtual public void DetectedHurtBox(Collider2D other, Transform hitBoxTR)
    {
        // DamageInfo 세팅 (기본 세팅 : 평범한 밀치기)
        Vector2 dir = (other.transform.position - damageInfo.causer.transform.position).normalized;
        damageInfo.attackDirection = dir;
        damageInfo.pushDirection = dir;
        damageInfo.sightDirection = -dir;

        // 상대 HurtBox 인터페이스 받아와서 GetHit 호출, DamageInfo 전달
        IHasHurtBox hurtBox = other.transform.root.GetComponent<IHasHurtBox>();
        hurtBox.GetHit(damageInfo);
    }
}
