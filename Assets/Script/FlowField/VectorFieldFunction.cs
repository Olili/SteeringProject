using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class VectorFieldFunction  {

	public enum Type
    {
        SpiralIn2D,
        SpiralOut2D,
        SyphonOut,
        TripleSinusoid,
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
            case Type.TripleSinusoid:
                return TripleSinusoïd(pos);
            default:
                Debug.Log("Invalind Vector field function Type");
                return Vector3.zero;
        }
    }
    static Vector3 SpiralIn2D(Vector3 p)
    {
        return new Vector3(-p.x + p.z, 0, -p.x - p.z);
    }
    static Vector3 SpiralOut2D(Vector3 p)
    {
        return new Vector3(p.x - p.z, 0, p.x + p.z);
    }
    static Vector3 SyphonOut(Vector3 p)
    {
        return new Vector3(Mathf.Cos(p.magnitude*0.5f)  , 0, Mathf.Sin(p.magnitude*0.5f));
    }
    static Vector3 TripleSinusoïd(Vector3 p)
    {
        Vector3 flow = Vector3.zero;
        Vector3 centerOffset = p;
        centerOffset.Normalize();
        flow = new Vector3(-centerOffset.z, 0, centerOffset.x);
        Vector3 outV = 2.0F / p.magnitude *centerOffset;
        flow += outV;
        float a = Mathf.Atan2(p.x, p.z);
        float s = (float)Mathf.Sin(a * 3.0F) * 1.8F;
        centerOffset *= s;
        flow += centerOffset;
        flow.y = 0;
        return flow.normalized;
    }


    //return new Vector3(Mathf.Cos((p.z-p.x)*0.1f), 0, Mathf.Sin((-p.z + p.x)*0.1f));
    //return new Vector3(Mathf.Cos(p.x*p.z/), 0, Mathf.Sin(p.x*p.z*0.1f));
    //return new Vector3(Mathf.Sin((p.x) * 0.3f * p.z>0?1:-1), 0, Mathf.Cos((p.x) * 0.3f));
}
