using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit : entity
{
    [SerializeField]
     NavMeshAgent agi;
    //So it's 10m per turn or 1 nodes
    public string Name = "Security";
    public float Speed = 10;
    public float attack = 5;
    public float defense = 5;

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        infotext.text = Name + " HP: " + Hp +"  " + " ATK: " + attack + " \nSPD:" + Speed + "\nDEF" + defense;
    }
    Vector3 previousTarget;
    public void MoveTo(Vector3 v)
    {
        previousTarget = v;
        agi.SetDestination(v);
        agi.isStopped = false;
    }
}
