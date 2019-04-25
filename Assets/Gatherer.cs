using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : building
{
    public GameObject[] RessourceAddon;
    bool HasResource = false;
    public Goods Primary;
    public int Yield = 1;
    [Range(.01f,1000)]
    public float GatheringSpeed = .25f;

    // Start is called before the first frame update


    public override void TakeDamage(float t, DamageType p = DamageType.Null)
    {
        base.TakeDamage(t, p);
        
    }
    public override void Death(bool destroy = true)
    {
        base.Death(destroy);
        print("lol");
    }
    private float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (GetOwner.HasResearch(6)) timer += Time.fixedDeltaTime * .25f;
        //Different ressourc, different timing OR we can use one fix timer
        if(timer > (  Primary.hardness/GatheringSpeed) && HasResource  )
        {
            GatherRessource();
            timer = 0;
        }
    }

 
    protected virtual void GatherRessource()
    {
        //Ressource are currently finite, we should find a way to replenish 'em
        if(Primary.getAmount > 0) GetOwner.Gain(Primary,Yield,transform.position);
 
    } 
    public override bool ApprovedBuilding(Vector3 pos, Owner g)
    {

        var x = Physics.OverlapSphere(pos, .5f);
        if (x.Length == 0) return false;

        for (int i = 0; i < x.Length; i++)
        {

            if (x[i].gameObject.GetComponent <node > ())
            {
          
                var t = x[i].gameObject.GetComponent<node>();

                if (t.resource.getAmount > 1 && t.resource.Name != "NILL")
                    return true && base.ApprovedBuilding(pos, g);
            }
        }
        return false;
    }
    protected override void Construction(float x)
    {
        base.Construction(x);
        if (!BeingBuild)
        {

            var w = Physics.OverlapSphere(transform.position, .5f);

            for (int i = 0; i < w.Length; i++)
            {

                if (w[i].gameObject.GetComponent<node>())
                {

                    var t = w[i].gameObject.GetComponent<node>();

                    if (t.resource.getAmount > 1 && t.resource.Name != "NILL")
                    {
                        Primary = t.resource;
                             HasResource = true;
                        if (t.resource.Name == "Wood")
                            RessourceAddon[0].SetActive(true);
                        else if (t.resource.Name == "Stone")
                            RessourceAddon[1].SetActive(true);
                    }
                }
            }
           /* if (  affectednodes.Count > 0)
            {
                foreach (var item in affectednodes)
                {
                    if (item.resource.getAmount  > 0)
                    {
                        Primary = item.resource;
                        HasResource = true;
                        if (item.resource.Name == "Wood")
                            RessourceAddon[0].SetActive(true);
                        else if (item.resource.Name == "Stone")                
                            RessourceAddon[1].SetActive(true);
 
                        return;
                    }
                }
           
               
            }*/
        }
    }
}
