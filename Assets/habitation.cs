using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class habitation : building
{
    public float size = 10;
    public float Taxe = 1;

    public override void PerTick()
    {
        base.PerTick();
        GetOwner.GainGold((Taxe * GetOwner.totalPopulation)/GetOwner.totalPopulation);
    }
}
