using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Text Population, Gold, Security, Growth;
    public GameObject Jobs;
    

   public Text Builder, Merchant, Research,Civilian;
    public Text UnitInfo;
    Owner lastOwner;


    
    private void Awake()
    {
      
    }
    public void ShowUI(Owner n,entity e)
    {
        lastOwner = n;
        Population.text = "POP: " + n.totalPopulation.ToString() + "/ HOUSE:" + n.getHousingSpace;
        Gold.text = "Gold: " + n.Gold.ToString("0.00");
        float s = Mathf.Clamp(((n.getSecurity + 10) / (1 + n.totalPopulation)), 0, 1.5f) * 100;
        Security.text = "Security: "+ s.ToString("0") + "%";
        Growth.text = n.getPopulationGrowth.ToString("0");
        Builder.gameObject.SetActive(n.BuilderCenter);
        Merchant.gameObject.SetActive(n.MerchantCenter);
        Research.gameObject.SetActive(n.ResearchCenter);
        Civilian.text = "Civilian: " +(n.totalPopulation - n.AllocateBuilder(0) - n.AddFighter(0)).ToString(); 
        //For now
        Builder.text = "Builder:" + n.AllocateBuilder(0).ToString();
        Merchant.text = "Merchant:" + 0.ToString();
        Research.text = "Researcher:" + 0.ToString();

       Jobs.SetActive( Builder.gameObject.activeSelf || Merchant.gameObject.activeSelf || Research.gameObject.activeSelf);


        if (e)
        {
            UnitInfo.text = e.ToString();
        }
    }

    public void AddBuilder(int x) { lastOwner.AllocateBuilder(x); }
    public void AddMerchant(int x) { }
    public void AddResearch(int x) { }


}
