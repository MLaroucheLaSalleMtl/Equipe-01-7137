using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCore : building
{
    public override bool ApprovedBuilding(Vector3 pos, Owner z)
    {
        return true;
    }
    public override void TakeDamage(float t,DamageType p = DamageType.Null)
    {
        if (GameManager.DEBUG_GODMODE) return;
        base.TakeDamage(t);
    }
    public override void TakeDamage(float t, entity e, DamageType p = DamageType.Null)
    {
        base.TakeDamage(t, e, p);
        GetOwner.modRelation(e.GetOwner, -100);
    }
    public override void Death()
    {
        base.Death();
        Time.timeScale = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
