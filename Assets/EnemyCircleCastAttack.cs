using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class EnemyCircleCastAttack : Attack
{
    [SerializeField] private HitBox_CircleCast _hitBox;
    [SerializeField] private Vector2 hitBoxCenterOffset;
    [SerializeField] private float hitBoxRadius;
    [SerializeField] private float hitBoxDistance;
    [SerializeField] private Vector2 leftDirection;
    [SerializeField] private Vector2 leftUpDirection;
    [SerializeField] private Vector2 leftDownDirection;
    [SerializeField] private float hitBoxDuration = 0.3f;
    [SerializeField] private bool lineDebug;
    private SharedSpriteDirection spriteDirection;

    void Start()
    {
        BehaviorTree bt = this.transform.root.GetComponent<BehaviorTree>();
        spriteDirection = (SharedSpriteDirection)bt.GetVariable("spriteDirection");

        leftDirection.Normalize();
        leftUpDirection.Normalize();
        leftDownDirection.Normalize();

        // _hitBox.SetHitBoxUser(this);
    }

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        _hitBox.SetHitBoxUser(this);
        
        base.DoAttack(targetDir, targetLoc);
        
        SetHitBoxByDirection();

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

    void SetHitBoxByDirection()
    {
        switch(spriteDirection.Value)
        {
            case SpriteDirection.BackLeft :
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, leftUpDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            case SpriteDirection.BackRight :
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, -leftDownDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            case SpriteDirection.Left :
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, leftDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            case SpriteDirection.Right :  
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, -leftDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            case SpriteDirection.FrontLeft :
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, leftDownDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            case SpriteDirection.FrontRight :  
                _hitBox.SetHitBox((Vector2)transform.position + hitBoxCenterOffset, -leftUpDirection,
                hitBoxRadius, hitBoxDistance);
                break;
            default :
                Debug.LogWarning("SpriteDirection error");
                break;
        }
    }

    IEnumerator AttackHitBox()
    {
        float timer = 0;

        while(timer <= hitBoxDuration)
        {
            SetHitBoxByDirection();
            _hitBox.UpdateHitBox();
            yield return null;
            timer += Time.deltaTime;
        }

        _hitBox.DisableHitBox();
        yield break;
    }
}
