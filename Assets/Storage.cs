using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : building
{
    [Header("Storage")]
    public string StorageType = "Wood";
    [Range(1,10000)]
    public float MaxSpace = 100;
    public GameObject StoraVisual;
    public Vector3 FullnessOffset = Vector3.one;

   

    public bool HasEnoughSpace(Goods x)
    {
        return (GetTotalSpace + x.getAmount * x.StorageSize) <= MaxSpace;
    }

    public bool HasEnoughSpace(float x)
    {
        return (GetTotalSpace + x) <= MaxSpace;
    }
    public override void interact(entity e, float efficiency = 0)
    {
        base.interact(e, efficiency);

        StoraVisual.transform.position = transform.position + FullnessOffset * (currentstorage/ MaxSpace);
    }
    float currentstorage = 0;
    public float GetTotalSpace
    {
        get
        {
            return currentstorage;
        }
    }
   
    public void addStorage(float x)
    {
        currentstorage += x;
    }
    
    public override void Death()
    {
        base.Death();
        if (GetOwner!=null) GetOwner.Pay(new Goods(StorageType, currentstorage));
    }

}
