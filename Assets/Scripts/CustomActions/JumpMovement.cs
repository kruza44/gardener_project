using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Custom")]
    public class JumpMovement : FsmStateAction
    {
        [RequiredField] public Transform spriteTransform;    // 점프에 필요
        [RequiredField] public FsmBool isJumping;
        public Animator _animator;
        public float airSpeed;
        public float jumpHeight;
        public float jumpTime;
        public float maxSpeed;  // 속도 제한
        public Ease jumpEaseType;
        public Ease fallEaseType;
        private Vector2 airMoveDirection;
        private MoveManager _move;
        private JumpColliderManager _jumpCollider;
        private Vector2 startMove;
        private string tweenID;

        public override void Awake()
        {
            _move = Owner.GetComponent<MoveManager>();
            _jumpCollider = Owner.GetComponent<JumpColliderManager>();

            tweenID = Owner.GetInstanceID().ToString(); // 트윈 ID 할당
        }

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

        public override void OnEnter()
        {
            isJumping.Value = true;

            startMove = Vector3.ClampMagnitude(_move.previousMove, maxSpeed);

            _jumpCollider.DisableColiders();

            // Sprite vertical 움직임           
            DG.Tweening.Sequence jumpSequence = DOTween.Sequence();

            jumpSequence.Append(spriteTransform.DOLocalMoveY(jumpHeight, jumpTime/2f).SetEase(jumpEaseType))
            .Append(spriteTransform.DOLocalMoveY(0, jumpTime/2f).SetEase(fallEaseType)).SetId(tweenID)
            .OnComplete(()=>
            {
                isJumping.Value = false;
                _jumpCollider.EnableColliders();

                if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) // 점프 상태일 때만 Finished
                {
                    _animator.SetTrigger("Finished");
                    Fsm.Event("FINISHED");
                }
            });

            StartCoroutine(HorizontalJumpMove());
        }
        
        IEnumerator HorizontalJumpMove()
        {
            while(true)
            {
                if(!isJumping.Value) { yield break; }

                airMoveDirection.x = Input.GetAxisRaw("Horizontal");
                airMoveDirection.y = Input.GetAxisRaw("Vertical");
                airMoveDirection.Normalize();

                // 점프 방향으로 움직이지 않으면 서서히 느려지게 하려고 했으나
                // 속도가 너무 빠르게 감속되거나
                // 다른 방향 공중 움직임이 너무 느려지는 문제가 있어 이대로 사용
                Vector2 totalMove = startMove + (airMoveDirection * airSpeed) - (startMove.normalized * airSpeed);

                _move.AddMove(totalMove);

                yield return new WaitForFixedUpdate();
            }
        }

        public override void OnExit()
        {
            // isJumping.Value = false;
            // _jumpCollider.EnableColliders();
        }
    }
}