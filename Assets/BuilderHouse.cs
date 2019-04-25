using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderHouse : building
{
    public float constructionBonus = 0;

    protected override void Construction(float x)
    {
        base.Construction(x);
        if (!IsBeingBuild) GetOwner.BuilderCenter = true;
    }
}
