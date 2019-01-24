using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class building : entity
{
    public List<node> affectednodes = new List<node>();
    public Goods[] Costs;
    [Header("UI")]
    public GameObject ContextMenu;
    public Text ContextMenuText;
    public GameObject[] buttons;
    [TextArea]
    public string description= " a normal building";
    //Priority of attack = Higher mean more interest for the enemy
    public float getValue
    {
        get { var x = 0f;
            foreach (var item in Costs)
                x += item.getAmount * item.Value;
            return x * (Hp/maximumHp);
        }
    }
    private void Start()
    {
        ContextMenu.SetActive(false);
    }
    public float ConstructionEffort = 20;
     bool BeingBuild = false;
    public bool HasEnoughRessource(Goods[] x)
    {

        if (x.Length == 0) return true;
        foreach (var item in Costs)
        {
            bool ok = false;
            foreach (var z in x)
                if (item.Name== z.Name) { ok = true; ok = item.getAmount >= z.getAmount; }

            if (!ok) return false;

        }
        return true;
    }
    public bool IsBeingBuild { get { return BeingBuild; } }
    float currEffort = 0;
    [SerializeField]
    private GameObject gauge;
    public GameObject[] graphics;


    public void build(Vector3 position, Owner n)
    {
        BeingBuild = true;
         
        n.Building.Add(this);
        transform.position = position;
        foreach (var item in graphics)
            item.SetActive(false);

        graphics[0].SetActive(true);
    }

    bool ctxmenu = false;
    public virtual void OpenContextMenu()
    {

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
        if (!BeingBuild)
            interact(GameManager.selection);

        OpenContextMenu();
    }
    public virtual void interact(entity e, float efficiency = 0)
    {
        if (e == this && BeingBuild) { Construction(efficiency); return; }
    }
    protected virtual void  Construction(float x )
    {
        currEffort += Time.fixedDeltaTime * x;
        if(currEffort > ConstructionEffort)
        {
            graphics[0].SetActive(false);
            graphics[1].SetActive(true);
            BeingBuild = false;
        }
    }
}
