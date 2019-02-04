using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class BorderCalculation
{
    
    
    GameManager gameManager;
    

    public BorderCalculation()
    {
        List<node> NodesList = new List<node>();

        Vector3 pointPivot = new Vector3(5, 5, 5);
        List<Faction> factions = new List<Faction>();
        Faction faction = new Faction("Wessex", pointPivot, NodesList);
        factions.Add(faction);
        UnityEngine.Debug.Log(factions[0].CityCapitalPosition);
       
        foreach (var item in GameManager.instance.Nodes)
        {
            float dist = Vector3.Distance(pointPivot, item.transform.position);
            if (dist < 100)
            {
                factions[0].NodesList.Add(item);
                
                //UnityEngine.Debug.Log(item.transform.position);

            }
           
        }
        UnityEngine.Debug.Log(factions[0].CityCapitalPosition);
        List<Vector3> positions = new List<Vector3>();
        int counter = 0;
        foreach (var item in factions[0].NodesList)
        {
            positions.Add(item.transform.position);
            counter++;
        }
       
        for (int i = 0; i < 360; i++)
        {
            foreach (var item in positions)
            {
                Vector3 targetDir = pointPivot - item;
                targetDir.y = 0;
                pointPivot.y = 0;
               
                float angle = Vector3.Angle(pointPivot.normalized, item.normalized);
                float angle2 = Vector3.Angle(targetDir.normalized, item.normalized);
                var quaternionAngle = Quaternion.AngleAxis(angle, Vector3.up);
                var quaternionAngle2 = Quaternion.AngleAxis(angle2, Vector3.up);
                var smt = Quaternion.Angle(quaternionAngle, quaternionAngle2);
                //UnityEngine.Debug.Log("node at " + item + " degrees is :" + smt + ", angle position in the circle is : " + i);
                //var rotation = Vector3.SignedAngle(pointPivot.normalized, item.normalized, new Vector3(0, 1, 0));
                //UnityEngine.Debug.Log("node at " + item + " degrees is :" + angle + ", angle position in the circle is : "+ i);
                if ((int)smt == i)
                    UnityEngine.Debug.Log("node at " + item + " degrees is :" + smt + ", angle position in the circle is : " + i);
                



            }
            
        }
        

        UnityEngine.Debug.Log("@@@@@");





    }
    
    
}

