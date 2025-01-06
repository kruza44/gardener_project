using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum DamageEffectType
{
    FlashWhite, None
}

public class EffectManager : MonoBehaviour
{
    // 싱글톤 패턴
    private static EffectManager instance = null;

    private Material matDefault;    // 스프라이트의 원본 material
    private Material matWhite;  // 하얗게 만들어 주는 material
    [Header("SpriteShake Test Parameters")]
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakevibe;
    [SerializeField] private float shakeRandom;
    [SerializeField] private bool shakeSnap;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;

            // 씬 전환 후에도 파괴되지 않도록
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            // 중복 방지
            Destroy(this.gameObject);
        }

        matDefault = new Material(Shader.Find("Sprites/Default"));
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;  // FlashWhite를 위한 매터리얼
    }

    public static EffectManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            
            return instance;
        }
    }

    public void DamageEffect(SpriteRenderer _sprite, DamageEffectType type)
    {
        switch(type)
        {
            case DamageEffectType.FlashWhite:
                StartCoroutine(FlashWhiteCoroutine(_sprite));
                break;
            case DamageEffectType.None:
                break;
            default:
                Debug.Log("Unavailable DamageEffectType");
                break;
        }
    }

    public void SpriteShake(SpriteRenderer _sprite, float shakeDuration)
    {
        string tweenID = _sprite.gameObject.GetInstanceID().ToString();
        DOTween.Kill(tweenID);

        // 흔든 후 local position (0, 0, 0)으로 복구
        _sprite.transform.DOShakePosition(shakeDuration, shakeStrength, shakevibe, shakeRandom, shakeSnap).SetId(tweenID).OnComplete(()=> {
            _sprite.transform.DOLocalMove(Vector3.zero, 0.05f); });
    }

    IEnumerator FlashWhiteCoroutine(SpriteRenderer _sprite)
    {
        // Material matDefault = _sprite.material;
        _sprite.material = matWhite;
        yield return new WaitForSeconds(0.1f);
        _sprite.material = matDefault;
        yield break;
    }
}
