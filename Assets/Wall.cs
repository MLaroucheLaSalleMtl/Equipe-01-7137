using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class Wall : fortification
{
    //Reduce this amount from attack
 
    public GameObject[] WallBitMateral;
    public Wall boundTo;
    fortification[] totalWall;
    float totalwallcost = 0;
    public float CalculateWallCost(Vector3 x)
    {
        //The price is m * defense * tier /10 per Tick
        //This function is costly , thus saving it to a variable. Calling it once, but will be usefull later;
        var e = Vector3.Distance(x, transform.position);
        //Mexico will pay for it 
        totalwallcost = e * defense * costs[Tier].Gold / 10;
        return totalwallcost;

    }
    public override void PerTick()
    {
        base.PerTick();
        if(boundTo)        transform.LookAt(boundTo.transform, Vector3.up);
        GetOwner.Gold -= totalwallcost;
    }
    public void ProduceWall()
    {
        gameObject.SetActive(true);
        StartCoroutine(_produceWall()); 
    }
    public override bool ApprovedBuilding(Vector3 pos, Owner g)
    {

        return base.ApprovedBuilding(pos, g);
    }
    public override bool HasEnoughRessource(Dictionary<string, Goods> x, float g)
    {
        return base.HasEnoughRessource(x, g);
    }
    IEnumerator _produceWall( )
    {
    
        while (!boundTo) yield return null;
        while (boundTo.BeingBuild && BeingBuild) yield return null;
     
        
        var e = Vector3.Distance(boundTo.transform.position, transform.position);


      
        transform.LookAt(boundTo.transform, Vector3.up);
        var z = transform.forward;//(boundTo.transform.position - transform.position).normalized ; doesn't work for some reason!?
 
        print("Generating Walls for" + e + " m");
        for (float i = 0; i < (e - .125); i += .25f)
        {
             yield return new WaitForSeconds(.01f);
       
            var pos = transform.position - Vector3.up * .5f + z * i;
            
            var h = Instantiate(WallBitMateral[Tier].gameObject, pos, Quaternion.identity).GetComponent<fortification>();
            h.transform.rotation = transform.rotation;
            h.transform.parent = transform;
            h.transform.localPosition = Vector3.up * .5f + z * i;
            h.graphics[1].SetActive(true);
            h.graphics[0].SetActive(false);
            var y = 0f;
            while ( y <= .15f)
            {
                y += Time.smoothDeltaTime;
                h.transform.position = transform.position + Vector3.up * (.5f -.5f * y/.15f) + transform.forward * i;
                yield return null;
            }
            h.transform.position = transform.position + transform.forward * i;
            h.TransferOwner(GetOwner);
            h.defense = defense;
        }
        CalculateWallCost(boundTo.transform.position);
        yield break;
    }
    protected override void Construction(float x)
    {
        base.Construction(x);
        ProduceWall();
    }


}
