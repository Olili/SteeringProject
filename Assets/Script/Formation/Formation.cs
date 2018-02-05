using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Système  : 
 * Chaques unité cherche à atteindre une formation. 
 * Une formation peut être controllée et déplacée. 
*/

abstract public class Formation : MonoBehaviour {

    protected class Slot
    {
        public Vector3 position;
        public bool occupied;
    }
    protected List<Slot> slots;
    public void Awake()
    {
        slots = new List<Slot>();
    }
    abstract public Vector3 GetSlot(Vector3 position);
    abstract public void UpdateFormation(Vector3 origin, Quaternion orientation,int nbSlots);
}
public class CircleFormation : Formation
{
    public override Vector3 GetSlot(Vector3 position)
    {
        throw new NotImplementedException();
    }

    public override void UpdateFormation(Vector3 origin, Quaternion orientation, int nbSlots)
    {
        if (transform.position == origin && orientation == transform.rotation && nbSlots == slots.Count)
            return;
        float angle = 2*Mathf.PI / slots.Count;
        for (int i = 0; i < slots.Count;i++)
        {
            //slots[i].position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            slots[i].occupied = false;
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (slots!=null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                Gizmos.DrawSphere(slots[i].position, 1);
            }
        }
    }
    
}
