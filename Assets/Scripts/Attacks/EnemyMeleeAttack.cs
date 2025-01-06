using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class EnemyMeleeAttack : Attack
{
    [SerializeField] private HitBox _hitBox;
    [SerializeField] private Vector2 frontHitBoxSize;
    [SerializeField] private Vector2 leftHitBoxOffset;
    [SerializeField] private float hitBoxDuration = 0.3f;  // 히트박스 유지 시간
    private SharedSpriteDirection spriteDirection;
    private Vector2 rightHitBoxOffset;
    // private Vector2 sideHitBoxSize;

    void Start()
    {
        BehaviorTree bt = this.transform.root.GetComponent<BehaviorTree>();
        spriteDirection = (SharedSpriteDirection)bt.GetVariable("spriteDirection");

        _hitBox.SetHitBoxUser(this);
        
        rightHitBoxOffset = new Vector2(-leftHitBoxOffset.x, leftHitBoxOffset.y);
        // sideHitBoxSize = new Vector2(frontHitBoxSize.y, frontHitBoxSize.x);
    }

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        _hitBox.SetHitBoxUser(this);
        
        base.DoAttack(targetDir, targetLoc);
        
        SetHitBoxByDirection();

        _hitBox.EnableHitBox();
        StartCoroutine(AttackhitBox());
    }

    void SetHitBoxByDirection()
    {
        switch(spriteDirection.Value)
        {
            case SpriteDirection.BackLeft :
                _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, frontHitBoxSize);
                break;
            case SpriteDirection.BackRight :
                _hitBox.SetHitBox((Vector2)transform.position + rightHitBoxOffset, frontHitBoxSize);
                break;
            case SpriteDirection.Left :
                _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, frontHitBoxSize);
                break;
            case SpriteDirection.Right :  
                _hitBox.SetHitBox((Vector2)transform.position + rightHitBoxOffset, frontHitBoxSize);
                break;
            case SpriteDirection.FrontLeft :
                _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, frontHitBoxSize);
                break;
            case SpriteDirection.FrontRight :  
                _hitBox.SetHitBox((Vector2)transform.position + rightHitBoxOffset, frontHitBoxSize);
                break;
            default :
                Debug.LogWarning("SpriteDirection error");
                break;
        }
    }

    //     void SetHitBoxByDirection()
    // {
    //     switch(spriteDirection.Value)
    //     {
    //         case SpriteDirection.BackLeft :
    //             _hitBox.SetHitBox((Vector2)transform.position - leftHitBoxOffset, frontHitBoxSize);
    //             break;
    //         case SpriteDirection.BackRight :
    //             _hitBox.SetHitBox((Vector2)transform.position - leftHitBoxOffset, frontHitBoxSize);
    //             break;
    //         case SpriteDirection.Left :
    //             _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, sideHitBoxSize);
    //             break;
    //         case SpriteDirection.Right :  
    //             _hitBox.SetHitBox((Vector2)transform.position - leftHitBoxOffset, sideHitBoxSize);
    //             break;
    //         case SpriteDirection.FrontLeft :
    //             _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, frontHitBoxSize);
    //             break;
    //         case SpriteDirection.FrontRight :  
    //             _hitBox.SetHitBox((Vector2)transform.position + leftHitBoxOffset, frontHitBoxSize);
    //             break;
    //         default :
    //             Debug.LogWarning("SpriteDirection error");
    //             break;
    //     }
    // }

    IEnumerator AttackhitBox()
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
