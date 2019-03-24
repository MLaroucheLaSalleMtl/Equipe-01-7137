using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BuildingUI : MonoBehaviour
{
    public GameObject Highlight;
    public building bref;
    public LayerMask BuildingLayer;
    public GameObject CancelUI;
    
    public Animator BuildingSticker;
    public GameObject RoadPiece;
    MeshRenderer[] colz;

    public GameObject BList;
    public void SetBList(bool x)
    {
        BList.SetActive(x);
    }
    [HideInInspector]
    public static LayerMask _blayer;

    public GameObject[] Uis;

  
    public void BuildingMenu(int x)
    {
        if(x > 0)
        {
           
            BuildingSticker.SetTrigger("open");
        }
        else
        {
            GameManager.instance.CancelBuilding();
           
        
        }
     
    }

    [SerializeField]
    NavMeshAgent agi;
    NavMeshPath RoadCreator;

    private void Awake()
    {
        _blayer = BuildingLayer;
    }
    void _createRoad(Vector3 pos, Vector3 dest,GameObject g)
    {
        var x = (dest - pos).normalized;
        var dif = Vector3.Distance(pos, dest);
        for (float i = .5f; i < dif; i += .5f)
        {
            var xa = Instantiate(RoadPiece, pos + x * i, Quaternion.Euler(x));
            xa.transform.forward = x;
            xa.transform.parent = g.transform;
            
        }
    }
    public void CreateRoad(Vector3 pos ,GameObject g)
    {
        RoadCreator = new NavMeshPath();
        var e = agi.CalculatePath(pos, RoadCreator);

        Vector3 lastpos = pos;
        if(RoadCreator.corners.Length > 1)
        {
            if (e)
            {
                foreach (var item in RoadCreator.corners)
                {
                    _createRoad(lastpos, item,g);

                    lastpos = item;
                }
            }
        }
        else _createRoad(agi.transform.position, lastpos,g);
       
    }
    public void SetStartingPoint(Vector3 pos)
    {
        if (!agi) return;
        agi.transform.position = pos;
    }
    public void Planing(GameObject z, building b)
    {
        colz = z.GetComponentsInChildren<MeshRenderer>();
        bref = b;
    }
    public bool Buildmode = false;
    void InputInteraction()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Buildmode = !Buildmode;
           /* foreach (var item in GameManager.owners[0].Building)
                item.SetRadius(Buildmode);*/
        }





    }
    private void Update()
    {
        InputInteraction();   
    }
    //Need to move to Building
    bool Intersect(Vector3 p)
    {
      //  var e = Physics.OverlapBox(p, bref.SpaceNeed, GameManager.instance.building_highlight.transform.rotation, BuildingLayer);
       
         return !Physics.CheckBox(p, bref.SpaceNeed,GameManager.instance.building_highlight.transform.rotation, BuildingLayer);//Physics.CheckBox(p + Vector3.up * .65f, new Vector3(bref.SpaceNeed,.24f,bref.SpaceNeed),Quaternion.identity, BuildingLayer);
    }
    public bool CanBePlaceThere(Vector3 pos,Owner k)
    {
        var z = bref.ApprovedBuilding(pos,k) && Intersect(pos);
        for (int i = 0; i < colz.Length; i++)
        {
            if(z)
            colz[i].material.color = Color.green;
            else
                colz[i].material.color = Color.red;

        }

        if (agi) agi.transform.position = pos;
        return z;
    }
}
