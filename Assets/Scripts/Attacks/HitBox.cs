using UnityEngine;

public class HitBox : HitBoxBase
{
    public Vector2 hitBoxPosition;
    public Vector2 hitBoxSize;  // Halfextent로 결정??
    public float hitBoxRotation;
    private Vector2 gizmoSize;  // Halfextent가 아님??

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(hitBoxPosition, new Vector3(hitBoxSize.x, hitBoxSize.y, 1f)); // Halfextent??
    }

    private void ChangeGizmosColor()
    {
        switch(hitBoxState)
        {
            case BattleBoxState.Disabled:
                gizmoColor = disabledColor;
                gizmoSize = Vector2.zero;   // HitBox 비활성 시 gizmo 보이지 않도록
                break;
            case BattleBoxState.Enabled:
                gizmoColor = enabledColor;
                gizmoSize = hitBoxSize;
                break;
            case BattleBoxState.Detected:
                gizmoColor = detectedColor;
                gizmoSize = hitBoxSize;
                break;
        }
    }

    public void SetHitBox(Vector2 boxPos, Vector2 boxSize)
    {
        hitBoxPosition = boxPos;
        hitBoxSize = boxSize;
    }

    public override void UpdateHitBox()
    {
        ChangeGizmosColor();    // Debug용 Gizmo
        
        if(hitBoxState == BattleBoxState.Disabled) { return; }  // 히트박스 비활성화 시 return

        // 공격에게 피격자들 전달
        Collider2D[] hurtBoxes = Physics2D.OverlapBoxAll(hitBoxPosition, hitBoxSize, 0f, mask);
        foreach(Collider2D hb in hurtBoxes)
        {
            hitBoxUser.DetectedHurtBox(hb, this.transform);
        }

        // Debug용 Gizmo 색 변환
        if(hurtBoxes.Length > 0)
        {
            hitBoxState = BattleBoxState.Detected;
        } else
        {
            hitBoxState = BattleBoxState.Enabled;
        }
    }
}
