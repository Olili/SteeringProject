
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
        public Slot(Vector3 _pos) { position = _pos; occupied = false; }
    }
    protected List<Slot> slots;
    public void Awake()
    {
        slots = new List<Slot>();
    }
    abstract public Vector3 GetSlot(Vector3 position);
    public virtual void UpdateFormation(Vector3 origin, Quaternion rotation, int nbSlots)
    {
            // Check if slot need update
        if (transform.position == origin && rotation == transform.rotation && nbSlots == slots.Count)
            return;
        if (nbSlots <= 0)
            return;
            // remove unnecessary slots
        for (int i = slots.Count-1; i >= nbSlots; i--)
            slots.RemoveAt(i);
            // updateSlots
        for (int i = 0; i < nbSlots; i++)
        {
            if (i < slots.Count)
                slots[i].position = GetSlotPos(origin, rotation, nbSlots, i);
            else
                slots.Add(new Slot(GetSlotPos(origin, rotation, nbSlots, i)));
        }
    }

    abstract protected Vector3 GetSlotPos(Vector3 origin, Quaternion orientation,int nbSlots,int i);
}
