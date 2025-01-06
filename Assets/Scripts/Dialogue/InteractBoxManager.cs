using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using PixelCrushers.DialogueSystem;

public class InteractBoxManager : MonoBehaviour
{
    [SerializeField] private CircleCollider2D interactCollider;
    [SerializeField] private CircleCollider2D preventPushCollider;
    [SerializeField] private PlayMakerFSM _fsm;
    private FsmVector2 sightDirection;
    private FsmBool jumpLocked;
    private FsmBool interactLocked;
    private FsmBool inputLocked;
    private Vector2 pushCollideroffset;
    private float pushColliderradius;
    private Collider2D usableTarget;

    void Start()
    {
        sightDirection = _fsm.FsmVariables.GetFsmVector2("sightDirection");
        jumpLocked = _fsm.FsmVariables.GetFsmBool("jumpLocked");
        interactLocked = _fsm.FsmVariables.GetFsmBool("interactLocked");
        inputLocked = _fsm.FsmVariables.GetFsmBool("inputLocked");

        pushCollideroffset = preventPushCollider.offset;
        pushColliderradius = preventPushCollider.radius;

        usableTarget = null;
    }

    void Update()
    {
        // 시야 방향에 따라 interactBox 위치 변경
        interactCollider.offset = pushCollideroffset + sightDirection.Value * pushColliderradius;

        if(usableTarget != null)
        {
            if(!interactLocked.Value)
            {
                if(Input.GetButtonDown("A_Button"))
                {
                    usableTarget.GetComponent<DialogueSystemTrigger>().OnUse(this.transform.root);
                }
            }
        }
    }

    /// *Idle 상태에만 interact 가능하도록... 더 좋은 구현 방법이 있을 것 같음*
    void OnTriggerEnter2D(Collider2D other)
    {
        usableTarget = other;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        usableTarget = null;
    }

    public void OnSelectUsable()
    {
        jumpLocked.Value = true;
    }

    public void OnDeSelectUsable()
    {
        jumpLocked.Value = false;
    }

    public void ConverationStart()
    {
        // 시간 정지도 해야할까?
        inputLocked.Value = true;
        interactLocked.Value = true;
    }

    public void ConversationEnd()
    {
        Invoke("ConversationEndDelay", 0.1f);
    }

    void ConversationEndDelay()
    {
        inputLocked.Value = false;
        interactLocked.Value = false;
    }
}
