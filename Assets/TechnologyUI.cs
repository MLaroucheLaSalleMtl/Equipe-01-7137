using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyUI : textBox
{
    public int Tech;

    public void SetTech(Technology t)
    {
        Tech = t.ID;
        gameObject.SetActive(true);
        Header.text = t.Name + "\nTake " + t.PtsNeed / Owner.Player.GetScientificOutput + "s to research."; ;
        Texts[1].text = t.Name[0].ToString(); 
    }
}
