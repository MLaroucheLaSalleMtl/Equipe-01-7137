using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCore : building
{
    private void Start()
    {
      /*  if (GetOwner == GameManager.owners[0])
            StartCoroutine(DeathTEST());*/
    }
    IEnumerator DeathTEST()
    {
        yield return new WaitForSecondsRealtime(5);
        Death();
        yield break;
    }
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
    public override void Death(bool t = true)
    {
        base.Death();
        GameManager.SetGameOver();
        Camera.main.transform.position = transform.position - Vector3.right * 2 + Vector3.up * 2;
        Camera.main.transform.LookAt(transform);
        
    }

}
