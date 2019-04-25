using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class Wall : fortification
{
    //Reduce this amount from attack
 
    public GameObject[] WallBitMateral;
    static bool Choosing = false;
    static Wall las;
    bool diag;
    public Wall boundTo;
    fortification[] totalWall = new fortification[0];
    public DraggableTbox Dtox, annoyingTbox;
    float totalwallcost = 0;
    public void ChangeConnection(Wall z)
    {
        if (z == boundTo) return;
        Choosing = false;
        las = null;
        diag = false;
        annoyingTbox.gameObject.SetActive(false);
        Dtox.gameObject.SetActive(false);
        boundTo = z;
        StartCoroutine(switchWall(z));
         
    }
    public void ChangeTo()
    {
        if (Choosing) return;
        annoyingTbox.gameObject.SetActive(true);
        Dtox.gameObject.SetActive(false);
        diag = true;
        Choosing = true;
    }
    bool producingwall = false;
    IEnumerator switchWall(Wall x)
    {
        producingwall = true;
        yield return _RemoveWall();
        yield return new WaitForSeconds(.5f);
        yield return _produceWall();
        yield return new WaitForSeconds(.5f);
        producingwall = false;
        
        yield break;
    }
    public float CalculateWallCost(Vector3 x)
    {
        //The price is m * defense * tier /10 per Tick
        //This function is costly , thus saving it to a variable. Calling it once, but will be usefull later;
        var e = Vector3.Distance(x, transform.position);
        //Mexico will pay for it 
        totalwallcost = e * defense * costs[Tier].Gold / 10;
        return totalwallcost;

    }
    private void FixedUpdate()
    {
        if (diag)
        {
            if (las)
            {
                if (las != this) ChangeConnection(las);
            }
            else
            if (GameManager.LastClick)
            {
                var h = GameManager.LastClick;
                if (h != this &&h is Wall)
                    ChangeConnection(h as Wall);
            }       
               
                         
        }
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
    public void RemoveWall()
    {
        StartCoroutine(_RemoveWall());
    }
    public override bool ApprovedBuilding(Vector3 pos, Owner g)
    {

        return base.ApprovedBuilding(pos, g);
    }
    public override bool HasEnoughRessource(Dictionary<string, Goods> x, float g,bool t = false)
    {
        return base.HasEnoughRessource(x, g);
    }
    IEnumerator _produceWall()
    {
    
        while (!boundTo) yield return null;
        while (boundTo.BeingBuild && BeingBuild) yield return null;
     
        
        var e = Vector3.Distance(boundTo.transform.position, transform.position);

        var ex = new List<fortification>();
      
        transform.LookAt(boundTo.transform, Vector3.up);
        var z = transform.forward;//(boundTo.transform.position - transform.position).normalized ; doesn't work for some reason!?
 
        
        for (float i = 0; i < (e - .125); i += .25f)
        {
             yield return new WaitForSeconds(.01f);
       
            var pos = transform.position - Vector3.up * .5f + z * i;
            
            var h = Instantiate(WallBitMateral[Tier].gameObject, pos, Quaternion.identity).GetComponent<fortification>();
            h.TransferOwner(GetOwner);
            ex.Add(h);
            
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
        totalWall = ex.ToArray();
        CalculateWallCost(boundTo.transform.position);
        yield break;
    }
    IEnumerator _RemoveWall()
    {
        var e = new List<fortification>();
        if (totalWall == null || totalWall.Length <= 0) yield break;
        foreach (var item in totalWall)
            e.Add(item);

        if (e.Count == 0) yield break;
        var ef = transform.rotation;
    
        while (e.Count > 0)
        {
            yield return new WaitForSeconds(.01f);
            var y = 0f;
            var h = e[e.Count-1];
            while (y <= .15f)
            {
                transform.rotation = ef;
                y += Time.smoothDeltaTime;

                h.transform.position -= transform.up * Time.smoothDeltaTime;
                yield return new WaitForSeconds(.01f);
            }
            e.Remove(h);
            h.GetOwner.onLostEntites(h);
            Destroy(h.gameObject);
        }
     
        yield break;
    }
    public override void OpenContextMenu()
    {
        if (Choosing)
        {
            las = this;
            return;
        }
        base.OpenContextMenu();
        //   Dtox.Header.text = "Wall at " + transform.position;
        //  Dtox.gameObject.SetActive(true);
        annoyingTbox.gameObject.SetActive(true);
        ChangeTo();
    }
    public override void CloseContextMenu()
    {
        base.CloseContextMenu();
        Dtox.gameObject.SetActive(false);
        diag = false;
        annoyingTbox.gameObject.SetActive(false);
        Choosing = false;
    }

    protected override void Construction(float x)
    {
        base.Construction(x);
        gameObject.SetActive(true);
        // ProduceWall();
    }


}
