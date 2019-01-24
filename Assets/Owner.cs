using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owner  
{
    public string Name = "";

    public delegate void EntitiesHandler(entity e);
    public EntitiesHandler onNewEntites, onLostEntites;

    public List<Resource> Inventory = new List<Resource>();
    public List<unit> Units = new List<unit>();
    public List<building> Building = new List<building>();
    public List<entity> GetEntities
    {
        get
        {
            var e = new List<entity>();
            foreach (var item in Units) e.Add(item);
            foreach (var item in Building) e.Add(item);

            return e;
        }
    }
    float constructEffortBase = 2f;

    void OnEntitesReceived(entity e)
    {
        if (e is building) Building.Add(e as building);
        else if (e is unit) Units.Add(e as unit);
    }
    void OnEntitiesLost(entity e)
    {
        if (e is building) Building.Remove(e as building);
        else if (e is unit) Units.Remove(e as unit);
    }
    public Owner()
    {
        onLostEntites += OnEntitiesLost;
        onNewEntites += OnEntitesReceived;
    }
    public void Routine()
    {
        var x = 
        ProductionEfficiency;
    }
    
    public float ProductionEfficiency
    {
        get
        {
            var x = 0f;
            x += constructEffortBase;
            foreach (var item in Building)
            {
                if (item.IsBeingBuild) { x -= item.ConstructionEffort;
                    var s = x;
                    if (s > item.ConstructionEffort)
                        s = item.ConstructionEffort;
                        item.interact(item,s ); }
            }
            return x;
        }
    }
}
