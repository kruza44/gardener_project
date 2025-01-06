using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
    [Tooltip("Manage worm to start, end")]
    public class ManageWorm : FsmStateAction
    {
        public FsmVector2 targetDirection;
        [RequiredField]
        public Worm _worm;
        [RequiredField]
        public Animator _animator;
        private bool isTriggerDone; // Animator trigger 한번만 실행되도록
        
        public override void OnEnter()
        {
            _worm.DoWorm(targetDirection.Value);
        }

        public override void OnUpdate()
        {
            if(_worm.isShieldBash)
            {
                if(!isTriggerDone)
                {
                    isTriggerDone = true;
                    _animator.SetTrigger("DoShieldBash");
                }
            }
            if(_worm.isFinished) { Fsm.Event("FINISHED"); } // WormSequence가 끝난 경우 <Worm> state 종료 
        }

        public override void OnExit()
        {
            isTriggerDone = false;
            
            // WormSequnce가 도중에 중단된 경우
            if(!_worm.isFinished)
            {
                _worm.StartCoroutine("EndWormSequence");
            } else
            {
                _animator.SetTrigger("Finished");   // 도중에 중단되지 않은 경우에만 Finished trigger (Staggered 등과 겹치면 안됨)
            }
        }
    }
}