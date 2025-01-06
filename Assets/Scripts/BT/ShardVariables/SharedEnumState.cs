using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public enum AIState
    {
        Peace, TargetFound, Dead
    }

    [System.Serializable]
    public class SharedAIState : SharedVariable<AIState>
    {
        public static implicit operator SharedAIState(AIState value) { return new SharedAIState { Value = value }; }
    }

    [System.Serializable]
    public enum AICombatState
    {
        Cautious, Attack, Heal
    }

    [System.Serializable]
    public class SharedAICombatState : SharedVariable<AICombatState>
    {
        public static implicit operator SharedAICombatState(AICombatState value) { return new SharedAICombatState { Value = value }; }
    }

    [System.Serializable]
    public enum SpriteDirection
    {
        BackLeft, BackRight, Left, Right, FrontLeft, FrontRight
    }

    [System.Serializable]
    public class SharedSpriteDirection : SharedVariable<SpriteDirection>
    {
        public static implicit operator SharedSpriteDirection(SpriteDirection value) { return new SharedSpriteDirection { Value = value }; }
    }

    [System.Serializable]
    public enum AIStunState
    {
        Staggered, GuardStunned, GuardBroke
    }

    [System.Serializable]
    public class SharedAIStunState : SharedVariable<AIStunState>
    {
        public static implicit operator SharedAIStunState(AIStunState value) { return new SharedAIStunState { Value = value }; }
    }

    [System.Serializable]
    public enum AIPhaseState
    {
        Phase1, Phase2, Phase3, Phase4, Phase5
    }

    [System.Serializable]
    public class SharedAIPhaseState : SharedVariable<AIPhaseState>
    {
        public static implicit operator SharedAIPhaseState(AIPhaseState value) { return new SharedAIPhaseState { Value = value }; }
    }
}
