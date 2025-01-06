using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    [TaskCategory("Custom")]
    public class WaitAttackTimingByTime : Action
    {
        [SerializeField] private float attackTime;  // 공격이 시작되는 시간
        [RequiredField] public Attack _attack;
        [RequiredField] public SharedVector2 attackDirection;
        [RequiredField] public SharedTransform target;
        private float timer;

        public override void OnStart()
        {
            timer = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;

            if(timer >= attackTime)
            {
                _attack.DoAttack(attackDirection.Value, target.Value.position);
                return TaskStatus.Success;
            } else
            {
                return TaskStatus.Running;
            }
        }
    }
}
