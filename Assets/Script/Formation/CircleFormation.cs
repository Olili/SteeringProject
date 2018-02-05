using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFormation : Formation
{
    public override Vector3 GetSlot(Vector3 position)
    {
        throw new NotImplementedException();
    }

    protected override  Vector3 GetSlotPos(Vector3 origin, Quaternion orientation, int nbSlots, int i)
    {
        float deltaA = 2 * (float)Math.PI / nbSlots;
        float ray = nbSlots * 0.5f; // should use entity Size.
        return new Vector3(Mathf.Cos(deltaA * i) * ray, 0, Mathf.Sin(deltaA * i) * ray);
    }

    [SerializeField] int nbSlots = 5;
    public void Update() // for debug Purposes
    {
        UpdateFormation(Vector3.zero, Quaternion.identity, nbSlots);
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (slots != null)
            for (int i = 0; i < slots.Count; i++)
                Gizmos.DrawSphere(slots[i].position, 0.25f);
    }
}
