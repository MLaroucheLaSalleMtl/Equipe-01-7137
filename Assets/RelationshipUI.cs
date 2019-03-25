using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipUI : textBox
{
    Owner lastowner;
    string lastkey;
    public void ShowRelationship(Owner x,string y)
    {
      
        if (!x.Relation.ContainsKey(y)) return;
        lastowner = x;
        lastkey = y;
        gameObject.SetActive(true);
        var z = x.Relation[y];
        GetImageText.text = y[0].ToString().ToUpper();
        GetDesc.text = "Affinity : " + z.ToString() + "\n";

        if (z >= 80) GetDesc.text += "Admired";
        else if (z >= 60) GetDesc.text += "Respected";
        else if (z >= 40) GetDesc.text += "Friendly";
        else if (z >= 20) GetDesc.text += "Liked";
        else if (z >= 10) GetDesc.text += "Indifferent";
        else if (z >= 0) GetDesc.text += "Neutral";
        else if (z >= -10) GetDesc.text += "Disliked";
        else if (z >= -20) GetDesc.text += "Adversion";
        else if (z >= -52) GetDesc.text += "Enemy";
        else if (z >= -70) GetDesc.text += "Nemeis";
        else if (z >= -90) GetDesc.text += "Hated";
        else GetDesc.text += " Enemy of Humanity";
        RelationBar.maxValue = 100;
        RelationBar.minValue = -100;
        RelationBar.value = z;
        Header.text = y;
    }
    public void GTFO()
    {
        gameObject.SetActive(false);
        lastowner = null;
        lastkey = null;
    }
    public Slider RelationBar;
    public Text GetImageText
    {
        get { return Texts[1]; }
    }
    public Text GetDesc
    {
        get { return Texts[2]; }
    }
    private void FixedUpdate()
    {
        if (lastowner != null) ShowRelationship(lastowner, lastkey);
    }
}
