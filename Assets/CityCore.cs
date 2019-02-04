using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCore : building
{
    public override bool ApprovedBuilding(Vector3 pos, Owner z)
    {
        return true;
    }
    public override void TakeDamage(float t)
    {
        if (GameManager.DEBUG_GODMODE) return;
        base.TakeDamage(t);
    }
}
