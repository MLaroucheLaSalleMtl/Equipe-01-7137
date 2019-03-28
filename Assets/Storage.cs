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

    protected override void Start()
    {
        base.Start();
        StoraVisual.transform.position = transform.position + FullnessOffset * (currentstorage / MaxSpace);

    }

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
        StoraVisual.transform.position = transform.position + FullnessOffset * (currentstorage / MaxSpace);
        anim?.SetTrigger("Gain");
    }
    [SerializeField]
    Animator anim;
    
    public override void Death()
    {
        base.Death();
        if (GetOwner!=null) GetOwner.Pay(new Goods(StorageType, currentstorage));
    }

}
