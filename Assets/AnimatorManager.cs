using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class AnimatorManager : MonoBehaviour
{
    public Animator _animator;
    private BehaviorTree _bt;
    private SharedBool isGuarding;
    private SharedBool isWalking;
    private int guardHashID;
    private int walkHashID;

    void Awake()
    {
        _bt = this.GetComponent<BehaviorTree>();
    }

    void Start()
    {
        isGuarding = (SharedBool)_bt.GetVariable("isGuarding");
        isWalking = (SharedBool)_bt.GetVariable("isWalking");

        guardHashID = UnityEngine.Animator.StringToHash("IsGuarding");
        walkHashID = UnityEngine.Animator.StringToHash("IsWalking");

    }

    void Update()
    {
        _animator.SetBool(guardHashID, isGuarding.Value);
        _animator.SetBool(walkHashID, isWalking.Value);
    }
}
