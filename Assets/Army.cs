using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 
    Army : unit
{
    public List<unit> Soldier = new List<unit>();
    public override float Hp { get {

            var x = 0f;
            for (int i = 0; i < Soldier.Count; i++)
                x += Soldier[i].Hp;

            return x;
        } set { Hp = Hp; } }
    public override void TakeDamage(float t,DamageType p = DamageType.Null)
    { 
        var x = Random.Range(0, Soldier.Count);
        Soldier[x].TakeDamage(t,p);
    }
    public override float getAttack
    {
        get
        {
            var x = 0f;
            for (int i = 0; i < Soldier.Count; i++)
                x += Soldier[i].getAttack;

            return x;
        }


    }
  /*  public override float GetMovingSpeed
    {
        get
        {
            var x = 0f;
            for (int i = 0; i < Soldier.Count; i++)
            {
                x += Soldier[i].Speed;
            }
            x /= (Soldier.Count *  1.33f);
            
            return x;
        }
    }
    */
    private void Awake()
    {
        Name = name;
    }
    public void AddToArmy (unit e)
    {
        Soldier.Add(e);
        e.transform.parent = transform;
        e.gameObject.SetActive(false);
        transform.localScale = Vector3.one * (1 + Soldier.Count / 100);
        
    }
    public void AddToArmy(unit[] e)
    {
        foreach (var item in e)
        {
            AddToArmy(item);
        }
    }

    public void RegainRank(unit e)
    {
        e.transform.parent = transform;
        e.gameObject.SetActive(false);
        transform.localScale = Vector3.one * (1 + Soldier.Count / 100);
    }
    public void SendScout()
    {
        var e = Soldier[Soldier.Count-1];
        e.transform.position = transform.position + -transform.forward;
        e.transform.parent = null;
    }
    public void MergeArmy(Army z)
    {
        foreach (var item in z.Soldier)
        {
            Soldier.Add(item);
            item.transform.parent = transform;
        }
        Destroy(z.gameObject);
    }
    public override string ToString()
    {
       
                

        return "Army made up of " + Soldier.Count + " units";
    }

}
