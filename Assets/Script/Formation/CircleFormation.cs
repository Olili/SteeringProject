using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CircleFormation : Formation
{
    float radiusFactor =1;
    public override void Awake()
    {
        base.Awake();
    }
    protected override  Vector3 GetSlotPos(int nbSlots, int i)
    {
        float deltaA = 2 * (float)Math.PI / nbSlots;
        float ray = nbSlots * radiusFactor * 0.5f; // should use entity Size.

        return new Vector3(Mathf.Cos(deltaA * i) * ray, 0, Mathf.Sin(deltaA * i) * ray);
    }
    public override void GUIShow(ref int yIdent, ref int iPos)
    {
        base.GUIShow(ref yIdent, ref iPos);
        GUI.Label(new Rect(25, (iPos++) * yIdent, 200, yIdent), "radiusFactor " + radiusFactor);
        radiusFactor =GUI.HorizontalSlider(new Rect(0, (iPos++) * yIdent, 100, 25), radiusFactor, 0.1f, 10);
    }

}
