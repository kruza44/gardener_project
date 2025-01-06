using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
    [Tooltip("Order weapon to start Attack")]
    public class WeaponStartAttack : FsmStateAction
    {
        public FsmVector2 targetDirection;
        public FsmVector2 targetLocation;

        /*
            프로토타입용. Tag 등 사용하여 스크립트 내에서
            weapon 찾을 수 있도록 수정해야 함
            (무기 변경 가능하도록)

            일단 무기 변경 시스템을 어떻게 구현할 것인지
            방법부터 생각해야 함
        */
        [RequiredField]
        public GameObject weapon;
        [RequiredField]
        [UIHintAttribute(UIHint.Variable)]
        public FsmBool attackTiming;

        private Attack _weaponAttack;

        public override void Awake()
        {
            _weaponAttack = weapon.GetComponent<Attack>();
        }

        public override void OnUpdate()
        {
            if(attackTiming.Value)
            {
                attackTiming.Value = false;
                _weaponAttack.DoAttack(targetDirection.Value, targetLocation.Value);

                // update 끝내는 법??
            }
        }

        public override void OnExit()
        {
            attackTiming.Value = false;
        }
    }
}
