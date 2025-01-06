using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : RegenParam
{
    private Unit _unit;

    void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    void Start()
    {
        StartCoroutine(RegenerateParam());
    }

    protected override IEnumerator RegenerateParam()    // 스태미나, poise와 다르게 briefStopRegen이 없도록 override
    {
        while(true)
        {
            if(!StopRegen)
            {
                if(currentParam < maxParam) // 수치 Max 시 회복 x
                {
                    AddCurrentParam((float)maxParam * regenRate * Time.deltaTime);
                }
                yield return null;
            }
        }
    }

    public override void ReduceCurrentParam(float value)
    {
        base.ReduceCurrentParam(value);

        if(currentParam <= 0f)
        {
            _unit.IsDead();
        }
    }
}
