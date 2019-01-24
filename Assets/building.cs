using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class building : entity
{
    public List<node> affectednodes = new List<node>();
    public Resource[] Costs;
    //Priority of attack = Higher mean more interest for the enemy
    public float getValue
    {
        get { var x = 0f;
            foreach (var item in Costs)
                x += item.getAmount * item.Value;
            return x * (Hp/maximumHp);
        }
    }
    public float ConstructionEffort = 20;
     bool BeingBuild = false;
    public bool HasEnoughRessource(Resource[] x)
    {

        if (x.Length == 0) return true;
        foreach (var item in Costs)
        {
            bool ok = false;
            foreach (var z in x)
                if (item.name == z.name) { ok = true; ok = item.getAmount >= z.getAmount; }

            if (!ok) return false;

        }
        return true;
    }
    public bool IsBeingBuild { get { return BeingBuild; } }
    float currEffort = 0;
    [SerializeField]
    private GameObject gauge;
    public GameObject[] graphics;


    public void build(Vector3 position, Owner n)
    {
        BeingBuild = true;
         
        n.Building.Add(this);
        transform.position = position;
        foreach (var item in graphics)
            item.SetActive(false);

        graphics[0].SetActive(true);
    }


    private void OnMouseDown()
    {
        
    }
    public virtual void interact(entity e, float efficiency = 0)
    {
        if (e == this && BeingBuild) { Construction(efficiency); return; }
    }
    protected virtual void  Construction(float x )
    {
        currEffort += Time.fixedDeltaTime * x;
        if(currEffort > ConstructionEffort)
        {
            graphics[0].SetActive(false);
            graphics[1].SetActive(true);
            BeingBuild = false;
        }
    }
}
