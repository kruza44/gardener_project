using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class HollowSwordAttack : Attack
{
    public HitBox _hitbox;
    private SharedSpriteDirection spriteDirection;

    void Start()
    {
        BehaviorTree bt = this.transform.root.GetComponent<BehaviorTree>();
        spriteDirection = (SharedSpriteDirection)bt.GetVariable("spriteDirection");
        _hitbox.SetHitBoxUser(this);
    }

    public override void DoAttack(Vector2 targetDir, Vector2 targetLoc)
    {
        SetHitBoxByDirection();

        _hitbox.EnableHitBox();
        StartCoroutine(AttackHitBox());
    }

    void SetHitBoxByDirection()
    {
        switch(spriteDirection.Value)
            {
                case SpriteDirection.BackLeft :
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(0, 0.7f), new Vector2(1.5f, 0.8f));
                    break;
                case SpriteDirection.BackRight :
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(0, 0.7f), new Vector2(1.5f, 0.8f));
                    break;
                case SpriteDirection.Left :
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(-0.7f, 0), new Vector2(0.8f, 1.5f));
                    break;
                case SpriteDirection.Right :  
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(0.7f, 0), new Vector2(0.8f, 1.5f));
                    break;
                case SpriteDirection.FrontLeft :
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(0, -0.7f), new Vector2(1.5f, 0.8f));
                    break;
                case SpriteDirection.FrontRight :  
                    _hitbox.SetHitBox((Vector2)transform.position + new Vector2(0, -0.7f), new Vector2(1.5f, 0.8f));
                    break;
                default :
                    Debug.LogWarning("SpriteDirection error");
                    break;
		    }
    }

    IEnumerator AttackHitBox()
    {
        float timer = 0;

        while(timer <= 0.3f)
        {
            SetHitBoxByDirection();
            _hitbox.UpdateHitBox();
            yield return null;
            timer += Time.fixedDeltaTime;
        }

        _hitbox.DisableHitBox();
        yield break;
    }
}
