using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static float HpMult = 1, SpeedMult = 1, TimeScaleMult =1, TPSmult = 1;

    public Text[] Txt;
    public GameObject Jobs, attack, cursor;
    public RectTransform BSelection;

    public TechnologyUI[] TUIS;
    public TechnologyUI currentTUI;
    public Text ResearchDesc;
    public SliderTextBox ResearchBar;
    public GameObject ResearchButton, ResearchWindow;
    public textBox RelationshipWindow;
    public Owner Selected;

    public Text aicom, speedmul, healthmul, timescale;

    public void OpenRelationshipWindow( int x)
    {
        OpenRelationshipWindow(GameManager.owners[x]);
    }


    public void OpenRelationshipWindow(Owner x)
    {
        Selected = x;
        RelationshipWindow.gameObject.SetActive(true);
        RelationshipWindow.Header.text = "Relationship with " + Selected.Name + " at " + GameManager.owners[0].Relation[GameManager.owners[0].Name];
        RelationshipWindow.Texts[1].text = "Your current relation with " + Selected.Name + " is " + GameManager.owners[0].Relation[GameManager.owners[0].Name]
            + ". You can trade ressource or lands to augments your relationship. There is the possibility to start a war or stop it depending on your score. ";
    }
    public void MakeWar()
    {
        //TODO add your logic here
        Selected.modRelation(GameManager.owners[0], -100);
    }
    public void MakePeace()
    {
        //TODO add your logic here
        Selected.modRelation(GameManager.owners[0], 100);
    }
    public void CloseRelationWindow()
    {
        Selected = null;
        RelationshipWindow.gameObject.SetActive(false);
    }
    public void OpenResearchWinodw (bool z)
    {
        if (!Owner.Player.ResearchCenter)
        {
            ResearchButton.SetActive(false);
            ResearchWindow.SetActive(false);
            return;
        }
        ResearchWindow.SetActive(z);
        UpdateTechUI();

    }
    public void ShowResearchDESC(TechnologyUI t)
    {
        var I = t.Tech;
        ResearchDesc.text = Technology.ResearchKnownToMankind[I].Description;
        if(Technology.ResearchKnownToMankind[I].Dependencies.Length > 0)
        {
            ResearchDesc.text += "Prerequisites: ";
            foreach (var item in Technology.ResearchKnownToMankind[I].Dependencies)
            {
                ResearchDesc.text += Technology.ResearchKnownToMankind[item].Name + "\n";
            }
        }
        ResearchDesc.text +=
        "\n\nCost: " + Technology.ResearchKnownToMankind[I].PtsNeed + " RP"
            + "\nYour current RP/s is " + Owner.Player.GetScientificOutput + " RP/s";
    }
    public void SetTech(TechnologyUI T)
    {
      
        Owner.Player.SetTechnology(T.Tech);
       
        UpdateTechUI();
        ResearchBar?.gameObject.SetActive(true);

   

    }
    public void OnResearchDone(Technology T)
    {
        UpdateTechUI();
        ResearchBar?.gameObject.SetActive(false);
        if(ResearchBar)ResearchBar.slider.value = 0;
    }


    void UpdateTechUI()
    {
      
        currentTUI.gameObject.SetActive(false);
        if (Owner.Player.CurrentTechnology != null) {
            currentTUI.SetTech(Owner.Player.CurrentTechnology);
            if (ResearchBar)
            {
                ResearchBar.Header.text = Owner.Player.CurrentTechnology.Name;
                ResearchBar.Texts[1].text = Owner.Player.CurrentTechnology.Name;
                ResearchBar.slider.minValue = 0;
                ResearchBar.slider.maxValue = Owner.Player.CurrentTechnology.PtsNeed;
            }
      
        }
        var y = Owner.Player.GetAvaillableTech;
        foreach (var item in TUIS)       
           item.gameObject.SetActive(false);       
        for (int i = 0; i < TUIS.Length && i < y.Length; i++)
            TUIS[i].SetTech(y[i]);

  

        ResearchDesc.text = "Select a Tech to research";
        
    }
    private void Start()
    {
        Owner.Player.OnAccomplishResearch += OnResearchDone;
        Owner.Player.OnEntitiesChange += OnPlayerEntitiesChanges;
        ResearchButton.SetActive(false);
        ResearchWindow.SetActive(false);
        RelationshipWindow?.gameObject.SetActive(false);
    }

    private void OnPlayerEntitiesChanges(entity e)
    {
       ResearchButton.SetActive( Owner.Player.ResearchCenter);
     
    }

    public Text Builder, Merchant, Research,Civilian;
    public Text UnitInfo;
    public textBox Desc, StatsInfo;

    public Animator Action_sticker;
    Owner lastOwner;
    [Header("Pause Menu")]


    public bool GameisPaused = false;
    public GameObject InGamePause;
    public GameObject Params, Options;

    public void OpenParams(bool x)
    {
        Params.gameObject.SetActive(x);
        Options.gameObject.SetActive(false);
    }
    public void OpenOptions(bool x)
    {
        Options.gameObject.SetActive(x);
        Params.gameObject.SetActive(false);
    }
    public void SetVolume(Slider z)
    {
        AudioListener.volume = z.value;
    }
    public void SetFullscreen(Toggle x)
    {
        Screen.fullScreen = x;
    }
    public void SetMusic(Slider z)
    {
        var e = GetComponent<AudioSource>();
        e.volume = z.value;
    }
    public void SetGodmode(Toggle x)
    {
        GameManager.DEBUG_GODMODE = x.isOn;

    }
    void UpdateStatsInfo()
    {
        if (!StatsInfo) return;
        if (!StatsInfo.gameObject.activeSelf) return;
        var own = GameManager.owners[0];
        var txt = "";
        txt += "Name : " + own.Name + "\n";
        txt += "Fertility Rate : " + own.FertilityRate+ "\n";
        txt += "Security : " + own.getSecurity + "\n";
        txt += "Housing Space : " + own.getHousingSpace + "\n";
        txt += "Production Efficiency : " + own.ProductionEfficiency + "\n";
        txt += "Total Population : " + own.totalPopulation + "\n";
        txt += "Science Rate : " + own.ScienceMod;


        StatsInfo.Texts[1].text = txt; 
    }
    Camera _main;
    GameObject draggor;
    private void Awake()
    {
        _main = Camera.main;
        draggor = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), transform.position, Quaternion.identity);
        if (draggor.GetComponent<Collider>()) Destroy(draggor.GetComponent<Collider>());


    }
    private void LateUpdate()
    {
        if (ResearchWindow.activeSelf)
        {
            UpdateTechUI();
        }
    }
    private void Update()
    {
        if (Owner.Player.CurrentTechnology != null && ResearchBar) ResearchBar.slider.value = Mathf.Lerp(ResearchBar.slider.value,  Owner.Player.CurrentTechnology.GetCurrentPtsInvest, 5* Time.smoothDeltaTime);
        if (!GameisPaused) {

            UpdateDesc();
            UpdateStatsInfo();
        } 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameisPaused)
            {
                Resume();
            }
            else
            
            {
                Pause();

            }
            
        }
       
    }

    
    public void showDescription(string title, string x)
    {
        Desc.gameObject.SetActive(true);
        Desc.Header.text = title;
        Desc.Texts[1].text = x;
    }
    public void UpdateDesc()
    {
        if (!Desc.gameObject.activeSelf) return;
        
        var e = Input.mousePosition;
        var w = Screen.width; var h = Screen.height;
        var saa = 100;
        e.y = Mathf.Clamp(e.y, 0  , h - saa);
        e.x = Mathf.Clamp(e.x, 0  , w - saa);
        Desc.transform.position = e;
    }
    public void EndDescription()
    {
        Desc.gameObject.SetActive(false);
    }
    public void Resume()
    {
        if (GameManager.owners[0].Cores[0] == null) return;
        InGamePause.SetActive(false);
        Time.timeScale = TimeScaleMult;
        GameisPaused = false;
        OpenOptions(false);
        OpenParams(false);
    }


    public void Pause()
    {
        InGamePause.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;

    }

    public void SetHpMult(Slider x )
    {
        HpMult = x.value;
        healthmul.text = x.value.ToString();
    }

    public void SetSpdMult(Slider x)
    {
        SpeedMult = x.value;
        speedmul.text = x.value.ToString();

    }
    public void SetTimeMult(Slider x)
    {
        TimeScaleMult = x.value;
        timescale.text = x.value.ToString();
    }

    public void setAi(Slider x)
    {
        TPSmult = x.value;
        aicom.text = x.value.ToString();
    }
    public void Exit( )
    {
        Application.Quit();
    }

    Rect lastbox;
    public void BoxSelection(Vector3 MouseClickPos, Vector3 MouseReleasePos)
    {
        var e = new List<entity>();
        //   
 
        BSelection.gameObject.SetActive(true);
         var a = _main.WorldToScreenPoint(MouseClickPos);
        var b = _main.WorldToScreenPoint(MouseReleasePos);
        BSelection.position = (a + b) / 2f;
        var size = new Vector2(Mathf.Abs(b.x - a.x),Mathf.Abs( b.y - a.y));
        BSelection.sizeDelta = size; 
    }
    string uiressource(string w,Owner n)
    {
        if (!n.Inventory.ContainsKey(w)) return "0";
        else return n.Inventory[w].getAmount.ToString();

    }
    public void ShowUI(Owner n,entity e)
    {
        lastOwner = n;

        Txt[0].text = uiressource("Wood", n);
        Txt[1].text = n.Gold.ToString("0.00");
        Txt[2].text = n.totalPopulation.ToString();
        Txt[3].text = uiressource("Stone", n);
        /*  if(Txt.Length > 0)
          {
              Txt[0].text = n.totalPopulation.ToString();
              Txt[1].text = n.getHousingSpace.ToString();
              Txt[4].text = n.Gold.ToString("0.00");
              Txt[2].text = n.getSecurity.ToString();
              Txt[3].text = n.ProductionEfficiency.ToString();
              Txt[5].text = uiressource("Wood", n);
              Txt[6].text = uiressource("Stone", n);

          }
          */

        //  Growth.text =  "     +" +n.getPopulationGrowth.ToString("0");
        Builder.gameObject.SetActive(n.BuilderCenter);
        Merchant.gameObject.SetActive(n.MerchantCenter);
        Research.gameObject.SetActive(n.ResearchCenter);
        Civilian.text = "Civilian: " +(n.totalPopulation - n.AllocateBuilder(0) - n.AddFighter(0)).ToString(); 
        //For now
        Builder.text = "Builder:" + n.AllocateBuilder(0).ToString();
        Merchant.text = "Merchant:" + 0.ToString();
        Research.text = "Researcher:" + n.AllocateScientist(0).ToString();

       Jobs.SetActive( Builder.gameObject.activeSelf || Merchant.gameObject.activeSelf || Research.gameObject.activeSelf);


        if (e)
        {
            UnitInfo.text = e.ToString();
        }
    }


    public void ShowStats(bool x)
    {
        StatsInfo.gameObject.SetActive(x);
    }
    public RelationshipUI[] RUI;
    public void ShowStatsToggle()
    {
        StatsInfo.gameObject.SetActive(!StatsInfo.gameObject.activeSelf);
        var i = 0;
        foreach (var item in GameManager.owners[0].Relation.Keys)
        {
            RUI[i].ShowRelationship(GameManager.owners[0], item);
            i++;
        }
       
          
        
    }

    public void AddBuilder(int x) { lastOwner.AllocateBuilder(x); }
    public void AddMerchant(int x) { }
    public void AddResearch(int x) { lastOwner.AllocateScientist(x); }



}
