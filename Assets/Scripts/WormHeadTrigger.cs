using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHeadTrigger : MonoBehaviour
{
    public bool collided = false;   // Worm 스크립트에서 충돌 여부 확인
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.root.CompareTag("Gardener")) { return; } // 플레이어 PushBox 무시
        Debug.Log("collided");
        collided = true;
    }
}
