using UnityEngine;

public class HitBoxBase : MonoBehaviour
{
    public LayerMask mask;

    public Color disabledColor;
    public Color enabledColor;
    public Color detectedColor;
    protected Color gizmoColor;

    protected BattleBoxState hitBoxState;
    protected IHitBoxUser hitBoxUser; // 이 HitBox를 사용하는 공격

    // private void ChangeGizmosColor()
    // {
    //     switch(hitBoxState)
    //     {
    //         case BattleBoxState.Disabled:
    //             gizmoColor = disabledColor;
    //             gizmoSize = Vector2.zero;   // HitBox 비활성 시 gizmo 보이지 않도록
    //             break;
    //         case BattleBoxState.Enabled:
    //             gizmoColor = enabledColor;
    //             gizmoSize = hitBoxSize;
    //             break;
    //         case BattleBoxState.Detected:
    //             gizmoColor = detectedColor;
    //             gizmoSize = hitBoxSize;
    //             break;
    //     }
    // }

    // virtual public void SetHitBox(Vector2 boxPos, Vector2 boxSize)
    // {
            // * CircleCast 같은 경우는 전혀 다른 값을 인수로 받아야 함... *
    // }

    virtual public void UpdateHitBox()
    {

    }

    public void EnableHitBox()
    {
        hitBoxState = BattleBoxState.Enabled;
    }

    public void DisableHitBox()
    {
        hitBoxState = BattleBoxState.Disabled;
    }

    // 이 HitBox를 사용하는 공격이 함수를 부름
    public void SetHitBoxUser(IHitBoxUser user)
    {
        hitBoxUser = user;
    }
}
