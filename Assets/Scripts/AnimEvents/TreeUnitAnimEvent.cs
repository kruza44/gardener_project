using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TreeUnitAnimEvent : MonoBehaviour
{
    private BehaviorTree behaviorTree;
    private SharedBool attackTiming;

    void Awake()
    {
        behaviorTree = this.transform.root.GetComponent<BehaviorTree>();
    }

    void Start()
    {
        attackTiming = (SharedBool)behaviorTree.GetVariable("attackTiming");
    }

    public void RefreshTreeState()
    {
        if(behaviorTree != null)
        {
            Debug.Log("RefreshState");
            behaviorTree.SendEvent("RefreshState");
        } else
        {
            Debug.LogWarning("Tree null");
        }
    }

    public void AttackTiming()
    {
        attackTiming.Value = true;
    }

    public void CanAction()
    {
        // 나중에 컴보 구현할때 
    }
}
