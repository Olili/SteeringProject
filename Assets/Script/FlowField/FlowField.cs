using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlowField : MonoBehaviour {

    [SerializeField] int fieldSize;
    [SerializeField] int cellSize;
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
    // Use this for initialization
    void Start () {
	}

	public Vector3 GetValue(Vector3 position)
    {
        return VectorFieldFunction.GetValue(vectorFieldType,position);
    }

    [SerializeField] bool showGizmo = true;
    public void OnDrawGizmos()
    {
        if (!showGizmo)
            return;
        Gizmos.color = Color.black;
        // 2D Grid
        for (float i = -fieldSize * 0.5f; i <= fieldSize * 0.5f; i++)
        {
            Vector3 begin = new Vector3(i,0, -fieldSize * cellSize * 0.5f);
            Vector3 end = new Vector3(i, 0, fieldSize * cellSize * 0.5f);
            Gizmos.DrawLine(begin,end);

            begin = new Vector3(-fieldSize * cellSize * 0.5f, 0, i);
            end = new Vector3(fieldSize * cellSize * 0.5f, 0, i);
            Gizmos.DrawLine(begin, end);
        }
        
        // Arrow
        for (float x = -fieldSize * 0.5f; x < fieldSize * 0.5f; x++) 
        {
            for (float z = -fieldSize * 0.5f; z < fieldSize * 0.5f; z++) 
            {
                Vector3 pos = new Vector3(x+(cellSize*0.5f), 0, z + (cellSize * 0.5f));
                Vector3 dir = GetValue(pos);
                Gizmos.color = new Color(dir.magnitude/fieldSize,0, 1);
                DrawArrow.ForGizmo(pos, dir.normalized*0.5f);
            }
        }

    }
}
