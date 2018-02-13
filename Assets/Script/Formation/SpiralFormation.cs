using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralFormation : Formation
{
    private int nbBranch = 3;
    private float angleCircle = (float)Math.PI * 0.05f;

    protected override Vector3 GetSlotPos(int nbSlots, int i)
    {
        int nbCircle = Mathf.CeilToInt((float)nbSlots / nbBranch);

        float angleBranch = 2 * (float)Math.PI / nbBranch;

        int idCircle = (i ) % (nbCircle);
        int idBranch = Mathf.FloorToInt((float)i / nbCircle);

        float angle = angleCircle * idCircle + angleBranch * idBranch;
        float length = 0;
        length =2+ idCircle*1.5f;

        return new Vector3(Mathf.Cos(angle) * length, 0, Mathf.Sin(angle) * length);
    }
    public override void GUIShow(ref int yIdent,ref int iPos)
    {
        base.GUIShow(ref yIdent, ref  iPos);

        GUI.Label(new Rect(0, (iPos++)* yIdent, 200, yIdent), "NbBranch " + nbBranch);
        nbBranch =(int) GUI.HorizontalSlider(new Rect(0, (iPos++) * yIdent, 100, 10), nbBranch, 1, 10);

        GUI.Label(new Rect(0, (iPos++) * yIdent, 200, yIdent), "Delta Spiral Angle " + angleCircle);
        angleCircle = GUI.HorizontalSlider(new Rect(0, (iPos++) * yIdent, 100, 10), angleCircle, 0, 0.5f*(float) Math.PI);
    }
}
