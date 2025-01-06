using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_LinearMove : MonoBehaviour
{
    // 투사체의 경우, attack 스크립트가 아닌 projectile에서 hitbox 업데이트
    [SerializeField] private Vector2 hitBoxSize;
    [SerializeField] private Vector2 hitBoxOffset;
    public Vector2 moveDirection;
    public float moveSpeed;
    private Rigidbody2D _rb;
    private HitBox _hitBox;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hitBox = GetComponent<HitBox>();
    }

    void Start()
    {
        _hitBox.EnableHitBox();
    }
    
    void Update()
    {
        _hitBox.SetHitBox((Vector2)this.transform.position + hitBoxOffset, hitBoxSize);
        _hitBox.UpdateHitBox();
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(this.gameObject);       
    }

    public void SetProjectile(Vector2 dir, IHitBoxUser user)
    {
        moveDirection = dir;
        _hitBox.SetHitBoxUser(user);
    }
}
