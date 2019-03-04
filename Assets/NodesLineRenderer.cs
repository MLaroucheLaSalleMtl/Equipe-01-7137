using System.Collections;
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

        Random random = new Random();
        

    }

    public void Gen(Faction faction)
    {

        UnityEngine.Debug.Log("eeee");
        Vector3[] vector3s = new Vector3[faction.NodesList.Count];
        lineRenderer.SetVertexCount(faction.NodesList.Count);
        
        int counter = 0;
        foreach (var item in faction.NodesList)
        {
            vector3s.SetValue(item.transform.position, counter);

            counter++;
        }
        lineRenderer.SetPositions(vector3s);
        //lineRenderer.SetPositions(vector3s);


    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
