using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public DamageInfo wallDmgInfo;

    [SerializeField] private Color wallColor;

    public void WallCrumbleEffect()
    {
        // 이펙트 매니저에서 wallColor에 해당되는 이펙트 실행
    }
}
