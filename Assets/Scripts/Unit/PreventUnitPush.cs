using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventUnitPush : MonoBehaviour
{
    public Collider2D unitCollider;
    public Collider2D preventPushCollider;
    void Start()
    {
        Physics2D.IgnoreCollision(unitCollider, preventPushCollider, true);
    }
}
