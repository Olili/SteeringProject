using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlowField : MonoBehaviour {

    [Range(1, 200)]
    [SerializeField] int fieldSize;
    [Range(0.1f,5)]
    [SerializeField] float cellSize;
    #region GetterSetters
    public int FieldSize
    {
        get
        {
            return fieldSize;
        }

        protected set
        {
            fieldSize = value;
        }
    }

    #endregion

    [SerializeField] VectorFieldFunction.Type vectorFieldType;

	public Vector3 GetValue(Vector3 position)
    {
        return VectorFieldFunction.GetValue(vectorFieldType,position);
    }

    [SerializeField] bool showGizmo = true;
    [SerializeField] int yGizmoPos = 1;
    public void OnDrawGizmosSelected()
    {
        if (!showGizmo)
            return;
        Gizmos.color = Color.black;
        // 2D Grid
        for (float i = -fieldSize * 0.5f; i <= fieldSize * 0.5f; i++)
        {
            Vector3 begin = new Vector3(i * cellSize, yGizmoPos, -fieldSize * cellSize * 0.5f);
            Vector3 end = new Vector3(i * cellSize, yGizmoPos, fieldSize * cellSize * 0.5f);
            Gizmos.DrawLine(begin,end);

            begin = new Vector3(-fieldSize * cellSize * 0.5f, yGizmoPos, i * cellSize);
            end = new Vector3(fieldSize * cellSize * 0.5f, yGizmoPos, i * cellSize);
            Gizmos.DrawLine(begin, end);
        }
        // 2d FlowField Arrow
        for (float x = -fieldSize * 0.5f; x < fieldSize * 0.5f; x++) 
        {
            for (float z = -fieldSize * 0.5f; z < fieldSize * 0.5f; z++) 
            {
                Vector3 pos = new Vector3(x * cellSize + (cellSize * 0.5f), yGizmoPos, z * cellSize + (cellSize * 0.5f));
                Vector3 dir = GetValue(pos);

                Gizmos.color = new Color(dir.magnitude/fieldSize,0, 1);
                DrawArrow.ForGizmo(pos, dir.normalized * 0.5f * cellSize);
            }
        }

    }
}
