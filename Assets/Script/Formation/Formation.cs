
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 * Système  : 
 * Chaques unité cherche à atteindre une formation. 
 * Une formation peut être controllée et déplacée. 
 * 	1 Génération 
--> Une formation est un ensemble de positions liée entre elles. 
	--> Ces formation peuvent être générée en fonction d'une position et du nombre d'entités la composant.
	--> Ces peuvent être orienter et se déplacer.
	2 link agent -- emplacement :
Il existe une formation pour beaucoup d'entités. 
	--> Il faut lier les entités et les emplacement d'une formation.
	--> Une entité doit trouver l'emplacement à suivre. 
	--> On doit savoir si l'emplacement est pris ou non.
	-->
*/
//[RequireComponent(typeof(Steering))]
abstract public class Formation : MonoBehaviour {

    List<Steering> steeringUnits;
    Vector3 lastPos;
    Quaternion lastRotation;
    public class Slot
    {
        public Vector3 position;
        public bool occupied;
        public Slot(Vector3 _pos) { position = _pos; occupied = false; }
    }
    protected List<Slot> slots;
    public Formation()
    {

    }

    public virtual void Awake()
    {
        slots = new List<Slot>();
        steeringUnits = new List<Steering>();
    }
    Slot GetSlot(Vector3 position)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].occupied == false)
            {
                slots[i].occupied = true;
                return slots[i];
            }
        }
        //Debug.Log("No position Found");
        return null;
    }
    public void AddUnit(Steering newUnit)
    {
        steeringUnits.Add(newUnit);
        UpdateFormation(steeringUnits);
    }
    
    public virtual void UpdateFormation(List<Steering> _newSteeringUnits = null, bool forcedUpdate = false)
    {
        if (_newSteeringUnits == null)
        {
            _newSteeringUnits = steeringUnits;
        }
        else if (!forcedUpdate)
        {
                // Check if slot need update
            if (transform.position == lastPos && lastRotation == transform.rotation && _newSteeringUnits.Count == slots.Count
            && (_newSteeringUnits != null || _newSteeringUnits == steeringUnits))
                return;
        }
      
        if (_newSteeringUnits.Count <= 0)
            return;

        Vector3 direction = transform.position - lastPos;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // remove unnecessary slots
        for (int i = slots.Count-1; i >= _newSteeringUnits.Count; i--)
            slots.RemoveAt(i);
        // updateSlots
        
        Vector3 slotPos = Vector3.zero;
        for (int i = 0; i < _newSteeringUnits.Count; i++)
        {
            slotPos = transform.rotation * GetSlotPos(_newSteeringUnits.Count, i) + transform.position;
            
            if (i < slots.Count)
                slots[i].position = slotPos;
            else
            {
                slots.Add(new Slot(slotPos));
            }
        }
        lastPos = transform.position;
        lastRotation = transform.rotation;
        steeringUnits = _newSteeringUnits;
        ChooseSlotForEntity();
    }
    public void ChooseSlotForEntity()
    {
        slots.ForEach((slot) => { slot.occupied = false; });
        for (int i = 0; i < steeringUnits.Count; i++)
        {
            FormationFolllowing formationFollowing = steeringUnits[i].FindBehavior<FormationFolllowing>();
            formationFollowing.UpdateSlot(GetSlot(steeringUnits[i].transform.position)); 
        }
    }
    abstract protected Vector3 GetSlotPos(int nbSlots, int i);

    public void GUIShow()
    {
        int yIdent = 30;
        int iPos = 0;
        GUIShow(ref yIdent,ref iPos);
    }
    public virtual void GUIShow(ref int yIdent, ref int iPos)
    {
        GUI.Label(new Rect(25, (iPos++) * yIdent, 1000, yIdent), name.ToUpper());
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (slots != null)
            for (int i = 0; i < slots.Count; i++)
                Gizmos.DrawSphere(slots[i].position, 0.25f);
    }
}
