using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCore : building
{
    public override bool ApprovedBuilding(Vector3 pos, Owner z)
    {
        return true;
    }
}
