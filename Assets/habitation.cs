using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class habitation : building
{
    public float[] size;
    public float[] Taxe = new float[3] { 1, 5, 35 };


    public override void PerTick()
    {
        base.PerTick();
        float bonus = 1;

        if (GetOwner.HasResearch(0)) bonus += .1f;
        if (GetOwner.HasResearch(1)) bonus += .2f;
        if (GetOwner.HasResearch(12)) bonus += .5f;
        GetOwner.GainGold((Taxe[Tier] * bonus * GetOwner.totalPopulation)/GetOwner.totalPopulation);
    }
}
