using UnityEngine;

public interface IHitBoxUser
{
    void DetectedHurtBox(Collider2D other, Transform hitBoxTR);
}
