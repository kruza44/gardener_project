using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerOrder : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Transform rootTransform;
    // [SerializeField] private Transform feetTransform;

    void Awake()
    {
        _sprite = this.GetComponent<SpriteRenderer>();
        rootTransform = this.transform.root.GetComponent<Transform>();
    }

    void Update()
    {
        _sprite.sortingOrder = (int)(rootTransform.position.y * -1000f);
    }

    // IEnumerator ChangeOrderLayer()
    // {
    //     while(true)
    //     {
    //         _sprite.sortingOrder = (int)(feetTransform.position.y * -1000f);
    //         yield return new WaitForSeconds(0.2f);
    //     }
    // }
}
