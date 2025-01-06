using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWhiteEffect : MonoBehaviour
{
    public SpriteRenderer _sprite;   // FlashWhite 효과를 줄 스프라이트
    private Material matDefault;    // 스프라이트의 원본 material
    private Material matWhite;  // 하얗게 만들어 주는 material
    
    private void Awake()
    {
        matDefault = _sprite.material;
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
    }

    public void StartEffect()
    {
        StartCoroutine("FlashWhite");
    }

    IEnumerator FlashWhite()
    {
        _sprite.material = matWhite;
        yield return new WaitForSeconds(0.1f);
        _sprite.material = matDefault;
        yield break;
    }

}
