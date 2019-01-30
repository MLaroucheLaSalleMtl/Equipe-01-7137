using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    public GameObject Highlight;
    public building bref;
    public LayerMask BuildingLayer;
    public GameObject CancelUI;
    MeshRenderer[] colz;


    public GameObject[] Uis;


    public void Planing(GameObject z, building b)
    {
        colz = z.GetComponentsInChildren<MeshRenderer>();
        bref = b;
    }
    bool Intersect(Vector3 p)
    {
        return !Physics.CheckSphere(p, bref.SpaceNeed, BuildingLayer);//Physics.CheckBox(p + Vector3.up * .65f, new Vector3(bref.SpaceNeed,.24f,bref.SpaceNeed),Quaternion.identity, BuildingLayer);
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
        return z;
    }
}
