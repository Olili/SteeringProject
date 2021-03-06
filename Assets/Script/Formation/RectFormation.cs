﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectFormation : Formation
{
    //[SerializeField] float ratio = 1; // largeur / hauteur.
    //float forcedWidth = 0;
  
    protected override Vector3 GetSlotPos(int nbSlots, int i)
    {
        Vector3 position;
        float squareSizeF = (Mathf.Sqrt(nbSlots));
        int squareSizeI = Mathf.CeilToInt(squareSizeF);

        //float width = squareSizeF;
        //float height = squareSizeI;
        position = new Vector3(i % squareSizeI, 0, i / squareSizeI) *2 - new Vector3(squareSizeF, 0, squareSizeF);

        return position;
    }
}
