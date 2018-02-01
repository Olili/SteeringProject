using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SteeringGizmos  {

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawNormalGizmos(Steering steering, GizmoType drawnGizmoType)
    {
        Vector3 position = steering.transform.position;
        Vector3 velocity = steering.GetComponent<Rigidbody>().velocity;
        Vector3 steeringForce = steering.giz.steeringForce;

        if (steering.giz.Velocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + velocity);
        }
        if (steering.giz.Steering)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + steeringForce);
        }
        if (steering.giz.Separate)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + steering.giz.separateForce);
        }
        if (steering.giz.collisionAvoidance)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(position, position + steering.giz.avoidance);
        }
        if (steering.giz.SeparateCheckSphere)
        {
            Gizmos.color = new Color(0,1,0,0.5f);
            Gizmos.DrawSphere(position, steering.giz.separateSphereLenght);
        }
        if (steering.giz.collisionAvoidance)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(position, steering.collisionAvoidanceRay);
        }


    }
}