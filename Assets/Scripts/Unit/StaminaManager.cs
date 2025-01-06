using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : RegenParam
{
    protected Unit _unit;

    void Reset()
    {
        regenRate = 0.25f;
    }
    
    virtual protected void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    virtual protected void Start()
    {
        StartCoroutine(RegenerateParam());
    }
}
