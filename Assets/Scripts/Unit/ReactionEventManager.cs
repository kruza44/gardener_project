using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionEventManager : MonoBehaviour
{
    // 가드 브레이크 관련도 추가해야 할까?
    // 가드 브레이크 같이 오래 지속되는 경우는 어떻게 이벤트를 전달해야 할까?
    // (보통 가드 브레이크를 일으킨 상대는 cautious 상태가 아닌 공격 후 딜레이 상태일 것)
    public delegate void NoticeMeleeAttack(float attackRange);
    public event NoticeMeleeAttack noticeMeleeAttack;
    public delegate void NoticeRangeAttack();   // Dodge 방향 추가할 것, 정 힘들면 UnityEvent 쓸 것
    public event NoticeRangeAttack noticeRangeAttack;
    public delegate void NoticeHealing();
    public event NoticeHealing noticeHealing;

    public void SendMeleeAttackEvent(float attackRange)
    {
        if(noticeMeleeAttack != null)
            noticeMeleeAttack(attackRange);
    }

    public void SendRangeAttackEvent()
    {
        if(noticeRangeAttack != null)
            noticeRangeAttack();
    }

    public void SendHealingEvent()
    {
        if(noticeHealing != null)
            noticeHealing();
    }

}
