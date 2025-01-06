using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(TargetManager))]
public class TargetManagerEditor : Editor
{
    void OnSceneGUI()
    {
        TargetManager manager = (TargetManager)target;

        Handles.color = Color.cyan;
        Handles.DrawWireArc(manager.transform.position, Vector3.forward, Vector3.right, 360f, manager.sightRadius);
        // Vector3 viewAngle1 = GetAngleLine((-manager.sightAngle / 2), manager.transform);
        // Vector3 viewAngle2 = GetAngleLine((manager.sightAngle / 2), manager.transform);
        // Handles.DrawLine(manager.transform.position, manager.transform.position + viewAngle1 * manager.sightRadius);
        // Handles.DrawLine(manager.transform.position, manager.transform.position + viewAngle2 * manager.sightRadius);
    }

    Vector3 GetAngleLine(float angle, Transform tr)
    {
        angle += tr.eulerAngles.z;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
    }
}
