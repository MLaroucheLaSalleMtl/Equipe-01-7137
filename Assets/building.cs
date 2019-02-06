using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class building : entity
{
     
    public List<node> affectednodes = new List<node>();
    public int Tier =0;
 //   public Goods[] _cost;
    [System.Serializable]
    public struct Cost
    {
        public Goods[] materials;
        public float Gold;
        public float ConstructionEffort;
    }
    public Cost[] costs;
    public float SpaceNeed = 1;
    public float RequiredCloseness = 5;
    public enum BuildingType
    {
        Simple = 0,
        Habitation = 1,
        Defense = 2,
        Utilities =3
     
    }
    public BuildingType type;
    [Header("UI")]
    public GameObject ContextMenu;
    public Text ContextMenuText;
    public GameObject[] buttons;
    public GameObject Bar;
    [TextArea]
    public string description= " a normal building";
    //Priority of attack = Higher mean more interest for the enemy
    public float getValue
    {
        get { var x = 0f;
            foreach (var item in costs[Tier].materials)
                x += item.getAmount * item.Value;
            return x * (Hp/maximumHp);
        }
    }
    private void Start()
    {
     if(ContextMenu)   ContextMenu.SetActive(false);
    }
  
    protected bool BeingBuild = false;

 


    public virtual bool HasEnoughRessource(Dictionary<string ,Goods> x, float g)
    {
        if (GameManager.DEBUG_GODMODE) return true;
        if(g < costs[Tier].Gold + GoldCost  )
        {
            print("Not enough gold!");
            return false;
        }
        
     
        foreach (var item in costs[Tier].materials)
        {
            bool ok = false;
            if (x.ContainsKey(item.Name)){
                ok = x[item.Name].getAmount >= item.getAmount;

                print(ok);
            }

            if (!ok) return false;

        }
        return true;
    }
    public void Upgrade()
    {
        var e = Tier;
        Tier++;
        if(Tier >= costs.Length)
        {
            Tier = e;
            print("Maximum upgrades reach");
            return;
        }
        if (!HasEnoughRessource(GetOwner.Inventory,GetOwner.Gold))
        {
            Tier = e;
            return;
        }

        
        build(transform.position, GetOwner );
        CloseContextMenu();
    }
    public void GetRidOf()
    {
        if (Tier == 0)
            GetOwner.Gold += costs[0].Gold;
        else
            GetOwner.Gold += costs[Tier].Gold/2;

        GetOwner.onLostEntites(this);
        Destroy(this.gameObject);
    }
    public bool IsBeingBuild { get { return BeingBuild; } }
    float currEffort = 0;
    [SerializeField]
    private GameObject gauge;
    public GameObject[] graphics;


    public virtual bool ApprovedBuilding(Vector3 pos, Owner g)
    {
        bool z = false;
        var x = Physics.OverlapSphere(pos, RequiredCloseness, GameManager.instance.Interatable,QueryTriggerInteraction.Collide);
        if (x.Length == 0) return false;
      
        for (int i = 0; i < x.Length; i++)
        {
          
            if (x[i].gameObject.GetComponent<building>())
            {
         
                var t = x[i].gameObject.GetComponent<building>();

                //Walls doesn't count
                if (!(this is Wall) && ((t is Wall) || t is fortification)) continue;

                    if (t.GetOwner == g)
                    return true;
            }
        }

        return z;
    }
    public void build(Vector3 position, Owner n)
    {
        BeingBuild = true;
        n.Gold -= (GoldCost + costs[Tier].Gold);
       // n.Building.Add(this);
        transform.position = position;
        foreach (var item in graphics)
            item.SetActive(false);

        if (Bar) Bar.transform.parent.gameObject.SetActive(true);
        graphics[0].SetActive(true);
    }


bool ctxmenu = false;
    public virtual void OpenContextMenu()
    {
        if (BeingBuild) return;
        if (!ctxmenu) ContextMenu.SetActive(true);
        ContextMenuText.text = description;

    }
    public void CloseContextMenu()
    {
        ctxmenu = false;
        ContextMenu.SetActive(false);
    }

    private void OnMouseDown()
    {
        //So we do not click when there is an UI;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!BeingBuild)
            interact(GameManager.selection[0]);

        OpenContextMenu();
    }
    float tim = 0;
    public virtual void PerTick()
    {

    }
    public virtual void interact(entity e, float efficiency = 0)
    {
        if (e == this && BeingBuild) { Construction(efficiency); return; }


        tim += Time.fixedDeltaTime;
        if (tim > 5)
        {
            PerTick();
            tim = 0;
        }
    }
    protected virtual void  Construction(float x )
    {
        currEffort += Time.fixedDeltaTime * x;

        if (Bar && costs[Tier].ConstructionEffort > 0) { Bar.transform.localScale = new Vector3((currEffort/costs[Tier].ConstructionEffort)*8f,1,1);} 
        if(currEffort >=costs[Tier].ConstructionEffort)
        {
            graphics[0].SetActive(false);
            graphics[Tier+1].SetActive(true);

            var e = Physics.OverlapBox(transform.position, Vector3.one / 2f);
            foreach (var item in e)
            {

                if (item.gameObject.GetComponent<node>())
                {
                    affectednodes.Add(item.gameObject.GetComponent<node>());
                }
            }
            BeingBuild = false;
            if (Bar) Bar.transform.parent.gameObject.SetActive(false);
        }
    }
}
