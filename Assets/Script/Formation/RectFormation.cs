using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectFormation : Formation
{
    [SerializeField] float ratio = 1; // largeur / hauteur.
    public override Vector3 GetSlot(Vector3 position)
    {
        throw new NotImplementedException();
    }

    protected override Vector3 GetSlotPos(Vector3 origin, Quaternion orientation, int nbSlots, int i)
    {
        Vector3 position;
        return Vector3.zero;
    }
}
