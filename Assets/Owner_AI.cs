using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Owner_AI : MonoBehaviour
{


    float _lifetime = 0;
    [Tooltip("Time between each command")]
    public float TBC = 5;

    public Owner owner;
 
    building lastbuilding;
    private void Start()
    {
        if (owner.Cores.Count == 0) return;
        StartCoroutine(Act());
        owner.ai = this;

        owner.modRelation(GameManager.owners[0], -100);
        owner.modRelation(GameManager.owners[0], -100);
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
    { var core = owner.Cores[0];

        SwitchBuilding(1);
        while (true)
        {

            if (core == null || owner.Cores[0] == null) break; 

            
            if (!lastbuilding || !lastbuilding.IsBeingBuild)
            {

                if (HasBuilding(typeof(habitation))){
                    foreach (var item in owner.Building)
                    {
                        if(item is habitation)
                        {
                            var y = (item as habitation);
                            if (y.CanUpgrade)
                            {
                                y.Upgrade();
                                lastbuilding = y;
                                break;
                            }
                        }
                    }
                }
                if (( owner.getHousingSpace < owner.totalPopulation  )   && HasEnoughRessource(0))
                    SwitchBuilding(0);



                if ( owner.getSecurity < owner.totalPopulation && HasEnoughRessource(1) && !HasBuilding(typeof(Garison)))
                {
                    if (garisson < 3)
                    {
                        garisson++;

                        SwitchBuilding(1);
                    }
                  
                }

                if(NumberOfBuilding(typeof(Tower)) * 10 < owner.Building.Count)
                {
                    SwitchBuilding(6);
                }
                if (!owner.BuilderCenter && HasEnoughRessource(2))
                    SwitchBuilding(2 );

                if (!HasBuilding(typeof(Gatherer)))
                {
                    var tt = Physics.OverlapSphere(owner.Cores[0].transform.position, 10);
                    foreach (var item in tt)
                    {
                        var h = item.gameObject.GetComponent<node>();
                        if (h)
                        {
                            if (h.resource.Value > 0 && h.resource.Name != "NILL")
                            {

                                NearBuildingPlanning(3,
                                    new Vector3(h.transform.position.x, owner.Cores[0].transform.position.y + 0.661f,
                                    h.transform.position.z));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (!HasBuilding(typeof(Storage))
                        && owner.Storages.Count * 3< owner.Building.Count)
                        SwitchBuilding(7); 
                }

 
            }
            if (owner.getSecurity > owner.Units.Count || owner.Units.Count < 40)
                foreach (var item in owner.Building)
                {
                    if (item is Garison)
                    {
                        var v = item as Garison;
                        v.ProduceUnit(0);
                    }
                }

            //Give 60 sec before doing anything
            if (AvaillableUnit > 5 && _lifetime > 60)
                foreach (var item in owner.OnBadTerm)
                {

                    foreach (var x in owner.Units)
                    {
                        if (item.Building.Count > 0)
                            x.Attack(item.Building[Random.Range(0, item.Building.Count)]);
                        else x.Attack(item.Cores[0]);
                    }

                }

            GameManager.Formation(owner.Cores[0].transform.position + Vector3.right  * 3+ ( core.transform.right * .5f - core.transform.forward/3)* (+ Mathf.Sqrt(owner.Units.Count)),
                owner.Cores[0].transform.right,
                GetAvaillableUnit(owner.Units.Count),.15f);

           /* print("ENEMIES : " + owner.OnBadTerm.Count);*/
            // If there are people on bad term with me , attack them  if there is 10 availlable unit
          

           
            yield return new WaitForSeconds(TBC / MainUI.TPSmult);
        }
      
        

    }
    int AvaillableUnit 
    {
        get {
            var x = 0;
            for (int i = 0; i < owner.Units.Count ; i++)
            {
                if (owner.Units[i].HasIssuesCommand) continue;
                x++;
            }

            return x;
        }
    }
    unit[] GetAvaillableUnit(int z)
    {

          List<unit> lol = new List<unit>();
            var x = 0;
            for (int i = 0; i < owner.Units.Count && x < z; i++)
            {
            if (owner.Units[i].HasIssuesCommand) continue;
            x++;
            lol.Add(owner.Units[i]);
            }
        return lol.ToArray();
   
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

        if (GameManager.DEBUG_GODMODE) return true;
       var w = GameManager.instance.Buildings[z].GetComponent<building>().HasEnoughRessource(owner.Inventory, owner.Gold);
    
        return w;
    }


    int building = 0;
    public void SwitchBuilding(int c)
    {
        if (!HasEnoughRessource(c) && !GameManager.DEBUG_GODMODE)
        {
            print(owner.Name + " do not have enough ressource for " + GameManager.instance.Buildings[c].name);
            return;
        }
            

            if (building > 7)
            building = 0;

      
        var dec = Vector3.right;
        switch (building)
        {
            case 4:
            case 0: dec = Vector3.forward;
                break;
            case 5:
            case 1 :
                dec = Vector3.left;
                break;
            case 6:
            case 2:
                dec = - Vector3.forward;
                break;
            case 7:
            case 3 :
                dec = Vector3.right;
                break;
            default:
                break;
        }

        if (building > 3)
            BuildingPlanning(c, Quaternion.AngleAxis(45, Vector3.up) * dec);
        else BuildingPlanning(c, dec);


    }
    public void BuildingPlanning(int c,Vector3 dir)
    {
        building++;
        var y = GameManager.instance.Buildings[c].GetComponent<building>();
        building e = null;
         
        e = owner.Cores[0];
        /*
        if(owner.Building.Count > 0)
            e = owner.Building[owner.Building.Count-1];
        foreach (var item in owner.Building)
        {
          
            if(Vector3.Distance(item.transform.position,owner.Cores[0].transform.position) 
                > Vector3.Distance(e.transform.position, owner.Cores[0].transform.position))
            {
                e = item;
            }
        }*/
        if (e == null) {  return; }  
    
        
        var f = (owner.Cores[0].transform.position - e.transform.position).normalized ;
        var fx = e.transform.position + e.transform.TransformDirection(dir)* (1 + (owner.Building.Count/8))* (3);
       
        /*
         for (int i = 0; i < 360; i+= 15)
        {
            f = Quaternion.Euler(0, 0, i).eulerAngles;
            f.y = 0;
            f.Normalize();
            fx = e.transform.position + e.transform.TransformDirection(dir) * (.6f + y.SpaceNeed.magnitude);
            if (y.Intersect(fx)) break;
        }

         */


        var f22 = new RaycastHit();
        var rc = Physics.Raycast(fx + Vector3.up * 2, Vector3.down, out f22, 8, GameManager.instance.Interatable);

        lastbuilding = GameManager.instance.PlaceBuilding(c,fx , Quaternion.identity, owner);
        lastbuilding.transform.position = fx;
        lastbuilding.transform.LookAt(owner.Cores[0].transform, Vector3.up);
       
  
        if (rc) {   fx.y = f22.point.y + .1f; }
    
      
        print(owner.Name + " built building " + lastbuilding.name);
    }
    public void NearBuildingPlanning(int c, Vector3 pos)
    {

        var fx = pos;

        var f22 = new RaycastHit();
        var rc = Physics.Raycast(fx + Vector3.up * 3, Vector3.down, out f22, 5, GameManager.instance.Interatable);
        lastbuilding = GameManager.instance.PlaceBuilding(c, pos ,
            Quaternion.identity, owner);


       
        if (rc) { fx = f22.point; }


        lastbuilding.transform.position = fx;
        print(owner.Name + " built building " + lastbuilding.name);
    }

    int NumberOfBuilding(System.Type t)
    {
        var ss = 0;
        foreach (var item in owner.Building)
            if (item.GetType() == t)
                ss++;

        return ss;
    }

    private void FixedUpdate()
    {
        if (TBC > 1) TBC -= Time.deltaTime * 0.01f;

        _lifetime += Time.deltaTime;
    }
}
