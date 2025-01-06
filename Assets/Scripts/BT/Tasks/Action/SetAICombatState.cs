using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class SetAICombatState : Action
    {
        // [RequiredField] public SharedAICombatState currentState;
        public AICombatStateHandler _combatStateHandler;
        public AICombatState setState;

        public override void OnStart()
        {
            switch(setState)
            {
                case(AICombatState.Cautious):
                    _combatStateHandler.SetStateToCautious();
                    break;
                case(AICombatState.Attack):
                    _combatStateHandler.SetStateToAttack();
                    break;
            }
        }
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
