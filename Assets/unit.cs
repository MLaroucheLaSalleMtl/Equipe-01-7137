using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit : entity
{
    [SerializeField]
     NavMeshAgent agi;
    //So it's 10m per turn or 1 nodes
    public float Speed = 10;
    Vector3 previousTarget;
    public void MoveTo(Vector3 v)
    {
        previousTarget = v;
        agi.SetDestination(v);
        agi.isStopped = false;
    }
}
