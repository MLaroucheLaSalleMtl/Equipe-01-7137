using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owner  
{
    public string Name = "";
    public Faction faction;
    public Vector3 vector3;
    public NodesLineRenderer nodeLineRenderer;
    public Color MainColor;
    public delegate void OnGainHandler(Goods g, Vector3 p);
    public delegate void OnRelationship(Owner n, Owner z, float x);
    public event Technology.OnResearchHandler OnAccomplishResearch;
    public OnRelationship OnRelationModification;
    public OnGainHandler OnGain;
    public delegate void EntitiesHandler(entity e);
    public EntitiesHandler onNewEntites, onLostEntites;
    public event EntitiesHandler OnEntitiesChange; 
    int population = 10;
    public bool BuilderCenter = false, ResearchCenter = false, MerchantCenter = false;
    public int totalPopulation { get { return population + builder + fighter; } }
    public float Gold = 100;
    int builder = 0, fighter = 0,scientist;
 
    public float GetScientificOutput
    {
        get
        {
            var t = 0f; foreach (var item in Labs)
                t += item.ResearchPointsPerTick;
            return t * ScienceMod * 1 + (scientist * t / 100) ;
        }
    }
    int MaximumNumberOfscientist
    {
        get
        {
            var t = 0; foreach (var item in Labs)
                t += item.MaximumScientist;
            return t;
        }
    }
    public void OnResearchDone(Technology T)
    {   
        Researched.Add(T.ID, T);
        CurrentTechnology = null;
        OnAccomplishResearch?.Invoke(T);

    }
    public Technology[] GetAvaillableTech
    {
        get
        {
            var e = new List<Technology>();

            foreach (var item in Technology.ResearchKnownToMankind)
            {
                if (Researched.ContainsValue(item.Value)) continue;
                if (CurrentTechnology == item.Value) continue;
                bool addit = true;               
                if(item.Value.Dependencies.Length> 0)
                    foreach (var c in item.Value.Dependencies )
                        if (!Researched.ContainsKey(c)) addit = false;

                if (addit) e.Add(item.Value);


            }
            return e.ToArray(); ;
        }
    }
    public bool HasResearch(int x)
    {
        return Researched.ContainsKey(x);
    }
    public void AdvancedResearch(float x)
    {
        if (CurrentTechnology == null) return;

        if(Gold > 0)
        {
            //each scientist give one extra percent on the yield, but cost 1 gold per tick
            x += scientist * x / 100;
            Pay(scientist);
        }
     
        
        CurrentTechnology.Research(x * ScienceMod);
    }
 

    public void SetTechnology(int T)
    {
        foreach (var item in Technology.ResearchKnownToMankind[T].Dependencies)
        {
            if (!Researched.ContainsKey(item))
            {
                if (IsPlayer) GameManager.ShowMessage("You can't research This yet");
                return;
            } 
        }
        CurrentTechnology = Technology.ResearchKnownToMankind[T];
    }
    public Technology CurrentTechnology;
    public Dictionary<string, int> Relation = new Dictionary<string, int>();
  
    public Dictionary<int, Technology> Researched = new Dictionary<int, Technology>();

    public  bool IsPlayer
    {
        get { return this == GameManager.owners[0]; }
    }

 

    public void modRelation(Owner x, int z)
    {
        if (x == this) return;

        if (  Relation.ContainsKey(x.Name))
             Relation[x.Name]+= z;
        else Relation.Add(x.Name, z);

        if (Relation[x.Name] <= -10 && !OnBadTerm.Contains(x)) OnBadTerm.Add(x);
        else if (Relation[x.Name] > 50 && !OnGoodTerm.Contains(x)) OnGoodTerm.Add(x);
        OnRelationModification?.Invoke(this, x, z);
    }
    //Quick Access
    public List<Owner> OnBadTerm = new List<Owner>(), OnGoodTerm = new List<Owner>();
    public Owner_AI ai;
    //Some population initiated coitus more often than other
    float baseFertilityRate = 1.00f;
    float fertilityMod = 1f, EconomyMod = 1f, ScienceMod = 1f;
    public float FertilityRate { get { return baseFertilityRate * fertilityMod; } }

    // instantiate factions
    BorderCalculation border = new BorderCalculation();
    NodesLineRenderer nodesLineRenderer = new NodesLineRenderer();
    List<List<node>> nodes = new List<List<node>>();
    List<node> nodesToRender = new List<node>();
    
     public void Start()
    {
        Gold += 100;
        GenFactions();
        foreach (var item in Technology.ResearchKnownToMankind)
            item.Value.OnFinish += OnResearchDone;
    }

    public void GenBorder()
    {
        nodesToRender = border.CornerDraw(border.GetInitBorderCalculation(vector3, this), this);
      
    }
   
    void GenFactions()
    {
        nodes = border.GetInitBorderCalculation(vector3, this);
        nodesToRender = border.CornerDraw(nodes, this);

        //     nodeLineRenderer = GameManager.instance.transform.Find(Name).gameObject.GetComponent<NodesLineRenderer>();
        nodeLineRenderer = GameObject.Instantiate(GameManager.instance.NodeRendererPrefab, GameManager.instance.transform).GetComponent<NodesLineRenderer>();
        nodeLineRenderer.name = Name;
        faction = new Faction(Name, vector3, nodesToRender, nodes, nodeLineRenderer);
        faction.GenFrontieres();



         Vector3[] borderpath = new Vector3[nodeLineRenderer.lineRenderer.positionCount];
         nodeLineRenderer.lineRenderer.GetPositions(borderpath);
        var center = Vector3.zero;

        //Adjust the Heigth of each position for visual appeal
            //for (int i = 0; i < nodeLineRenderer.lineRenderer.positionCount; i++)
            //   borderpath[i] += Vector3.up * 4;
            //nodeLineRenderer.lineRenderer.SetPositions(borderpath);



        //Get the center from the borer
        foreach (var item in borderpath)
            center += item;

        center /= borderpath.Length;

        //Set the City center in... the center
        var ccpos = center;
        var rch = new RaycastHit();
        //raycast for more precision
        var hitConfirmed = Physics.Raycast(ccpos + Vector3.up * 3, Vector3.down, out rch, 111, GameManager.instance.Interatable);
        var cc = GameObject.Instantiate(GameManager.instance.Buildings[4], center - Vector3.up * 4, Quaternion.identity).GetComponent<CityCore>();
        if (hitConfirmed) { ccpos.y = rch.point.y + .2f;   }
        cc.transform.position = ccpos;
        cc.TransferOwner(this);
        
        //Change the NLR center so we see the name 
        nodeLineRenderer.transform.position = center - Vector3.up * 11;
        //And the color
        nodeLineRenderer.lineRenderer.material.color= MainColor;
        nodeLineRenderer.lineRenderer.endColor = MainColor;
        //Naming
        nodeLineRenderer.txt.text = Name;
        //odesLineRenderer.Gen(faction);
    }


    [System.Serializable]
 public struct multiplier
    {
        public float fertility, economy, storageEfficiency;

        //One For Each type A B C, There will also be one for each unit
        public struct SoliderMultiplier
        {
            public float AttackMod,
                SpeedMod,
                DefenseMod,
                HpMod,
                CostMod,
                RecoveryMod,
                RangeMod;


        }
    }
    public void Gain(Goods r,int h)
    {
        int ok = -1;
        for (int i = 0; i < Storages.Count; i++)
        {
            if (Storages[i].StorageType == r.Name)
            {

                if (Storages[i].HasEnoughSpace(h* r.StorageSize))
                {

                    ok = i;
                    break;
                }
            }
        }
         


        if (ok< 0)
        {
            //Debug.Log("Not enough storage");
            return;
        }
        var s = r.Exploit(h);
        if(s == null)
              return;
     

        if (Inventory.ContainsKey(r.Name))
        {
            Inventory[s.Name].Merge(s);
            Storages[ok].addStorage(Mathf.Abs(h));
            return;
        }
       // Debug.Log(Name + " gains " + h.ToString() + " " + r.Name);
       
        Inventory.Add(s.Name,s);
        Storages[ok].addStorage(Mathf.Abs(h));
    }
    public void Gain(Goods r, int h, Vector3 pos)
    {
        Gain(r, h);
        if(OnGain != null)OnGain(r, pos);
    }
    public void Pay(Goods  x)
    {

        foreach (var item in Storages)
            if (item.StorageType == x.Name)
            {
                item.addStorage(-x.getAmount);
                break;
            }
        
        if (Inventory.ContainsKey(x.Name))
        {
            if (x.getAmount >= Inventory[x.Name].getAmount)
                Inventory.Remove(x.Name);
            else
            {
                Inventory[x.Name].amount -= x.getAmount;
            }
        }
    }
    public void Pay( Goods[] x)
    {
        foreach (var item in x)
            Pay(item);


    }
    public void Pay(float x)
    {
        Gold -= x;
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

    public int AllocateScientist(int m)
    {
        if (m == 0) return scientist;
        if (m >= population)
        {
     
                scientist += population;
                population = 0;
 
        }
        else if (m < 0 && Mathf.Abs(m) >= scientist)
        {
            population += scientist;
            scientist = 0;
        }
        else
        {
            if(scientist + m <= MaximumNumberOfscientist)
            {
                population -= m;
                scientist += m;
            }
            else
            {
                scientist = MaximumNumberOfscientist;
                population -= (MaximumNumberOfscientist - scientist);

            }
      
        }
        return scientist;
    }
    public int getHousingSpace { get
        {
            var x = 0f;
            for (int i = 0; i < Building.Count; i++)
            {
                if(Building[i].type == building.BuildingType.Habitation)
                {
                    if (!Building[i].IsBeingBuild)
                        x +=Mathf.Ceil((Building[i] as habitation).size[(Building[i] as habitation).Tier]);
                }
            }
            return (int)x;

        } }
    public float getSecurity
    {

        get
        {
            // 1 fighter = 5 def so 5 person protect
            float x = fighter * 5;
            for (int i = 0; i < Building.Count; i++)
            {
                if (Building[i].type == building.BuildingType.Defense)
                {
                    x += Building[i].getValue / 100;
                }
            }
            //lol should be game over at this point ahah but let's be safe
            if (totalPopulation == 0) return x / 1;
            else
            return  x/ totalPopulation;
        }
    }

   
    public Dictionary<string, Goods> Inventory = new Dictionary<string, Goods>();
    public List<unit> Units = new List<unit>();
    public List<building> Building = new List<building>();
    public List<CityCore> Cores = new List<CityCore>();
    public List<Storage> Storages = new List<Storage>();
    public List<Laboratory> Labs = new List<Laboratory>();
    public bool Settled = false;
    public List<entity> GetEntities
    {
        get
        {
            var e = new List<entity>();
            //foreach are costly, let's change 'em later when we need to optimize
            foreach (var item in Units) e.Add(item);
            foreach (var item in Building) e.Add(item);
            foreach (var item in Cores) e.Add(item);

            return e;
        }
    }
    float constructEffortBase = 10f;

    void OnEntitesReceived(entity e)
    {

        if (e is CityCore)
        {
            Settled = true;

            Cores.Add(e as CityCore);
        }
        else if (e is Storage)
            Storages.Add(e as Storage);
        else if (e is Laboratory)    
            Labs.Add((e as Laboratory));       
        else if (e is building)
            Building.Add(e as building);
        else if (e is unit) {(e as unit).ChangeColor(MainColor); Units.Add(e as unit); }
    

        _getbuildings = GetBuildings();
        OnEntitiesChange?.Invoke(e);
    }
    void OnEntitiesLost(entity e)
    {
        if (e is CityCore) Cores.Remove(e as CityCore);
        else if (e is Storage) Storages.Remove(e as Storage);
        else if (e is building) Building.Remove(e as building);
        else if (e is unit) Units.Remove(e as unit);
        else if (e is Laboratory)
            Labs.Remove((e as Laboratory));



        _getbuildings = GetBuildings();
        OnEntitiesChange?.Invoke(e);
    }
    System.Random randy = new System.Random();
    public Owner()
    {

        //Random.Range(.6f, 1.5f);
        baseFertilityRate = randy.Next(50,150)/100;
        ScienceMod  = randy.Next(50, 150) / 100; 

        onLostEntites += OnEntitiesLost;
        onNewEntites += OnEntitesReceived;
    }
    float timer = 0,tim2;

    int generations = 0;
    public void Generation()
    {
        //Mitosis doesn't exist in human
        if(totalPopulation >= 2)
        populationChange = (int)((totalPopulation * KidPerPerson)/2);
        population =(int)(population * KidPerPerson);
        generations++;
    }

    
    public void Routine()
    {
        var x = 
        ProductionEfficiency;
        buildingRoutine();
        timer += Time.fixedDeltaTime;
        tim2 += Time.fixedDeltaTime;
        if(timer > 5)
        {
            populationChange = immigration;
            timer = 0;
        }
        if(tim2 > GameManager.SecondPerGenerations)
        {
            Generation();
            tim2 = 0;
        }


        for (int i = 0; i < Units.Count; i++)        
            Units[i].AI();
        
       
    }
    void buildingRoutine()
    {
        for (int i = 0; i < Building.Count; i++)
            Building[i].interact(Building[i]);
 
        foreach (var item in Cores)
            item.interact(item);
        foreach (var item in Labs)
            item.interact(item);
    }
    private int populationChange;
    public int getPopulationGrowth
    {
        get { return populationChange; }
    }
    public static Owner Player
    {
        get { return GameManager.owners[0]; }
    }

    public void GainGold(float g)
    {
        Gold += g * EconomyMod; 

    }

    private List<building> _getbuildings = new List<building>();
    public List<building> GetBuildings()
    {

        var x = new List<building>();
        BuilderCenter = false;
        ResearchCenter = Labs.Count > 0;
        for (int i = 0; i < Building.Count; i++)
        {
            x.Add(Building[i]);
            if (Building[i] is BuilderHouse) BuilderCenter = true; 
        }
     

        for (int i = 0; i < Storages.Count; i++)
            x.Add(Storages[i]);
        for (int i = 0; i < Labs.Count; i++)
            x.Add(Labs[i]);
    
        return x;
    }
    float KidPerPerson
    {
        get
        {
            if (totalPopulation == 0) return 1;
            // This will force the player to expand more and more

            // Fertility Rate > Security > Wealth > Space >Infrastructure efficiency
          
            // 10% of Production Efficiency + 20% of Ratio of Gold/Person + 30% security + base Fertility rate* 30% + HousingSpace/People Ratio 20%
            return 1 +  (FertilityRate * .1f +  (Gold / totalPopulation) * 0.1f + ProductionEfficiency * .1f) * Mathf.Clamp((getHousingSpace / totalPopulation), 0f, 1) *Mathf.Clamp(getSecurity,.1f,1.5f);
        }
    }
       
        
     int immigration
    {
        get
        {

            var y = population;
            var def = getSecurity;
            var mult = 1;
            if (population > 1000) mult = 100;
           
            
            
            
            
            //Foreigner
            if (getHousingSpace > population * .75f)
            {
                //Moderate growth , randomize too for the small scope of a small town
                if (Random.Range(0, 1f) > .66f)
                {

                    //Some amount of randomness is  always fun tbh if you have enough defense, growth is prolific
                    if (def >1.5f) population += Random.Range(2, 5 * mult);
                    else
                    if (def > .5f) population += 1 * mult;
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
            var e = 0f;
            bool useB = false;
            foreach (var item in Cores)
                item.interact(item, 1);
 
            foreach (var item in _getbuildings)
            {
                if (item.IsBeingBuild) {

                    useB = builder > 0;
                    e += item.costs[item.Tier].ConstructionEffort;

                    if (x - item.costs[item.Tier].ConstructionEffort <= 0)
                    {
                        item.interact(item, x);
                    }
                    else
                    {
                        x -= item.costs[item.Tier].ConstructionEffort;
                        var s = Mathf.Clamp(x, 0, item.costs[item.Tier].ConstructionEffort);
                        item.interact(item, s);
                    }
                }
               
            }
            //1 gold per second per builder
            if (useB) Gold -= builder * Time.fixedDeltaTime;
            if (e <= 0) e = 1;
            return (constructEffortBase + builder * 5)/e;
        }
    }
}
