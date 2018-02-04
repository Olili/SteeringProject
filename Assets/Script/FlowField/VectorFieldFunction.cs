using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class VectorFieldFunction  {

	public enum Type
    {
        SpiralIn2D,
        SpiralOut2D,
    }
    public static Vector3 GetValue(Type type,Vector3 pos)
    {
        switch (type)
        {
            case Type.SpiralIn2D:
                return SpiralIn2D(pos);
            case Type.SpiralOut2D:
                return SpiralOut2D(pos);
            default:
                Debug.Log("Invalind Vector field function Type");
                return Vector3.zero;
        }
    }
    static Vector3 SpiralIn2D(Vector3 pos)
    {
        return new Vector3(-pos.x + pos.z, 0, -pos.x - pos.z);
    }
    static Vector3 SpiralOut2D(Vector3 pos)
    {
        return new Vector3(pos.x - pos.z, 0, pos.x + pos.z);
    }
}
