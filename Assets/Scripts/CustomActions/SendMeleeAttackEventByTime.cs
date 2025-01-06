using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class SendMeleeAttackEventByTime : FsmStateAction
    {
        public FsmFloat sendTime;
        public FsmFloat attackRange;
        private ReactionEventManager _reaction;
        private Coroutine timerCoroutine;

        public override void Awake()
        {
            _reaction = Owner.GetComponent<ReactionEventManager>();
        }

        public override void OnEnter()
        {
            timerCoroutine = StartCoroutine(SendEventAfterTime());
        }

        public override void OnExit()
        {
            StopCoroutine(timerCoroutine);            
        }

        IEnumerator SendEventAfterTime()
        {
            yield return new WaitForSeconds(sendTime.Value);
            _reaction.SendMeleeAttackEvent(attackRange.Value);
            yield break;
        }
    }
}
