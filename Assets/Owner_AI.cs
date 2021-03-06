﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Owner_AI : MonoBehaviour
{


    float _lifetime = 0;
    [Tooltip("Time between each command")]
    public float TBC = 5;

    public Owner owner;

    int rand = 0;
    building lastbuilding;
    private void Start()
    {
        if (owner.Cores.Count == 0) return;
        StartCoroutine(Act());
        owner.ai = this;
        rand = Random.Range(0, 5);
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



    bool church = false;

    IEnumerator Act()
    { var core = owner.Cores[0];

        SwitchBuilding(1);
     
        while (true)
        {
            if (core == null) break;
            Owner en = null;
            if (owner.OnBadTerm.Count > 0)
            {
                string ahah = owner.OnBadTerm[0].Name;
                foreach (var item in owner.OnBadTerm)
                {
                   if( owner.Relation[item.Name] > owner.Relation[ahah])
                    {
                        ahah = item.Name;
                    }  
                }
                en = GameManager.GetOwner(ahah);


            }
         

            
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
                      
                        v.ProduceUnit(Random.Range(0,6));
                    }
                }

           if(owner.Gold > 500 && !church)
            {
                SwitchBuilding(10);
                church = true;
            }
            
           
            //Give 60 sec before doing anything
            if (AvaillableUnit > 5 && _lifetime > 60)
                foreach (var item in owner.OnBadTerm)
                {

                    foreach (var x in owner.Units)
                    {
                        if (item.Building.Count > 0)
                            x.OrderedAttack(item.Building[Random.Range(0, item.Building.Count)]);
                        else x.OrderedAttack(item.Cores[0]);
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
    public static building Build(int c, Vector3 dir, CityCore s)
    {
        var y = GameManager.instance.Buildings[c].GetComponent<building>();
        dir.y = 0;
        building e = null;

        e = s;
        if (e == null) { return null; }

 
        var f = (s.transform.position - e.transform.position).normalized;
        var fx = e.transform.position + e.transform.TransformDirection(dir) * (1 + (s.GetOwner.Building.Count / 8)) * (3);

        var f22 = new RaycastHit();
        var rc = Physics.Raycast(fx + Vector3.up * 2, Vector3.down, out f22, 8, GameManager.instance.Interatable);

      var t = GameManager.instance.PlaceBuilding(c, fx, Quaternion.identity, s.GetOwner);
       t.transform.position = fx;
        t.transform.LookAt(s.GetOwner.Cores[0].transform, Vector3.up);


        if (rc) { fx.y = f22.point.y + .1f; }


     


        print(s.GetOwner.Name + " built building " + t.name);
        return t;

    }
    public void BuildingPlanning(int c,Vector3 dir)
    {
        building++;
        var y = GameManager.instance.Buildings[c].GetComponent<building>();
        building e = null;
         
        e = owner.Cores[0];
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
        if (lastbuilding.GetComponent<Garison>())
        {
            var u = lastbuilding.GetComponent<Garison>();
            u.WhereToGo.transform.position = e.transform.position + e.transform.TransformDirection(dir) * 3 * (1 + (owner.Building.Count / 8)) * (3);
        }


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
        

        _lifetime += Time.deltaTime;
    }
}
