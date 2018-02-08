using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFormation : Formation
{
    protected override Vector3 GetSlotPos(int nbSlots, int i)
    {
        return Vector3.right * (i - (nbSlots * 0.5f)) * 2f;
    }
}
