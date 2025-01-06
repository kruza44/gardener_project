using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class JumpColliderManager : MonoBehaviour
{
    [SerializeField] private Collider2D[] offColliders;   // 점프 시 끌 콜라이더
    private SharedBool isJumpCollider;

    public void EnableColliders()
    {
        foreach(Collider2D col in offColliders)
        {
            col.enabled = true;
        }
    }

    public void DisableColiders()
    {
        foreach(Collider2D col in offColliders)
        {
            col.enabled = false;
        }
    }
}
