using UnityEngine;

public interface IHasHurtBox
{
    void GetHit(DamageInfo dmgInfo);

    void Shake(float shakeDuration);
}
