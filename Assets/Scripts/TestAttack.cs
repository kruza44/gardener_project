using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : Attack
{
    public HitBox _hitbox;

    void Start()
    {
        _hitbox.SetHitBoxUser(this);
        Vector3 hitBoxPos = this.transform.position + new Vector3(0.82f, 0.09f, 0f);
        _hitbox.SetHitBox(hitBoxPos, new Vector2(0.99f, 1.6f));
        _hitbox.EnableHitBox();
    }

    // Update is called once per frame
    void Update()
    {
        _hitbox.UpdateHitBox();
    }
}
