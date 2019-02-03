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
        GetOwner.GainGold((Taxe[Tier] * GetOwner.totalPopulation)/GetOwner.totalPopulation);
    }
}
