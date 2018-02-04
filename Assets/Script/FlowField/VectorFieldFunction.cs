using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class VectorFieldFunction  {

	public enum Type
    {
        SpiralIn2D,
        SpiralOut2D,
        SyphonOut,
        Test
    }
    public static Vector3 GetValue(Type type,Vector3 pos)
    {
        switch (type)
        {
            case Type.SpiralIn2D:
                return SpiralIn2D(pos);
            case Type.SpiralOut2D:
                return SpiralOut2D(pos);
            case Type.SyphonOut:
                return SyphonOut(pos);
            case Type.Test:
                return Test(pos);
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
    static Vector3 SyphonOut(Vector3 pos)
    {
        return new Vector3(Mathf.Cos(pos.magnitude*0.5f)  , 0, Mathf.Sin(pos.magnitude*0.5f));
    }
    static Vector3 Test(Vector3 pos)
    {
        return new Vector3(Mathf.Sin(pos.magnitude*0.5f), 0, Mathf.Sin(pos.magnitude*0.5f));
    }
}
