using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class AICombatStateHandler : Action
    {
        [RequiredField] public SharedVector2 targetDirection;
        [RequiredField] public SharedFloat targetDistance;
        [RequiredField] public SharedAICombatState combatState;
        public SharedBool attackFinishedFlag;
        public SharedBool firstEncounterFlag;
        public SharedBool canGuard; // 가드를 할 수 있는 unit인지 여부
        public SharedBool isGuarding;   // Cautious 동안 true
        [Range(0, 100)] public int encounterAttack;   // 첫 encounter시 바로 공격할 확률
        [Range(0, 100)] public int refreshAttack;   // 트리에서 refresh시 공격할 확률
        public float closeDistance; // 이 유닛이 가깝다고 인식하는 거리
        [Header("Cautious State")]
        [SerializeField] private float cautiousMaxTime = 4f;
        [SerializeField] private float cautiousMinTime = 2f;
        private float cautiousTime; // <Cautious> CombatState 지속시간. 위 Max와 Min 사이 random 값이 할당된다.
        private float cautiousTimer;


        public override void OnStart()
        {
            if(firstEncounterFlag.Value)
            {
                firstEncounterFlag.Value = false;

                int randomNum = Random.Range(1, 100);
                if(encounterAttack > randomNum)
                {
                    SetStateToAttack();
                } else
                {
                    SetStateToCautious();
                }
            }

            // RefreshState();
        }

        public override TaskStatus OnUpdate()
        {
            switch(combatState.Value)
            {
                case AICombatState.Cautious:
                    CautiousState();
                    break;
                case AICombatState.Attack:
                    AttackState();
                    break;
                default:
                    break;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            isGuarding.Value = false;   // Stunned, Dodge 등 상태에서 false
        }

        public void SetStateToCautious()
        {
            if(canGuard.Value)
            {
                isGuarding.Value = true;
            }

            cautiousTime = Random.Range(cautiousMinTime, cautiousMaxTime);
            cautiousTimer = 0f;
            if(combatState.Value != AICombatState.Cautious)
            {
                combatState.Value = AICombatState.Cautious;
            } else
            {
                Owner.SendEvent("RefreshState");
            }
        }

        public void SetStateToAttack()
        {
            if(canGuard.Value)
            {
                isGuarding.Value = false;
            }

            if(combatState.Value != AICombatState.Attack)
            {
                combatState.Value = AICombatState.Attack;
            } else
            {
                Owner.SendEvent("RefreshState");
            }
        }

        void CautiousState()
        {
            cautiousTimer += Time.deltaTime;

            if(cautiousTimer >= cautiousTime)
            {
                RefreshState();
            }
        }

        void AttackState()
        {
            if(attackFinishedFlag.Value)
            {
                attackFinishedFlag.Value = false;
                RefreshState();
            }
        }

        bool IsTargetClose(float dist)
        {
            if(targetDistance.Value <= dist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void RefreshState()
        {
            int randomNum = Random.Range(1, 100);
            int attackChance = refreshAttack;

            // 상대가 가까울 경우 공격확률 커짐
            if(targetDistance.Value <= closeDistance)
            {
                attackChance += 45;
            }

            if(randomNum > attackChance)
            {
                // Debug.Log("ToCautious");
                SetStateToCautious();
            } else
            {
                // Debug.Log("ToAttack");
                SetStateToAttack();
            }
        }
    }
}
