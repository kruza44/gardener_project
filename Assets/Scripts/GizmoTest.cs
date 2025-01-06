using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoTest : MonoBehaviour
{
    public Vector3 hitBoxPosition;
    public Vector3 hitBoxSize;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(hitBoxPosition, new Vector3(hitBoxSize.x * 2, hitBoxSize.y * 2, 1f)); // Halfextent??
    }
}
