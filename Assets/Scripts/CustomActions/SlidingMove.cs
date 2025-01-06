using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
    [Tooltip("Used for movements such backstep, attack, staggered, etc")]
    public class SlidingMove : FsmStateAction
    {
        public FsmVector2 targetDirection;
        public Ease easeType;
        public FsmFloat easeDuration;
        public FsmBool moveOppositeDir;   // targetDirection과 반대방향으로 움직일지 여부

        public FsmFloat moveSpeed;
        public FsmFloat minSpeed;
        // [RequiredField]
        // public Rigidbody2D rb;
        private MoveManager _move;

        private float direction;
        private float tempSpeed;

        public override void Awake()
        {
            _move = Owner.GetComponent<MoveManager>();
        }

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }     

        public override void OnEnter()
        {
            if(moveOppositeDir.Value)
            {
                direction = -1;
            } else
            {
                direction = 1;
            }

            tempSpeed = moveSpeed.Value;

            DOTween.To(()=> tempSpeed, x=> tempSpeed = x, minSpeed.Value, easeDuration.Value).SetEase(easeType).SetId("speedTween");
        }

        public override void OnFixedUpdate()
        {
            _move.AddMove(targetDirection.Value * direction * tempSpeed);
            // rb.MovePosition(rb.position + targetDirection.Value * direction * tempSpeed * Time.fixedDeltaTime);
        }

        public override void OnExit()
        {
            DOTween.Kill("speedTween");
        }
    }
}
