using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owner  
{
    public string Name = "";

    public delegate void EntitiesHandler(entity e);
    public EntitiesHandler onNewEntites, onLostEntites; 
    int population = 10;
    public bool BuilderCenter = false, ResearchCenter = false, MerchantCenter = false;
    public int totalPopulation { get { return population + builder; } }
    public float Gold = 100;
    int builder = 0, fighter = 0;

    public void Gain(Goods r)
    {
        var s = r.Exploit();

        if (Inventory.ContainsKey(r.Name))
        {
            Inventory[s.Name].Merge(s);
            return;
        }
 
        Inventory.Add(s.Name,s);
    }
    public void Gain(Goods r,int h)
    {
      if(r.getAmount <= 0)
        {
            Debug.Log("Not enough");
        }
      
        var s = r.Exploit(h);
        if(s == null)
        {

            return;
        }

        if (Inventory.ContainsKey(r.Name))
        {
            Inventory[s.Name].Merge(s);
            return;
        }
        Debug.Log(Name + " gains " + h.ToString() + " " + r.Name);
        Inventory.Add(s.Name,s);
    }
    //Two similar function, we could add a function for those type of stuff
    public int AddFighter(int m)
    {
        if (m == 0) return fighter;
        if (m >= population)
        {
            fighter += population;
            population = 0;
        }
        else if (m < 0 && Mathf.Abs(m) >= fighter)
        {
            population += fighter;
            fighter = 0;
        }
        else
        {
            population -= m;
            fighter += m;
        }

        
        return fighter;
    }
    public int AllocateBuilder(int m)
    {
        if(m == 0) return builder;
        if (m >= population)
        {
            builder += population;
            population = 0;
        }
        else if (m < 0 && Mathf.Abs(m) >= builder)
        {
            population += builder;
            builder = 0;
        }
        else
        {
            population -= m;
            builder += m;
        }

        
        return builder;
    }
    public int getHousingSpace { get
        {
            var x = 0f;
            for (int i = 0; i < Building.Count; i++)
            {
                if(Building[i].type == building.BuildingType.Habitation)
                {
                    if (!Building[i].IsBeingBuild)
                        x +=Mathf.Ceil((Building[i] as habitation).size);
                }
            }
            return (int)x;

        } }
    public float getSecurity
    {

        get
        {

            float x = fighter * 5;
            for (int i = 0; i < Building.Count; i++)
            {
                if (Building[i].type == building.BuildingType.Defense)
                {
                    x += Building[i].getValue / 10;
                }
            }
            return  x;
        }
    }
    public Dictionary<string, Goods> Inventory = new Dictionary<string, Goods>();
    public List<unit> Units = new List<unit>();
    public List<building> Building = new List<building>();
    
    public List<entity> GetEntities
    {
        get
        {
            var e = new List<entity>();
            //foreach are costly, let's change 'em later when we need to optimize
            foreach (var item in Units) e.Add(item);
            foreach (var item in Building) e.Add(item);

            return e;
        }
    }
    float constructEffortBase = 10f;

    void OnEntitesReceived(entity e)
    {
        if (e is building)
        {
            Building.Add(e as building);
 
        }
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
    float timer = 0;
    public void Routine()
    {
        var x = 
        ProductionEfficiency;
        
        timer += Time.fixedDeltaTime;
        if(timer > 5)
        {
            populationChange = popchange;
            timer = 0;
        }
       
    }

    private int populationChange;
    public int getPopulationGrowth
    {
        get { return populationChange; }
    }
    
       
        
     int popchange
    {
        get
        {

            var y = population;
            var def = getSecurity;
            var mult = 1;
            if (population > 1000) mult = 100;
            if (getHousingSpace > population * .75f)
            {
                //Moderate growth , randomize too for the small scope of a small town
                if (Random.Range(0, 1f) > .66f)
                {

                    //Some amount of randomness is  always fun tbh if you have enough defense, growth is prolific
                    if (def > population * 1.5f) population += Random.Range(2, 5 * mult);
                    else
                    if (def > population * .5f) population += 1 * mult;
                    else
                    //When you have that much amount of people , securite become more and more a concern
                    if (def < population * .33f && population > 1000)
                        population -= Random.Range(1, 2 * mult);
                }
            }
            return (population - y);
        }
  
    }
    public float ProductionEfficiency
    {
        get
        {
            var x = constructEffortBase + builder * 5;
            bool useB = false;
            foreach (var item in Building)
            {
                if (item.IsBeingBuild) {

                    useB = builder > 0;


                    if (x - item.ConstructionEffort <= 0)
                    {
                        item.interact(item, x);
                    }
                    else
                    {
                        x -= item.ConstructionEffort;
                        var s = Mathf.Clamp(x, 0, item.ConstructionEffort);
                        item.interact(item, s);
                    }
                }
               
            }
            //1 gold per second per builder
            if (useB) Gold -= builder * Time.fixedDeltaTime;
            return x;
        }
    }
}
