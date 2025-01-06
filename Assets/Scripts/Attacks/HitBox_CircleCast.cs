using UnityEngine;

public class HitBox_CircleCast : HitBoxBase
{
    public Vector2 startPosition;
    public Vector2 targetDirection;
    public float radius;
    public float targetDistance;
    private float gizmoRadius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireSphere((Vector3)(startPosition + (targetDirection * targetDistance)), radius);
    }

    private void ChangeGizmosColor()
    {
        switch(hitBoxState)
        {
            case BattleBoxState.Disabled:
                gizmoColor = disabledColor;
                gizmoRadius = 0f;
                break;
            case BattleBoxState.Enabled:
                gizmoColor = enabledColor;
                gizmoRadius = radius;
                Debug.DrawLine(startPosition, startPosition + targetDirection * targetDistance, enabledColor);
                break;
            case BattleBoxState.Detected:
                gizmoColor = detectedColor;
                gizmoRadius = radius;
                Debug.DrawLine(startPosition, startPosition + targetDirection * targetDistance, detectedColor);
                break;
        }
    }

    public void SetHitBox(Vector2 startPos, Vector2 targetDir, float rad, float targetDis)
    {
        startPosition = startPos;
        targetDirection = targetDir;
        radius = rad;
        targetDistance = targetDis;
    }

    public override void UpdateHitBox()
    {
        ChangeGizmosColor();    // Debug용 Gizmo

        if(hitBoxState == BattleBoxState.Disabled) { return; }  // 히트박스 비활성화 시 return

        // 공격에게 피격자들 전달
        RaycastHit2D[] hurtBoxes = Physics2D.CircleCastAll(startPosition, radius, targetDirection, targetDistance, mask);
        foreach(RaycastHit2D hb in hurtBoxes)
        {
            Collider2D col = hb.collider;
            hitBoxUser.DetectedHurtBox(col, this.transform);
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
