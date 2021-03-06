﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Garison : building
{
    [System.Serializable]
    public struct unitCreation
    {
        public Goods[] Costs;
        [Space]
        public float goldCost;
        public int civilianCost;
        public GameObject unit;
        public float timeToDeploy;
      
        [TextArea]
        public string DESC;
    }
    public GameObject WhereToGo;
    public unitCreation[] Units;
    public DraggableTbox tbox;
    public bool CanCreateCustomizable = false;
    public bool OverrideStats = false;
  
 
    [System.Serializable]
    public struct DeployementOrder
    {
        public unitCreation Unit;
        public SpecialUnit.stats stat;
        public float ExtraExperience ;
        public List<Goods> AdditionalCost;
        public float GetTimeToDeploy
        {
            get
            {
                var t = stat.AGI + stat.DEX + stat.END + stat.PER + stat.STR;
                t += Unit.timeToDeploy;
                return 1 + ExtraExperience / 20 + t;
            }
        }
        public float extraGold
        {
            get
            {
                var e = 0f;
                e += stat.STR * 33;

                e += ExtraExperience * .5f;
                e += stat.PER * 40;
                e += stat.END * 20;
                e += stat.DEX * 70;
                e += stat.AGI * 50;
                return e;

            }
        }
        public DeployementOrder (unitCreation u, SpecialUnit.stats s, float f, List<Goods> tr)
        {
            Unit = u;
            stat = s;
            ExtraExperience = f;
            AdditionalCost = tr;
        }
        public DeployementOrder(unitCreation u, SpecialUnit.stats s, float f)
        {
            Unit = u;
            stat = s;
            ExtraExperience = f;
            AdditionalCost = new List<Goods>();
        }
    }
    public Queue<DeployementOrder> UnitsToDeploy = new Queue<DeployementOrder>();

 
    private void Awake()
    {
        tbox = GetComponentInChildren<DraggableTbox>();
       
        /*if (GetOwner == null)
        {
            print("No owner,setting to owner[0]");
            TransferOwner(GameManager.owners[0]);
        }*/
        foreach (var item in buttons)
            item.SetActive(false);
        

        for (int i = 0; i < Units.Length; i++)
            buttons[i].SetActive(true);

        OnCompletedUnit += QUI.ForceDeQueue;

    }
    protected override void Start()
    {
        base.Start();
        if (GetOwner.IsPlayer) GameManager.instance.OnClick += OnClick;
    }
    public override void TransferOwner(Owner n)
    {
        base.TransferOwner(n);
        
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

    public bool HasEnoughRessource(DeployementOrder x, Dictionary<string,Goods> c)
    {
        if (GetOwner.Gold < (x.Unit.goldCost + x.extraGold)) return false;
        var et = x.AdditionalCost;
        foreach (var item in x.Unit.Costs)
            et.Add(item);
        if (et.Count == 0) return true;
  


        foreach (var item in et)
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
        if (z >= Units.Length) z= Random.Range(0, Units.Length);
        ProduceUnit(Units[z], GetOwner, !OverrideStats);
    }

    public void ProduceSpecialUnit(int v)
    {
        if (v >= Units.Length) v = Random.Range(0, Units.Length);
        ProduceUnit(Units[v], GetOwner);
    }
    public DeployementOrder deplToUse;
    public override void Death(bool destroy = true)
    {
        if(GetOwner.IsPlayer)
        GameManager.instance.OnClick -= OnClick;
        base.Death(destroy);
    }
    public void ProduceUnit(unitCreation c,Owner b, bool avoid = false)
    {
    
        var we = new DeployementOrder(c, deplToUse.stat, deplToUse.ExtraExperience);
        if (avoid) we = new DeployementOrder(c, new SpecialUnit.stats(), 0);
      
        if (!GameManager.DEBUG_GODMODE)
        {
            if (!HasEnoughRessource(we, b.Inventory))
            {if(b.IsPlayer) GameManager.ShowMessage("Not enough materials for unit!");
                return; }

        }
        b.Pay(c.unit.gameObject.GetComponent<unit>().GoldCost + we.extraGold);
        foreach (var item in we.Unit.Costs)
            we.AdditionalCost.Add(item);
        b.Pay(we.AdditionalCost.ToArray());

        QUI.CreateNewIcon(we );
        UnitsToDeploy.Enqueue(we);
    }
    bool setPlace;
    [SerializeField] GameObject setplaceUI;
    public void SetPosition()
    {
        setPlace = true;
        setplaceUI?.SetActive(false);
    }
    
    void OnClick(Vector3 a)
    {
        if (setPlace)
        {
            WhereToGo.transform.position = a;
            setplaceUI?.SetActive(true);
            setPlace = false;
        }
    }
    public override void GetRidOf()
    {
        if(GetOwner == Owner.Player)
        {
            GameManager.instance.OnClick -= OnClick;
        }
        base.GetRidOf();
    }
    void ActuallyDeploy(DeployementOrder de, Owner b)
    {
        var e = Instantiate(de.Unit.unit, transform.position + transform.forward, Quaternion.identity).GetComponent<unit>();
        e.TransferOwner(b);
       
        b.AddFighter(de.Unit.civilianCost);

        if(!e.Ordered)
        e.MoveTo(WhereToGo.transform.position);
        if(e is SpecialUnit)
        {
            var s = e as SpecialUnit;
            s.GainEXP(de.ExtraExperience);
            s.Stat += de.stat;
        }
        OnCompletedUnit?.Invoke(e);

    }
    
    public override void OpenContextMenu()
    {
        CanCreateCustomizable = GetOwner.HasResearch(18);
        base.OpenContextMenu();
        tbox.gameObject.SetActive(true);
        tbox.Header.text = "Garisson at" + transform.position.ToString();
        tbox.Texts[1].text = description;
        CustomizeButton.gameObject.SetActive(CanCreateCustomizable);
        OpenCustomization(false);
        SetButtons();
      

    }
    public virtual void SetButtons()
    {
        foreach (var item in buttons)
            item?.SetActive(true);


        if (!GetOwner.HasResearch(5)) buttons[1].gameObject.SetActive(false);
        if (!GetOwner.HasResearch(8)) buttons[3].gameObject.SetActive(false);
        if (!GetOwner.HasResearch(17)) buttons[9].gameObject.SetActive(false);
    }
    public override void interact(entity e, float efficiency = 0)
    {
        base.interact(e, efficiency);
    }
    float timer = 0;
    public void FixedUpdate()
    {
       
        if(UnitsToDeploy.Count > 0)
        {
            timer += Time.fixedDeltaTime;
            if (GetOwner.HasResearch(3)) timer += Time.fixedDeltaTime * .15f;
            if (timer > UnitsToDeploy.Peek().GetTimeToDeploy)
            {

                ActuallyDeploy(UnitsToDeploy.Dequeue(), this.GetOwner);
              
                    var t = unit.GetAlliesAtPosition(WhereToGo.transform.position, 5, GetOwner);
                    if (t.Length > 0)  GameManager.Formation(WhereToGo.transform.position, Vector3.zero, t, .1f);

            

                timer = 0;

            }
        
        }
        else
        {
            timer = 0;
        }
    }

    public void SetExperience(Slider x)
    {
       deplToUse.ExtraExperience = x.value;
        UpdateCustomUI();
    }
    [Header("Extra UI")]
    [SerializeField]
    public Text ui_end;
    [SerializeField]
     Text ui_str, ui_per, ui_agi, ui_dex, ui_experience, ui_TimeForCreate, ui_goldcost;

    public void UpdateDesc(int z )
    {
        tbox.Texts[1].text = Units[z].DESC + "\n\nCosts:" + Units[z].goldCost + " gp";
    }
    void UpdateCustomUI()
    {
        ui_str.text = deplToUse.stat.STR.ToString("00.0");
        ui_end.text = deplToUse.stat.END.ToString("00.0");
        ui_per.text = deplToUse.stat.PER.ToString("00.0");
        ui_agi.text = deplToUse.stat.AGI.ToString("00.0");
        ui_dex.text = deplToUse.stat.DEX.ToString("00.0");
        ui_goldcost.text = "Total Gold Cost : " + deplToUse.extraGold.ToString("00.0");
        ui_TimeForCreate.text = "Deployement Time:" + deplToUse.GetTimeToDeploy.ToString("00.0");
        ui_experience.text =  deplToUse.ExtraExperience.ToString("00.0");

    }
    public void StatsSetEND(Slider x) { deplToUse.stat.END = x.value; UpdateCustomUI(); }
    public void StatsSetSTR(Slider x) { deplToUse.stat.STR = x.value; UpdateCustomUI(); }
    public void StatsSetPER(Slider x) { deplToUse.stat.PER = x.value; UpdateCustomUI(); }
    public void StatsSetAGI(Slider x) { deplToUse.stat.AGI = x.value; UpdateCustomUI(); }
    public void StatsSetDEX(Slider x) { deplToUse.stat.DEX = x.value; UpdateCustomUI(); }

    public GameObject ExtraCuztomization;
    public void SetOverrideStats(Toggle t)
    {
        OverrideStats = t.isOn;
    }
    public void OpenCustomization(bool z)
    {
        ExtraCuztomization.gameObject.SetActive(z);
        UpdateCustomUI();
    }
    public GameObject CustomizeButton;

    public delegate void onUnitCompletedHandler(unit x);
    public event onUnitCompletedHandler OnCompletedUnit;
    public QueueCreatorUI QUI;


}
