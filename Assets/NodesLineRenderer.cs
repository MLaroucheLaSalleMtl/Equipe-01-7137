﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(LineRenderer))]
public class NodesLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {

        
        

    }

    public void Gen(Faction faction)
    {


        Vector3[] vector3s = new Vector3[faction.NodesList.Count];
        lineRenderer.SetVertexCount(faction.NodesList.Count);
        lineRenderer.SetWidth(.5f, .5f);
        int counter = 0;
        foreach (var item in faction.NodesList)
        {
            Vector3 vector = new Vector3();
            vector = item.transform.position;
            vector.y += 5;
            vector3s.SetValue(vector, counter);

            counter++;
        }
        lineRenderer.SetPositions(vector3s);
       


    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(GameManager.instance.GetZoomLevel >= 15)
                  gameObject.layer = LayerMask.NameToLayer("Default");
        else      
                  gameObject.layer = LayerMask.NameToLayer("UI");
        // lineRenderer.enabled = 
    }
}
