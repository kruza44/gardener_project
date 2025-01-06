using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class DoAttack : Action
    {
        [RequiredField] public Attack _attack;
        [RequiredField] public SharedVector2 attackDirection;
        [RequiredField] public SharedTransform target;

        public override void OnStart()
        {
            // _attack.DoAttack(attackDirection);
        }


    }
}
