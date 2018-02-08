using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFormation : Formation
{
    float VAngleOpening = Mathf.PI*0.5f- Mathf.PI*0.15f;
    protected override Vector3 GetSlotPos(int nbSlots, int i)
    {
        int centerId = Mathf.CeilToInt(nbSlots / 2);
        int id = i - centerId;
        float angle = VAngleOpening;
        Vector3 position = new Vector3(Mathf.Cos(angle ) , 0, Mathf.Sin(angle + (id > 0 ? Mathf.PI : 0)) ) *2 * id;
        return position;
    }
}
