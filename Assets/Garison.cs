using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garison : building
{
    [System.Serializable]
    public struct unitCreation
    {
        public Goods[] Costs;
        public float goldCost;
        public int civilianCost;
        public GameObject unit;
    }
    public GameObject WhereToGo;
    public unitCreation[] Units;

    private void Awake()
    {
        /*if (GetOwner == null)
        {
            print("No owner,setting to owner[0]");
            TransferOwner(GameManager.owners[0]);
        }*/
        foreach (var item in buttons)
            item.SetActive(false);
        

        for (int i = 0; i < Units.Length; i++)
            buttons[i].SetActive(true);


    }
    public bool HasEnoughRessource(unitCreation x, Goods[] c)
    {
        if (GetOwner.Gold < x.goldCost) return false;
        if (x.Costs.Length == 0) return true;
        foreach (var item in x.Costs)
        {
            bool ok = false;
            foreach (var z in c)
                if (item.Name == z.Name)
                { ok = z.getAmount >= item.getAmount; }

            if (!ok) return false;

        }
        return true;
    }

    public bool HasEnoughRessource(unitCreation x, Dictionary<string,Goods> c)
    {
        if (GetOwner.Gold < x.goldCost) return false;
        if (x.Costs.Length == 0) return true;
        foreach (var item in x.Costs)
        {
            bool ok = false;
     
            if(c.ContainsKey(item.Name))
            {
                ok = c[item.Name].getAmount >= item.getAmount;

            }

            if (!ok) return false;

        }
        return true;
    }
    public void ProduceUnit(int z )
    {
        ProduceUnit(Units[z], GetOwner);
    }
    public void ProduceUnit(unitCreation c,Owner b)
    {
        if (!GameManager.DEBUG_GODMODE)
        {
            if (!HasEnoughRessource(c, b.Inventory))
            { print("Not enough ressources!"); return; }

        }

        var e = Instantiate(c.unit, transform.position + transform.forward, Quaternion.identity).GetComponent<unit>();
        e.TransferOwner(b);
        b.Gold -= e.GoldCost;
        b.AddFighter(c.civilianCost);
        e.MoveTo(WhereToGo.transform.position);
    }
    public override void interact(entity e, float efficiency = 0)
    {
        base.interact(e, efficiency);
    }
}
