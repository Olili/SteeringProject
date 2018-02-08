using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFormation : Formation
{

    protected override  Vector3 GetSlotPos(int nbSlots, int i)
    {
        float deltaA = 2 * (float)Math.PI / nbSlots;
        float ray = nbSlots * 0.5f; // should use entity Size.
        return new Vector3(Mathf.Cos(deltaA * i) * ray, 0, Mathf.Sin(deltaA * i) * ray);
    }
}
