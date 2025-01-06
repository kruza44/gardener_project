using UnityEngine;

public class HitBox_OverlapCircle : HitBoxBase
{
    public Vector2 hitBoxPosition;
    public float hitBoxRadius;
    private float gizmoSize;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireSphere((Vector3)hitBoxPosition, hitBoxRadius);
    }

    private void ChangeGizmosColor()
    {
        switch(hitBoxState)
        {
            case BattleBoxState.Disabled:
                gizmoColor = disabledColor;
                gizmoSize = 0f;   // HitBox 비활성 시 gizmo 보이지 않도록
                break;
            case BattleBoxState.Enabled:
                gizmoColor = enabledColor;
                gizmoSize = hitBoxRadius;
                break;
            case BattleBoxState.Detected:
                gizmoColor = detectedColor;
                gizmoSize = hitBoxRadius;
                break;
        }
    }

    public void SetHitBox(Vector2 boxPos, float radius)
    {
        hitBoxPosition = boxPos;
        hitBoxRadius = radius;
    }

    public override void UpdateHitBox()
    {
        ChangeGizmosColor();    // Debug용 Gizmo
        
        if(hitBoxState == BattleBoxState.Disabled) { return; }  // 히트박스 비활성화 시 return

        // 공격에게 피격자들 전달
        Collider2D[] hurtBoxes = Physics2D.OverlapCircleAll(hitBoxPosition, hitBoxRadius, mask);
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
