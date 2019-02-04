using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Faction
{
    public string FactionName { get; set; }
    public Vector3 CityCapitalPosition { get; set; }   
    public List<node> NodesList { get; set; } = new List<node>();


    public Faction(string factionName, Vector3 cityCapitalPosition, List<node> nodesList)
    {
        FactionName = factionName;
        CityCapitalPosition = cityCapitalPosition;
        NodesList = nodesList;
        
    }

}

