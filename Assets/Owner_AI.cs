using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Owner_AI : MonoBehaviour
{

    [Tooltip("Time between each command")]
    public float TBC = 1;

    public Owner owner;
    public bool Test;
    building lastbuilding;
    private void Start()
    {
        StartCoroutine(Act());
    }
    int garisson = 0;
    /*0 habitation
     * 1 Garison
     * 2 builder house
     * 3 gathering houe
     * 5 Wooden wall
     * 6 Tower
     * 7 Woods Storage
     * 
     1*/
    IEnumerator Act()
    {
        while (true)
        {
 
            if (!lastbuilding || !lastbuilding.IsBeingBuild)
            {

                if (( owner.getHousingSpace < owner.totalPopulation || owner.Gold > 20)   && HasEnoughRessource(0))
                    BuildingPlanning(0, Vector3.right);



                if ( owner.getSecurity < owner.totalPopulation && HasEnoughRessource(1) && !HasBuilding(typeof(Garison)))
                {
                    if (garisson < 3)
                    {
                        garisson++;

                      //  BuildingPlanning(1, Vector3.right);
                    }
                  
                }

                if (!owner.BuilderCenter && HasEnoughRessource(2))
                    BuildingPlanning(1, Vector3.left);

                if (!HasBuilding(typeof(Gatherer)))
                {
                    var tt = Physics.OverlapSphere(owner.Cores[0].transform.position, 10);
                    foreach (var item in tt)
                    {
                        var h = item.gameObject.GetComponent<node>();
                        if (h)
                        {
                            if (h.resource.Value > 1)
                            {

                                NearBuildingPlanning(3,
                                    new Vector3(h.transform.position.x, owner.Cores[0].transform.position.y + 0.661f,
                                    h.transform.position.z));
                                break;
                            }
                        }
                    }
                }

                if (!HasBuilding(typeof(Storage)) && HasBuilding(typeof(Gatherer)) && owner.Storages.Count < 5)
                    BuildingPlanning(7, Vector3.left);



             

            }
            if (owner.getSecurity > owner.Units.Count || owner.Units.Count < 20)
                foreach (var item in owner.Building)
                {
                    if (item is Garison)
                    {
                        var v = item as Garison;
                        v.ProduceUnit(0);
                    }
                }
            yield return new WaitForSeconds(TBC);
        }
       
    }
    bool HasBuilding(System.Type t)
    {
        foreach (var item in owner.Building)
            if (item.GetType() == t)
                return true;

        return false;
    }
    bool HasEnoughRessource(int z)
    {

        if (Test) return true;
       var w = GameManager.instance.Buildings[z].GetComponent<building>().HasEnoughRessource(owner.Inventory, owner.Gold);
    
        return w;
    }
    
    public void BuildingPlanning(int c,Vector3 dir)
    {

        var y = GameManager.instance.Buildings[c].GetComponent<building>();
        building e = null;
        foreach (var item in owner.Building)
        {
            e = item;
            if(Vector3.Distance(item.transform.position,owner.Cores[0].transform.position) 
                > Vector3.Distance(e.transform.position, owner.Cores[0].transform.position))
            {
                e = item;
            }
        }
        if (e == null) return;

       
        var f = (owner.Cores[0].transform.position - e.transform.position).normalized ;

        for (int i = 0; i < 360; i+= 15)
        {
            f = Quaternion.Euler(0, 0, i).eulerAngles;
            f.y = 0;
            f.Normalize();
            if (y.Intersect(e.transform.position + (f + dir) * (1f + y.SpaceNeed.magnitude))) break;
        }
      
            
        lastbuilding = GameManager.instance.PlaceBuilding(c, e.transform.position + (f + dir) * (1f +y.SpaceNeed.magnitude) , Quaternion.Euler(f), owner);
        print(owner.Name + " built building " + lastbuilding.name);
    }
    public void NearBuildingPlanning(int c, Vector3 pos)
    {
     

        lastbuilding = GameManager.instance.PlaceBuilding(c, pos ,
            Quaternion.identity, owner);
        print(owner.Name + " built building " + lastbuilding.name);
    }
    private void FixedUpdate()
    {
       
       
    }
}
