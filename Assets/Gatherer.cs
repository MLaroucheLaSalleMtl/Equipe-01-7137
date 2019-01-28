using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : building
{
    public GameObject[] RessourceAddon;
    bool HasResource = false;
    public Goods Primary;
    public int Yield = 1;
    
    // Start is called before the first frame update


    private float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        //Different ressource, different timing OR we can use one fix timer
        if(timer > 1 && HasResource  )
        {
            GatherRessource();
            timer = 0;
        }
    }
    protected virtual void GatherRessource()
    {
        //Ressource are currently finite, we should find a way to replenish 'em
        if(Primary.getAmount > 0) GetOwner.Gain(Primary,Yield);
    }
    protected override void Construction(float x)
    {
        base.Construction(x);
        if (!BeingBuild)
        {
            if (  affectednodes.Count > 0)
            {
                foreach (var item in affectednodes)
                {
                    if (item.resource.getAmount  > 0)
                    {
                        Primary = item.resource;
                        HasResource = true;
                        if (item.resource.Name == "Wood")
                        {
                            RessourceAddon[0].SetActive(true);
                        }
                        return;
                    }
                }
           
               
            }
        }
    }
}
