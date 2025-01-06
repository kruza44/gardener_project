using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    HP, 스태미나, Poise 등 
    리젠될 수 있는 값을 위한 클래스
*/
public class RegenParam : MonoBehaviour
{
    [HideInInspector] public float floatParam;   // 실수 단위로 증감하기 위한 임시값
    [HideInInspector] public int maxParam;
    [HideInInspector] public int currentParam;
    public float regenRate;  // 초당 회복하는 비율 (예: 0.25f -> 초당 전체의 1/4 회복)
    public UIBar _bar;
    [SerializeField] private float reRegenTime = 1f; // 멈추었다가 다시 리젠하기까지 걸리는 시간
    protected bool briefStopRegen = false; // 잠시동안 리젠을 멈추기 위함 (Health에서는 쓰이지 않는다)
    protected bool StopRegen = false; // 지속적으로 리젠을 멈추고 싶은 경우


    public void InitializeParam(int max, int current)
    {
        maxParam = max;
        currentParam = current;
        floatParam = currentParam;

        if(_bar != null)    // UI 할당한 경우
        {
            _bar.SetMaxBar(maxParam);
            _bar.SetBar(currentParam);
        }
    }

    virtual public void ReduceCurrentParam(float value)
    {
        if(value < 0) { return; }

        floatParam -= value;
        currentParam = (int)floatParam;
        briefStopRegen = true;

        if(floatParam < 0f) // 0보다 아래로 내려가지 않도록
        {
            floatParam = 0f;
            currentParam = 0;
        }

        if(_bar != null)    // UI
        {
            _bar.SetBar(currentParam);
        }
    }

    virtual public void AddCurrentParam(float value)
    {
        if(value < 0) { return; }

        floatParam += value;
        currentParam = (int)floatParam;

        if(floatParam > maxParam)   // 최대치 넘지 않도록
        {
            floatParam = maxParam;
            currentParam = maxParam;
        }

        if(_bar != null)    // UI
        {
            _bar.SetBar(currentParam);
        }
    }

    virtual public void SetCurrentParam(int value)
    {
        currentParam = value;
        floatParam = currentParam;

        if(floatParam < 0f) // 0보다 아래로 내려가지 않도록
        {
            floatParam = 0f;
            currentParam = 0;
        } else if(floatParam > maxParam)   // 최대치 넘지 않도록
        {
            floatParam = maxParam;
            currentParam = maxParam;
        }

        if(_bar != null)    // UI
        {
            _bar.SetBar(currentParam);
        }
    }

    virtual protected IEnumerator RegenerateParam()
    {
        while(true)
        {
            if(!StopRegen)
            {
                if(briefStopRegen)   // stopRegenType인 경우 잠시 리젠 중단
                {
                    briefStopRegen = false;
                    yield return new WaitForSeconds(reRegenTime);
                    continue;
                }

                if(currentParam < maxParam) // 수치 Max 시 회복 x
                {
                    AddCurrentParam((float)maxParam * regenRate * Time.deltaTime);
                }
            }
            yield return null;
        }
    }
}
