using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiseManager : RegenParam
{
    private Unit _unit;
    
    void Reset()
    {
        regenRate = 0.25f;
    }
    
    void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    void Start()
    {
        StartCoroutine(RegenerateParam());
    }
}
