using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCircleSpreadAttack : Attack
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private int projectileCount;

    private HitBox[] _hitBoxes;

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        // Count 만큼 direction 생성
        Vector2[] dirs = new Vector2[projectileCount];
        dirs[0] = Vector2.up;
        for(int i=1; i < projectileCount; i++)
        {
            // projectileCount 수 만큼 등분 함
            dirs[i] = Quaternion.AngleAxis(360/projectileCount, Vector3.forward) * dirs[i-1];
        }

        // Count 만큼 투사체 생성, 방향 지정, user 설정
        for(int i=0; i < projectileCount; i++)
        {
            GameObject p = Instantiate(projectile, this.transform.root.position, transform.rotation);
            p.GetComponent<Projectile_LinearMove>().SetProjectile(dirs[i], this);
        }

        base.DoAttack(targetDir, targetLoc);
    }

    public override void DetectedHurtBox(Collider2D other, Transform hitBoxTR)
    {
        // 투사체 별 위치에 따라 밀리도록
        Vector2 dir = (other.transform.position - hitBoxTR.position).normalized;
        damageInfo.attackDirection = dir;
        damageInfo.pushDirection = dir;
        damageInfo.sightDirection = -dir;

        // 상대 HurtBox 인터페이스 받아와서 GetHit 호출, DamageInfo 전달
        IHasHurtBox hurtBox = other.transform.root.GetComponent<IHasHurtBox>();
        hurtBox.GetHit(damageInfo);
    }
}
